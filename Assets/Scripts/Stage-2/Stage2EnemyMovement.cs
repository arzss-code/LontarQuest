using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage2EnemyMovement : MonoBehaviour
{
    public enum MovementMode { Chase, KeepDistance }

    [Header("Movement Mode")]
    [SerializeField] private MovementMode mode = MovementMode.Chase;

    [Header("General Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRadius = 8f;
    [SerializeField] private float leashRadius = 12f;

    [Header("Physics & Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Chase Settings (Melee)")]
    [SerializeField] private float stopDistance = 1.2f;

    [Header("KeepDistance Settings (Ranged)")]
    [SerializeField] private float preferredDistance = 6f;
    [SerializeField] private float retreatDistance = 3f;
    [SerializeField] private float retreatSpeed = 3.5f;

    [Header("Visual Hover Effect")]
    [Tooltip("Parent dari visual sprite musuh (anak object) untuk animasi hover/melayang")]
    [SerializeField] private Transform visualParent;
    [SerializeField] private float sinusoidalFloatSpeed = 2f;
    [SerializeField] private float sinusoidalFloatAmount = 0.15f;

    // Components
    private Rigidbody2D rb;
    private Collider2D myCollider;
    private Stage2EnemyAnimator animatorScript;
    private Stage2EnemyAttack attackScript;
    private Stage2EnemyStats statsScript;

    // AI State
    private Vector2 startPosition;
    private Transform playerTransform;
    private bool isReturning = false;

    public Transform PlayerTransform => playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        animatorScript = GetComponent<Stage2EnemyAnimator>();
        attackScript = GetComponent<Stage2EnemyAttack>();
        statsScript = GetComponent<Stage2EnemyStats>();
    }

    private void Start()
    {
        // Konfigurasi Rigidbody2D agar sesuai kebutuhan top-down 2D
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Auto-assign layer rintangan ke "Wall" jika belum diatur di Inspector
        if (obstacleLayer.value == 0)
        {
            obstacleLayer = LayerMask.GetMask("Wall");
        }

        startPosition = transform.position;

        // Cari player di scene
        FindPlayer();
    }

    private void Update()
    {
        // Efek melayang sinusoidal untuk musuh melayang (misal: Yaksa)
        if (mode == MovementMode.KeepDistance && visualParent != null && (statsScript == null || !statsScript.IsDead))
        {
            float newY = Mathf.Sin(Time.time * sinusoidalFloatSpeed) * sinusoidalFloatAmount;
            visualParent.localPosition = new Vector3(visualParent.localPosition.x, newY, visualParent.localPosition.z);
        }
    }

    private void FixedUpdate()
    {
        // Berhenti bergerak jika mati
        if (statsScript != null && statsScript.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Jangan bergerak jika sedang menyerang
        if (attackScript != null && attackScript.IsAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            if (animatorScript != null) animatorScript.SetSpeed(0f);

            // Selama menyerang, pastikan tetap menghadap ke player
            if (playerTransform != null && animatorScript != null)
            {
                Vector2 lookDir = ((Vector2)playerTransform.position - rb.position).normalized;
                animatorScript.SetDirection(lookDir);
            }
            return;
        }

        if (playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            if (animatorScript != null) animatorScript.SetSpeed(0f);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceFromStart = Vector2.Distance(transform.position, startPosition);

        Vector2 movementVelocity = Vector2.zero;

        // Logika Batas Leash (Kembali ke Spawn jika terlalu jauh mengejar)
        if (distanceFromStart > leashRadius)
        {
            isReturning = true;
        }

        if (isReturning)
        {
            if (distanceFromStart < 0.2f)
            {
                isReturning = false;
                movementVelocity = Vector2.zero;
            }
            else
            {
                Vector2 returnDir = (startPosition - (Vector2)transform.position).normalized;
                movementVelocity = AvoidObstacles(returnDir) * moveSpeed;
            }
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            // Player terdeteksi
            if (mode == MovementMode.Chase)
            {
                // Mode Melee: Kejar sampai batas stopDistance
                if (distanceToPlayer > stopDistance)
                {
                    Vector2 chaseDir = ((Vector2)playerTransform.position - rb.position).normalized;
                    movementVelocity = AvoidObstacles(chaseDir) * moveSpeed;
                }
                else
                {
                    movementVelocity = Vector2.zero;
                    if (attackScript != null)
                    {
                        attackScript.TryAttack();
                    }
                }
            }
            else if (mode == MovementMode.KeepDistance)
            {
                // Mode Ranged: Menjaga jarak ideal
                if (distanceToPlayer < retreatDistance)
                {
                    // Terlalu dekat -> Mundur lebih cepat
                    Vector2 retreatDir = (rb.position - (Vector2)playerTransform.position).normalized;
                    movementVelocity = AvoidObstacles(retreatDir) * retreatSpeed;
                }
                else if (distanceToPlayer > preferredDistance)
                {
                    // Terlalu jauh -> Dekati
                    Vector2 approachDir = ((Vector2)playerTransform.position - rb.position).normalized;
                    movementVelocity = AvoidObstacles(approachDir) * moveSpeed;
                }
                else
                {
                    // Jarak ideal -> Berhenti dan Serang
                    movementVelocity = Vector2.zero;
                    if (attackScript != null)
                    {
                        attackScript.TryAttack();
                    }
                }
            }
        }
        else
        {
            // Player di luar jangkauan deteksi -> Kembali ke spawn
            if (distanceFromStart > 0.5f)
            {
                Vector2 returnDir = (startPosition - (Vector2)transform.position).normalized;
                movementVelocity = AvoidObstacles(returnDir) * moveSpeed;
            }
            else
            {
                movementVelocity = Vector2.zero;
            }
        }

        // Terapkan kecepatan ke Rigidbody2D
        rb.linearVelocity = movementVelocity;

        // Hubungkan pergerakan fisik ke parameter Animator
        if (animatorScript != null)
        {
            if (movementVelocity.sqrMagnitude > 0.01f)
            {
                animatorScript.SetSpeed(movementVelocity.magnitude);
                animatorScript.SetDirection(movementVelocity.normalized);
            }
            else
            {
                animatorScript.SetSpeed(0f);
                if (playerTransform != null && distanceToPlayer <= detectionRadius)
                {
                    // Saat diam memandang player
                    Vector2 lookDir = ((Vector2)playerTransform.position - rb.position).normalized;
                    animatorScript.SetDirection(lookDir);
                }
            }
        }
    }

    /// <summary>
    /// Menghindari rintangan/dinding dengan membelokkan arah pergerakan agar menyusuri dinding
    /// </summary>
    private Vector2 AvoidObstacles(Vector2 desiredDirection)
    {
        if (desiredDirection.sqrMagnitude < 0.001f) return Vector2.zero;

        // Ambil radius deteksi berdasarkan collider fisik musuh
        float castRadius = 0.35f;
        if (myCollider is CircleCollider2D circleCol)
        {
            castRadius = circleCol.radius * transform.localScale.x;
        }
        else if (myCollider is BoxCollider2D boxCol)
        {
            castRadius = Mathf.Min(boxCol.size.x, boxCol.size.y) * 0.5f * transform.localScale.x;
        }

        // Lakukan CircleCast ke arah pergerakan
        float castDistance = 0.4f;
        RaycastHit2D hit = Physics2D.CircleCast(rb.position, castRadius, desiredDirection, castDistance, obstacleLayer);

        if (hit.collider != null)
        {
            Vector2 hitNormal = hit.normal;

            // Hitung arah slide sejajar dengan dinding
            Vector2 slideDir = desiredDirection - Vector2.Dot(desiredDirection, hitNormal) * hitNormal;

            // Jika tegak lurus (perpendicular), paksa belok 90 derajat ke kiri/kanan normal
            if (slideDir.sqrMagnitude < 0.05f)
            {
                Vector2 tangent1 = new Vector2(-hitNormal.y, hitNormal.x);
                Vector2 tangent2 = new Vector2(hitNormal.y, -hitNormal.x);

                // Pilih tangent yang paling searah dengan arah ke target
                slideDir = Vector2.Dot(tangent1, desiredDirection) > Vector2.Dot(tangent2, desiredDirection) ? tangent1 : tangent2;
            }

            slideDir = slideDir.normalized;

            // Pastikan arah slide yang baru juga tidak menabrak rintangan lain (sudut/pojokan)
            RaycastHit2D slideHit = Physics2D.CircleCast(rb.position, castRadius, slideDir, castDistance, obstacleLayer);
            if (slideHit.collider == null)
            {
                return slideDir;
            }
            else
            {
                // Jika terhalang pojok, coba arah sebaliknya (tangent yang berlawanan)
                Vector2 altSlideDir = -slideDir;
                RaycastHit2D altHit = Physics2D.CircleCast(rb.position, castRadius, altSlideDir, castDistance, obstacleLayer);
                if (altHit.collider == null)
                {
                    return altSlideDir;
                }
            }

            // Jika semua terhalang, berhenti agar tidak bergetar menabrak dinding
            return Vector2.zero;
        }

        return desiredDirection;
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + new Vector3(0, 0.7f, 0);

        // Visualisasi radius deteksi (Merah)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, detectionRadius);

        // Visualisasi radius leash (Hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? (Vector3)startPosition + new Vector3(0, 0.7f, 0) : center, leashRadius);

        if (mode == MovementMode.KeepDistance)
        {
            // Visualisasi jarak mundur (Kuning)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, retreatDistance);

            // Visualisasi jarak ideal (Biru)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, preferredDistance);
        }
        else
        {
            // Visualisasi jarak stop melee
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, stopDistance);
        }
    }
}
