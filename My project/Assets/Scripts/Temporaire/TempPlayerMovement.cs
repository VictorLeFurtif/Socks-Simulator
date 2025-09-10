using UnityEngine;

namespace Temporaire
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TempPlayerMovement : MonoBehaviour
    {
        #region Fields
    
        [Header("Movement Settings")]
        [SerializeField, Range(1f, 20f), Tooltip("Movement speed")]
        private float moveSpeed = 5f;
    
        [Header("Player Input")]
        [SerializeField, Tooltip("Which player controls this character")]
        private PlayerInput playerInput = PlayerInput.Player1;
    
        private Rigidbody2D rb;
        private float horizontalInput;
    
        #endregion
    
        #region Unity Methods
    
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }
    
        private void Update()
        {
            HandleInput();
        }
    
        private void FixedUpdate()
        {
            MovePlayer();
        }
    
        #endregion
    
        #region Movement Methods
    
        private void HandleInput()
        {
            switch (playerInput)
            {
                case PlayerInput.Player1: // ZQSD
                    horizontalInput = 0f;
                    if (Input.GetKey(KeyCode.Q)) horizontalInput = -1f;
                    if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;
                    break;
                
                case PlayerInput.Player2: // Flèches directionnelles
                    horizontalInput = 0f;
                    if (Input.GetKey(KeyCode.LeftArrow)) horizontalInput = -1f;
                    if (Input.GetKey(KeyCode.RightArrow)) horizontalInput = 1f;
                    break;
            }
        }
    
        private void MovePlayer()
        {
            Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
    
        #endregion
    
        #region Enums
    
        private enum PlayerInput
        {
            Player1,    // ZQSD
            Player2     // Arrows
        }
    
        #endregion
    }
}