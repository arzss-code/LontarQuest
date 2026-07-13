using UnityEngine;
using System.Collections;

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

    [Header("Knockback Settings (Khusus Dwarapala)")]
    [SerializeField] private bool useHeavyKnockback = true;
    [SerializeField] private float heavyKnockbackForce = 22f;
    [SerializeField] private float heavyKnockbackDuration = 0.35f;

    [Header("Ranged Settings (Yaksa)")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Components
    private Stage2EnemyAnimator animatorScript;
    private Stage2EnemyMovement movementScript;
    private Stage2EnemyStats statsScript;

    // Internal State
    private bool isAttacking = false;
    private float cooldownTimer = 0f;
    private Coroutine attackTimeoutCoroutine;

    public bool IsAttacking => isAttacking;
    public float AttackRange => attackRange;
    public float MaxEffectiveRange
    {
        get
        {
            if (attackType == AttackType.MeleeAoE)
            {
                // Jangkauan fisik melee tidak boleh melebihi total jangkauan hitbox (offset + radius)
                float physicalReach = Mathf.Abs(meleeHitboxOffset.x) + meleeRadius;
                return Mathf.Min(attackRange, physicalReach);
            }
            return attackRange;
        }
    }

    private void Awake()
    {
        animatorScript = GetComponent<Stage2EnemyAnimator>();
        movementScript = GetComponent<Stage2EnemyMovement>();
        statsScript = GetComponent<Stage2EnemyStats>();
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
        if (statsScript != null && statsScript.IsDead) return;
        if (isAttacking || cooldownTimer > 0f) return;
        if (movementScript == null || movementScript.PlayerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, movementScript.PlayerTransform.position);
        if (distanceToPlayer <= MaxEffectiveRange)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;

        // Update arah hadap ke player sesaat SEBELUM memicu trigger animasi serang
        if (movementScript != null && movementScript.PlayerTransform != null && animatorScript != null)
        {
            Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;
            animatorScript.SetDirection(lookDir);
        }

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

            // Safety Timeout. Jika Unity Animation Event OnAttackEnd macet/gagal dipanggil
            // paksa reset status serang setelah jeda.
            if (attackTimeoutCoroutine != null)
            {
                StopCoroutine(attackTimeoutCoroutine);
            }
            attackTimeoutCoroutine = StartCoroutine(AttackTimeoutSafety(1.5f));
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

    private IEnumerator AttackTimeoutSafety(float timeout)
    {
        yield return new WaitForSeconds(timeout);
        if (isAttacking)
        {
            Debug.LogWarning($"[Stage2EnemyAttack] Peringatan: Serangan di {gameObject.name} terkena Safety Timeout! OnAttackEnd tidak dipicu dari Animator. Melakukan pembersihan state secara paksa.");
            OnAttackEnd();
        }
    }

    /// <summary>
    /// Eksekusi Hit Melee (Dipanggil oleh Animation Event OnAttackHit via Relay)
    /// </summary>
    public void OnAttackHit()
    {
        if (statsScript != null && statsScript.IsDead) return;
        if (movementScript == null || movementScript.PlayerTransform == null) return;

        // Hitung arah hadap ke player saat hantaman terjadi
        Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;

        // Proyeksikan hitbox melee melingkar di depan musuh (mulai dari pusat tubuh Y=0.7)
        Vector2 bodyCenter = (Vector2)transform.position + new Vector2(0f, 0.7f);
        Vector2 actualOffset = lookDir * Mathf.Abs(meleeHitboxOffset.x);
        Vector2 smashCenter = bodyCenter + actualOffset;

        // Spawn visual effect hantaman jika di-assign
        if (attackVFXPrefab != null)
        {
            GameObject vfx = Instantiate(attackVFXPrefab, smashCenter, Quaternion.identity);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            vfx.transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(vfx, vfxLifetime);
        }

        // Fallback LayerMask. Jika targetLayer di Inspector kosong (Nothing / 0),
        // cari di semua layer lalu filter menggunakan Tag "Player" agar serangan tetap mendeteksi Saka.
        Collider2D[] hits;
        if (targetLayer.value != 0)
        {
            hits = Physics2D.OverlapCircleAll(smashCenter, meleeRadius, targetLayer);
        }
        else
        {
            hits = Physics2D.OverlapCircleAll(smashCenter, meleeRadius);
        }

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

                // Berikan efek Knockback ke pemain
                PlayerController pController = hit.GetComponent<PlayerController>();
                Rigidbody2D pRb = hit.GetComponent<Rigidbody2D>();
                if (pController != null && pRb != null)
                {
                    if (useHeavyKnockback)
                    {
                        // Jalankan coroutine di player (pController) agar tidak terputus jika musuh dihancurkan (Destroy) saat mati
                        pController.StartCoroutine(ApplyCustomKnockback(pController, pRb, lookDir, heavyKnockbackForce, heavyKnockbackDuration));
                    }
                    else
                    {
                        // Gunakan knockback default bawaan player
                        pController.ApplyKnockback(lookDir);
                    }
                }
                break; // Melee hanya melukai player sekali per smash
            }
        }
    }

    /// <summary>
    /// Coroutine knockback custom (Mengontrol gerakan Rigidbody player secara langsung tanpa menyentuh script global)
    /// </summary>
    private IEnumerator ApplyCustomKnockback(PlayerController player, Rigidbody2D playerRb, Vector2 direction, float force, float duration)
    {
        // Nonaktifkan komponen pergerakan player agar tidak menimpa linearVelocity
        player.enabled = false;

        Vector2 knockbackVel = direction.normalized * force;
        float timer = duration;

        while (timer > 0f)
        {
            if (playerRb == null) break;
            playerRb.linearVelocity = knockbackVel;
            timer -= Time.deltaTime;
            yield return null;
        }

        if (player != null && playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            player.enabled = true; // Aktifkan kembali kontrol player setelah durasi selesai
        }
    }

    /// <summary>
    /// Spawn Proyektil Panah (Dipanggil oleh Animation Event OnShootProjectile via Relay)
    /// </summary>
    public void OnShootProjectile()
    {
        if (statsScript != null && statsScript.IsDead) return;
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
        if (attackTimeoutCoroutine != null)
        {
            StopCoroutine(attackTimeoutCoroutine);
            attackTimeoutCoroutine = null;
        }

        isAttacking = false;
        cooldownTimer = attackCooldown; // Mulai cooldown setelah animasi selesai
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + new Vector3(0, 0.7f, 0);

        // Visualisasi jarak jangkauan serang (Cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, attackRange);

        // Visualisasi batas jangkauan efektif (Kuning) jika berbeda dengan jangkauan dasar
        float maxRange = MaxEffectiveRange;
        if (Mathf.Abs(maxRange - attackRange) > 0.01f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, maxRange);
        }

        if (attackType == AttackType.MeleeAoE)
        {
            // Tampilkan visualisasi area hitbox melee di Inspector
            Gizmos.color = Color.magenta;
            Vector2 bodyCenter = (Vector2)transform.position + new Vector2(0f, 0.7f);
            Vector2 actualOffset = Vector2.right * meleeHitboxOffset.x;
            if (Application.isPlaying && movementScript != null && movementScript.PlayerTransform != null)
            {
                Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;
                actualOffset = lookDir * Mathf.Abs(meleeHitboxOffset.x);
            }
            Gizmos.DrawWireSphere(bodyCenter + actualOffset, meleeRadius);
        }
    }
}
