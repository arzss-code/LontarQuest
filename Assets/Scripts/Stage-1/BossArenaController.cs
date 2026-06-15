using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [Header("Arena Setup")]
    [Tooltip("Dinding gaib atau pintu yang akan aktif memblokir jalan saat pertarungan dimulai")]
    [SerializeField] private GameObject[] magicWalls;
    
    [Header("Boss Setup")]
    [Tooltip("Objek bos fisik yang akan dipantau kematiannya (Kala, Gana, dll)")]
    [SerializeField] private GameObject bossObject;
    
    [Tooltip("Script AI Bos (contoh: KalaAI / GanaAI) yang akan dibangunkan saat Saka masuk arena")]
    [SerializeField] private MonoBehaviour bossAI;

    private bool isArenaLocked = false;

    void Start()
    {
        // 1. FASE TIDUR: Pastikan saat game mulai, dinding mati dan bos tertidur
        SetWallsActive(false);
        
        if (bossAI != null)
        {
            bossAI.enabled = false;
        }
    }

    void Update()
    {
        // 3. FASE KEMENANGAN: Jika arena sedang terkunci, pantau terus objek Bos
        if (isArenaLocked)
        {
            if (bossObject == null) // Jika objek bos dihancurkan (Destroy)
            {
                UnlockArena();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 2. FASE LOCK-IN: Jika Saka memasuki arena dan arena belum terkunci
        if (other.CompareTag("Player") && !isArenaLocked)
        {
            // Jangan mengunci arena jika bos ternyata sudah mati sebelumnya
            if (bossObject != null)
            {
                LockArena();
            }
        }
    }

    private void LockArena()
    {
        isArenaLocked = true;
        SetWallsActive(true); // Munculkan tembok
        
        if (bossAI != null)
        {
            bossAI.enabled = true; // Bangunkan bos
        }
        
        Debug.Log("Arena Terkunci! Pertarungan Bos Dimulai!");
    }

    private void UnlockArena()
    {
        isArenaLocked = false;
        SetWallsActive(false); // Hilangkan tembok
        
        Debug.Log("Bos Dikalahkan! Arena Terbuka!");
        
        // Hancurkan trigger pintu masuk ini agar tidak memakan memori/terpanggil lagi
        Destroy(gameObject);
    }

    private void SetWallsActive(bool isActive)
    {
        // Loop untuk menyalakan/mematikan semua tembok yang didaftarkan
        foreach (GameObject wall in magicWalls)
        {
            if (wall != null)
            {
                wall.SetActive(isActive);
            }
        }
    }
}
