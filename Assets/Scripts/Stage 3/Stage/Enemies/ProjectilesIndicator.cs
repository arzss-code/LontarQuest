using UnityEngine;

public class ProjectileIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    public void Initialize(Vector2 startPosition, Vector2 targetPosition)
    {
        transform.position = startPosition;

        Vector2 direction =
            targetPosition - startPosition;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        float length = direction.magnitude;

        Vector3 scale = visual.localScale;

        scale.x = length;

        visual.localScale = scale;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}