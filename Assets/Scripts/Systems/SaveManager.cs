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
}
