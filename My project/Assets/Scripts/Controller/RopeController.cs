using System;
using System.Collections;
using Attack;
using DG.Tweening;
using Enum;
using Interface;
using Manager;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controller
{
    public class RopeController : MonoBehaviour, IDraggable
    {
        #region Fields

        [SerializeField] Animator animator;

        [Header("Enemy")]
        [SerializeField, Tooltip("Reference to the opponent player")]
        private RopeController enemy;

        [Header("Anchor Points")]
        [Tooltip("Bottom anchor point for rope attachment")]
        public Transform Ass;
        [Tooltip("Top anchor point for rope attachment")]
        public Transform Head;

        private int koIndex = 0;

        private float stunValue;

        [SerializeField] private Slider goldenSlider;
        [SerializeField] private GameObject fxSlider;

        public float StunValue
        {
            get => stunValue;
            private set
            {
                stunValue = Mathf.Clamp(value, 0f, commonData.playerDataCommon.RopeData.maxStunValue[koIndex]);

                if (stunSlider != null)
                {
                    stunSlider.maxValue = commonData.playerDataCommon.RopeData.maxStunValue[koIndex];
                    stunSlider.minValue = 0f;
                    stunSlider.value = (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko) ? stunValue : 0f;
                }


                if (stunValue <= 0)
                {
                    CurrentKoState = KoState.NotKo;
                    enemy.animator.SetBool("Dragging", false);
                    animator.SetBool("IsStunned", false);
                    koIndex += 1;
                    enemy.commonData.playerDataCommon.RopeData.dragging = false;
                    lineRenderer.enabled = false;
                    HideAllFlags();
                    attackManager.ResetSlider();
                    stunSlider.value = stunSlider.maxValue;
                    attackManager.ToggleSlidersAttack(true);
                }
            }
        }


        [Header("Input Keys (New Input System)")]
        [SerializeField, Tooltip("Key to spam for breaking free when stunned")]
        private Key releaseKey = Key.E;
        [SerializeField, Tooltip("Key to initiate rope drag")]
        private Key dragKey = Key.F;
        

        [SerializeField, Tooltip("Left or Right player position")]
        public PlayerPlacement currentPlayerPlacement = PlayerPlacement.Left;

        private PlayerState currentPlayerState = PlayerState.Idle;

        public PlayerState CurrentPlayerState
        {
            get => currentPlayerState;
            private set => currentPlayerState = value;
        }

    
        
        public KoState CurrentKoState
        {
            get => commonData.playerDataCommon.RopeData.currentKoState;
            set
            {
                commonData.playerDataCommon.RopeData.currentKoState = value;

                koIndex = Mathf.Clamp(koIndex, 0, commonData.playerDataCommon.RopeData.maxStunValue.Length - 1);

                if (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko)
                {
                    StartCoroutine(SliderTransitionForKo());
                }
                else
                {
                    pc.rb.bodyType = RigidbodyType2D.Dynamic;
                    ToggleSlidersStun(false);
                }
            }
        }

        [Header("Visual Components")]
        [SerializeField, Tooltip("Line renderer for rope visualization")]
        public LineRenderer lineRenderer;
        

        [Header("UI Components")]
        [SerializeField, Tooltip("Slider representing stun value")]
        private Slider stunSlider;

        [Header("Player Controller & Attack Manager")]
        [SerializeField]
        private PlayerController pc;

        [SerializeField] private AttackManager attackManager;

        private DataHolderManager commonData;

        [SerializeField] private GameObject leftFlag;
        [SerializeField] private GameObject rightFlag;
        #endregion

        #region Unity Methods

        private void Start()
        {
            HideAllFlags();
            commonData = GetComponent<DataHolderManager>();
        }

        private void Update()
        {
            TimerStun();

            if (CurrentKoState == KoState.NotKo)
            {
                DragRope();
            }
            else
            {
                TryReleaseRope();
            }
            
            RopeTension();
        }

        private void LateUpdate()
        {

            DrawLineRenderer(enemy?.Ass);
        }

        private void OnEnable()
        {
            PlayerScoreManager.OnRoundReset += ResetElement;
        }

        private void OnDisable()
        {
            PlayerScoreManager.OnRoundReset -= ResetElement;
        }

        #endregion

        private void ResetElement()
        {
            commonData.playerDataCommon.RopeData.dragging = false;
            CurrentKoState = KoState.NotKo;
            CurrentPlayerState = PlayerState.Idle;
            HideAllFlags();
            stunSlider.value = commonData.playerDataCommon.RopeData.maxStunValue[0]; //reset slider
            koIndex = 0;
            StunValue = 0;
            CurrentPlayerState = PlayerState.Idle;
            animator.ResetTrigger("Dead");
            animator.Play("Idle");
        }
        
        #region RopeController Methods For Attacker

        private void RopeTension()
        {
            
            if (commonData.playerDataCommon.RopeData.dragging && pc.rb.linearVelocity.x != 0) 
            {
                if (currentPlayerPlacement == PlayerPlacement.Left)
                {
                    pc.rb.AddForce(-commonData.playerDataCommon.RopeData.tensionStrength * Vector2.left, ForceMode2D.Force);   
                }
                else
                {
                    pc.rb.AddForce(-commonData.playerDataCommon.RopeData.tensionStrength * Vector2.right, ForceMode2D.Force);  
                }
            }
        }
        
        public void DragRope()
        {

            if (Keyboard.current[dragKey].wasPressedThisFrame && !commonData.playerDataCommon.RopeData.dragging &&
                enemy.commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko && CanGrab(enemy.transform))
            {
                animator.SetBool("Dragging", true);
                StartDragging();
            }

            if (commonData.playerDataCommon.RopeData.dragging)
            {
                CheckWinCondition();
            }
        }

        private void StartDragging()
        {
            commonData.playerDataCommon.RopeData.dragging = true;
            enemy.pc.rb.bodyType = RigidbodyType2D.Kinematic;
            lineRenderer.enabled = true;
            CheckForFlagsVisuals();

        }

        private void CheckWinCondition()
        {
            bool won = false;

            if (currentPlayerPlacement == PlayerPlacement.Left)
            {
                won = IsPlayerCloseToFlag(commonData.playerDataCommon.RopeData.epsilon, leftFlag.transform);
                
            }       
            
            if (currentPlayerPlacement == PlayerPlacement.Right)
            {
                won = IsPlayerCloseToFlag(commonData.playerDataCommon.RopeData.epsilon, rightFlag.transform);
                
            }

            if (won && GameManager.Instance?.CurrentState != GameState.GameOver && !PlayerScoreManager._instance.win)
            {
                enemy.CurrentPlayerState = PlayerState.Dead;
                enemy.animator.SetTrigger("Dead"); // enemy is dying
                StatePlayerWhenWinning();
                HandleScoring();
            }
        }

        private void HandleScoring()
        {
            PlayerScoreManager._instance.AddScore(currentPlayerPlacement);
        }

        private void StatePlayerWhenWinning()
        {
            commonData.playerDataCommon.RopeData.dragging = false;
            lineRenderer.enabled = false;
            animator.SetBool("Dragging", false);
            animator.Play("Idle");
        }
        
        private bool IsPlayerCloseToFlag(float epsilon, Transform flag)
        {
            if (flag == null) return false;
            return Mathf.Abs(transform.position.x - flag.position.x) < epsilon;
        }

        [SerializeField] private float yPositionFlags;
        private void CheckForFlagsVisuals()
        {
            HideAllFlags();

            if (currentPlayerPlacement == PlayerPlacement.Left)
            {
                leftFlag.SetActive(true);
                leftFlag.transform.position = new Vector2(enemy.transform.position.x - commonData.playerDataCommon.RopeData.flagDistance, yPositionFlags);
            }
            else
            {
                rightFlag.SetActive(true);
                rightFlag.transform.position = new Vector2(enemy.transform.position.x + commonData.playerDataCommon.RopeData.flagDistance, yPositionFlags);
            }

        }

        private void HideAllFlags()
        {
            if (leftFlag != null) 
                leftFlag.SetActive(false);
            if (rightFlag != null) 
                rightFlag.SetActive(false);
        }

        private bool CanGrab(Transform enemyTransform)
        {
            if (enemyTransform == null) return false;
            return Mathf.Abs(enemyTransform.position.x - transform.position.x) < commonData.playerDataCommon.RopeData.minDistanceToGrab;
        }

        #endregion

        #region RopeController Methods For Defenseur

        private bool isShaking = false;
        private bool goingLeft = true; 

        public void TryReleaseRope()
        {
            if (Keyboard.current[releaseKey].wasPressedThisFrame && 
                commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko && 
                StunValue > 0)
            {
                StunValue -= commonData.playerDataCommon.RopeData.stunValueToTakeOut;
        
                if (!isShaking)
                {
                    fxSlider.SetActive(true);
                    isShaking = true;
                    float targetAngle = goingLeft ? -45f : 45f;
            
                    stunSlider.transform.DORotate(new Vector3(0, 0, targetAngle), 0.1f)
                        .SetEase(Ease.InFlash)
                        .OnComplete(() => {
                            stunSlider.transform.DORotate(Vector3.zero, 0.1f)
                                .SetEase(Ease.Linear)
                                .OnComplete(() => {
                                    isShaking = false;
                                    fxSlider.SetActive(false);
                                });
                        });
            
                    goingLeft = !goingLeft;
                }
            }
        }

        private void TimerStun()
        {
            if (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko && StunValue > 0)
            {
                StunValue -= Time.deltaTime;
            }
        }

        #endregion

        #region Line Renderer Handler

        private void DrawLineRenderer(Transform enemyPosition)
        {
            if (!commonData.playerDataCommon.RopeData.dragging || enemyPosition == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, Head.position);
            lineRenderer.SetPosition(1, enemyPosition.position);
        }

        #endregion
        
        #region UI

        private void ToggleSlidersStun(bool _bool)
        {
            stunSlider.gameObject.SetActive(_bool);
        }

        #endregion
        
        #region Debug

  #if UNITY_EDITOR 
          private void OnDrawGizmos()
          {
              if (enemy != null && commonData != null)
              {
                  Gizmos.color = CanGrab(enemy.transform) ? Color.green : Color.red;
                  Gizmos.DrawWireSphere(transform.position, commonData.playerDataCommon.RopeData.minDistanceToGrab);
              }
              
          }
          
  #endif

          #endregion

          #region Coroutine
          
          private IEnumerator SliderTransitionForKo()
          {
              goldenSlider.gameObject.SetActive(true);
              yield return goldenSlider.transform.DOShakeRotation(1f, new Vector3(0, 0, 45f)).WaitForCompletion();
              goldenSlider.gameObject.SetActive(false);
              ToggleSlidersStun(true);
              yield return new WaitForSeconds(1f);
              StunValue = commonData.playerDataCommon.RopeData.maxStunValue[koIndex];
          }

          #endregion
    }

    
}
