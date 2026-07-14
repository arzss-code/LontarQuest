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

    private bool AreUIElementsDestroyed()
    {
        if (boonSelectionPanel == null) return true;
        if (boonUIElements == null || boonUIElements.Count == 0) return true;
        
        foreach (var element in boonUIElements)
        {
            if (element == null) return true;
        }
        return false;
    }

    private void RebindUI()
    {
        BoonUIElement[] foundElements = FindObjectsOfType<BoonUIElement>(true); // true = cari yang non-aktif juga
        if (foundElements.Length > 0)
        {
            boonUIElements = new List<BoonUIElement>(foundElements);
            boonSelectionPanel = foundElements[0].transform.parent.gameObject;
            Debug.Log("[BoonUIManager] UI Canvas berhasil ditemukan dan diikat secara otomatis.");
        }
        else
        {
            boonUIElements = new List<BoonUIElement>();
            boonSelectionPanel = null;
            Debug.LogWarning("[BoonUIManager] Tidak menemukan BoonUIElement di scene aktif ini.");
        }
    }

    private void Start()
    {
        // Auto-Binding: Cari UI Canvas secara otomatis agar user tidak perlu drag-and-drop manual
        if (boonSelectionPanel == null || boonUIElements == null || boonUIElements.Count == 0 || AreUIElementsDestroyed())
        {
            RebindUI();
        }

        if (boonSelectionPanel != null)
        {
            boonSelectionPanel.SetActive(false);
        }

        // Cari PlayerModifier di arena
        playerModifier = FindFirstObjectByType<PlayerModifier>();
    }

    public void ShowBoonSelection()
    {
        // Selalu re-bind UI jika referensi saat ini hilang, rusak, atau dari scene sebelumnya
        if (boonSelectionPanel == null || boonUIElements == null || boonUIElements.Count == 0 || AreUIElementsDestroyed())
        {
            RebindUI();
        }

        // Pastikan kita punya referensi ke PlayerModifier sebelum menyaring
        if (playerModifier == null)
        {
            playerModifier = FindFirstObjectByType<PlayerModifier>();
        }

        // 1. Saring BOON berdasarkan Slot dan Level
        List<BoonData> validBoons = new List<BoonData>();

        foreach (BoonData boon in allAvailableBoons)
        {
            if (boon == null) continue; // Antisipasi jika ada element null di Inspector list

            if (playerModifier != null && playerModifier.HasBoonInSlot(boon.slot, out BoonData currentBoon))
            {
                // Jika slot sudah terisi oleh boon yang SAMA persis
                if (currentBoon.boonName == boon.boonName)
                {
                    // Tawarkan upgrade jika ada next level
                    if (currentBoon.nextLevelBoon != null)
                    {
                        if (!validBoons.Contains(currentBoon.nextLevelBoon))
                            validBoons.Add(currentBoon.nextLevelBoon);
                    }
                    // Jika tidak ada nextLevelBoon (sudah max level), buang dari opsi
                }
                else
                {
                    // Slot sudah terisi tapi dengan boon yang BERBEDA (opsi Replace)
                    if (!validBoons.Contains(boon))
                        validBoons.Add(boon);
                }
            }
            else
            {
                // Slot masih kosong
                if (!validBoons.Contains(boon))
                    validBoons.Add(boon);
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
            playerModifier = FindFirstObjectByType<PlayerModifier>();
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
