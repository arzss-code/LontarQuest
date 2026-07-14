using UnityEngine;

// Pasang di sebuah GameObject di dalam scene stage (Stage1/Stage2/Stage3).
// Saat scene dibuka, nomor stage ini dicatat ke SaveManager supaya
// SafeHub tahu portal mana yang harus terbuka.
public class StageProgressTracker : MonoBehaviour
{
    [Tooltip("Nomor stage untuk scene ini: 1, 2, atau 3")]
    [SerializeField] private int stageNumber = 1;

    private void Start()
    {
        // Kalau SaveManager belum ada (mis. saat test scene stage langsung tanpa
        // lewat StartMenu), buat instance-nya di sini supaya progress tetap tercatat.
        if (SaveManager.Instance == null)
        {
            Debug.Log("[StageProgressTracker] SaveManager belum ada, membuat instance baru.");
            new GameObject("SaveManager (Auto)").AddComponent<SaveManager>();
        }

        SaveManager.Instance.SetLastStageReached(stageNumber);
        Debug.Log($"[StageProgressTracker] lastStageReached diset ke {stageNumber}");
    }
}
