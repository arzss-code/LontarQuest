using UnityEngine;
using System.Collections.Generic;

public class BoonUIManager : MonoBehaviour
{
    public static BoonUIManager Instance { get; private set; }

    [Header("Data Boons Keseluruhan")]
    public List<BoonData> allAvailableBoons;

    [Header("UI Canvas")]
    public GameObject boonSelectionPanel;
    
    [Tooltip("Daftar komponen BoonUIElement (biasanya 3 tombol) yang ada di dalam panel")]
    public List<BoonUIElement> boonUIElements;

    private PlayerModifier playerModifier;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Auto-Binding: Cari UI Canvas secara otomatis agar user tidak perlu drag-and-drop manual
        if (boonSelectionPanel == null || boonUIElements == null || boonUIElements.Count == 0)
        {
            BoonUIElement[] foundElements = FindObjectsOfType<BoonUIElement>(true); // true = cari yang non-aktif juga
            if (foundElements.Length > 0)
            {
                boonUIElements = new List<BoonUIElement>(foundElements);
                boonSelectionPanel = foundElements[0].transform.parent.gameObject;
                Debug.Log("[BoonUIManager] UI Canvas berhasil ditemukan dan diikat secara otomatis.");
            }
        }

        if (boonSelectionPanel != null)
        {
            boonSelectionPanel.SetActive(false);
        }

        // Cari PlayerModifier di arena menggunakan tipe komponennya (bukan Tag, untuk menghindari salah objek)
        playerModifier = FindObjectOfType<PlayerModifier>();
    }

    public void ShowBoonSelection()
    {
        // Pastikan kita punya referensi ke PlayerModifier sebelum menyaring
        if (playerModifier == null)
        {
            playerModifier = FindObjectOfType<PlayerModifier>();
        }

        // 1. Saring BOON berdasarkan Slot dan Level
        List<BoonData> validBoons = new List<BoonData>();

        foreach (BoonData boon in allAvailableBoons)
        {
            if (playerModifier != null)
            {
                bool hasElement = playerModifier.HasBoonOfType(boon.type, out BoonData currentBoon);
                
                if (boon.level == 1)
                {
                    // Hanya tawarkan Lv 1 jika belum punya elemen ini
                    if (!hasElement && !validBoons.Contains(boon))
                    {
                        validBoons.Add(boon);
                    }
                }
                else if (boon.level == 2)
                {
                    // Hanya tawarkan Lv 2 jika sudah punya Lv 1 dari elemen ini
                    if (hasElement && currentBoon.level == 1 && !validBoons.Contains(boon))
                    {
                        validBoons.Add(boon);
                    }
                }
            }
            else
            {
                // Fallback jika PlayerModifier tidak ditemukan
                if (boon.level == 1 && !validBoons.Contains(boon))
                {
                    validBoons.Add(boon);
                }
            }
        }

        // 2. Acak urutan daftar boon (Fisher-Yates shuffle)
        for (int i = 0; i < validBoons.Count; i++)
        {
            BoonData temp = validBoons[i];
            int randomIndex = Random.Range(i, validBoons.Count);
            validBoons[i] = validBoons[randomIndex];
            validBoons[randomIndex] = temp;
        }

        // 3. Tampilkan di UI
        if (boonSelectionPanel != null && boonUIElements.Count > 0)
        {
            boonSelectionPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game saat memilih

            // Setup masing-masing tombol
            for (int i = 0; i < boonUIElements.Count; i++)
            {
                if (i < validBoons.Count)
                {
                    // Masih ada boon yang tersisa di daftar yang terfilter
                    boonUIElements[i].Setup(validBoons[i]);
                }
                else
                {
                    // Jika boon kurang dari jumlah tombol, sembunyikan tombol berlebih
                    boonUIElements[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            // JIKA UI BELUM DIBUAT (Otomatis ambil boon pertama untuk testing)
            if (validBoons.Count > 0 && playerModifier != null)
            {
                Debug.LogWarning("UI Canvas belum dipasang! Mengambil Boon pertama secara otomatis untuk testing.");
                SelectBoon(validBoons[0]);
            }
        }
    }

    // Fungsi ini dipanggil dari Tombol UI via BoonUIElement
    public void SelectBoon(BoonData selectedBoon)
    {
        // Pastikan kita punya referensi ke PlayerModifier
        if (playerModifier == null)
        {
            playerModifier = FindObjectOfType<PlayerModifier>();
        }

        if (playerModifier != null)
        {
            playerModifier.AddBoon(selectedBoon);
            Debug.Log($"[BoonUIManager] Sukses menerapkan {selectedBoon.boonName}!");
            
            // Log semua status untuk membuktikan efeknya masuk
            if (selectedBoon.movementSpeedBonus > 0) Debug.Log($"-> Move Speed Naik: +{selectedBoon.movementSpeedBonus * 100}%");
            if (selectedBoon.attackSpeedBonus > 0) Debug.Log($"-> Attack Speed Naik: +{selectedBoon.attackSpeedBonus * 100}%");
            if (selectedBoon.damageReduction > 0) Debug.Log($"-> Damage Reduction: {selectedBoon.damageReduction * 100}%");
            if (selectedBoon.extraStamina > 0) Debug.Log($"-> Extra Stamina Naik: +{selectedBoon.extraStamina}");
            if (selectedBoon.hasElementalEffect) Debug.Log($"-> Efek Elemental: AKTIF!");
        }
        else
        {
            Debug.LogError("[BoonUIManager] PlayerModifier tidak ditemukan! Pastikan objek Saka memiliki tag 'Player' dan komponen PlayerModifier.");
        }
        
        // Tutup UI dan Lanjut Main
        if (boonSelectionPanel != null)
            boonSelectionPanel.SetActive(false);
            
        Time.timeScale = 1f;
    }
}
