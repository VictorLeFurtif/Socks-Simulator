using Unity.VisualScripting;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private float radius;
    private Vector3 startRaycast;
    private bool isInArea = false;
    public bool canCounter;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController enemy;
    [SerializeField] private PlayerController player;
    [SerializeField] private AttackManager ennemyAttack;

    private void OnEnable()
    {
        EventManager.attackInput += DetectPlayer;
    }

    void Start()
    {
        radius = spriteRenderer.bounds.extents.x + 0.2f;
    }

    public void DetectPlayer()
    {
      if(isInArea)
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
        animator.SetTrigger("IsAttacking");
        //TODO hurt animation and counter
    }

    public void OnAttackEnd()
    {
        Debug.Log("dealDamage");
        if(canCounter)
            return;
        EventManager.UpdateStunAction?.Invoke(enemy);
    }

    public void SetCounterPossible() => canCounter = !canCounter ;

    public void PerformCounter()
    {
        if (ennemyAttack.canCounter)
        {
            Debug.Log("countering");

            animator.SetTrigger("IsCounter");
            ennemyAttack.SetCounterPossible();
        }
    }

}
