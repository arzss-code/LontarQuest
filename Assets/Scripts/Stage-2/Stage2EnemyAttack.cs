using UnityEngine;
using System.Collections;

public class Stage2EnemyAttack : MonoBehaviour
{
    public enum AttackType { MeleeAoE, RangedProjectile }

    [Header("Attack Configuration")]
    [SerializeField] private AttackType attackType = AttackType.MeleeAoE;
    [SerializeField] private int damage = 15;
    [SerializeField] private Vector2 attackRange = new Vector2(1.5f, 1.5f);
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Vector2 bodyCenterOffset = new Vector2(0f, 0.7f);

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
    [SerializeField] private float aimDuration = 0.5f;
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
    private LineRenderer aimLineRenderer;
    private Vector2 lockedDirection;

    public bool IsAttacking => isAttacking;
    public bool IsCooldownReady => cooldownTimer <= 0f && !isAttacking;
    public float AttackRange => attackRange.x;
    public float MaxEffectiveRange => GetMaxEffectiveRange(Vector2.right);

    public float GetAttackRange(Vector2 direction)
    {
        float xProj = Mathf.Abs(direction.x);
        float yProj = Mathf.Abs(direction.y);
        if (xProj < 0.001f && yProj < 0.001f) return attackRange.x;

        float a = attackRange.x;
        float b = attackRange.y;
        float denom = (xProj / a) * (xProj / a) + (yProj / b) * (yProj / b);
        return denom > 0.001f ? 1f / Mathf.Sqrt(denom) : a;
    }

    public float GetMaxEffectiveRange(Vector2 direction)
    {
        if (attackType == AttackType.MeleeAoE)
        {
            // Jangkauan fisik melee tidak boleh melebihi total jangkauan hitbox (offset + radius)
            float targetOffsetX = direction.x * meleeHitboxOffset.x;
            float targetOffsetY = direction.y * meleeHitboxOffset.y;
            float physicalReach = Mathf.Sqrt(targetOffsetX * targetOffsetX + targetOffsetY * targetOffsetY) + meleeRadius;
            return Mathf.Min(GetAttackRange(direction), physicalReach);
        }
        return GetAttackRange(direction);
    }

    private void Awake()
    {
        animatorScript = GetComponent<Stage2EnemyAnimator>();
        movementScript = GetComponent<Stage2EnemyMovement>();
        statsScript = GetComponent<Stage2EnemyStats>();
    }

    private void Start()
    {
        SetupAimLine();
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void SetupAimLine()
    {
        if (aimLineRenderer == null)
        {
            aimLineRenderer = GetComponent<LineRenderer>();
            if (aimLineRenderer == null)
            {
                aimLineRenderer = gameObject.AddComponent<LineRenderer>();
            }
        }

        if (aimLineRenderer != null)
        {
            aimLineRenderer.startWidth = 0.05f;
            aimLineRenderer.endWidth = 0.05f;
            aimLineRenderer.positionCount = 2;
            aimLineRenderer.useWorldSpace = true;
            aimLineRenderer.enabled = false;
            
            // Atur material default agar tidak berwarna magenta
            aimLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            
            // Warna biru semi transparan default
            Color aimColor = new Color(0f, 0.5f, 1f, 0.4f);
            aimLineRenderer.startColor = aimColor;
            aimLineRenderer.endColor = aimColor;
            
            aimLineRenderer.sortingLayerName = "Default";
            aimLineRenderer.sortingOrder = 10;
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

        Vector2 dirToPlayer = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, movementScript.PlayerTransform.position);
        if (distanceToPlayer <= GetMaxEffectiveRange(dirToPlayer))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        if (attackType == AttackType.RangedProjectile)
        {
            StartCoroutine(AimAndShootRoutine());
        }
        else
        {
            ExecuteMeleeAttack();
        }
    }

    private void ExecuteMeleeAttack()
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
            animatorScript.TriggerAttack();

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
            OnAttackHit();
            OnAttackEnd();
        }
    }

    private IEnumerator AimAndShootRoutine()
    {
        isAttacking = true;

        if (aimLineRenderer != null)
        {
            aimLineRenderer.enabled = true;
            Color aimColor = new Color(0f, 0.5f, 1f, 0.4f);
            aimLineRenderer.startColor = aimColor;
            aimLineRenderer.endColor = aimColor;
        }

        float trackingDuration = aimDuration * 0.7f;
        float lockDuration = aimDuration * 0.3f;
        float elapsed = 0f;

        // FASE 1: Tracking (Mengikuti pergerakan player)
        while (elapsed < trackingDuration)
        {
            if (statsScript != null && statsScript.IsDead)
            {
                if (aimLineRenderer != null) aimLineRenderer.enabled = false;
                yield break;
            }

            if (movementScript != null && movementScript.PlayerTransform != null)
            {
                Vector3 startPos = firePoint != null ? firePoint.position : transform.position;
                Vector3 playerPos = movementScript.PlayerTransform.position;
                
                Transform aimPoint = movementScript.PlayerTransform.Find("AimPoint");
                if (aimPoint != null) playerPos = aimPoint.position;

                lockedDirection = ((Vector2)playerPos - (Vector2)startPos).normalized;

                if (animatorScript != null)
                {
                    animatorScript.SetDirection(lockedDirection);
                }

                if (aimLineRenderer != null)
                {
                    aimLineRenderer.SetPosition(0, startPos);
                    aimLineRenderer.SetPosition(1, startPos + (Vector3)lockedDirection * 20f);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // FASE 2: Locked (Arah mengunci, lintasan memerah dan berkedip cepat)
        if (aimLineRenderer != null)
        {
            Color lockColor = new Color(1f, 0f, 0f, 0.8f);
            aimLineRenderer.startColor = lockColor;
            aimLineRenderer.endColor = lockColor;
        }

        elapsed = 0f;
        while (elapsed < lockDuration)
        {
            if (statsScript != null && statsScript.IsDead)
            {
                if (aimLineRenderer != null) aimLineRenderer.enabled = false;
                yield break;
            }

            Vector3 startPos = firePoint != null ? firePoint.position : transform.position;
            if (aimLineRenderer != null)
            {
                aimLineRenderer.SetPosition(0, startPos);
                aimLineRenderer.SetPosition(1, startPos + (Vector3)lockedDirection * 20f);
                
                // Efek kedipan bahaya cepat
                float blink = Mathf.PingPong(Time.time * 15f, 1f);
                Color blinkColor = new Color(1f, 0.1f, 0.1f, 0.3f + blink * 0.6f);
                aimLineRenderer.startColor = blinkColor;
                aimLineRenderer.endColor = blinkColor;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Matikan garis bidik sebelum benar-benar melepas tembakan
        if (aimLineRenderer != null)
        {
            aimLineRenderer.enabled = false;
        }

        if (statsScript != null && statsScript.IsDead) yield break;

        if (animatorScript != null)
        {
            animatorScript.TriggerShoot();

            if (attackTimeoutCoroutine != null)
            {
                StopCoroutine(attackTimeoutCoroutine);
            }
            attackTimeoutCoroutine = StartCoroutine(AttackTimeoutSafety(1.5f));
        }
        else
        {
            OnShootProjectile();
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
        Vector2 bodyCenter = (Vector2)transform.position + bodyCenterOffset;
        Vector2 actualOffset = new Vector2(lookDir.x * meleeHitboxOffset.x, lookDir.y * meleeHitboxOffset.y);
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
            // Tentukan arah terbang akhir
            Vector2 finalDir = lockedDirection;
            if (finalDir == Vector2.zero)
            {
                finalDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)firePoint.position).normalized;
            }

            // Kirim target, stat, dan arah bidikan yang terkunci ke proyektil (non-homing agar bisa dihindari)
            arrow.Init(movementScript.PlayerTransform, damage, false, finalDir);
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
        lockedDirection = Vector2.zero; // Reset arah bidikan
        cooldownTimer = attackCooldown; // Mulai cooldown setelah animasi selesai
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position + new Vector3(bodyCenterOffset.x, bodyCenterOffset.y, 0f);

        // Visualisasi jarak jangkauan serang (Cyan)
        Gizmos.color = Color.cyan;
        DrawGizmosEllipse(center, attackRange);

        // Visualisasi batas jangkauan efektif (Kuning) jika berbeda dengan jangkauan dasar
        Gizmos.color = Color.yellow;
        Vector2 effectiveRangeVector = new Vector2(GetMaxEffectiveRange(Vector2.right), GetMaxEffectiveRange(Vector2.up));
        DrawGizmosEllipse(center, effectiveRangeVector);

        if (attackType == AttackType.MeleeAoE)
        {
            // Tampilkan visualisasi area hitbox melee di Inspector
            Gizmos.color = Color.magenta;
            Vector2 bodyCenter = (Vector2)transform.position + bodyCenterOffset;
            Vector2 actualOffset = Vector2.right * meleeHitboxOffset.x;
            if (Application.isPlaying && movementScript != null && movementScript.PlayerTransform != null)
            {
                Vector2 lookDir = ((Vector2)movementScript.PlayerTransform.position - (Vector2)transform.position).normalized;
                actualOffset = new Vector2(lookDir.x * meleeHitboxOffset.x, lookDir.y * meleeHitboxOffset.y);
            }
            Gizmos.DrawWireSphere(bodyCenter + actualOffset, meleeRadius);
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
