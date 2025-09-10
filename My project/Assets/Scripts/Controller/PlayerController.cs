using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveDirection = new Vector2();
    [SerializeField] private int speed = 10;

    private void FixedUpdate()
    {
        transform.position += new Vector3(moveDirection.x, 0f, 0f) * Time.fixedDeltaTime * speed;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            moveDirection = ctx.ReadValue<Vector2>();
        }
    }

    public void Attack()
    {

    }
}
