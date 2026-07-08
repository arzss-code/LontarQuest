using UnityEngine;

public class ExitBarrierTrigger : MonoBehaviour
{
    [SerializeField]
    private ExitBarrier barrier;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        barrier.PlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        barrier.PlayerExit();
    }
}