using UnityEngine;

public class EndingCameraTrigger : MonoBehaviour
{
    [SerializeField] private EndingCameraSequence cameraSequence;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        cameraSequence.Play();

        gameObject.SetActive(false);
    }
}