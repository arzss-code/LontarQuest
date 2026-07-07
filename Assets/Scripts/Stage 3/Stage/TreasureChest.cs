using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Loot")]
    [SerializeField] private Transform lootSpawn;
    [SerializeField] private GameObject lootPrefab;

    [Header("Interaction")]
    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private BoxCollider2D interactionCollider;
    [SerializeField] private BoxCollider2D chestCollider;

    //------------------------------------------------

    private bool playerInside;
    private bool opened;

    //------------------------------------------------

    private void Awake()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

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

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        animator.SetTrigger("Open");
    }

    //------------------------------------------------
    // Dipanggil oleh ChestInteraction.cs
    //------------------------------------------------

    public void PlayerEnter()
    {
        if (opened)
            return;

        playerInside = true;

        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    public void PlayerExit()
    {
        playerInside = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    //------------------------------------------------
    // Animation Event
    //------------------------------------------------

    public void SpawnLoot()
    {
        if (lootPrefab == null)
            return;

        if (lootSpawn == null)
            return;

        Instantiate(
            lootPrefab,
            lootSpawn.position,
            Quaternion.identity);
    }

    //------------------------------------------------
    // Animation Event
    //------------------------------------------------

    public void DisableChest()
    {
        playerInside = false;

        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        if (interactionCollider != null)
            interactionCollider.enabled = false;

        if (chestCollider != null)
            chestCollider.enabled = false;
    }
}