using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "TempleScene";

    private bool canEnter = false;

    public void EnableDoor()
    {
        canEnter = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canEnter)
            return;

        if (!other.CompareTag("Player"))
            return;

        SceneManager.LoadScene(nextSceneName);
    }
}