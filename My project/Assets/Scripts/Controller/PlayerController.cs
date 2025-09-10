using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ePlayerNum
{
    player1,
    player2
}
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
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float durationJump = 1f;
    [SerializeField] private AttackManager attackManager;
    public ePlayerNum playerNum;

    private void OnEnable()
    {
        EventManager.UpdateStunAction += UpdateStun;
        inputSystem = new InputSystem_Actions();
        switch (playerNum)
        {
            case ePlayerNum.player1:
                Debug.Log("player1");
                inputSystem.Player.Enable();
                inputSystem.Player.Move.performed += Move;
                inputSystem.Player.Move.canceled += ctx => moveDirection = Vector2.zero;
                inputSystem.Player.Attack.performed += Attack;
                inputSystem.Player.Counter.performed += Counter;
                break;
            case ePlayerNum.player2:
                Debug.Log("player2");
                inputSystem.Player1.Enable();
                inputSystem.Player1.Move.performed += Move;
                inputSystem.Player1.Move.canceled += ctx => moveDirection = Vector2.zero;
                inputSystem.Player1.Attack.performed += Attack;
                inputSystem.Player1.Counter.performed += Counter;
                break;
        }

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
        pControler.isStunt = true;
        if (pControler.isStunt)
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
        Debug.Log(isStunt);
        moveDirection = ctx.ReadValue<Vector2>();
        moveDirection = new Vector2(moveDirection.x, 0f);
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (isStunt)
            return;
        attackManager.DetectPlayer();
    }

    private void Counter(InputAction.CallbackContext ctx)
    {
        if(isStunt)
            return;
        attackManager.PerformCounter();
    }

    private void StunBackward(PlayerController pControler)
    {
        if (transform.position.x <= 0f)
        {
            pControler.transform.DOJump(new Vector2(markers[0].transform.position.x, 0f), jumpPower, 1, durationJump).SetEase(Ease.OutQuad);
        }
        else
        {
            pControler.transform.DOJump(new Vector2(markers[1].transform.position.x, 0f), jumpPower, 1, durationJump).SetEase(Ease.OutQuad);
        }
    }
}
