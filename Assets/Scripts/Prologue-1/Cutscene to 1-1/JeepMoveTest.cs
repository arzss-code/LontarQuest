using UnityEngine;

public class JeepMoveTest : MonoBehaviour
{
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [SerializeField] private float moveSpeed = 3f;

    private bool hasReachedEnd;

    public bool HasReachedEnd => hasReachedEnd;

    private void Start()
    {
        transform.position = startPoint.position;
    }

    private void Update()
    {
        if (hasReachedEnd)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        float distance =
            Vector3.Distance(
                transform.position,
                endPoint.position
            );

        if (distance < 0.05f)
        {
            hasReachedEnd = true;

            Debug.Log("JEEP SAMPAI TUJUAN");
        }
    }

    public void ResetMovement(
        Transform newStart,
        Transform newEnd)
    {
        startPoint = newStart;
        endPoint = newEnd;

        transform.position = startPoint.position;

        hasReachedEnd = false;
    }
}