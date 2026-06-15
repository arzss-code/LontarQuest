using UnityEngine;

public class MushroomEnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float moveSpeed = 2f;

    [Header("Detection")]
    [SerializeField]
    float chaseRange = 5f;

    [Header("Patrol")]
    [SerializeField]
    float patrolDistance = 3f;

    [SerializeField]
    float patrolWaitTime = 1f;

    [SerializeField]
    LayerMask wallLayer;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Transform player;

    Vector2 startPosition;
    Vector2 patrolTarget;

    float waitTimer;

    enum EnemyState
    {
        Patrol,
        Chase,
        Return
    }

    EnemyState currentState;

    void Awake()
    {
        rb =
        GetComponent<Rigidbody2D>();

        animator =
        GetComponentInChildren<Animator>();

        spriteRenderer =
        GetComponentInChildren<SpriteRenderer>();


        GameObject playerObject =

        GameObject
        .FindGameObjectWithTag(
            "Player"
        );

        if(playerObject != null)
        {
            player =
            playerObject.transform;
        }

        startPosition =
        transform.position;

        // BENAR
        SetNewPatrolPoint();

        currentState =
        EnemyState.Patrol;
    }



    void FixedUpdate()
    {
        if(player == null)
            return;


        float playerDistance =

        Vector2.Distance(
            transform.position,
            player.position
        );


        switch(currentState)
        {
            //====================
            // PATROL
            //====================

            case EnemyState.Patrol:

                Patrol();

                if(playerDistance <=
                   chaseRange)
                {
                    currentState =
                    EnemyState.Chase;
                }

                break;



            //====================
            // CHASE
            //====================

            case EnemyState.Chase:

                ChasePlayer();

                if(playerDistance >
                   chaseRange)
                {
                    currentState =
                    EnemyState.Return;
                }

                break;



            //====================
            // RETURN
            //====================

            case EnemyState.Return:

                ReturnHome();

                break;
        }
    }



    //====================
    // PATROL
    //====================

    void Patrol()
    {
        float distance =

        Vector2.Distance(
            transform.position,
            patrolTarget
        );


        // bergerak ke target patroli
        if(distance > 0.2f)
        {
            MoveTo(
                patrolTarget
            );
        }

        // sampai tujuan
        else
        {
            StopMoving();

            waitTimer +=
            Time.fixedDeltaTime;

            // tunggu sebentar
            if(waitTimer >=
               patrolWaitTime)
            {
                waitTimer = 0;

                SetNewPatrolPoint();
            }
        }
    }



    //====================
    // CHASE
    //====================

    void ChasePlayer()
    {
        MoveTo(
            player.position
        );
    }



    //====================
    // RETURN
    //====================

    void ReturnHome()
    {
        float distance =

        Vector2.Distance(
            transform.position,
            startPosition
        );


        // kembali ke rumah
        if(distance > 0.2f)
        {
            MoveTo(
                startPosition
            );
        }

        // sampai rumah
        else
        {
            StopMoving();

            SetNewPatrolPoint();

            currentState =
            EnemyState.Patrol;
        }
    }

    //====================
    // TAKE DAMAGE
    //====================

    public void TakeDamage(int damage)
    {
        // Asumsi ada variabel currentHealth, jika tidak, cukup tampilkan pop-up
        // currentHealth -= damage;
        DamagePopupManager.Create(transform.position, damage, false);
    }



    //====================
    // MOVE
    //====================

    void MoveTo(
    Vector2 target)
    {
        Vector2 direction =

        (
            target -
            (Vector2)transform.position
        ).normalized;


        rb.linearVelocity =

        direction *
        moveSpeed;


        animator.SetFloat(
            "Speed",
            1f
        );


        // sprite asli menghadap kiri

        if(direction.x > 0)
        {
            spriteRenderer.flipX =
            true;
        }

        else if(direction.x < 0)
        {
            spriteRenderer.flipX =
            false;
        }
    }



    //====================
    // STOP
    //====================

    void StopMoving()
    {
        rb.linearVelocity =
        Vector2.zero;

        animator.SetFloat(
            "Speed",
            0f
        );
    }



    //====================
    // RANDOM PATROL
    //====================

    void SetNewPatrolPoint()
    {
        int maxAttempts = 10;

        for(int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomDirection =

            Random.insideUnitCircle *
            patrolDistance;

            Vector2 candidatePoint =

            startPosition +
            randomDirection;


            // cek apakah titik kena wall
            Collider2D wallHit =

            Physics2D.OverlapCircle(
                candidatePoint,
                0.2f,
                wallLayer
            );


            // kalau aman
            if(wallHit == null)
            {
                patrolTarget =
                candidatePoint;

                return;
            }
        }

        // fallback
        patrolTarget =
        startPosition;
    }



    //====================
    // GIZMOS
    //====================

    void OnDrawGizmosSelected()
    {
        // CHASE RANGE

        Gizmos.color =
        Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            chaseRange
        );


        // PATROL AREA

        Gizmos.color =
        Color.green;

        Gizmos.DrawWireSphere(
            Application.isPlaying
            ? startPosition
            : (Vector2)transform.position,
            patrolDistance
        );
    }
}