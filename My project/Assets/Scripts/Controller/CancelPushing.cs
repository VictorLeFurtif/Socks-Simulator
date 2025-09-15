using System;
using UnityEngine;

namespace Controller
{
    public class CancelPushing : MonoBehaviour
    {
        private PlayerController parentController;
        private bool isPlayerInZone = false;
        private Transform otherPlayerTransform;

        void Start() 
        {
            gameObject.layer = LayerMask.NameToLayer("AntiPushZone");
            parentController = GetComponentInParent<PlayerController>();
            GetComponent<Collider2D>().isTrigger = true;
            //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), 
              //                            LayerMask.NameToLayer("Player"));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("AntiPushZone") && 
                other.gameObject != transform.parent.gameObject)
            {
                isPlayerInZone = true;
                otherPlayerTransform = other.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("AntiPushZone") &&
                other.gameObject != transform.parent.gameObject)
            {
                isPlayerInZone = false;
                otherPlayerTransform = null;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("AntiPushZone") && 
                isPlayerInZone && otherPlayerTransform != null)
            {
                Vector2 moveDirection = parentController.GetMoveDirection();
                Vector2 directionToOther = (otherPlayerTransform.position - transform.parent.position).normalized;
                
                if (Vector2.Dot(moveDirection.normalized, directionToOther) > 0)
                {
                    parentController.rb.linearVelocity = new Vector2(0, parentController.rb.linearVelocity.y);
                }
            }
        }

        public bool IsPlayerInAntiPushZone()
        {
            return isPlayerInZone;
        }

        public Transform GetOtherPlayerTransform()
        {
            return otherPlayerTransform;
        }
        
    }
}
