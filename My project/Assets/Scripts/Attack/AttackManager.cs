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
        [Header("States")]
        public bool isAttacking;
        public bool canCounter;

        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private AttackManager enemyAttack;
        [SerializeField] private Slider playerSlider;
        [SerializeField] private RopeController rp;
        [SerializeField] private GameObject shaderObj;
        [SerializeField] private Renderer shaderMat;
        [SerializeField] private Rigidbody2D rb;

        [Header("Settings")]
        [SerializeField] private float distanceToAttack;

        private DataHolderManager commonData;
        private bool wasCountered;
        private bool isInArea = false;
        private bool shoulNotDoEvent = true;
        private bool shouldNotCounter = true;
        private bool dontRepeatCounter;


        private void Start()
        {
            commonData = GetComponent<DataHolderManager>();
            ResetSlider();
        }
        private void OnEnable()
        {
            PlayerScoreManager.OnRoundReset += ResetElement;
        }

        private void OnDisable()
        {
            PlayerScoreManager.OnRoundReset -= ResetElement;
        }

        private void Update()
        {
            CheckDistance();
        }

        public void DetectPlayer()
        {

            if (isInArea && !isAttacking && !enemyAttack.isAttacking && enemyAttack.rp.CurrentKoState == KoState.NotKo)
            {
                shoulNotDoEvent = false;
                StartAttack();
            }
            else if(!isAttacking && enemyAttack.rp.CurrentKoState == KoState.NotKo)
                StartAttack();
        }

        public void PerformCounter()
        {
            if (enemyAttack.canCounter && !wasCountered && !isAttacking && !dontRepeatCounter && enemyAttack.rp.CurrentKoState != KoState.Ko)
            {
                dontRepeatCounter = true;
                isAttacking = true;
                shouldNotCounter = false;
                wasCountered = true;
                canCounter = false;

                enemyAttack.InterruptAttack();
                animator.SetTrigger("IsCounter");
                enemyAttack.animator.SetBool("AttackedBad", true);
            }
            else if (!isAttacking && enemyAttack.rp.CurrentKoState != KoState.Ko)
                animator.SetTrigger("IsCounter"); 
        }

        private void CheckDistance()
        {
            if (Mathf.Abs(transform.position.x - enemyAttack.gameObject.transform.position.x) < distanceToAttack)
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
        }

        public void OnAttackEnd()
        {
            if (shoulNotDoEvent)
                return;
            
            if (!wasCountered)
            {
                enemyAttack.UpdateSlider();
            }

            StartCoroutine(ResetAttackState());
        }

        private IEnumerator ResetAttackState()
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            animator.Play("Idle");


            yield return new WaitForSeconds(commonData.playerDataCommon.AttackManagerData.attackCooldown);


            dontRepeatCounter = false;
            isAttacking = false;
            canCounter = false;
            wasCountered = false;
            shoulNotDoEvent = true;
            shouldNotCounter = true;

        }

        public void SetCounterPossible()
        {
            if (shoulNotDoEvent)
                return;
            canCounter = !canCounter;
        }

        private void InterruptAttack()
        {
            if (isAttacking)
            {
                animator.Play("Idle");
                animator.ResetTrigger("IsAttacking");
                StartCoroutine(ResetAttackState());
            }
        }


        public void OnCounterEnd()
        {
            if (!shouldNotCounter)
            {
                enemyAttack.UpdateSlider();
                StartCoroutine(ResetAttackState());
                animator.SetTrigger("CounteredBad");
            }
        }

        private void UpdateSlider()
        {
            playerController.UpdateStun();

            if (playerSlider.value + commonData.playerDataCommon.AttackManagerData.looseSlider < playerSlider.maxValue)
            {
                UiHelper.UpdateSlider(this, playerSlider, playerSlider.value + commonData.playerDataCommon.AttackManagerData.looseSlider);
                animator.SetTrigger("TakingDamage");
            }
            else
                StartCoroutine(UpdateSliderIfKo());
        }

        IEnumerator UpdateSliderIfKo()
        {
            yield return UiHelper.UpdateSliderCoroutine(this, playerSlider, playerSlider.maxValue);
            ToggleSlidersAttack(false);
            rp.CurrentKoState = KoState.Ko;
            animator.SetBool("IsStunned", true);
        }

        public void ToggleSlidersAttack(bool _bool)
        {
            playerSlider.gameObject.SetActive(_bool);
        }

        public void CheckShouldInterrupt()
        {
            if (!isInArea && enemyAttack.rp.CurrentKoState == KoState.NotKo)
            {
                shoulNotDoEvent = true;
                StartCoroutine(ResetAttackState());
                return;
            }
            shoulNotDoEvent = false;
        }

        public void ResetSlider()
        {
            playerSlider.value = 0;
            ToggleSlidersAttack(true);
        }

        public IEnumerator ShockWave()
        {
            if (shoulNotDoEvent)
                yield break;
            string lName = "_WaveDistanceFromCenter";
            shaderObj.transform.position = enemyAttack.gameObject.transform.position;
            while (shaderMat.material.GetFloat(lName) < 1)
            {
                shaderMat.material.SetFloat(lName, shaderMat.material.GetFloat(lName) + 0.015f);
                yield return null;
            }
            if (!(shaderMat.material.GetFloat(lName) < 1))
            {
                shaderMat.material.SetFloat(lName, -0.1f);
            }
        }

        private void ResetElement()
        {
            ResetSlider();
        }


    }
}
