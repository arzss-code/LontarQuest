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

        startPosition = transform.position;

        FindPlayer();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            FindPlayer();
        }

        // Efek hover/melayang sinusoidal untuk musuh melayang (misal: Yaksa)
        if (mode == MovementMode.KeepDistance && visualParent != null && (statsScript == null || !statsScript.IsDead))
        {
            float newY = Mathf.Sin(Time.time * sinusoidalFloatSpeed) * sinusoidalFloatAmount;
            visualParent.localPosition = new Vector3(visualParent.localPosition.x, newY, visualParent.localPosition.z);
        }
    }

    private void FixedUpdate()
    {
        // Jangan bergerak jika mati
        if (statsScript != null && statsScript.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            if (animatorScript != null) animatorScript.SetSpeed(0f);
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
                movementVelocity = returnDir * moveSpeed;
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
                    movementVelocity = chaseDir * moveSpeed;
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
                    movementVelocity = retreatDir * retreatSpeed;
                }
                else if (distanceToPlayer > preferredDistance)
                {
                    // Terlalu jauh -> Dekati
                    Vector2 approachDir = ((Vector2)playerTransform.position - rb.position).normalized;
                    movementVelocity = approachDir * moveSpeed;
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
                movementVelocity = returnDir * moveSpeed;
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

    private void FindPlayer()
    {
        PlayerController playerObj = Object.FindFirstObjectByType<PlayerController>();
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi radius deteksi (Merah)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Visualisasi radius leash (Hijau)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transform.position, leashRadius);

        if (mode == MovementMode.KeepDistance)
        {
            // Visualisasi jarak mundur (Kuning)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, retreatDistance);

            // Visualisasi jarak ideal (Biru)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, preferredDistance);
        }
        else
        {
            // Visualisasi jarak stop melee
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }
    }
}
