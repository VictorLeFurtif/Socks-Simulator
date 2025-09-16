using Controller;
using Enum;
using Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utilitary;
using static UnityEngine.EventSystems.EventTrigger;

namespace Attack
{
    public class AttackManager : MonoBehaviour
    {
        public bool isAttacking;
        private bool isInArea = false;
        public bool canCounter;
        private bool wasCountered;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        //[SerializeField] private PlayerController enemy;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AttackManager ennemyAttack;
        [SerializeField] private Slider playerSlider;
        [SerializeField] private RopeController rp;
        [SerializeField] private GameObject shaderObj;
        [SerializeField] private Renderer shaderMat;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float distanceToAttack;

        private DataHolderManager commonData;

        public void DetectPlayer()
        {

            if (isInArea && !isAttacking && !ennemyAttack.isAttacking && ennemyAttack.rp.CurrentKoState == KoState.NotKo)
            {
                StartAttack();
            }
        }

        private void Start()
        {
            commonData = GetComponent<DataHolderManager>();
            ResetSlider();
        }


        private void Update()
        {
            CheckDistance();
        }
        private void CheckDistance()
        {
            if (Mathf.Abs(transform.position.x - ennemyAttack.gameObject.transform.position.x) < distanceToAttack)
            {
                isInArea = true;
                return;
            }
            isInArea = false;

        }

        private void StartAttack()
        {
            isAttacking = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector3.zero;
            animator.SetTrigger("IsAttacking");


            //TODO hurt animation
        }

        public void OnAttackEnd()
        {
            if (!wasCountered)
                ennemyAttack.UpdateSlider();

            StartCoroutine(ResetAttackState());
        }

        private IEnumerator ResetAttackState()
        {
            yield return new WaitForSeconds(commonData.playerDataCommon.AttackManagerData.attackCooldown);

            isAttacking = false;
            canCounter = false;
            wasCountered = false;
            rb.bodyType = RigidbodyType2D.Dynamic;

        }

        public void SetCounterPossible() => canCounter = !canCounter;

        public void PerformCounter()
        {
            if (ennemyAttack.canCounter && !wasCountered)
            {
                ennemyAttack.InterruptAttack();

                wasCountered = true;
                canCounter = false;
                animator.SetTrigger("IsCounter");
            }
        }

        public void InterruptAttack()
        {
            if (isAttacking)
            {
                animator.ResetTrigger("IsAttacking");
                animator.Play("Idle");
                StartCoroutine(ResetAttackState());
            }
        }


        public void OnCounterEnd()
        {
            ennemyAttack.UpdateSlider();
            StartCoroutine(ResetAttackState());
        }

        private void UpdateSlider()
        {
            if (playerSlider.value + commonData.playerDataCommon.AttackManagerData.looseSlider < playerSlider.maxValue)
            {
                UiHelper.UpdateSlider(this,playerSlider,playerSlider.value + commonData.playerDataCommon.AttackManagerData.looseSlider);
                playerController.UpdateStun();
            }
            else
            {
                StartCoroutine(UpdateSliderIfKo());
            }
        }

        IEnumerator UpdateSliderIfKo()
        {
            yield return UiHelper.UpdateSliderCoroutine(this,playerSlider,playerSlider.maxValue);
            ToggleSlidersAttack(false);
            rp.CurrentKoState = KoState.Ko;
        }
        
        public void ToggleSlidersAttack(bool _bool)
        {
            playerSlider.gameObject.SetActive(_bool);
        }
        public  void CheckShouldInterrupt()
        {
            if (!isInArea)
            {
                Debug.Log(isInArea + " should interrupt");
                InterruptAttack();
            }
        }

        public void ResetSlider()
        {
            playerSlider.value = 0;
            ToggleSlidersAttack(true);
        }

        public IEnumerator ShockWave()
        {
            string lName = "_WaveDistanceFromCenter";
            shaderObj.transform.position = ennemyAttack.gameObject.transform.position;
            while (shaderMat.material.GetFloat(lName) < 1)
            {
                shaderMat.material.SetFloat(lName, shaderMat.material.GetFloat(lName) + 0.005f);
                yield return null;
            }
            if (!(shaderMat.material.GetFloat(lName) < 1))
            {
                shaderMat.material.SetFloat(lName, -0.1f);
            }
        }

        private void OnEnable()
        {
            PlayerScoreManager.OnRoundReset += ResetElement;
        }

        private void OnDisable()
        {
            PlayerScoreManager.OnRoundReset -= ResetElement;
        }

        private void ResetElement()
        {
            ResetSlider();
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (isInArea)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, ennemyAttack.gameObject.transform.position);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, ennemyAttack.gameObject.transform.position);
            }
        }
#endif
    }
}
