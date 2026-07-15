using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    // --- State Data --- //
    [System.Serializable]
    public class GameSaveData
    {
        public bool hasSeenMechanicTips = false;
        public int lastStageReached = 1; // Stage terakhir yang dimasuki player (1/2/3)
        // Tambahkan data lain di sini nanti jika dibutuhkan
    }

    public GameSaveData SaveData = new GameSaveData();

    // --- Temporary Memory Data (Transisi Antar Scene, Hilang jika Game Ditutup) --- //
    public class RunData
    {
        public bool isRunActive = false;
        public int currentHP = -1; // -1 artinya HP belum tersimpan (harus pakai Max HP)
        public System.Collections.Generic.List<BoonData> activeBoons = new System.Collections.Generic.List<BoonData>();
    }

    public RunData CurrentRun = new RunData();

    // --- Checkpoint System (Untuk Retry Stage) --- //
    public class CheckpointData
    {
        public bool hasCheckpoint = false;
        public int currentHP = -1;
        public System.Collections.Generic.List<BoonData> activeBoons = new System.Collections.Generic.List<BoonData>();
    }

    public CheckpointData CurrentCheckpoint = new CheckpointData();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "game_save.json");
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(SaveData, true);
        File.WriteAllText(GetSavePath(), json);
        Debug.Log("Game Saved to: " + GetSavePath());
    }

    public void LoadGame()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(json);
            if (loadedData != null)
            {
                SaveData = loadedData;
                Debug.Log("Game Loaded");
            }
        }
        else
        {
            Debug.Log("No save file found. Creating new save data.");
        }
    }

    // --- Helper Functions --- //
    public void SetHasSeenMechanicTips(bool value)
    {
        SaveData.hasSeenMechanicTips = value;
        SaveGame();
    }

    public bool HasSeenMechanicTips()
    {
        return SaveData.hasSeenMechanicTips;
    }

    // Catat stage yang sedang dimasuki player. Cukup pakai nilai terbaru
    // supaya portal yang terbuka = stage terakhir yang dilalui.
    public void SetLastStageReached(int stage)
    {
        SaveData.lastStageReached = stage;
        SaveGame();
    }

    public int GetLastStageReached()
    {
        return SaveData.lastStageReached;
    }

    // --- Roguelite Run Management --- //
    public void SaveRunState(int hp, System.Collections.Generic.List<BoonData> boons)
    {
        CurrentRun.isRunActive = true;
        CurrentRun.currentHP = hp;
        CurrentRun.activeBoons = new System.Collections.Generic.List<BoonData>(boons);
    }

    public void ClearRun()
    {
        CurrentRun.isRunActive = false;
        CurrentRun.currentHP = -1;
        CurrentRun.activeBoons.Clear();
        Debug.Log("Run Data Cleared (Player Mati/Mulai Baru)");
    }

    // --- Checkpoint Management --- //
    public void SaveCheckpoint()
    {
        if (CurrentRun.isRunActive)
        {
            CurrentCheckpoint.hasCheckpoint = true;
            CurrentCheckpoint.currentHP = CurrentRun.currentHP;
            CurrentCheckpoint.activeBoons = new System.Collections.Generic.List<BoonData>(CurrentRun.activeBoons);
            Debug.Log($"Checkpoint Tersimpan! Menyimpan {CurrentCheckpoint.activeBoons.Count} Boon.");
        }
    }

    public void RestoreCheckpoint()
    {
        if (CurrentCheckpoint.hasCheckpoint)
        {
            CurrentRun.isRunActive = true;
            CurrentRun.currentHP = CurrentCheckpoint.currentHP;
            CurrentRun.activeBoons = new System.Collections.Generic.List<BoonData>(CurrentCheckpoint.activeBoons);
            Debug.Log("Checkpoint Dipulihkan! Mengembalikan Boon dari awal Stage.");
        }
        else
        {
            ClearRun();
        }
    }
}
