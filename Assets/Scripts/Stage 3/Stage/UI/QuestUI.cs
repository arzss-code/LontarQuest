using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;

    [Header("UI")]
    [SerializeField] private TMP_Text objectiveText;

    //------------------------------------------------


    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //------------------------------------------------

    public void SetObjective(string text)
    {
        if (objectiveText == null)
            return;

        objectiveText.text = text;
    }

    //------------------------------------------------

    public void ClearObjective()
    {
        if (objectiveText == null)
            return;

        objectiveText.text = "";
    }
}