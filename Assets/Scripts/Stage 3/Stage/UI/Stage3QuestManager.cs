using UnityEngine;

public class Stage3QuestManager : MonoBehaviour
{
    public static Stage3QuestManager Instance;

    //------------------------------------------------
    [Header("Objectives")]
    [SerializeField] private ObjectiveData[] objectives;

    private int currentObjectiveIndex = 0;


    private void Start()
    {
        currentObjectiveIndex = 0;

        foreach (ObjectiveData objective in objectives)
        {
            objective.current = 0;
        }

        RefreshObjective();
    }
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

    

    public void SetCurrentObjective(int index)
    {
        if (index < 0 || index >= objectives.Length)
            return;

        currentObjectiveIndex = index;

        objectives[currentObjectiveIndex].current = 0;

        RefreshObjective();
    }

    public ObjectiveType CurrentObjectiveType
    {
        get
        {
            if (CurrentObjective == null)
                return ObjectiveType.ReachRoom;

            return CurrentObjective.type;
        }
    }

    private ObjectiveData CurrentObjective
    {
        get
        {
            if (currentObjectiveIndex >= objectives.Length)
                return null;

            return objectives[currentObjectiveIndex];
        }
    }

    private void RefreshObjective()
    {
        ObjectiveData objective = CurrentObjective;

        if (objective == null)
            return;

        string text = objective.description;

        if (objective.type == ObjectiveType.KillEnemy ||
            objective.type == ObjectiveType.KillBoss)
        {
            text +=
                "\n(" +
                objective.current +
                "/" +
                objective.target +
                ")";
        }

        QuestUI.Instance.SetObjective(text);
    }

    public void NextObjective()
    {
        currentObjectiveIndex++;

        if (currentObjectiveIndex >= objectives.Length)
        {
            QuestUI.Instance.SetObjective("Semua Quest Selesai");
            return;
        }

        RefreshObjective();
    }

    public void AddProgress(int amount = 1)
    {
        ObjectiveData objective = CurrentObjective;

        if (objective == null)
            return;

        objective.current += amount;

        if (objective.current > objective.target)
            objective.current = objective.target;

        RefreshObjective();

        if (objective.current >= objective.target)
        {
            NextObjective();
        }
    }

    public void RegisterEnemyKill()
    {
        if (CurrentObjective == null)
            return;

        if (CurrentObjective.type != ObjectiveType.KillEnemy)
            return;

        AddProgress();
    }

    public void RegisterBossKill()
    {
        if (CurrentObjective == null)
            return;

        if (CurrentObjective.type != ObjectiveType.KillBoss)
            return;

        AddProgress();
    }

    public int CurrentObjectiveIndex
    {
        get
        {
            return currentObjectiveIndex;
        }
    }

    public bool IsCompleted
    {
        get
        {
            return currentObjectiveIndex >= objectives.Length;
        }
    }

    //------------------------------------------------

    
}