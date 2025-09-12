using System;
using System.Collections;
using Controller;
using Enum;
using Manager;
using UnityEngine;
using UnityEngine.UI;

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

        private DataHolderManager commonData;

        public void DetectPlayer()
        {
       
            if (isInArea && !isAttacking && !ennemyAttack.isAttacking)
            {
                StartAttack();
            }
        }

        private void Start()
        {
            commonData = GetComponent<DataHolderManager>();
        }

        private void OnTriggerEnter2D(Collider2D pCollider)
        {
            if (pCollider.gameObject.CompareTag("Player") && !isInArea)
                isInArea = true;
        }

        private void OnTriggerExit2D(Collider2D pCollider)
        {
            if (pCollider.gameObject.CompareTag("Player"))
                isInArea = false;
        }

        private void StartAttack()
        {
            isAttacking = true;
       
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
                isAttacking = false;
                canCounter = false;
                wasCountered = true;

                animator.ResetTrigger("IsAttacking");
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
            }
        }

        public void ResetSlider()
        {
            playerSlider.value = playerSlider.maxValue;
        }

    }
}
