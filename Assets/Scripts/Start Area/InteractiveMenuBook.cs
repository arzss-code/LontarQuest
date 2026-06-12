using UnityEngine;

public class InteractiveMenuBook : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    GameObject interactPrompt;

    [SerializeField]
    MenuAnimator menuAnimator;

    bool playerInside;

    void Start()
    {
        if(interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if(!playerInside)
            return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            menuAnimator.OpenMenu();

            interactPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInside = true;

            interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInside = false;

            interactPrompt.SetActive(false);
        }
    }
}