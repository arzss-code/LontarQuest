using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [Header("References")]
    public Transform visual;
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Vector3 moveDirection;

    [Header("Camera Reference")]
    public Transform cameraTransform;
    private Animator animator;
    private float lastMoveX;
    private float lastMoveY;
    private Vector2 aimDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }
    

    void Update()
    {
        HandleInput();
        // RotateTowardsMouse();
        HandleAimDirection();
    }

    void FixedUpdate()
    {
        Move();
    }


    void HandleAimDirection()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    float rayDistance;

    if (groundPlane.Raycast(ray, out rayDistance))
    {
        Vector3 mouseWorldPos = ray.GetPoint(rayDistance);

        Vector3 direction = mouseWorldPos - transform.position;

        direction.y = 0f;

        aimDirection = new Vector2(direction.x, direction.z).normalized;

        animator.SetFloat("AimX", aimDirection.x);
        animator.SetFloat("AimY", aimDirection.y);
    }
}
    void RotateTowardsMouse()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    float rayDistance;

    if (groundPlane.Raycast(ray, out rayDistance))
    {
        Vector3 point = ray.GetPoint(rayDistance);

        Vector3 direction = point - transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            visual.rotation = Quaternion.Euler(0f, angle - 45f, 0f);
        }
    }
}
    void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            lastMoveX = horizontal;
            lastMoveY = vertical;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection =
            (cameraForward * vertical +
             cameraRight * horizontal).normalized;

        animator.SetFloat("MoveX", horizontal);
        animator.SetFloat("MoveY", vertical);
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetFloat("LastMoveX", lastMoveX);
        animator.SetFloat("LastMoveY", lastMoveY);
    }

    void Move()
    {
        rb.MovePosition(
            rb.position +
            moveDirection * moveSpeed * Time.fixedDeltaTime
        );
    }
}