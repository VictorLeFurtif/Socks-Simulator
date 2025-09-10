using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveDirection = new Vector2();
    private float radius;
    private bool isStunt;
    private InputSystem_Actions inputSystem;

    [SerializeField] private int speed = 100;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ForceMode2D forceType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject[] markers;
    [SerializeField] private float jumpPower = 3f;
    [SerializeField] private float durationJump = 1f;

    private void OnEnable()
    {
        EventManager.UpdateStunAction += UpdateStun;

    }
    private void Start()
    {
        radius = spriteRenderer.bounds.extents.x;
    }
    private void FixedUpdate()
    {
        CheckBorderCollision();
        rb.AddForce(moveDirection * speed, forceType);
    }

    private void UpdateStun(PlayerController pControler)
    {
        pControler.isStunt = !pControler.isStunt;
        if(pControler.isStunt)
            StunBackward(pControler);
    }

    private void CheckBorderCollision()
    {

        float camWidth = Camera.main.orthographicSize * Camera.main.aspect;
        Vector3 posToCam = new Vector3(Mathf.Clamp(transform.position.x, -camWidth + radius, camWidth - radius), transform.position.y, transform.position.z);
        transform.position = posToCam;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (isStunt)
            return;
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
        if (isStunt)
            return;
        EventManager.attackInput?.Invoke();
    }

    private void StunBackward(PlayerController pControler)
    {
        if(transform.position.x < 0f)
        {
            pControler.transform.DOJump(new Vector2(markers[0].transform.position.x, 0f), jumpPower, 1, durationJump).SetEase(Ease.OutQuad);
        }
        else
        {
            pControler.transform.DOJump(new Vector2(markers[1].transform.position.x, 0f), jumpPower, 1, durationJump).SetEase(Ease.OutQuad);
        }
    }
}
