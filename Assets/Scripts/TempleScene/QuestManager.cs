using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TMP_Text questText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateQuest(string quest)
    {
        questText.text = quest;
    }
}