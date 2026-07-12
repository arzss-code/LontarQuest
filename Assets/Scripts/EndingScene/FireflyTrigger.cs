using UnityEngine;

public class FireflyTrigger : MonoBehaviour
{
    [SerializeField] private FireflyGuide fireflyGuide;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        fireflyGuide.ContinueGuide();

        Debug.Log(name + " Triggered");
    }
}