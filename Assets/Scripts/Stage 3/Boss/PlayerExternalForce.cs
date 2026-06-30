using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerExternalForce : MonoBehaviour
{
    private Rigidbody2D rb;

    private bool isPulling = false;

    private Transform pullTarget;

    [SerializeField]
    private float maxPullForce = 12f;

    [SerializeField]
    private float pullRadius = 6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartPull(Transform target)
    {
        pullTarget = target;
        isPulling = true;
    }

    public void StopPull()
    {
        isPulling = false;
    }

    private void FixedUpdate()
    {
        if (!isPulling)
            return;

        if (pullTarget == null)
            return;

        Vector2 dir =
            (pullTarget.position - transform.position);

        float distance = dir.magnitude;

        if (distance <= 0.05f)
            return;

        dir.Normalize();

        // Semakin dekat semakin kuat
        float strength =
            Mathf.Lerp(
                maxPullForce,
                2f,
                distance / pullRadius);

        rb.AddForce(dir * strength, ForceMode2D.Force);
    }
}