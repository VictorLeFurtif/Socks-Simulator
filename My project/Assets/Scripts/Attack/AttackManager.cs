using Controller;
using Enum;
using Manager;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
        }


        private void Update()
        {
            Debug.Log($"{isInArea} and player : {gameObject.name}");
            CheckDistance();
            Debug.Log(isInArea);
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
            playerController.UpdateStun();

            if (playerSlider.value - commonData.playerDataCommon.AttackManagerData.looseSlider > 0)
            {
                playerSlider.value -= commonData.playerDataCommon.AttackManagerData.looseSlider;
            }
            else
            {
                playerSlider.value = 0;
                rp.CurrentKoState = KoState.Ko;
                //StartCoroutine(ShockWave());
            }
        }

        public void ResetSlider()
        {
            playerSlider.value = playerSlider.maxValue;
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
