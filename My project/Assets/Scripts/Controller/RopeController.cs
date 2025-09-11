using System;
using Enum;
using Interface;
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

        [Header("Grab Settings")]
        [SerializeField, Range(0.1f, 5f), Tooltip("Minimum distance required to grab enemy")]
        private float minDistanceToGrab = 1.5f;

        [Header("Stun System")]
        [SerializeField, Range(1f, 10f), Tooltip("Maximum stun duration when grabbed")]
        private float maxStunValue = 5f;

        [SerializeField, Range(0.1f, 2f), Tooltip("Stun value reduced per button press")]
        private float stunValueToTakeOut = 0.2f;

        [SerializeField, Range(0.1f, 5f), Tooltip("Distance from ko player and the flag")]
        private float flagDistance;

        private float stunValue;

        public float StunValue
        {
            get => stunValue;
            private set
            {
                stunValue = Mathf.Clamp(value, 0f, maxStunValue);

                if (stunSlider != null)
                {
                    stunSlider.maxValue = maxStunValue;
                    stunSlider.minValue = 0f;
                    stunSlider.value = (currentKoState == KoState.Ko) ? stunValue : 0f;
                }


                if (stunValue <= 0)
                {
                    CurrentKoState = KoState.NotKo;
                    pc.isStunt = false;
                    enemy.dragging = false;
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

        [Header("State Machine")]
        [SerializeField] public KoState currentKoState = KoState.NotKo;

        [SerializeField, Tooltip("Left or Right player position")]
        public PlayerPlacement currentPlayerPlacement = PlayerPlacement.Left;

        private PlayerState currentPlayerState = PlayerState.Idle;

        public PlayerState CurrentPlayerState
        {
            get => currentPlayerState;
            private set { currentPlayerState = value; }
        }

        public KoState CurrentKoState
        {
            get => currentKoState;
            set
            {
                currentKoState = value;
                if (value == KoState.Ko)
                {
                    StunValue = maxStunValue;
                }
            }
        }

        [Header("Visual Components")]
        [SerializeField, Tooltip("Line renderer for rope visualization")]
        public LineRenderer lineRenderer;

        [SerializeField, Tooltip("Flag shown when left player can win")]
        private GameObject leftFlag;
        [SerializeField, Tooltip("Flag shown when right player can win")]
        private GameObject rightFlag;

        [Header("Win Condition")]
        [SerializeField, Range(0.1f, 2f), Tooltip("Distance threshold to flag for winning")]
        private float epsilon = 0.5f;

        [Header("Runtime State")]
        [SerializeField]
        private bool dragging = false;

        [Header("UI Components")]
        [SerializeField, Tooltip("Slider representing stun value")]
        private Slider stunSlider;

        [Header("Player Controller & Attack Manager")]
        [SerializeField]
        private PlayerController pc;

        [SerializeField] private AttackManager attackManager;

        [SerializeField] private float tensionStrength;
        #endregion

        #region Unity Methods

        private void Start()
        {
            HideAllFlags();
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
            if (dragging && pc.rb.linearVelocity.x != 0) 
            {
                if (currentPlayerPlacement == PlayerPlacement.Left)
                {
                    pc.rb.AddForce(-tensionStrength * Vector2.left, ForceMode2D.Force);   
                }
                else
                {
                    pc.rb.AddForce(-tensionStrength * Vector2.right, ForceMode2D.Force);  
                }
            }
        }
        
        public void DragRope()
        {

            Debug.Log(CanGrab(enemy.transform));

            if (Keyboard.current[dragKey].wasPressedThisFrame && !dragging && enemy.CurrentKoState == KoState.Ko && CanGrab(enemy.transform))
            {
                StartDragging();
            }

            if (dragging)
            {
                CheckWinCondition();
            }
        }

        private void StartDragging()
        {
            dragging = true;
            lineRenderer.enabled = true;
            CheckForFlagsVisuals();

        }

        private void CheckWinCondition()
        {
            bool won = false;

            if (currentPlayerPlacement == PlayerPlacement.Left)
                won = IsPlayerCloseToFlag(epsilon, leftFlag.transform);
            if (currentPlayerPlacement == PlayerPlacement.Right)
                won = IsPlayerCloseToFlag(epsilon, rightFlag.transform);

            if (won)
            {
                enemy.CurrentPlayerState = PlayerState.Dead;
                Debug.Log($"{enemy} Lose");
            }
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
                leftFlag.transform.position = new Vector2(enemy.transform.position.x - flagDistance, enemy.transform.position.y);
            }
            else
            {
                rightFlag.SetActive(true);
                rightFlag.transform.position = new Vector2(enemy.transform.position.x + flagDistance, enemy.transform.position.y);
            }

        }

        private void HideAllFlags()
        {
            if (leftFlag != null) leftFlag.SetActive(false);
            if (rightFlag != null) rightFlag.SetActive(false);
        }

        private bool CanGrab(Transform enemyTransform)
        {
            if (enemyTransform == null) return false;
            return Mathf.Abs(enemyTransform.position.x - transform.position.x) < minDistanceToGrab;
        }

        #endregion

        #region RopeController Methods For Defenseur

        public void TryReleaseRope()
        {
            if (Keyboard.current[releaseKey].wasPressedThisFrame && currentKoState == KoState.Ko)
            {
                StunValue -= stunValueToTakeOut;
            }
        }

        private void TimerStun()
        {
            if (currentKoState == KoState.Ko && StunValue > 0)
            {
                StunValue -= Time.deltaTime;
            }
        }

        #endregion

        #region Line Renderer Handler

        private void DrawLineRenderer(Transform enemyPosition)
        {
            if (!dragging || enemyPosition == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, Head.position);
            lineRenderer.SetPosition(1, enemyPosition.position);
        }

        #endregion

        
        #region Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (enemy != null)
            {
                Gizmos.color = CanGrab(enemy.transform) ? Color.green : Color.red;
                Gizmos.DrawWireSphere(transform.position, minDistanceToGrab);
            }

            if (leftFlag != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(leftFlag.transform.position, new Vector3(epsilon * 2, 1f, 1f));
            }

            if (rightFlag != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(rightFlag.transform.position, new Vector3(epsilon * 2, 1f, 1f));
            }

            if (dragging && enemy != null && enemy.Ass != null)
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

        #endregion
    }

    
}
