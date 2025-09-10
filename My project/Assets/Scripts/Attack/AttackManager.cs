using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private float radius;
    private Vector3 startRaycast;

    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        radius = spriteRenderer.bounds.extents.x + 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        startRaycast = new Vector3(transform.position.x - radius, transform.position.y, transform.position.z);

        RaycastHit2D hit = Physics2D.Raycast(startRaycast, Vector2.left, 2f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && hit.collider.gameObject != gameObject)
            return;

        
    }

    private void StartAttack()
    {

    }
}
