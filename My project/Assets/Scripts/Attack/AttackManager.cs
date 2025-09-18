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

            //if (!isAttacking && !dontRepeatCounter)
            //    rb.bodyType = RigidbodyType2D.Dynamic;
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
                playerController.blockMovement = true;
                dontRepeatCounter = true;
                isAttacking = true;
                shouldNotCounter = false;
                wasCountered = true;
                canCounter = false;
                
                enemyAttack.InterruptAttack();
                animator.SetTrigger("IsCounter");
            }
            else if (!isAttacking && enemyAttack.rp.CurrentKoState != KoState.Ko)
            {
                playerController.blockMovement = true;
                animator.SetTrigger("IsCounter");
            } 
            
        }

        public void PlayCounterMusic()
        {
            SoundManager.Instance.PlayMusicOneShot(rp.currentPlayerPlacement == PlayerPlacement.Left
                ? SoundManager.Instance.SoundData.NoseCounter
                : SoundManager.Instance.SoundData.AssCounter);
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
            
            SoundManager.Instance.PlayMusicOneShot(rp.currentPlayerPlacement == PlayerPlacement.Left
                ? SoundManager.Instance.SoundData.TongueAttack
                : SoundManager.Instance.SoundData.EyeAttack);
        }

        public void OnAttackEnd()
        {
            if (!wasCountered && !shoulNotDoEvent)
            {
                enemyAttack.UpdateSlider();
            }

            StartCoroutine(ResetAttackState());
        }

        public IEnumerator ResetAttackState()
        {
            Debug.Log("reset" + rp.currentPlayerPlacement);
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
            StartCoroutine(ResetAttackState());
            playerController.blockMovement = false;


            if (!shouldNotCounter)
            {
                enemyAttack.UpdateSlider();
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
                SoundManager.Instance.PlayMusicOneShot(SoundManager.Instance.SoundData.TakeDamage);
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
                //StartCoroutine(ResetAttackState());
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
            SoundManager.Instance.PlayMusicOneShot(SoundManager.Instance.SoundData.ShockWave);
    
            float currentValue = shaderMat.material.GetFloat(lName);
            float targetValue = 1f;
            float speed = 2f;
    
            while (currentValue < targetValue)
            {
                currentValue += speed * Time.deltaTime; 
                currentValue = Mathf.Clamp(currentValue, -0.1f, targetValue);
        
                shaderMat.material.SetFloat(lName, currentValue);
                yield return null;
            }
    
            shaderMat.material.SetFloat(lName, -0.1f);
        }

        private void ResetElement()
        {
            ResetSlider();
        }


    }
}
