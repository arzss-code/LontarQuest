using UnityEngine;
using UnityEngine.Events;

public class InteractionArea : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptCanvas;

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Event")]
    public UnityEvent OnInteract;

    private bool playerInside;

    //------------------------------------------------

    private void Awake()
    {
        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    //------------------------------------------------

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("Interact");

            OnInteract?.Invoke();
        }
    }

    //------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (promptCanvas != null)
            promptCanvas.SetActive(true);
    }

    //------------------------------------------------

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }
}