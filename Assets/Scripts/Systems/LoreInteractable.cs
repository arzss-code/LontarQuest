using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LoreInteractable : MonoBehaviour
{
    [Header("Pengaturan Lore")]
    public LoreData loreDataToUnlock;
    
    [Tooltip("Tombol untuk berinteraksi, misalnya E")]
    public KeyCode interactKey = KeyCode.E;

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Pastikan collider diset ke isTrigger
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            if (JournalManager.Instance != null && loreDataToUnlock != null)
            {
                JournalManager.Instance.UnlockLore(loreDataToUnlock);
                
                // Tambahkan efek partikel atau suara di sini jika perlu
                Debug.Log("Saka mencatat informasi di Buku Panduan Ekologi Gaib!");
                
                // Matikan objek ini agar tidak bisa di-interact lagi
                this.enabled = false; 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Tekan " + interactKey + " untuk meneliti Arca/Peninggalan ini.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
