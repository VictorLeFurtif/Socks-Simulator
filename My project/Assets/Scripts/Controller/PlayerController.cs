using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveDirection = new Vector2();
    private float radius;

    [SerializeField] private int speed = 100;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ForceMode2D forceType;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        radius = spriteRenderer.bounds.extents.x;
    }
    private void FixedUpdate()
    {
        CheckBorderCollision();
        rb.AddForce(moveDirection * speed, forceType);
    }

    private void CheckBorderCollision()
    {

        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Vector3 posToCam = new Vector3(Mathf.Clamp(transform.position.x, -camWidth + radius, camWidth - radius), transform.position.y, transform.position.z);
        transform.position = posToCam;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            moveDirection = ctx.ReadValue<Vector2>();
            moveDirection = new Vector2(moveDirection.x, 0f);
        }

        if (ctx.canceled)
            moveDirection = Vector2.zero;
    }

    public void Attack()
    {
        EventManager.attackInput?.Invoke();
    }
}
