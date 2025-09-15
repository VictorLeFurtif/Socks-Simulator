using System;
using System.Collections;
using Attack;
using DG.Tweening;
using Enum;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Controller
{
    public class PlayerController : MonoBehaviour
    {
        private Vector2 moveDirection = new Vector2();
        private float radius;
        private InputSystem_Actions inputSystem;
        private float camWidth;
        
        private bool isCollidingWithPlayer = false;
        private Transform otherPlayerTransform;

        public Rigidbody2D rb; // spageti but need
        [SerializeField] private ForceMode2D forceType;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AttackManager attackManager;
        [SerializeField] private PlayerPlacement playerNum;
        [SerializeField] private GameObject ennemy;
        [SerializeField] private RopeController ropeController;
        
        private DataHolderManager commonData;

        [SerializeField] private CancelPushing antiPushZone;

        private void OnEnable()
        {
            camWidth = Camera.main.orthographicSize * Camera.main.aspect;
            inputSystem = new InputSystem_Actions();

            //TODO trouver un moyens de faire plus propre c moche
            switch (playerNum)
            {
                case PlayerPlacement.Left:
                    inputSystem.Player.Enable();
                    inputSystem.Player.Move.performed += Move;
                    inputSystem.Player.Move.canceled += ctx => moveDirection = Vector2.zero;
                    //inputSystem.Player.Look.performed += GetMousePosX;
                    //inputSystem.Player.Look.canceled += ctx => moveDirection = Vector2.zero;
                    inputSystem.Player.Attack.performed += Attack;
                    inputSystem.Player.Counter.performed += Counter;
                    break;
                case PlayerPlacement.Right:
                    inputSystem.Player1.Enable();
                    inputSystem.Player1.Move.performed += Move;
                    inputSystem.Player1.Move.canceled += ctx => moveDirection = Vector2.zero;
                    //inputSystem.Player1.Look.performed += GetMousePosY;
                    //inputSystem.Player1.Look.canceled += ctx => moveDirection = Vector2.zero;
                    inputSystem.Player1.Attack.performed += Attack;
                    inputSystem.Player1.Counter.performed += Counter;
                    break;
            }
        }

        private void Start()
        {
            radius = spriteRenderer.bounds.extents.x;
            commonData = GetComponent<DataHolderManager>();
        }
        
        private void FixedUpdate()
        {
            CheckBorderCollision();
            Vector2 allowedMovement = GetAllowedMovement(moveDirection);
            rb.AddForce(allowedMovement * commonData.playerDataCommon.PlayerControllerData.speed, forceType);
        }

        /*
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = true;
                otherPlayerTransform = other.transform;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isCollidingWithPlayer = false;
                otherPlayerTransform = null;
            }
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Vector2 directionToOther = (other.transform.position - transform.position).normalized;
                
                if (Vector2.Dot(moveDirection.normalized, directionToOther) > 0)
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }
            }
        }*/

        public Vector2 GetMoveDirection()
        {
            return moveDirection;
        }
        
        private Vector2 GetAllowedMovement(Vector2 intendedMovement)
        {

            if (antiPushZone == null || !antiPushZone.IsPlayerInAntiPushZone())
            {
                return intendedMovement;
            }
            

            Transform otherPlayer = antiPushZone.GetOtherPlayerTransform();
            if (otherPlayer == null) return intendedMovement;

            float directionToOther = otherPlayer.position.x - transform.position.x;

            if ((intendedMovement.x > 0 && directionToOther > 0) || (intendedMovement.x < 0 && directionToOther < 0))
            {
                return new Vector2(0, intendedMovement.y); 
            }

            return intendedMovement;
        }
        
        private void GetMousePosX(InputAction.CallbackContext context)
        {
            Vector2 lTemp = context.ReadValue<Vector2>();
            if (lTemp.x > 1f || lTemp.x < -1f)
                moveDirection = new Vector2(lTemp.x, 0f);
        }

        private void GetMousePosY(InputAction.CallbackContext context)
        {
            Vector2 lTemp = context.ReadValue<Vector2>();
            if (lTemp.y > 1f || lTemp.y < -1f)
                moveDirection = new Vector2(lTemp.y, 0f);
        }

        public void UpdateStun()
        {
            int lNumPlayer = 0;
            if (transform.position.x > 0f)
                lNumPlayer = 1;
            StunBackward(lNumPlayer);
        }

        private void CheckBorderCollision()
        {
            Vector3 camPos = Camera.main.transform.position;
            Vector3 posToCam = new Vector3(Mathf.Clamp(transform.position.x, camPos.x - camWidth + radius, camPos.x + camWidth - radius), transform.position.y, transform.position.z);
            transform.position = posToCam;
        }

        private void CheckPlayerDistance()
        {

        }

        public void Move(InputAction.CallbackContext ctx)
        {
            if (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko)
                return;
            moveDirection = ctx.ReadValue<Vector2>();
            moveDirection = new Vector2(moveDirection.x, 0f);
        }

        public void Attack(InputAction.CallbackContext ctx)
        {
            if (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko)
                return;

            attackManager.DetectPlayer();
        }

        private void Counter(InputAction.CallbackContext ctx)
        {
            if (commonData.playerDataCommon.RopeData.currentKoState == KoState.Ko)
                return;
            attackManager.PerformCounter();
        }

        private void StunBackward(int pDirection)
        {
            Vector2 targetPosition;

            if (ropeController.currentPlayerPlacement == PlayerPlacement.Left)
            {
                targetPosition = new Vector2(transform.position.x - commonData.playerDataCommon.PlayerControllerData.distancePush, transform.position.y);
            }
            else
            {
                targetPosition = new Vector2(transform.position.x + commonData.playerDataCommon.PlayerControllerData.distancePush, transform.position.y);
            }

            StartCoroutine(MoveToExactPosition(targetPosition));
        }

        private IEnumerator MoveToExactPosition(Vector2 targetPosition)
        {
            Vector2 startPosition = transform.position;
            float duration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.fixedDeltaTime;
                float t = elapsedTime / duration;

                float height = 4f * t * (1f - t);

                Vector2 currentPos = Vector2.Lerp(startPosition, targetPosition, t);
                currentPos.y += height * commonData.playerDataCommon.PlayerControllerData.heightFactor; 
        
                rb.MovePosition(currentPos);

                yield return new WaitForFixedUpdate();
            }
            rb.MovePosition(targetPosition);
        }
    }
}