using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Stage2EnemyMovement : MonoBehaviour
{
    public enum MovementMode { Chase, KeepDistance }

    [Header("Movement Mode")]
    [SerializeField] private MovementMode mode = MovementMode.Chase;

    public enum LeashConstraintType { Radius, Box }

    [Header("General Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRadius = 8f;

    [Header("Leash Settings (Batas Pergerakan)")]
    [SerializeField] private LeashConstraintType leashType = LeashConstraintType.Radius;
    [SerializeField] private float leashRadius = 12f;
    [SerializeField] private Vector2 leashBoxSize = new Vector2(10f, 10f);
    [SerializeField] private Vector2 leashOffset = Vector2.zero;

    [Header("Physics & Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Chase Settings (Melee)")]
    [SerializeField] private Vector2 stopDistance = new Vector2(1.2f, 1.2f);

    public float GetStopDistance(Vector2 direction)
    {
        float xProj = Mathf.Abs(direction.x);
        float yProj = Mathf.Abs(direction.y);
        if (xProj < 0.001f && yProj < 0.001f) return stopDistance.x;

        float a = stopDistance.x;
        float b = stopDistance.y;
        float denom = (xProj / a) * (xProj / a) + (yProj / b) * (yProj / b);
        return denom > 0.001f ? 1f / Mathf.Sqrt(denom) : a;
    }

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
        // Gunakan Kinematic agar musuh tidak dapat didorong (terlempar) secara fisika
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Auto-assign layer rintangan ke "Wall" jika belum diatur di Inspector
        if (obstacleLayer.value == 0)
        {
            obstacleLayer = LayerMask.GetMask("Wall", "Obstacles", "Environment");
            if (obstacleLayer.value == 0) obstacleLayer = LayerMask.GetMask("Default"); // Fallback ekstrim
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
            return;
        }

        if (playerTransform == null)
        {
            FindPlayer();
            rb.linearVelocity = Vector2.zero;
            if (animatorScript != null) animatorScript.SetSpeed(0f);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceFromStart = Vector2.Distance(transform.position, startPosition);
        Vector2 leashCenter = startPosition + leashOffset;

        Vector2 movementVelocity = Vector2.zero;

        // Logika Batas Leash (Kembali ke Spawn jika melampaui batas pergerakan)
        bool outOfLeash = false;
        if (leashType == LeashConstraintType.Radius)
        {
            float distanceFromLeashCenter = Vector2.Distance(transform.position, leashCenter);
            outOfLeash = distanceFromLeashCenter > leashRadius;
        }
        else if (leashType == LeashConstraintType.Box)
        {
            float diffX = Mathf.Abs(transform.position.x - leashCenter.x);
            float diffY = Mathf.Abs(transform.position.y - leashCenter.y);
            outOfLeash = diffX > leashBoxSize.x / 2f || diffY > leashBoxSize.y / 2f;
        }

        if (outOfLeash)
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
                // Jangkauan serang dinamis berdasarkan script attack, fallback ke stopDistance jika null
                Vector2 dirToPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
                float effectiveStopDistance = (attackScript != null) ? attackScript.GetMaxEffectiveRange(dirToPlayer) : GetStopDistance(dirToPlayer);

                // Mode Melee: Kejar sampai batas effectiveStopDistance
                if (distanceToPlayer > effectiveStopDistance)
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
                // Mode Ranged: Menjaga jarak ideal & Prioritas Serang
                bool tryShoot = false;
                
                // Jika cooldown siap dan player di dalam jangkauan serang, prioritas berhenti untuk serang
                if (attackScript != null && attackScript.IsCooldownReady)
                {
                    Vector2 dirToPlayer = ((Vector2)playerTransform.position - rb.position).normalized;
                    if (distanceToPlayer <= attackScript.GetMaxEffectiveRange(dirToPlayer))
                    {
                        tryShoot = true;
                    }
                }

                if (tryShoot)
                {
                    movementVelocity = Vector2.zero;
                    attackScript.TryAttack();
                }
                else if (distanceToPlayer < retreatDistance)
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
                    // Jarak ideal tetapi cooldown belum siap -> Berhenti memandang player
                    movementVelocity = Vector2.zero;
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

        // Ambil radius deteksi berdasarkan collider fisik musuh, tambahkan buffer agar tidak tembus
        float castRadius = 0.5f;
        if (myCollider is CircleCollider2D circleCol)
        {
            castRadius = (circleCol.radius * Mathf.Abs(transform.localScale.x)) + 0.15f;
        }
        else if (myCollider is BoxCollider2D boxCol)
        {
            castRadius = (Mathf.Max(boxCol.size.x, boxCol.size.y) * 0.5f * Mathf.Abs(transform.localScale.x)) + 0.15f;
        }

        // Lakukan CircleCast ke arah pergerakan dengan jarak antisipasi kecepatan
        float distanceToPlayer = playerTransform != null ? Vector2.Distance(transform.position, playerTransform.position) : 999f;
        float speed = mode == MovementMode.KeepDistance && isReturning == false && distanceToPlayer < retreatDistance ? retreatSpeed : moveSpeed;
        float castDistance = 0.4f + (speed * Time.fixedDeltaTime);
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

        // Visualisasi batas leash (Hijau)
        Gizmos.color = Color.green;
        Vector3 baseCenter = Application.isPlaying ? (Vector3)startPosition : transform.position;
        Vector3 leashCenter = baseCenter + new Vector3(leashOffset.x, leashOffset.y + 0.7f, 0f);
        if (leashType == LeashConstraintType.Radius)
        {
            Gizmos.DrawWireSphere(leashCenter, leashRadius);
        }
        else if (leashType == LeashConstraintType.Box)
        {
            Gizmos.DrawWireCube(leashCenter, new Vector3(leashBoxSize.x, leashBoxSize.y, 0f));
        }

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
            if (attackScript != null)
            {
                Vector2 effectiveRangeVector = new Vector2(attackScript.GetMaxEffectiveRange(Vector2.right), attackScript.GetMaxEffectiveRange(Vector2.up));
                DrawGizmosEllipse(center, effectiveRangeVector);
            }
            else
            {
                DrawGizmosEllipse(center, stopDistance);
            }
        }
    }

    private void DrawGizmosEllipse(Vector3 center, Vector2 radius)
    {
        Vector3 prevPoint = center + new Vector3(radius.x, 0f, 0f);
        int segments = 36;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * 2 * Mathf.PI / segments;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(angle) * radius.x, Mathf.Sin(angle) * radius.y, 0f);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
}
