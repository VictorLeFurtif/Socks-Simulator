using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private float radius;
    private Vector3 startRaycast;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController enemy;

    private void OnEnable()
    {
        EventManager.attackInput += DetectPlayer;
    }

    void Start()
    {
        radius = spriteRenderer.bounds.extents.x + 0.2f;
    }

    private void DetectPlayer()
    {
        startRaycast = new Vector3(transform.position.x - radius, transform.position.y, transform.position.z);

        RaycastHit2D hit = Physics2D.Raycast(startRaycast, Vector2.left, 2f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && hit.collider.gameObject != gameObject)
            StartAttack();

    }

    private void StartAttack()
    {
        animator.SetTrigger("IsAttacking");
        //TODO hurt animation and counter
    }

    public void OnAttackEnd()
    {
        //TODO envoyer un signal pour stunt
        Debug.Log("dealDamage");
        EventManager.UpdateStunAction?.Invoke(enemy);
    }

}
