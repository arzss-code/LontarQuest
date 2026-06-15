using UnityEngine;
using System.Collections.Generic;

public class BoonUIManager : MonoBehaviour
{
    public static BoonUIManager Instance { get; private set; }

    [Header("Data Boons Keseluruhan")]
    public List<BoonData> allAvailableBoons;

    [Header("UI Canvas")]
    public GameObject boonSelectionPanel;
    // Nanti disini kita tambahkan referensi ke Button UI untuk memilih 3 Boon
    // public Button boonButton1, boonButton2, boonButton3;

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
        if (boonSelectionPanel != null)
            boonSelectionPanel.SetActive(false);

        // Cari PlayerModifier di arena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerModifier = player.GetComponent<PlayerModifier>();
        }
    }

    public void ShowBoonSelection(BoonType typeNeeded)
    {
        // 1. Filter Boon yang sesuai tipe (Lontara/Batak/Kawi)
        List<BoonData> filteredBoons = new List<BoonData>();
        foreach (var boon in allAvailableBoons)
        {
            if (boon.type == typeNeeded)
            {
                filteredBoons.Add(boon);
            }
        }

        // 2. Acak dan ambil 3 (Logika Randomizer)
        // ... (Akan disambungkan ke tombol UI nanti)

        Debug.Log("Menampilkan UI Pemilihan 3 Boon " + typeNeeded);

        if (boonSelectionPanel != null)
        {
            boonSelectionPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game saat memilih
        }
        else
        {
            // JIKA UI BELUM DIBUAT (Otomatis ambil boon pertama untuk testing)
            if (filteredBoons.Count > 0 && playerModifier != null)
            {
                Debug.LogWarning("UI Canvas belum dipasang! Mengambil Boon pertama secara otomatis untuk testing.");
                SelectBoon(filteredBoons[0]);
            }
        }
    }

    // Fungsi ini dipanggil dari Tombol UI
    public void SelectBoon(BoonData selectedBoon)
    {
        if (playerModifier != null)
        {
            playerModifier.AddBoon(selectedBoon);
        }

        // Tutup UI dan Lanjut Main
        if (boonSelectionPanel != null)
            boonSelectionPanel.SetActive(false);
            
        Time.timeScale = 1f;
    }
}
