using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VoidShot : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.5f;
    [SerializeField] private float maxLifeTime = 3.5f;

    [Header("Combat")]
    [SerializeField] private int damage = 24;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;

    private Vector2 direction;
    private Vector2 targetPosition;
    private GameObject owner;
    private bool hasHit;

    //--------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //--------------------------------------------------

    private void OnEnable()
    {
        hasHit = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

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

    public void SetOwner(GameObject obj)
    {
        owner = obj;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    //--------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        //------------------------------------
        // Abaikan semua Trigger
        //------------------------------------

        if (other.isTrigger)
            return;

        //------------------------------------
        // Abaikan semua collider milik Owner
        //------------------------------------

        if (owner != null)
        {
            if (other.transform.root.gameObject == owner)
                return;
        }

        //------------------------------------
        // Cari Object yang bisa menerima Damage
        //------------------------------------

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            hasHit = true;

            damageable.TakeDamage(damage);

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