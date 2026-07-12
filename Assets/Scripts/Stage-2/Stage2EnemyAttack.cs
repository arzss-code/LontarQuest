using UnityEngine;

public class Stage2EnemyAttack : MonoBehaviour
{
    public enum AttackType { MeleeAoE, RangedProjectile }

    [Header("Attack Configuration")]
    [SerializeField] private AttackType attackType = AttackType.MeleeAoE;
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Melee AoE Settings (Dwarapala)")]
    [SerializeField] private float meleeRadius = 1.5f;
    [SerializeField] private Vector2 meleeHitboxOffset = new Vector2(0.8f, 0f);
    [SerializeField] private GameObject attackVFXPrefab;
    [SerializeField] private float vfxLifetime = 0.5f;

    [Header("Ranged Settings (Yaksa)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Components
    private Stage2EnemyAnimator animatorScript;
    private Stage2EnemyMovement movementScript;

    // Internal State
    private bool isAttacking = false;
    private float cooldownTimer = 0f;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animatorScript = GetComponent<Stage2EnemyAnimator>();
        movementScript = GetComponent<Stage2EnemyMovement>();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Memicu proses serang jika cooldown siap dan target dalam jangkauan
    /// </summary>
    public void TryAttack()
    {
        if (isAttacking || cooldownTimer > 0f) return;
        if (movementScript == null || movementScript.PlayerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movementScript.PlayerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        if (animatorScript != null)
        {
            if (attackType == AttackType.MeleeAoE)
            {
                animatorScript.TriggerAttack();
            }
            else
            {
                animatorScript.TriggerShoot();
            }
        }
        else
        {
            // Fallback jika Animator tidak ada
            if (attackType == AttackType.MeleeAoE)
            {
                OnAttackHit();
            }
            else
            {
                OnShootProjectile();
            }
            OnAttackEnd();
        }
    }

    /// <summary>
    /// Eksekusi Hit Melee (Dipanggil oleh Animation Event OnAttackHit via Relay)
    /// </summary>
    public void OnAttackHit()
    {
        if (movementScript == null || movementScript.PlayerTransform == null) return;

        // Hitung arah hadap ke player saat hantaman terjadi
        Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;

        // Proyeksikan hitbox melee melingkar di depan musuh
        Vector2 actualOffset = lookDir * Mathf.Abs(meleeHitboxOffset.x);
        Vector2 smashCenter = (Vector2)transform.position + actualOffset;

        // Spawn visual effect hantaman jika di-assign
        if (attackVFXPrefab != null)
        {
            GameObject vfx = Instantiate(attackVFXPrefab, smashCenter, Quaternion.identity);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            vfx.transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(vfx, vfxLifetime);
        }

        // Deteksi tabrakan
        Collider2D[] hits = Physics2D.OverlapCircleAll(smashCenter, meleeRadius, targetLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // Kurangi darah pemain
                PlayerStats pStats = hit.GetComponent<PlayerStats>();
                if (pStats != null)
                {
                    pStats.TakeDamage(damage);
                }

                // Berikan efek Knockback ke pemain menjauh dari arah serangan
                PlayerController pController = hit.GetComponent<PlayerController>();
                if (pController != null)
                {
                    pController.ApplyKnockback(lookDir);
                }
                break; // Melee hanya melukai player sekali per smash
            }
        }
    }

    /// <summary>
    /// Spawn Proyektil Panah (Dipanggil oleh Animation Event OnShootProjectile via Relay)
    /// </summary>
    public void OnShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;
        if (movementScript == null || movementScript.PlayerTransform == null) return;

        // Spawn panah
        GameObject arrowObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnergyArrow arrow = arrowObj.GetComponent<EnergyArrow>();

        if (arrow != null)
        {
            // Kirim target dan stat ke proyektil
            arrow.Init(movementScript.PlayerTransform, damage);
        }
    }

    /// <summary>
    /// Akhir Animasi Serangan (Dipanggil oleh Animation Event OnAttackEnd via Relay)
    /// </summary>
    public void OnAttackEnd()
    {
        isAttacking = false;
        cooldownTimer = attackCooldown; // Mulai cooldown setelah animasi selesai
    }

    private void OnDrawGizmosSelected()
    {
        // Visualisasi jarak jangkauan serang (Cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (attackType == AttackType.MeleeAoE)
        {
            // Tampilkan visualisasi area hitbox melee di Inspector
            Gizmos.color = Color.magenta;
            Vector2 actualOffset = Vector2.right * meleeHitboxOffset.x;
            if (Application.isPlaying && movementScript != null && movementScript.PlayerTransform != null)
            {
                Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;
                actualOffset = lookDir * Mathf.Abs(meleeHitboxOffset.x);
            }
            Gizmos.DrawWireSphere((Vector2)transform.position + actualOffset, meleeRadius);
        }
    }
}
