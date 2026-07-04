using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private EnemyStats[] enemies;

    [Header("Barrier")]
    [SerializeField] private BarrierController exitBarrier;

    private bool roomCleared;

    private void Update()
    {
        if (roomCleared)
            return;

        CheckRoomClear();
    }

    private void CheckRoomClear()
    {
        foreach (EnemyStats enemy in enemies)
        {
            if (enemy != null)
                return;
        }

        RoomClear();
    }

    private void RoomClear()
    {
        roomCleared = true;

        Debug.Log("ROOM CLEAR!");

        if (exitBarrier != null)
        {
            exitBarrier.Open();
        }
    }
}