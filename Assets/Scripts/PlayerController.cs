using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    float moveSpeed = 5f;

    Rigidbody2D rb;
    Animator animator;

    Vector2 movement;

    float lastMoveX = 0f;
    float lastMoveY = -1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // FIX
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        float speed = movement.magnitude;

        if (animator != null)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", speed);

            if (movement != Vector2.zero)
            {
                lastMoveX = movement.x;
                lastMoveY = movement.y;

                animator.SetFloat("LastMoveX", lastMoveX);
                animator.SetFloat("LastMoveY", lastMoveY);
            }
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
                rb.MovePosition(
                rb.position +
                movement.normalized *
                moveSpeed *
                Time.fixedDeltaTime
            );
        }
        else
        {
            transform.position +=
                (Vector3)(movement * moveSpeed * Time.deltaTime);
        }
    }
}