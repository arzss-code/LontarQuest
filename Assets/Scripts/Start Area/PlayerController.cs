using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerStats playerStats;
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Melee Hitbox")]
    [SerializeField] GameObject attackUp;
    [SerializeField] GameObject attackDown;
    [SerializeField] GameObject attackLeft;
    [SerializeField] GameObject attackRight;

    [Header("Bow")]
    [SerializeField] float attackRange = 5f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] GameObject arrowPrefab;

    [SerializeField] Transform spawnUp;
    [SerializeField] Transform spawnDown;
    [SerializeField] Transform spawnLeft;
    [SerializeField] Transform spawnRight;

    [Header("Bow Indicator")]
    [SerializeField] GameObject bowRangeIndicator;

    [Header("Dash")]
    [SerializeField] float dashSpeed = 12f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 15f;
    [SerializeField] float knockbackDuration = 0.2f;
    bool isKnockedBack = false;
    float knockbackTimer;
    Vector2 knockbackDirection;

    public bool canMove = true;
    public bool isAutoWalking = false;

    bool isDashing = false;
    float dashTimer;
    float dashCooldownTimer;
    Vector2 dashDirection;

    Rigidbody2D rb;
    [SerializeField]
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 movement;
    // External movement (Gravity Pull, Conveyor, dll)
    private Vector2 externalMovement = Vector2.zero;

    float lastMoveX = 0f;
    float lastMoveY = -1f;

    bool isAttacking = false;

    float inputLockTimer = 0f;

    [Header("SFX")]
    [SerializeField] AudioClip footstepSound;
    [SerializeField] AudioClip meleeSound;
    [SerializeField] AudioClip rangedSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] float footstepInterval = 0.35f;
    float footstepTimer = 0f;

    PlayerAttackHitbox currentHitbox;
    Transform currentTarget;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        rb = GetComponent<Rigidbody2D>();
        

        Debug.Log("Animator ditemukan: " + animator.gameObject.name);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        attackUp.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackDown.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackLeft.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackRight.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();

        // Sembunyikan indikator bow di awal
        if (bowRangeIndicator != null)
        {
            bowRangeIndicator.SetActive(false);

            // Sesuaikan ukuran lingkaran indikator bow
            bowRangeIndicator.transform.localScale =
                new Vector3(
                    attackRange * 2,
                    attackRange * 2,
                    1
                );
        }
    }

    private bool UsePlayerMana(int amount)
    {
        if (playerStats != null)
            return playerStats.UseMana(amount);

        return false;
    }

    private bool UsePlayerStamina(int amount)
    {
        if (playerStats != null)
            return playerStats.UseStamina(amount);

        return false;
    }

    private int GetPlayerRangedDamage()
    {
        if (playerStats != null)
            return playerStats.rangedDamage;

        return 15;
    }

    void Update()
    {

        if (!canMove)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }

            return;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (inputLockTimer > 0)
        {
            inputLockTimer -= Time.deltaTime;
            movement = Vector2.zero;
            return;
        }

        if (isKnockedBack || isAttacking || isDashing)
            return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        float speed = movement.magnitude;

        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetFloat("Speed", speed);

        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                lastMoveX = Mathf.Sign(movement.x);
                lastMoveY = 0;
            }
            else
            {
                lastMoveX = 0;
                lastMoveY = Mathf.Sign(movement.y);
            }

            animator.SetFloat("LastMoveX", lastMoveX);
            animator.SetFloat("LastMoveY", lastMoveY);

            // Footstep SFX
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                if (footstepSound != null)
                {
                    AudioSource.PlayClipAtPoint(footstepSound, transform.position);
                }
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            // Reset timer agar langsung bunyi saat mulai jalan lagi
            footstepTimer = 0f;
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J))
        {
            MeleeAttack();
        }

        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.K))
        {
            Transform enemy = GetNearestEnemy();

            if (enemy == null)
            {
                if (bowRangeIndicator != null)
                    bowRangeIndicator.SetActive(true);
            }
            else
            {
                if (bowRangeIndicator != null)
                    bowRangeIndicator.SetActive(false);
            }
        }

        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.K))
        {
            if (bowRangeIndicator != null)
                bowRangeIndicator.SetActive(false);

            BowAttack();
        }

        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.LeftShift) ||
            Input.GetKeyDown(KeyCode.RightShift)
        )
        {
            Dash();
        }
    }

    

    void FixedUpdate()
    {
        if (isAutoWalking)
        {
            return;
        }

        if (!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (isKnockedBack)
        {
            rb.linearVelocity = knockbackDirection * knockbackForce;
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0)
            {
                EndDash();
            }

            return;
        }

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        

        PlayerModifier modifier = GetComponent<PlayerModifier>();
        float finalMoveSpeed = moveSpeed;
        if (modifier != null)
        {
            finalMoveSpeed *= (1f + modifier.TotalMovementSpeedBonus);
        }

        // Reset velocity agar Saka tidak tergelincir atau terdorong oleh musuh
        // (Ini tidak akan merusak fungsi Knockback karena Knockback me-return di atas)
        rb.linearVelocity = Vector2.zero;

        Vector2 finalMovement =
            movement * finalMoveSpeed +
            externalMovement;

        rb.MovePosition(
            rb.position +
            finalMovement *
            Time.fixedDeltaTime
        );
    }

    // ===================
    // CUTSCENE CONTROL
    // ===================

    public void SetCanMove(bool value)
    {
        canMove = value;

        Debug.Log("SetCanMove dipanggil: " + value);

        movement = Vector2.zero;
        isAttacking = false;
        isDashing = false;
        dashDirection = Vector2.zero;
        dashTimer = 0f;

        if (value)
        {
            inputLockTimer = 0.2f;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0);
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);

            animator.ResetTrigger("Attack");
            animator.ResetTrigger("BowAttack");
            animator.ResetTrigger("Dash");
        }

        if (bowRangeIndicator != null)
        {
            bowRangeIndicator.SetActive(false);
        }

        DeactivateHitbox();
    }

    public bool CanMove()
    {
        
        return canMove;
    }

    public void SetExternalMovement(Vector2 force)
    {
        externalMovement = force;
    }

    public void ClearExternalMovement()
    {
        externalMovement = Vector2.zero;
    }

    public void TriggerDeath()
    {
        // Kunci semua pergerakan
        SetCanMove(false);
        
        // Putar animasi mati (Pastikan parameter 'Die' ada di Animator Saka)
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    // ===================
    // MELEE
    // ===================

    void MeleeAttack()
    {
        isAttacking = true;

        movement = Vector2.zero;

        SetAttackDirection();

        PlayerModifier modifier = GetComponent<PlayerModifier>();
        if (modifier != null && animator != null)
        {
            animator.speed = 1f + modifier.TotalAttackSpeedBonus;
        }

        if (meleeSound != null)
        {
            AudioSource.PlayClipAtPoint(meleeSound, transform.position);
        }

        animator.SetTrigger("Attack");
    }

    void SetAttackDirection()
    {
        if (lastMoveY > 0)
        {
            currentHitbox =
                attackUp.GetComponent<PlayerAttackHitbox>();
        }
        else if (lastMoveY < 0)
        {
            currentHitbox =
                attackDown.GetComponent<PlayerAttackHitbox>();
        }
        else if (lastMoveX > 0)
        {
            currentHitbox =
                attackRight.GetComponent<PlayerAttackHitbox>();
        }
        else
        {
            currentHitbox =
                attackLeft.GetComponent<PlayerAttackHitbox>();
        }
    }

    public void ActivateHitbox()
    {
        if (currentHitbox != null)
        {
            currentHitbox.ActivateHitbox();
        }
    }

    public void DeactivateHitbox()
    {
        if (currentHitbox != null)
        {
            currentHitbox.DeactivateHitbox();
        }
    }

    // ===================
    // BOW
    // ===================

    void BowAttack()
    {
        if (bowRangeIndicator != null)
            bowRangeIndicator.SetActive(false);

        currentTarget = GetNearestEnemy();

        if (currentTarget == null)
        {
            Debug.Log("Tidak ada musuh");
            return;
        }

        bool useManaSuccess = false;

        if (playerStats != null)
        {
            useManaSuccess = playerStats.UseMana(25);
        }

        if (!useManaSuccess)
        {
            Debug.Log("Mana tidak cukup");
            return;
        }

        isAttacking = true;

        movement = Vector2.zero;

        Vector2 direction =
            currentTarget.position -
            transform.position;

        direction = direction.normalized;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            lastMoveX = Mathf.Sign(direction.x);
            lastMoveY = 0;
        }
        else
        {
            lastMoveX = 0;
            lastMoveY = Mathf.Sign(direction.y);
        }

        animator.SetFloat("LastMoveX", lastMoveX);
        animator.SetFloat("LastMoveY", lastMoveY);

        PlayerModifier modifier = GetComponent<PlayerModifier>();
        if (modifier != null && animator != null)
        {
            animator.speed = 1f + modifier.TotalAttackSpeedBonus;
        }

        if (rangedSound != null)
        {
            AudioSource.PlayClipAtPoint(rangedSound, transform.position);
        }

        animator.SetTrigger("BowAttack");
    }

    public void SpawnArrow()
    {
        if (currentTarget == null)
            return;

        Transform spawnPoint;

        if (lastMoveY > 0)
        {
            spawnPoint = spawnUp;
        }
        else if (lastMoveY < 0)
        {
            spawnPoint = spawnDown;
        }
        else if (lastMoveX > 0)
        {
            spawnPoint = spawnRight;
        }
        else
        {
            spawnPoint = spawnLeft;
        }

        GameObject arrow =
            Instantiate(
                arrowPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

        ArrowProjectile projectile =
            arrow.GetComponent<ArrowProjectile>();

        int arrowDamage = 15;

        if (playerStats != null)
        {
            arrowDamage = playerStats.rangedDamage;
        }
        
        PlayerModifier pm = GetComponent<PlayerModifier>();
        bool isFire = (pm != null && pm.HasElementalEffect);

        projectile.SetTarget(currentTarget, arrowDamage, isFire);
    }

    Transform GetNearestEnemy()
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                transform.position,
                attackRange,
                enemyLayer
            );

        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            float distance =
                Vector2.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    // ===================
    // DASH
    // ===================

    void Dash()
{
    Debug.Log("DASH");

    if (dashCooldownTimer > 0)
        return;

    if (isAttacking)
        return;

    if (isDashing)
        return;

    if (!UsePlayerStamina(20))
    {
        Debug.Log("Stamina tidak cukup");
        return;
    }

    // Jaga-jaga kalau player belum pernah bergerak
    if (lastMoveX == 0 && lastMoveY == 0)
    {
        lastMoveY = -1;
    }

    isDashing = true;

    movement = Vector2.zero;

    dashTimer = dashDuration;
    dashCooldownTimer = dashCooldown;

    if (Mathf.Abs(lastMoveX) > Mathf.Abs(lastMoveY))
    {
        dashDirection = new Vector2(Mathf.Sign(lastMoveX), 0);
    }
    else
    {
        dashDirection = new Vector2(0, Mathf.Sign(lastMoveY));
    }

    if (dashSound != null)
    {
        AudioSource.PlayClipAtPoint(dashSound, transform.position);
    }

    animator.SetTrigger("Dash");
}

    public void EndDash()
    {
        isDashing = false;

        rb.linearVelocity = Vector2.zero;
    }

    public void EndAttack()
    {
        DeactivateHitbox();

        isAttacking = false;

        if (animator != null)
        {
            animator.speed = 1f; // Kembalikan ke kecepatan normal
        }
    }

    // ===================
    // KNOCKBACK
    // ===================
    public void ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        knockbackDirection = direction.normalized;
        knockbackTimer = knockbackDuration;
        
        isAttacking = false;
        isDashing = false;
        
        DeactivateHitbox();
    }
}