using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class QuestTrigger : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private int objectiveIndex;
    

    private bool triggered;

    //------------------------------------------------

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    //------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        Stage3QuestManager quest = Stage3QuestManager.Instance;

        if (quest == null)
            return;

        triggered = true;

        quest.SetCurrentObjective(objectiveIndex);
    }
}