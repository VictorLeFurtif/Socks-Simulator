using UnityEngine;
using UnityEngine.InputSystem;

public class attack : MonoBehaviour
{

    private bool inZone = false;
    [SerializeField] private PlayerInput input;

    private void OnTriggerEnter(Collider pCollider)
    {
        if (pCollider.gameObject.CompareTag("Player"))
        {
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider pCollider)
    {
        if(inZone && pCollider.gameObject.CompareTag("Player"))
        {
            inZone = false;
        }
    }

    public void AttackInput(InputAction.CallbackContext ctx)
    {
        if (inZone)
            PerformAttack(); 
    }

    private void PerformAttack()
    {

    }
}
