using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnergyArrow : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private float homingStrength = 1.5f;
    [SerializeField] private float maxHomingAngle = 30f; // Batas belokan per detik (dalam derajat)

    [Header("Visual References")]
    [SerializeField] private TrailRenderer trailRenderer;

    private int damage;
    private Transform target;
    private float spawnTime;
    private Rigidbody2D rb;
    private bool isInitialized = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Kinematic agar tidak terpengaruh gravitasi atau hambatan fisik lainnya
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Inisialisasi properti proyektil saat di-spawn oleh Yaksa
    /// </summary>
    public void Init(Transform playerTarget, int arrowDamage)
    {
        damage = arrowDamage;
        spawnTime = Time.time;

        // Cari AimPoint di player untuk sasaran tembak yang lebih presisi
        Transform aimPoint = playerTarget.Find("AimPoint");
        target = aimPoint != null ? aimPoint : playerTarget;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Rotasi awal menghadap ke player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        if (target == null)
        {
            // Terbang lurus jika target hilang
            rb.linearVelocity = transform.right * speed;
            return;
        }

        float elapsedTime = Time.time - spawnTime;
        // Hanya homing selama 60% pertama dari lifetime proyektil
        bool shouldHoming = elapsedTime < (lifetime * 0.6f);

        if (shouldHoming)
        {
            Vector2 targetDirection = ((Vector2)target.position - rb.position).normalized;
            Vector2 currentDirection = rb.linearVelocity.normalized;

            // Hitung sudut antara arah terbang saat ini dengan target
            float angleToTarget = Vector2.SignedAngle(currentDirection, targetDirection);
            
            // Batasi laju rotasi maksimum per detik
            float maxRotateAngle = maxHomingAngle * homingStrength * Time.fixedDeltaTime;
            float clampAngle = Mathf.Clamp(angleToTarget, -maxRotateAngle, maxRotateAngle);

            // Putar vektor arah terbang
            Vector2 newDirection = Quaternion.Euler(0, 0, clampAngle) * currentDirection;
            rb.linearVelocity = newDirection.normalized * speed;
        }
        else
        {
            // Terbang lurus menggunakan arah terakhir
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }

        // Putar visual sprite mengikuti arah velocity
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit Player
        if (other.CompareTag("Player"))
        {
            PlayerStats pStats = other.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Hit Tembok / Obstacle
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.CompareTag("Wall") || other.gameObject.name.Contains("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Polishing: Detach Trail Renderer agar memudar alami di world space daripada terpotong instan
        if (trailRenderer != null)
        {
            GameObject trailObj = trailRenderer.gameObject;
            trailObj.transform.SetParent(null);
            
            // Hancurkan objek sisa trail setelah durasi trail selesai memudar
            Destroy(trailObj, trailRenderer.time);
        }
    }
}
