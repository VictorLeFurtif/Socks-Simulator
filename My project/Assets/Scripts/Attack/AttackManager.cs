using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private float radius;
    private Vector3 startRaycast;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        EventManager.attackInput += DetectPlayer;
    }

    void Start()
    {
        radius = spriteRenderer.bounds.extents.x + 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void DetectPlayer()
    {
        Debug.Log("here");
        startRaycast = new Vector3(transform.position.x - radius, transform.position.y, transform.position.z);

        RaycastHit2D hit = Physics2D.Raycast(startRaycast, Vector2.left, 2f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && hit.collider.gameObject != gameObject)
            StartAttack();

    }

    private void StartAttack()
    {
        animator.SetTrigger("isAttacking");
        //TODO hurt animation and counter
    }
}
