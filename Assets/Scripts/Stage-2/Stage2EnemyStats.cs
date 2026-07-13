using UnityEngine;
using System.Collections;

public class Stage2EnemyStats : MonoBehaviour, IDamageable
{
    [Header("HP Settings")]
    [SerializeField] private int maxHP = 100;
    [SerializeField] private bool isBoss = false;

    [Header("Visual Effects")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    [Header("UI Health Bar")]
    [SerializeField] private EnemyHealthBar healthBar;

    private int currentHP;
    private Color originalColor = Color.white;
    private bool isDead = false;
    private Coroutine flashCoroutine;

    // References to other components to disable on death
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private Animator animator;

    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;
    public bool IsDead => isDead;
    public bool IsBoss => isBoss;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        if (animator == null) animator = GetComponent<Animator>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Auto-detect health bar di child (seperti di HealthBarAnchor) jika kosong
        if (healthBar == null) healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    private void Start()
    {
        currentHP = maxHP;

        if (healthBar != null)
        {
            healthBar.Initialize(maxHP);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        // Spawn floating damage popup (false = enemy damage, i.e., white text)
        DamagePopupManager.Create(transform.position, damage, false);

        // Update health bar if present
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHP);
        }

        // Damage flash visual effect
        if (spriteRenderer != null && gameObject.activeInHierarchy)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashDamageColor());
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamageColor()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;

        // Disable AI components
        var movement = GetComponent<Stage2EnemyMovement>();
        if (movement != null) movement.enabled = false;

        var attack = GetComponent<Stage2EnemyAttack>();
        if (attack != null) attack.enabled = false;

        // Stop movement physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable colliders so player can walk through and projectiles ignore it
        if (colliders != null)
        {
            foreach (var col in colliders)
            {
                if (col != null) col.enabled = false;
            }
        }

        // Trigger Death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
            // Safety timeout: jika event OnDeathEnd dari animator macet atau tidak terpasang,
            // tetap hancurkan objek setelah 2.5 detik agar permainan tidak softlock.
            StartCoroutine(DeathTimeoutSafety(2.5f));
        }
        else
        {
            // Fallback if animator is not found
            OnDeathEnd();
        }
    }

    private IEnumerator DeathTimeoutSafety(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        Debug.LogWarning($"[Stage2EnemyStats] Safety Timeout dipicu untuk {gameObject.name}. Menghancurkan objek secara paksa.");
        OnDeathEnd();
    }

    // Called by Stage2AnimationRelay when the Die animation finishes playing
    public void OnDeathEnd()
    {
        Destroy(gameObject);
    }
}
