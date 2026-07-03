using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private enum MovementState
    {
        Idle,
        Moving,
        Waiting
    }

    [Header("References")]
    [SerializeField] private EnemyDetection detection;
    [SerializeField] private Animator animator;

    [Header("Room")]
    [SerializeField] private RoomArea currentRoom;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Distance")]
    [SerializeField] private float retreatDistance = 4f;
    [SerializeField] private float safeDistance = 7f;

    [Header("Retreat")]
    [SerializeField] private float retreatRadius = 5f;
    [SerializeField] private float randomOffsetRadius = 2f;
    [SerializeField] private float destinationTolerance = 0.15f;
    [SerializeField] private float waitAfterMove = 1.5f;

    [Header("Obstacle")]
    [SerializeField] private LayerMask wallLayer;

    private Rigidbody2D rb;

    private MovementState currentState = MovementState.Idle;

    private Vector2 destination;

    private static readonly int DirectionHash =
        Animator.StringToHash("Direction");

    //--------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        FindCurrentRoom();
    }

    //--------------------------------------------------

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

        if (currentRoom != null)
            Debug.Log($"{name} berada di {currentRoom.name}");
        else
            Debug.LogWarning($"{name} tidak berada di Room manapun.");
    }

    //--------------------------------------------------

    private void FixedUpdate()
    {
        if (!detection.HasTarget)
        {
            Idle();
            return;
        }

        switch (currentState)
        {
            case MovementState.Idle:

                float distance =
                    Vector2.Distance(
                        transform.position,
                        detection.Target.position);

                if (distance <= retreatDistance)
                {
                    if (GenerateDestination())
                    {
                        currentState = MovementState.Moving;
                    }
                }

                break;

            case MovementState.Moving:

                MoveToDestination();

                break;

            case MovementState.Waiting:

                rb.linearVelocity = Vector2.zero;

                break;
        }
    }

    //--------------------------------------------------
    // Generate posisi acak yang valid
    //--------------------------------------------------

    private bool GenerateDestination()
    {
        if (currentRoom == null)
            return false;

        const int MAX_TRIES = 20;

        for (int i = 0; i < MAX_TRIES; i++)
        {
            //---------------------------------
            // Menjauh dari Player
            //---------------------------------

            Vector2 retreatDirection =
                (
                    (Vector2)transform.position -
                    (Vector2)detection.Target.position
                ).normalized;

            //---------------------------------

            Vector2 candidate =
                (Vector2)transform.position +
                retreatDirection * retreatRadius +
                Random.insideUnitCircle * randomOffsetRadius;

            //---------------------------------
            // Di dalam Room?
            //---------------------------------

            if (!currentRoom.Contains(candidate))
                continue;

            //---------------------------------
            // Terlalu dekat Player?
            //---------------------------------

            if (Vector2.Distance(
                candidate,
                detection.Target.position)
                < safeDistance)
                continue;

            //---------------------------------
            // Ada Wall?
            //---------------------------------

            if (Physics2D.OverlapCircle(
                candidate,
                0.25f,
                wallLayer))
                continue;

            //---------------------------------

            destination = candidate;

            return true;
        }

        return false;
    }

    //--------------------------------------------------

    private void MoveToDestination()
    {
        Vector2 direction =
            (
                destination -
                (Vector2)transform.position
            ).normalized;

        rb.linearVelocity =
            direction * moveSpeed;

        animator.SetFloat(
            DirectionHash,
            direction.x >= 0 ? 1 : -1);

        //---------------------------------

        if (Vector2.Distance(
            transform.position,
            destination)
            <= destinationTolerance)
        {
            rb.linearVelocity = Vector2.zero;

            StartCoroutine(WaitRoutine());
        }
    }

    //--------------------------------------------------

    private IEnumerator WaitRoutine()
    {
        currentState = MovementState.Waiting;

        animator.SetFloat(DirectionHash, 0);

        yield return new WaitForSeconds(waitAfterMove);

        currentState = MovementState.Idle;
    }

    //--------------------------------------------------

    private void Idle()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetFloat(DirectionHash, 0);
    }

    //--------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            transform.position,
            retreatDistance);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            retreatRadius);

        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(
            destination,
            0.15f);
    }
}