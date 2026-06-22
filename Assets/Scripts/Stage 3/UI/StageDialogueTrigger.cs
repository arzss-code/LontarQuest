using UnityEngine;

public class StageDialogueTrigger : MonoBehaviour
{
    [SerializeField] private IntroDialogue dialogue;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Enemy"))
        {
            triggered = true;
            dialogue.StartDialogue();
        }
    }
}