using System;
using System.Timers;
using Enum;
using Interface;
using UnityEngine;

namespace Controller
{
    public class RopeController : MonoBehaviour, IDraggable
    {
        #region Fields

        [Header("Enemy")]
        [SerializeField]
        private RopeController enemy;

        [Header("Point")] public Transform Ass;
        public Transform Head;

        [Header("Value")] [SerializeField] private float minDistanceToGrab;
        

        [SerializeField] private float maxStunValue;
        
        private float stunValue;

        public float StunValue
        {
            get => stunValue;
            private set
            {
                stunValue = value;
                if (stunValue <= 0)
                {
                    CurrentKoState = KoState.NotKo;
                    enemy.lineRenderer.enabled = false;
                    CheckForFlagsVisuals();
                }
            }
        }

        [SerializeField] private float stunValueToTakeOut;

        [Header("Key")]

         [SerializeField] private KeyCode releaseKey;
         [SerializeField] private KeyCode dragKey;
         
         [Header("State Machine")]

         private KoState currentKoState;

         [SerializeField] private PlayerPlacement currentPlayerPlacement;

         private PlayerState currentPlayerState;

         public PlayerState CurrentPlayerState
         {
             get => currentPlayerState;
             private set { currentPlayerState = value; }
         }

         public KoState CurrentKoState
         {
             get => currentKoState;
             private set
             {
                 //Here we need to make sure that The input are or blocked or release 
                 currentKoState = value;
                 
             }
         }

         [Header("Visuals")] 
         public LineRenderer lineRenderer; //for the moment

         [SerializeField] private GameObject leftFlag;
         [SerializeField] private GameObject rightFlag;
         #endregion

         #region Unity Methods

         private void Start()
         {
             leftFlag.SetActive(false);
             rightFlag.SetActive(false);
         }

         private void Update()
         {
             TimerStun();
         }

         private void LateUpdate()
         {
             DrawLineRenderer(enemy.Ass);
         }

         #endregion

        #region RopeController Methods For Attacker
        
        //need to assign the maximum value at the begging or this will be called in Update to divide the logic
        //TODO : no idea for the moment

        private bool dragging = false;

        [SerializeField] private float epsilon;
        public void DragRope()
        {
            //Set active true a game object if left or right player then 
            if (Input.GetKeyDown(dragKey) && !dragging && CurrentKoState == KoState.NotKo && CanGrab(enemy.transform)) //first time
            {
                dragging = true;
                StunValue = maxStunValue;
                lineRenderer.enabled = true;

                CheckForFlagsVisuals();
            }

            bool won = false;
            
            if (currentPlayerPlacement == PlayerPlacement.Left) won = IsPlayerCloseToFlag(epsilon, leftFlag.transform);
            if (currentPlayerPlacement == PlayerPlacement.Right) won = IsPlayerCloseToFlag(epsilon, rightFlag.transform);

            if (won) //you killed your opponent
            {
                enemy.CurrentPlayerState = PlayerState.Dead;
            }
        }

        private bool IsPlayerCloseToFlag(float epsilon, Transform flag)
        {
            return Mathf.Abs(transform.position.x - flag.transform.position.x) < epsilon;
        }
        
        private void CheckForFlagsVisuals()
        {
            if (currentPlayerPlacement == PlayerPlacement.Left) rightFlag.SetActive(true);
            else leftFlag.SetActive(true);
        }
        
        private bool CanGrab(Transform enemy)
        {
            return Mathf.Abs(enemy.position.x - transform.position.x) < minDistanceToGrab;
        }
        
        #endregion

        #region RopeController Methods For Defenseur
        
        // Here you spam it to release yourself when you re KO
        //Please god have mercy sooo we need to spam to get the stunValue to 0
        // Then were good because we will make a Get Setter when we Update this value
        //A void will be call to change the state condition in setter if stun value
        // <= 0 so not KO can play and the Enemy release rope easier if also if enemy is 
        // a variable
        public void TryReleaseRope()
        {
            if (Input.GetKeyDown(releaseKey) && currentKoState == KoState.Ko ) //TODO if dead link to enum
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
            if (!dragging) return;
            lineRenderer.SetPosition(0,Head.position);
            lineRenderer.SetPosition(1,enemyPosition.position);
        }

        #endregion

        #region Debug

#if UNITY_EDITOR //for the final build here for Debug so all good no worry
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

            if (dragging && enemy != null)
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