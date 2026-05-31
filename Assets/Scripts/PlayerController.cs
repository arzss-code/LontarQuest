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
    [SerializeField]
    float dashSpeed = 12f;

    [SerializeField]
    float dashDuration = 0.2f;

    [SerializeField]
    float dashCooldown = 1f;

    bool isDashing = false;

    float dashTimer;

    float dashCooldownTimer;

    Vector2 dashDirection;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 movement;

    float lastMoveX = 0f;
    float lastMoveY = -1f;

    bool isAttacking = false;

    PlayerAttackHitbox currentHitbox;

    Transform currentTarget;

    

    void Awake()
    {

        playerStats =
        GetComponent<PlayerStats>();
        rb =
        GetComponent<Rigidbody2D>();

        animator =
        GetComponentInChildren<Animator>();

        spriteRenderer =
        GetComponentInChildren<SpriteRenderer>();


        attackUp
        .GetComponent<PlayerAttackHitbox>()
        .DeactivateHitbox();

        attackDown
        .GetComponent<PlayerAttackHitbox>()
        .DeactivateHitbox();

        attackLeft
        .GetComponent<PlayerAttackHitbox>()
        .DeactivateHitbox();

        attackRight
        .GetComponent<PlayerAttackHitbox>()
        .DeactivateHitbox();


        // sembunyikan indikator awal
        bowRangeIndicator.SetActive(false);

        // sesuaikan ukuran lingkaran
        bowRangeIndicator.transform.localScale =
        new Vector3(
            attackRange * 2,
            attackRange * 2,
            1
        );
    }

    void Update()
    {
            // cooldown dash
        if(dashCooldownTimer > 0)
        {
            dashCooldownTimer -=
            Time.deltaTime;
        }

        if(isAttacking ||
        isDashing)
            return;


        movement.x =
        Input.GetAxisRaw(
        "Horizontal");

        movement.y =
        Input.GetAxisRaw(
        "Vertical");

        movement =
        movement.normalized;


        float speed =
        movement.magnitude;


        animator.SetFloat(
        "MoveX",
        movement.x);

        animator.SetFloat(
        "MoveY",
        movement.y);

        animator.SetFloat(
        "Speed",
        speed);


        if(movement != Vector2.zero)
        {
            if(Mathf.Abs(movement.x) >
            Mathf.Abs(movement.y))
            {
                lastMoveX =
                Mathf.Sign(movement.x);

                lastMoveY = 0;
            }
            else
            {
                lastMoveX = 0;

                lastMoveY =
                Mathf.Sign(movement.y);
            }

            animator.SetFloat(
                "LastMoveX",
                lastMoveX);

            animator.SetFloat(
                "LastMoveY",
                lastMoveY);
        }


        //=================
        // MELEE
        //=================

        if(Input.GetMouseButtonDown(0))
        {
            MeleeAttack();
        }


        //=================
        // BOW
        //=================

        // tahan klik kanan
        // tahan klik kanan
        if(Input.GetMouseButton(1))
        {
            Transform enemy =
            GetNearestEnemy();

            // tampilkan indikator
            // hanya jika TIDAK ada musuh
            if(enemy == null)
            {
                bowRangeIndicator.SetActive(
                true);
            }
            else
            {
                bowRangeIndicator.SetActive(
                false);
            }
        }

        // lepas klik kanan
        if(Input.GetMouseButtonUp(1))
        {
            bowRangeIndicator.SetActive(
            false);

            BowAttack();
        }

        //=================
        // DASH
        //=================

        if(Input.GetKeyDown(KeyCode.Space) ||
        Input.GetKeyDown(KeyCode.LeftShift) ||
        Input.GetKeyDown(KeyCode.RightShift))
        {
            Dash();
        }
    }


    void FixedUpdate()
    {
        if(isDashing)
        {
            rb.linearVelocity =
            dashDirection *
            dashSpeed;

            dashTimer -=
            Time.fixedDeltaTime;

            if(dashTimer <= 0)
            {
                EndDash();
            }

            return;
        }

        if(isAttacking)
            return;


        rb.MovePosition(
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }



    //===================
    // MELEE
    //===================

    void MeleeAttack()
    {
        isAttacking = true;

        movement = Vector2.zero;

        SetAttackDirection();

        animator.SetTrigger(
        "Attack");
    }


    void SetAttackDirection()
    {
        if(lastMoveY > 0)
        {
            currentHitbox =
            attackUp.GetComponent
            <PlayerAttackHitbox>();
        }

        else if(lastMoveY < 0)
        {
            currentHitbox =
            attackDown.GetComponent
            <PlayerAttackHitbox>();
        }

        else if(lastMoveX > 0)
        {
            currentHitbox =
            attackRight.GetComponent
            <PlayerAttackHitbox>();
        }

        else
        {
            currentHitbox =
            attackLeft.GetComponent
            <PlayerAttackHitbox>();
        }
    }


    public void ActivateHitbox()
    {
        if(currentHitbox != null)
        {
            currentHitbox
            .ActivateHitbox();
        }
    }


    public void DeactivateHitbox()
    {
        if(currentHitbox != null)
        {
            currentHitbox
            .DeactivateHitbox();
        }
    }



    //===================
    // BOW
    //===================

    void BowAttack()
    {
        bowRangeIndicator.SetActive(
        false);

        currentTarget =
        GetNearestEnemy();

        if(currentTarget == null)
        {
            Debug.Log(
            "Tidak ada musuh");

            return;
        }

        if(!playerStats.UseMana(25))
        {
            Debug.Log(
            "Mana tidak cukup");

            return;
        }

        isAttacking = true;

        movement = Vector2.zero;

        Vector2 direction =
        currentTarget.position -
        transform.position;

        direction =
        direction.normalized;

        if(Mathf.Abs(direction.x) >
        Mathf.Abs(direction.y))
        {
            lastMoveX =
            Mathf.Sign(direction.x);

            lastMoveY = 0;
        }
        else
        {
            lastMoveX = 0;

            lastMoveY =
            Mathf.Sign(direction.y);
        }

        animator.SetFloat(
        "LastMoveX",
        lastMoveX);

        animator.SetFloat(
        "LastMoveY",
        lastMoveY);

        animator.SetTrigger(
        "BowAttack");
    }


    //===================
    // DASH
    //===================
    
    
   void Dash()
    {
        Debug.Log("DASH");

        if(dashCooldownTimer > 0)
            return;

        if(isAttacking)
            return;

        if(isDashing)
            return;

        // Cek stamina
        if(!playerStats.UseStamina(20))
        {
            Debug.Log(
            "Stamina tidak cukup"
            );

            return;
        }

        // jaga-jaga kalau player belum pernah bergerak
        if(lastMoveX == 0 &&
        lastMoveY == 0)
        {
            lastMoveY = -1;
        }

        isDashing = true;

        movement =
        Vector2.zero;

        dashTimer =
        dashDuration;

        dashCooldownTimer =
        dashCooldown;

        if(Mathf.Abs(lastMoveX) >
        Mathf.Abs(lastMoveY))
        {
            dashDirection =
            new Vector2(
                Mathf.Sign(lastMoveX),
                0
            );
        }
        else
        {
            dashDirection =
            new Vector2(
                0,
                Mathf.Sign(lastMoveY)
            );
        }

        animator.SetTrigger(
            "Dash"
        );
    }



    Transform GetNearestEnemy()
    {
        Collider2D[] enemies =

        Physics2D
        .OverlapCircleAll(
            transform.position,
            attackRange,
            enemyLayer
        );


        Transform nearest =
        null;

        float nearestDistance =
        Mathf.Infinity;


        foreach(
        Collider2D enemy
        in enemies)
        {
            float distance =

            Vector2.Distance(
            transform.position,
            enemy.transform.position
            );


            if(distance <
               nearestDistance)
            {
                nearestDistance =
                distance;

                nearest =
                enemy.transform;
            }
        }

        return nearest;
    }

    public void SpawnArrow()
{
    if(currentTarget == null)
        return;

    Transform spawnPoint;

    if(lastMoveY > 0)
    {
        spawnPoint = spawnUp;
    }
    else if(lastMoveY < 0)
    {
        spawnPoint = spawnDown;
    }
    else if(lastMoveX > 0)
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

    projectile.SetTarget(
        currentTarget
    );
}


    //===================
    public void EndDash()
    {
        isDashing = false;

        rb.linearVelocity =
        Vector2.zero;
    }
    //===================

    public void EndAttack()
    {
        DeactivateHitbox();

        isAttacking = false;
    }
}