using UnityEngine;

public class Stage2QuestManager : QuestManager
{
    private string activeBaseText = "";
    private int currentTargetCount = 0;
    private int currentProgressCount = 0;

    /// <summary>
    /// Memulai objektif misi dengan perhitungan angka progress
    /// </summary>
    public void StartProgressObjective(string description, int targetCount)
    {
        activeBaseText = description;
        currentTargetCount = targetCount;
        currentProgressCount = 0;
        UpdateProgressUI();
    }

    /// <summary>
    /// Memperbarui jumlah kemajuan musuh mati
    /// </summary>
    public void SetProgress(int currentProgress)
    {
        if (currentTargetCount <= 0) return;
        currentProgressCount = Mathf.Clamp(currentProgress, 0, currentTargetCount);
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        // Contoh: "Kalahkan Dwarapala Penjaga (2/5)"
        string progressText = $"{activeBaseText} ({currentProgressCount}/{currentTargetCount})";
        SetObjective(progressText);
    }
}
