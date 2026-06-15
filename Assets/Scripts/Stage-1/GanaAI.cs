using UnityEngine;
using System.Collections;

public class GanaAI : MonoBehaviour
{
    public enum GanaState { Idle, Chase, Charging, Cooldown, Return }

    [Header("State Machine")]
    [SerializeField] private GanaState currentState = GanaState.Idle;
    
    [Header("Patrol Settings")]
    [SerializeField] private float leashRadius = 8f;
    private Vector2 startPosition;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 6f;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat Settings")]
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackRadius = 1.5f;
    
    // Waktu tunggu animasi serangan sampai frame 4 (dikunci di 0.8 detik)
    private readonly float attackHitDelay = 0.8f;
    
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 15;
    
    [Tooltip("Geser posisi lingkaran serangan agar pas di tangan yang menyala")]
    [SerializeField] private Vector2 handHitboxOffset = new Vector2(0.8f, 0f);
    public LayerMask playerLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("Visual Flash")]
    [SerializeField] private float flashDuration = 0.15f;

    [Header("VFX Settings")]
    [Tooltip("Prefab efek visual (misal: debu/slash) yang muncul di lokasi hitbox")]
    [SerializeField] private GameObject attackVFXPrefab;
    [SerializeField] private float vfxLifetime = 0.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip deathSFX; // Masukkan file suara kematian di Inspector

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        anim = GetComponentInChildren<Animator>();
        if (anim == null) anim = GetComponent<Animator>();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // Agar tidak saling terpental secara fisika
        startPosition = transform.position;
        currentHealth = maxHealth;
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            originalColor = Color.white;
        }

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            return;
        }

        if (currentState == GanaState.Charging || currentState == GanaState.Cooldown)
        {
            return;
        }

        // Handle State Transitions
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        float distanceFromStart = Vector2.Distance(transform.position, startPosition);
        
        if (currentState == GanaState.Return)
        {
            if (distanceFromStart < 0.5f)
            {
                currentState = GanaState.Idle;
            }
            return;
        }

        if (distanceFromStart > leashRadius && currentState == GanaState.Chase)
        {
            currentState = GanaState.Return;
            return;
        }
        
        if (distanceToPlayer <= attackDistance)
        {
            StartCoroutine(SmashAttackRoutine());
        }
        else if (distanceToPlayer <= detectionRadius)
        {
            currentState = GanaState.Chase;
        }
        else
        {
            currentState = GanaState.Idle;
        }
    }

    private void FixedUpdate()
    {
        if (currentState == GanaState.Return)
        {
            Vector2 direction = (startPosition - rb.position).normalized;
            Vector2 targetPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
            UpdateAnimator(direction);
            rb.linearVelocity = Vector2.zero;
        }
        else if (currentState == GanaState.Chase && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // Anti-Cornering
            if (distanceToPlayer > stopDistance)
            {
                Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
                Vector2 targetPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(targetPos);
                
                UpdateAnimator(direction);
            }
            else
            {
                UpdateAnimator(Vector2.zero);
            }
            
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(Vector2.zero);
        }
    }

    private IEnumerator SmashAttackRoutine()
    {
        currentState = GanaState.Charging;
        rb.linearVelocity = Vector2.zero;

        Vector2 attackDirection = (playerTransform.position - transform.position).normalized;
        UpdateAnimator(attackDirection);

        if (anim != null)
        {
            anim.SetTrigger("Attack"); 
        }

        // Tunggu sampai animasi mencapai frame pukulan (frame ke-4)
        yield return new WaitForSeconds(attackHitDelay);

        // Agar tidak aneh saat Saka di atas/bawah, hitbox serangan akan selalu diarahkan 
        // ke posisi Saka secara melingkar (360 derajat), menggunakan jarak offset.x
        Vector2 actualHandOffset = attackDirection * Mathf.Abs(handHitboxOffset.x);
        Vector2 smashCenter = (Vector2)transform.position + actualHandOffset;

        // Tampilkan Efek Visual (VFX) di titik hitbox agar pemain bisa melihat serangannya
        if (attackVFXPrefab != null)
        {
            GameObject vfx = Instantiate(attackVFXPrefab, smashCenter, Quaternion.identity);
            
            // Putar efek agar mengarah ke pemain
            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
            vfx.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            Destroy(vfx, vfxLifetime);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(smashCenter, attackRadius, playerLayer);
        foreach(Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats pStats = hit.GetComponent<PlayerStats>();
                if (pStats != null) pStats.TakeDamage(attackDamage);

                // Knockback dihapus agar Saka tidak terpental sesuai permintaan
                break;
            }
        }

        currentState = GanaState.Cooldown;
        yield return new WaitForSeconds(attackCooldown);

        currentState = GanaState.Idle;
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;
        
        // Memunculkan angka damage putih (false = musuh)
        DamagePopupManager.Create(transform.position, damageAmount, false);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamageColor());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamageColor()
    {
        // Berkedip merah hanya saat menerima damage
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        // Memutar Death SFX sebelum objek dihancurkan
        if (deathSFX != null)
        {
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        }
        
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Visualisasi radius leash (area maksimal Gana bisa mengejar)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : (Vector2)transform.position, leashRadius);

        // Visualisasi area hitbox serangan di tangan (Warna Magenta)
        Gizmos.color = Color.magenta;
        Vector2 actualHandOffset;
        if (Application.isPlaying && playerTransform != null && currentState == GanaState.Charging)
        {
            // Tampilkan secara dinamis jika sedang nyerang
            Vector2 attackDirection = (playerTransform.position - transform.position).normalized;
            actualHandOffset = attackDirection * Mathf.Abs(handHitboxOffset.x);
        }
        else
        {
            // Tampilkan default Kiri/Kanan
            float flipMultiplier = 1f;
            if (Application.isPlaying && spriteRenderer != null)
            {
                flipMultiplier = spriteRenderer.flipX ? -1f : 1f;
            }
            actualHandOffset = new Vector2(handHitboxOffset.x * flipMultiplier, handHitboxOffset.y);
        }
        
        Gizmos.DrawWireSphere((Vector2)transform.position + actualHandOffset, attackRadius);
    }

    private void UpdateAnimator(Vector2 dir)
    {
        if (anim != null)
        {
            if (dir.magnitude > 0)
            {
                if (dir.x < 0)
                {
                    spriteRenderer.flipX = true; // Menghadap Kiri
                }
                else if (dir.x > 0)
                {
                    spriteRenderer.flipX = false; // Menghadap Kanan
                }

                anim.SetFloat("Speed", dir.magnitude);
            }
            else
            {
                anim.SetFloat("Speed", 0f);
            }
        }
    }
}