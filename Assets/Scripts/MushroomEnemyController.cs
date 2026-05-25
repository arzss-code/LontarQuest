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
        GameObject.FindGameObjectWithTag(
            "Player"
        );

        if(playerObject != null)
        {
            player =
            playerObject.transform;
        }

        startPosition =
        transform.position;

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
            case EnemyState.Patrol:

                Patrol();

                if(playerDistance <=
                   chaseRange)
                {
                    currentState =
                    EnemyState.Chase;
                }

                break;


            case EnemyState.Chase:

                ChasePlayer();

                if(playerDistance >
                   chaseRange)
                {
                    currentState =
                    EnemyState.Return;
                }

                break;


            case EnemyState.Return:

                ReturnHome();

                break;
        }
    }


    void Patrol()
    {
        float distance =

        Vector2.Distance(
            transform.position,
            patrolTarget
        );


        if(distance > 0.2f)
        {
            MoveTo(
                patrolTarget
            );
        }
        else
        {
            StopMoving();

            waitTimer +=
            Time.fixedDeltaTime;

            if(waitTimer >=
               patrolWaitTime)
            {
                waitTimer = 0;

                SetNewPatrolPoint();
            }
        }
    }


    void ChasePlayer()
    {
        MoveTo(
            player.position
        );
    }


    void ReturnHome()
    {
        float distance =

        Vector2.Distance(
            transform.position,
            startPosition
        );


        if(distance > 0.2f)
        {
            MoveTo(
                startPosition
            );
        }
        else
        {
            StopMoving();

            SetNewPatrolPoint();

            currentState =
            EnemyState.Patrol;
        }
    }


    void MoveTo(
    Vector2 target)
    {
        Vector2 direction =

        (
            target -
            rb.position
        ).normalized;


        rb.linearVelocity =
        direction *
        moveSpeed;


        animator.SetFloat(
            "Speed",
            1f
        );


        // flip kiri kanan

        if(direction.x > 0)
        {
            // kanan
            spriteRenderer.flipX =
            true;
        }
        else if(direction.x < 0)
        {
            // kiri (arah asli sprite)
            spriteRenderer.flipX =
            false;
        }
    }


    void StopMoving()
    {
        rb.linearVelocity =
        Vector2.zero;

        animator.SetFloat(
            "Speed",
            0f
        );
    }


    void SetNewPatrolPoint()
    {
        float randomX =

        Random.Range(
            -patrolDistance,
            patrolDistance
        );


        patrolTarget =

        startPosition +

        new Vector2(
            randomX,
            0
        );
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color =
        Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            chaseRange
        );


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