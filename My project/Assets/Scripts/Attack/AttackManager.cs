using System.Collections;
using Controller;
using Enum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour
{
    public bool isAttacking;
    private bool isInArea = false;
    public bool canCounter;
    private bool wasCountered;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    //[SerializeField] private PlayerController enemy;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AttackManager ennemyAttack;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Slider playerSlider;
    [SerializeField] private float looseSlider = 2f;
    [SerializeField] private RopeController rp;

    public void DetectPlayer()
    {
        Debug.Log($"bool : isInArea = {isInArea}, isAttacking = {isAttacking}, enemy is attacking = {ennemyAttack.isAttacking}");
        if (isInArea && !isAttacking && !ennemyAttack.isAttacking)
        {
            StartAttack();
        }
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
        Debug.Log("Attacking");
        animator.SetTrigger("IsAttacking");

        //TODO hurt animation
    }

    public void OnAttackEnd()
    {
        if (!wasCountered)
            ennemyAttack.UpdateSlider();

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
        ennemyAttack.UpdateSlider();
        StartCoroutine(ResetAttackState());
    }

    private void UpdateSlider()
    {
        EventManager.UpdateStunAction?.Invoke(playerController);

        if (playerSlider.value - looseSlider > 0)
            playerSlider.value -= looseSlider;
        else
        {
            playerSlider.value = 0;
            playerController.isStunt = true;
            rp.CurrentKoState = KoState.Ko;
        }

    }

}
