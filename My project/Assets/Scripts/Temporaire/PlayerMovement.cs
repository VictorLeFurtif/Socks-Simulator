using UnityEngine;
using UnityEngine.InputSystem;

namespace Temporaire
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 5f;
        
        [Header("Player Setup")]
        [SerializeField] private bool isPlayer1 = true;
        
        private Rigidbody2D rb;
        private float moveInput;
        
        // Input Actions pour les deux joueurs
        private InputAction player1MoveAction;
        private InputAction player2MoveAction;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // Créer les Input Actions manuellement
            if (isPlayer1)
            {
                player1MoveAction = new InputAction("Player1Move", InputActionType.Value);
                player1MoveAction.AddCompositeBinding("1DAxis")
                    .With("Negative", "<Keyboard>/a")
                    .With("Positive", "<Keyboard>/d");
            }
            else
            {
                player2MoveAction = new InputAction("Player2Move", InputActionType.Value);
                player2MoveAction.AddCompositeBinding("1DAxis")
                    .With("Negative", "<Keyboard>/leftArrow")
                    .With("Positive", "<Keyboard>/rightArrow");
            }
        }
        
        private void OnEnable()
        {
            if (isPlayer1)
            {
                player1MoveAction?.Enable();
                player1MoveAction.performed += OnMove;
                player1MoveAction.canceled += OnMove;
            }
            else
            {
                player2MoveAction?.Enable();
                player2MoveAction.performed += OnMove;
                player2MoveAction.canceled += OnMove;
            }
        }
        
        private void OnDisable()
        {
            if (isPlayer1)
            {
                player1MoveAction?.Disable();
                player1MoveAction.performed -= OnMove;
                player1MoveAction.canceled -= OnMove;
            }
            else
            {
                player2MoveAction?.Disable();
                player2MoveAction.performed -= OnMove;
                player2MoveAction.canceled -= OnMove;
            }
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<float>();
        }
        
        private void FixedUpdate()
        {
            
            Vector2 movement = new Vector2(moveInput * speed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
        
        private void OnDestroy()
        {
            player1MoveAction?.Dispose();
            player2MoveAction?.Dispose();
        }
    }
}