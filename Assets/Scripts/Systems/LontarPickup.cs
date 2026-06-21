using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class LontarPickup : MonoBehaviour
{
    [Header("Quest Updates")]
    [Tooltip("Misi yang muncul setelah Saka mengambil Lontar ini")]
    public string questOnPickup;

    [Header("Visual & Feedback")]
    [Tooltip("Objek teks/ikon yang muncul (misal tulisan '[E] Ambil Lontar') saat Saka mendekat")]
    public GameObject interactPromptUI;
    
    [Tooltip("Kecepatan efek perubahan warna")]
    public float blinkSpeed = 3f;
    
    [Tooltip("Warna cahaya/glow saat berkedip (Default: Emas)")]
    public Color glowColor = new Color(1f, 0.84f, 0f, 1f);

    private bool isPlayerInRange = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        // Pastikan collider sebagai trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }

        // Ambil komponen SpriteRenderer untuk efek berkelip warna
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Update()
    {
        // 1. Efek Berpendar (Glow warna Emas)
        if (spriteRenderer != null)
        {
            float pingPong = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            spriteRenderer.color = Color.Lerp(originalColor, glowColor, pingPong);
        }

        // Deteksi input 'E' jika Saka berada di dekat lontar
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupLontar();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactPromptUI != null) interactPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactPromptUI != null) interactPromptUI.SetActive(false);
        }
    }

    private void PickupLontar()
    {
        Debug.Log("Saka mengambil Lontar misterius!");

        // Panggil sistem pemilihan UI Boon (3 acak tanpa batasan tipe)
        if (BoonUIManager.Instance != null)
        {
            BoonUIManager.Instance.ShowBoonSelection();
        }

        // Perbarui Misi secara dinamis
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(questOnPickup))
        {
            QuestManager.Instance.SetObjective(questOnPickup);
        }

        // Hancurkan objek Lontar di arena
        Destroy(gameObject);
    }
}
