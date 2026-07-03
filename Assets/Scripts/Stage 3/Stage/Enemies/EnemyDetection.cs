using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public Transform Target { get; private set; }

    public bool HasTarget => Target != null;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Target = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (Target == other.transform)
            Target = null;
    }
}