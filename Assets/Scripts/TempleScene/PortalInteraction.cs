using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteraction : MonoBehaviour
{
    public GameObject ePrompt;

    private bool playerNearby;

    private void Start()
    {
        ePrompt.SetActive(false);
    }

    private void Update()
    {
        if(!playerNearby)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            EnterPortal();
        }
    }

    private void EnterPortal()
    {
        Debug.Log("Masuk Portal");

        SceneManager.LoadScene(
            "Stage1"
        );
    }

    private void OnTriggerEnter2D(
        Collider2D other)
    {
        if(!other.CompareTag("Player"))
            return;

        playerNearby = true;

        ePrompt.SetActive(true);
    }

    private void OnTriggerExit2D(
        Collider2D other)
    {
        if(!other.CompareTag("Player"))
            return;

        playerNearby = false;

        ePrompt.SetActive(false);
    }
}