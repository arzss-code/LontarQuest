using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Panel atau Container dari teks quest (bisa disembunyikan jika tidak ada quest)")]
    public GameObject questPanel;
    
    [Tooltip("Komponen teks untuk menampilkan tugas saat ini")]
    public TMP_Text objectiveText;

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
        // Default quest kosong di awal
        SetObjective("");
    }

    /// <summary>
    /// Memperbarui teks misi di pojok layar
    /// </summary>
    /// <param name="newObjective">Isi teks misi, contoh: "Kalahkan Kepala Kala"</param>
    public void SetObjective(string newObjective)
    {
        if (string.IsNullOrEmpty(newObjective))
        {
            if (questPanel != null) questPanel.SetActive(false);
            if (objectiveText != null) objectiveText.text = "";
        }
        else
        {
            if (questPanel != null) questPanel.SetActive(true);
            if (objectiveText != null) objectiveText.text = newObjective;
            
            Debug.Log($"[Quest Update] {newObjective}");
        }
    }

    /// <summary>
    /// Menyelesaikan misi saat ini dan menampilkan efek centang/coret (Opsional)
    /// </summary>
    public void CompleteCurrentObjective()
    {
        if (objectiveText != null)
        {
            // Tambahkan tag coret pada teks
            objectiveText.text = "<s>" + objectiveText.text + "</s> (Selesai)";
        }
        Debug.Log("[Quest Update] Objektif Selesai!");
    }
}
