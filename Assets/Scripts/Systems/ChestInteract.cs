using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ChestInteract : MonoBehaviour
{
    [Header("Isi Peti (Opsional)")]
    [Tooltip("Item yang akan dikeluarkan peti saat dibuka (Bisa Lontar, Heal, dsb)")]
    public GameObject dropItemPrefab;
    
    [Tooltip("Titik munculnya item dari peti")]
    public Transform dropSpawnPoint;

    [Header("Quest Updates")]
    [Tooltip("Misi yang muncul setelah peti ini berhasil dibuka")]
    public string questOnOpened;

    [Header("Visual & Feedback")]
    [Tooltip("Teks prompt saat Saka di dekat peti, misal '[E] Buka Peti'")]
    public GameObject interactPromptUI;

    private bool isPlayerInRange = false;
    private bool isOpened = false;

    private void Start()
    {
        // Pastikan box collider berfungsi sebagai trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
    }

    private void Update()
    {
        if (!isOpened && isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    private void OpenChest()
    {
        isOpened = true;
        
        // Sembunyikan teks prompt
        if (interactPromptUI != null) interactPromptUI.SetActive(false);

        // Keluarkan isi peti
        if (dropItemPrefab != null)
        {
            Vector3 spawnPos = dropSpawnPoint != null ? dropSpawnPoint.position : transform.position;
            Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
            Debug.Log("Peti dibuka! Item dijatuhkan.");
        }
        else
        {
            Debug.Log("Peti dibuka! (Tidak ada isi)");
        }

        // Perbarui Misi
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(questOnOpened))
        {
            QuestManager.Instance.SetObjective(questOnOpened);
        }

        // Ganti Sprite ke peti terbuka atau hancurkan peti (Opsional, di sini kita hancurkan saja untuk simpelnya)
        Destroy(gameObject);
    }
}
