#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class SaveResetTool : EditorWindow
{
    [MenuItem("LontarQuest/Reset All Save Data", false, 50)]
    public static void ResetAllSaveData()
    {
        string gameSavePath = Path.Combine(Application.persistentDataPath, "game_save.json");
        string journalSavePath = Path.Combine(Application.persistentDataPath, "journal_save.json");

        bool deletedAny = false;

        if (File.Exists(gameSavePath))
        {
            File.Delete(gameSavePath);
            Debug.Log("Game Save Data dihapus: " + gameSavePath);
            deletedAny = true;
        }

        if (File.Exists(journalSavePath))
        {
            File.Delete(journalSavePath);
            Debug.Log("Journal Save Data dihapus: " + journalSavePath);
            deletedAny = true;
        }

        // Karena kita menggunakan Singleton, kita harus mematikan state di memori jika game sedang berjalan
        if (Application.isPlaying)
        {
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveData = new SaveManager.GameSaveData();
            }
        }

        if (deletedAny)
        {
            Debug.Log("<color=green><b>Semua data Save berhasil di-reset!</b></color> Mechanic Tips dan Jurnal akan kembali seperti baru pertama main.");
        }
        else
        {
            Debug.Log("<color=yellow>Tidak ada file Save yang ditemukan.</color> Game sudah dalam keadaan bersih.");
        }
    }
}
#endif
