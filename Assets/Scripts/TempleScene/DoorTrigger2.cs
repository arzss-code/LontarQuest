using UnityEngine;

public class DoorTrigger2 : MonoBehaviour
{
    private bool alreadyTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            alreadyTriggered = true;

            Debug.Log("PINTU TERKUNCI");
        }
    }
}