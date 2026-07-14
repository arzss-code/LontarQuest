using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float stoppingDistance = 2f;

    private Rigidbody2D rb;
    private Transform player;

    private BossController bossController;
    private BossAttack bossAttack;

    private Vector2 moveDirection;
    private bool canMove = true;

    public bool CanMove => canMove;

    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!canMove)
            moveDirection = Vector2.zero;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        bossController = GetComponent<BossController>();
        bossAttack = GetComponent<BossAttack>();
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player tidak ditemukan!");
    }

    private void Update()
    {
        if (bossController == null)
            return;

        if (!bossController.IsAwaken)
            return;

        if (player == null)
            return;

        if (!canMove)
        {
            moveDirection = Vector2.zero;
            return;
        }

        // Boss sedang attack
        if (bossAttack != null && bossAttack.IsBusy)
        {
            moveDirection = Vector2.zero;
            return;
        }

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= stoppingDistance)
        {
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection =
            ((Vector2)player.position - rb.position).normalized;
    }

    private void FixedUpdate()
    {
        if (moveDirection == Vector2.zero)
            return;

        rb.MovePosition(
            rb.position +
            moveDirection *
            moveSpeed *
            Time.fixedDeltaTime);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
#endif
}