using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Attack Hitbox")]
    [SerializeField] GameObject attackUp;
    [SerializeField] GameObject attackDown;
    [SerializeField] GameObject attackLeft;
    [SerializeField] GameObject attackRight;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 movement;

    float lastMoveX = 0f;
    float lastMoveY = -1f;

    bool isAttacking = false;

    PlayerAttackHitbox currentHitbox;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        isAttacking = false;

        // Ambil komponen hitbox
        attackUp.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackDown.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackLeft.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
        attackRight.GetComponent<PlayerAttackHitbox>().DeactivateHitbox();
    }

    void Update()
    {
        if(isAttacking)
            return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        float speed = movement.magnitude;

        if(animator != null)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
            animator.SetFloat("Speed", speed);

            if(movement != Vector2.zero)
            {
                lastMoveX = movement.x;
                lastMoveY = movement.y;

                animator.SetFloat("LastMoveX", lastMoveX);
                animator.SetFloat("LastMoveY", lastMoveY);
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        if(isAttacking)
            return;

        rb.MovePosition(
            rb.position +
            movement *
            moveSpeed *
            Time.fixedDeltaTime
        );
    }

    void Attack()
    {
        isAttacking = true;

        movement = Vector2.zero;

        SetAttackDirection();

        animator.SetTrigger("Attack");
    }

    void SetAttackDirection()
    {
        if(lastMoveY > 0)
        {
            currentHitbox =
                attackUp.GetComponent<PlayerAttackHitbox>();
        }
        else if(lastMoveY < 0)
        {
            currentHitbox =
                attackDown.GetComponent<PlayerAttackHitbox>();
        }
        else if(lastMoveX > 0)
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

    // Animation Event
    public void ActivateHitbox()
    {
        Debug.Log("HITBOX ON");

        if(currentHitbox != null)
        {
            currentHitbox.ActivateHitbox();
        }
    }

    // Animation Event
    public void DeactivateHitbox()
    {
        Debug.Log("HITBOX OFF");

        if(currentHitbox != null)
        {
            currentHitbox.DeactivateHitbox();
        }
    }

    // Animation Event
    public void EndAttack()
    {
        DeactivateHitbox();

        isAttacking = false;
    }
}