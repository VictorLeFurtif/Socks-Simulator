using System;
using Attack;
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
                    koIndex += 1;
                    enemy.commonData.playerDataCommon.RopeData.dragging = false;
                    lineRenderer.enabled = false;
                    HideAllFlags();
                    attackManager.ResetSlider();
                    stunSlider.value = stunSlider.maxValue;
                }
            }
        }


        [Header("Input Keys (New Input System)")]
        [SerializeField, Tooltip("Key to spam for breaking free when stunned")]
        private Key releaseKey = Key.Space;
        [SerializeField, Tooltip("Key to initiate rope drag")]
        private Key dragKey = Key.E;
        

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
                    StunValue = commonData.playerDataCommon.RopeData.maxStunValue[koIndex];
                    //pc.rb.bodyType = RigidbodyType2D.Kinematic;
                }
                else
                {
                    pc.rb.bodyType = RigidbodyType2D.Dynamic;
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

        #endregion

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
                won = IsPlayerCloseToFlag(commonData.playerDataCommon.RopeData.epsilon, leftFlag.transform);
            if (currentPlayerPlacement == PlayerPlacement.Right)
                won = IsPlayerCloseToFlag(commonData.playerDataCommon.RopeData.epsilon, rightFlag.transform);

            if (won && GameManager.Instance?.CurrentState != GameState.GameOver )
            {
                enemy.CurrentPlayerState = PlayerState.Dead;
                HandleWin();
                GameManager.Instance?.GameOver();
            }
        }

        private void HandleWin()
        {
            string winnerPlacement = enemy.currentPlayerPlacement.ToString();
        }
        
        
        private bool IsPlayerCloseToFlag(float epsilon, Transform flag)
        {
            if (flag == null) return false;
            return Mathf.Abs(transform.position.x - flag.position.x) < epsilon;
        }

        private void CheckForFlagsVisuals()
        {
            HideAllFlags();

            if (currentPlayerPlacement == PlayerPlacement.Left)
            {
                leftFlag.SetActive(true);
                leftFlag.transform.position = new Vector2(enemy.transform.position.x - commonData.playerDataCommon.RopeData.flagDistance, enemy.transform.position.y);
            }
            else
            {
                rightFlag.SetActive(true);
                rightFlag.transform.position = new Vector2(enemy.transform.position.x + commonData.playerDataCommon.RopeData.flagDistance, enemy.transform.position.y);
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

        public void TryReleaseRope()
        {
            if (Keyboard.current[releaseKey].wasPressedThisFrame && commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko)
            {
                StunValue -= commonData.playerDataCommon.RopeData.stunValueToTakeOut;
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

        /*
          #region Debug

  #if UNITY_EDITOR
          private void OnDrawGizmos()
          {
              if (enemy != null)
              {
                  Gizmos.color = CanGrab(enemy.transform) ? Color.green : Color.red;
                  Gizmos.DrawWireSphere(transform.position, commonData.playerDataCommon.RopeData.minDistanceToGrab);
              }

              if (commonData.playerDataCommon.RopeData.leftFlag != null)
              {
                  Gizmos.color = Color.blue;
                  Gizmos.DrawWireCube(commonData.playerDataCommon.RopeData.leftFlag.transform.position,
                      new Vector3(commonData.playerDataCommon.RopeData.epsilon * 2, 1f, 1f));
              }

              if (commonData.playerDataCommon.RopeData.rightFlag != null)
              {
                  Gizmos.color = Color.yellow;
                  Gizmos.DrawWireCube(commonData.playerDataCommon.RopeData.rightFlag.transform.position,
                      new Vector3(commonData.playerDataCommon.RopeData.epsilon * 2, 1f, 1f));
              }

              if (commonData.playerDataCommon.RopeData.dragging && enemy != null && enemy.Ass != null)
              {
                  Gizmos.color = Color.magenta;
                  Gizmos.DrawLine(Head.position, enemy.Ass.position);
              }
          }

          private void OnDrawGizmosSelected()
          {
              Gizmos.color = Color.white;
              if (Head != null)
              {
                  Gizmos.DrawWireSphere(Head.position, 0.1f);
              }
              if (Ass != null)
              {
                  Gizmos.DrawWireSphere(Ass.position, 0.1f);
              }
          }
  #endif

          #endregion*/
    }

    
}
