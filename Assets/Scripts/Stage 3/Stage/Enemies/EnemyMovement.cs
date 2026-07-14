using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private enum MovementState
    {
        Patrol,
        Move,
        Wait,
        Combat,
        Retreat
    }

    [Header("References")]
    [SerializeField] private EnemyDetection detection;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visual;
    [SerializeField] private RoomArea currentRoom;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Patrol")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private float arriveDistance = 0.15f;

    [Header("Combat")]
    [SerializeField] private float retreatDistance = 2.5f;
    [SerializeField] private float safeDistance = 5f;
    [SerializeField] private float retreatDistanceMove = 4f;
    [SerializeField] private float retreatRandomOffset = 1.5f;

    [Header("Obstacle")]
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;
    private EnemyAttack attack;

    private MovementState currentState;

    private Vector2 destination;

    private static readonly int DirectionHash =
        Animator.StringToHash("Direction");

    private Coroutine waitRoutine;

    //--------------------------------------------------
    private bool IsValidPosition(Vector2 point)
    {
        if (currentRoom == null)
            return false;

        if (!currentRoom.Contains(point))
            return false;

        if (Physics2D.OverlapCircle(
            point,
            0.2f,
            wallLayer))
            return false;

        return true;
    }

    private bool GenerateRandomPatrolPoint()
    {
        if (currentRoom == null)
            return false;

        Bounds bounds = currentRoom.GetBounds();

        const int MAX_TRIES = 30;

        for(int i=0;i<MAX_TRIES;i++)
        {
            Vector2 randomCenter = new Vector2(
                Random.Range(
                    transform.position.x-wanderRadius,
                    transform.position.x+wanderRadius),

                Random.Range(
                    transform.position.y-wanderRadius,
                    transform.position.y+wanderRadius)
            );

            //---------------------------------
            // Clamp ke dalam Room
            //---------------------------------

            randomCenter.x =
                Mathf.Clamp(
                    randomCenter.x,
                    bounds.min.x,
                    bounds.max.x);

            randomCenter.y =
                Mathf.Clamp(
                    randomCenter.y,
                    bounds.min.y,
                    bounds.max.y);

            //---------------------------------

            if(!IsValidPosition(randomCenter))
                continue;

            //---------------------------------

            if(Vector2.Distance(
                transform.position,
                randomCenter)<1.5f)
                continue;

            destination = randomCenter;

            return true;
        }

        return false;
    }

    private bool GenerateRetreatPoint()
    {
        if (!detection.HasTarget || currentRoom == null)
            return false;

        Bounds bounds = currentRoom.GetBounds();

        const int MAX_TRIES = 25;

        for (int i = 0; i < MAX_TRIES; i++)
        {
            // Arah menjauh dari player
            Vector2 retreatDir =
                ((Vector2)transform.position -
                (Vector2)detection.Target.position).normalized;

            // Titik tujuan
            Vector2 candidate =
                (Vector2)transform.position +
                retreatDir * retreatDistanceMove +
                Random.insideUnitCircle * retreatRandomOffset;

            // Clamp agar tetap di dalam room
            candidate.x = Mathf.Clamp(candidate.x, bounds.min.x, bounds.max.x);
            candidate.y = Mathf.Clamp(candidate.y, bounds.min.y, bounds.max.y);

            if (!IsValidPosition(candidate))
                continue;

            if (Vector2.Distance(candidate, detection.Target.position) < safeDistance)
                continue;

            destination = candidate;

            Debug.Log($"Retreat -> {destination}");

            return true;
        }

        return false;
    }

    private bool MoveToDestination()
    {
        Vector2 direction =
            (destination - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        animator.SetFloat(
            DirectionHash,
            direction.x >= 0 ? 1 : -1);

        if (Vector2.Distance(
            transform.position,
            destination) <= arriveDistance)
        {
            rb.linearVelocity = Vector2.zero;

            return true;
        }

        return false;
    }

    private void FacePlayer()
    {
        if (!detection.HasTarget)
            return;

        Vector3 scale = visual.localScale;

        if (detection.Target.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        visual.localScale = scale;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attack = GetComponent<EnemyAttack>();

        FindCurrentRoom();
        if (currentRoom != null)
        {
            Debug.Log($"Room ditemukan : {currentRoom.name}");
        }

        currentState = MovementState.Patrol;
    }

    //--------------------------------------------------

    private void FixedUpdate()
    {
        //--------------------------------------------------
        // State Switching
        //--------------------------------------------------

       if (detection.HasTarget)
        {
            if (currentState == MovementState.Patrol ||
                currentState == MovementState.Move ||
                currentState == MovementState.Wait)
            {
                currentState = MovementState.Combat;

                rb.linearVelocity = Vector2.zero;

                if (waitRoutine != null)
                {
                    StopCoroutine(waitRoutine);
                    waitRoutine = null;
                }

                Debug.Log("Combat Mode");
            }
        }
        else
        {
            if (currentState == MovementState.Combat ||
                currentState == MovementState.Retreat)
            {
                currentState = MovementState.Patrol;

                Debug.Log("Back To Patrol");
            }
        }
        switch (currentState)
        {
            //----------------------------------
            // Cari titik patrol baru
            //----------------------------------

            case MovementState.Patrol:

                if (GenerateRandomPatrolPoint())
                {
                    currentState = MovementState.Move;
                }

                break;

            //----------------------------------
            // Bergerak menuju tujuan
            //----------------------------------

            case MovementState.Move:

                if (MoveToDestination())
                {
                    StartCoroutine(WaitRoutine());
                }

                break;

            //----------------------------------
            // Sedang menunggu
            //----------------------------------

            case MovementState.Wait:

                rb.linearVelocity = Vector2.zero;

                break;


            case MovementState.Retreat:

            if (MoveToDestination())
            {
                currentState = MovementState.Combat;
            }

            break;

            case MovementState.Combat:

            FacePlayer();

            //----------------------------------
            // Jangan lakukan apa-apa saat animasi Attack
            //----------------------------------

            if (attack.IsAttacking)
            {
                rb.linearVelocity = Vector2.zero;
                break;
            }

            //----------------------------------
            // Retreat jika Player terlalu dekat
            //----------------------------------

            float distance = Vector2.Distance(
                transform.position,
                detection.Target.position);

            if (distance <= retreatDistance)
            {
                if (GenerateRetreatPoint())
                {
                    currentState = MovementState.Retreat;
                    break;
                }
            }

            //----------------------------------
            // Attack
            //----------------------------------

            attack.TryAttack();

            break;
        }
    }


    
    private void FindCurrentRoom()
    {
        RoomArea[] rooms =
            FindObjectsByType<RoomArea>(
                FindObjectsSortMode.None);

        foreach (RoomArea room in rooms)
        {
            if (room.Contains(transform.position))
            {
                currentRoom = room;
                break;
            }
        }

        if(currentRoom==null)
        {
            Debug.LogError($"{name} tidak menemukan RoomArea.");
        }
    }

    private IEnumerator WaitRoutine()
    {
        currentState = MovementState.Wait;

        animator.SetFloat(DirectionHash, 0);

        yield return new WaitForSeconds(waitTime);

        currentState = MovementState.Patrol;
    }
}