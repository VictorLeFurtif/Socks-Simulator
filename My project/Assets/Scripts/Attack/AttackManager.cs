using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public bool isAttacking;
    private bool isInArea = false;
    public bool canCounter;
    private bool wasCountered;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController enemy;
    [SerializeField] private AttackManager ennemyAttack;
    [SerializeField] private float attackCooldown = 0.5f;

    public void DetectPlayer()
    {
        if (isInArea && !isAttacking && !ennemyAttack.isAttacking)
            StartAttack();
    }

    private void OnTriggerEnter2D(Collider2D pCollider)
    {
        if (pCollider.gameObject.CompareTag("Player") && !isInArea)
            isInArea = true;
    }

    private void OnTriggerExit2D(Collider2D pCollider)
    {
        if (pCollider.gameObject.CompareTag("Player"))
            isInArea = false;
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetTrigger("IsAttacking");

        //TODO hurt animation
    }

    public void OnAttackEnd()
    {
        if (!wasCountered)
            EventManager.UpdateStunAction?.Invoke(enemy);

        StartCoroutine(ResetAttackState());
    }

    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        canCounter = false;
        wasCountered = false;

    }

    public void SetCounterPossible() => canCounter = !canCounter;

    public void PerformCounter()
    {
        if (ennemyAttack.canCounter && !wasCountered)
        {
            ennemyAttack.InterruptAttack();

            wasCountered = true;
            canCounter = false;
            animator.SetTrigger("IsCounter");
        }
    }

    public void InterruptAttack()
    {
        if (isAttacking)
        {
            isAttacking = false;
            canCounter = false;
            wasCountered = true;

            animator.ResetTrigger("IsAttacking");
        }
    }


    public void OnCounterEnd()
    {
        StartCoroutine(ResetAttackState());
    }

}
