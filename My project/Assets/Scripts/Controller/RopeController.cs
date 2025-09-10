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
        
        [Header("Point")] 
        public Transform Ass { get; private set; }
        public Transform Head { get; private set; }

        [Header("Value")] [SerializeField] private float minDistanceToGrab;
        
        [SerializeField] private float temporaryLife;

        public float TemporaryLife
        {
            get => temporaryLife;
            private set
            {
                temporaryLife = value;
                
            }
        }

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
                }
            }
        }

        [SerializeField] private int stunValueToTakeOut;

        [Header("Key")]

         [SerializeField] private KeyCode releaseKey;
         [SerializeField] private KeyCode dragKey;
         
         [Header("State Machine")]

         private KoState currentKoState;

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
         [SerializeField] private LineRenderer lineRenderer;
         #endregion

         #region Unity Methods

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

        
        public void DragRope()
        {
            if (Input.GetKeyDown(dragKey) && !dragging && CurrentKoState == KoState.NotKo && CanGrab(enemy.transform))
            {
                dragging = true;
                StunValue = maxStunValue;
            }
            //DrawLineRenderer After ??
            //Calcul to check for life ??
            
            
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
            if (Input.GetKeyDown(releaseKey) && currentKoState == KoState.Ko && TemporaryLife > 0)
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
        
    }
}
