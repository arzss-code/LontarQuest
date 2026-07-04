using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;
    private bool opened;

    //------------------------------------------------

    private void Update()
    {
        if (!playerInside)
            return;

        if (opened)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            OpenChest();
        }
    }

    //------------------------------------------------

    private void OpenChest()
    {
        opened = true;

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        Debug.Log("Chest Opened");
    }

    //------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;
    }
}