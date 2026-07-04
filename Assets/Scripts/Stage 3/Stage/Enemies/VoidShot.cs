using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VoidShot : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float maxLifeTime = 5f;

    [Header("Combat")]
    [SerializeField] private int damage = 20;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;

    private Vector2 direction;
    private Vector2 targetPosition;
    private Collider2D ownerCollider;
    private bool hasHit;

    //--------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //--------------------------------------------------

    private void OnEnable()
    {
        Destroy(gameObject, maxLifeTime);
    }

    //--------------------------------------------------

    public void Initialize(
    Vector2 startPosition,
    Vector2 target)
    {
        hasHit = false;

        transform.position = startPosition;

        targetPosition = target;

        direction =
            (targetPosition - startPosition).normalized;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    //--------------------------------------------------

    private void FixedUpdate()
    {
        rb.MovePosition(
            rb.position +
            direction *
            moveSpeed *
            Time.fixedDeltaTime);
    }

    public void SetOwner(Collider2D owner)
    {
        ownerCollider = owner;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //--------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {


        //------------------------------------
        // Abaikan collider milik Enemy sendiri
        //------------------------------------

        if (ownerCollider != null && other == ownerCollider)
            return;

        //------------------------------------
        // Cari PlayerStats
        //------------------------------------

        PlayerStats player =
            other.GetComponentInParent<PlayerStats>();

        if (player != null)
        {
            hasHit = true;
            

            player.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }

        //------------------------------------
        // Wall
        //------------------------------------

        if (((1 << other.gameObject.layer) & wallLayer) != 0)
        {
            hasHit = true;

            Destroy(gameObject);

            return;
        }
    }

    //--------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawLine(
            transform.position,
            targetPosition);

        Gizmos.DrawSphere(
            targetPosition,
            0.12f);
    }
}