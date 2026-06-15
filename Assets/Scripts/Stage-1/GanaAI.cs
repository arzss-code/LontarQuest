    using UnityEngine;
using System.Collections;

public class GanaAI : MonoBehaviour
{
    public enum GanaState { Idle, Chase, Charging, Cooldown }

    [Header("State Machine")]
    [SerializeField] private GanaState currentState = GanaState.Idle;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 6f;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    [Header("Combat Settings")]
    [SerializeField] private float stopDistance = 1.2f;
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float chargeTime = 0.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private int attackDamage = 15;
    public LayerMask playerLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("Visual Flash")]
    [SerializeField] private float flashDuration = 0.15f;

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
            // Fallback: try to find player if they were spawned or missing initially
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
        if (currentState == GanaState.Chase && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // Anti-Cornering: Hanya bergerak maju jika jarak masih lebih besar dari stopDistance
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
            
            rb.linearVelocity = Vector2.zero; // Prevent persistent slide/momentum from other forces
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

        // Fase Ancang-ancang (Berkedip Merah sebagai Telegraph)
        float timer = 0;
        bool isRed = false;
        while (timer < chargeTime)
        {
            spriteRenderer.color = isRed ? originalColor : Color.red;
            isRed = !isRed;
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        spriteRenderer.color = originalColor;

        if (anim != null)
        {
            anim.SetTrigger("Attack"); // Jika ada parameter "Attack" di Animator
        }

        Vector2 smashCenter = (Vector2)transform.position + attackDirection * (attackRadius * 0.75f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(smashCenter, attackRadius, playerLayer);
        foreach(Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerStats pStats = hit.GetComponent<PlayerStats>();
                if (pStats != null) pStats.TakeDamage(attackDamage);

                PlayerController pCtrl = hit.GetComponent<PlayerController>();
                if (pCtrl != null)
                {
                    Vector2 kbDir = (hit.transform.position - transform.position).normalized;
                    pCtrl.ApplyKnockback(kbDir);
                }
                break;
            }
        }

        currentState = GanaState.Cooldown;
        yield return new WaitForSeconds(attackCooldown);

        currentState = GanaState.Idle;
    }



    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
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
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        // Add optional particles, sounds, or visual feedback here in the future
        Destroy(gameObject);
    }

    // Helper for visualising detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

    private void UpdateAnimator(Vector2 dir)
    {
        if (anim != null)
        {
            if (dir.magnitude > 0)
            {
                // Fokus Kiri/Kanan: Memutar sprite berdasarkan arah sumbu X
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
