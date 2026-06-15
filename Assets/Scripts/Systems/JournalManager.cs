using UnityEngine;
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    public static JournalManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Panel UI Jurnal yang akan muncul saat tekan Tab")]
    public GameObject journalUIPanel;

    // Menyimpan ID Lore yang sudah terbuka
    private HashSet<string> unlockedLoreIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Jurnal harus bertahan antar scene
            LoadJournalData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Toggle UI Jurnal saat menekan Tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (journalUIPanel != null)
            {
                bool isActive = !journalUIPanel.activeSelf;
                journalUIPanel.SetActive(isActive);
                
                // Pause game saat jurnal terbuka
                Time.timeScale = isActive ? 0f : 1f; 
            }
        }
    }

    public void UnlockLore(LoreData newLore)
    {
        if (newLore == null || string.IsNullOrEmpty(newLore.loreID)) return;

        if (!unlockedLoreIDs.Contains(newLore.loreID))
        {
            unlockedLoreIDs.Add(newLore.loreID);
            SaveJournalData();
            Debug.Log("Entri Jurnal Baru Terbuka: " + newLore.monsterName);
            
            // TODO: Tampilkan notifikasi "Jurnal Baru Terbuka!" di layar
        }
    }

    public bool IsLoreUnlocked(string loreID)
    {
        return unlockedLoreIDs.Contains(loreID);
    }

    // --- Sistem Save Game Sederhana (PlayerPrefs) --- //
    private void SaveJournalData()
    {
        // Gabungkan semua ID menjadi satu string dipisahkan koma
        string data = string.Join(",", unlockedLoreIDs);
        PlayerPrefs.SetString("UnlockedJournalLores", data);
        PlayerPrefs.Save();
    }

    private void LoadJournalData()
    {
        string data = PlayerPrefs.GetString("UnlockedJournalLores", "");
        if (!string.IsNullOrEmpty(data))
        {
            string[] ids = data.Split(',');
            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    unlockedLoreIDs.Add(id);
                }
            }
        }
    }
}
