using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Stage2RoomManager : MonoBehaviour
{
    [Header("Door Configuration")]
    [SerializeField] private GameObject entryDoor;
    [SerializeField] private GameObject[] exitDoors;
    [SerializeField] private GameObject[] nextRoomBlockades;

    [Header("Visual Direction Guide")]
    [SerializeField] private GameObject guideParticlePrefab;

    [Header("Enemies in Room")]
    [SerializeField] private List<GameObject> enemiesInRoom = new List<GameObject>();

    [Header("Quest Setup")]
    [SerializeField] private string questOnEnter = "Bersihkan Ruangan";
    [SerializeField] private string questOnClear = "Temukan Jalan Keluar";

    [Header("Split Enemies Configuration (Optional)")]
    [SerializeField] private bool splitEnemiesTracking = false;
    [SerializeField] private List<GameObject> miniBossEnemies = new List<GameObject>();
    [SerializeField] private string miniBossQuestLabel = "Kalahkan Dwarapala Raksasa";
    [SerializeField] private string normalQuestLabel = "Kalahkan Dwarapala Biasa";

    private bool isRoomActive = false;
    private bool isRoomCleared = false;
    private int initialEnemyCount = 0;
    private int lastDeadCount = -1;

    private int initialMiniBossCount = 0;
    private int lastMiniBossDeadCount = -1;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(false);
        }

        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(true);
            }
        }

        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null) enemy.SetActive(false);
        }

        initialEnemyCount = enemiesInRoom.Count;

        if (splitEnemiesTracking)
        {
            foreach (GameObject enemy in miniBossEnemies)
            {
                if (enemy != null) enemy.SetActive(false);
            }
            initialMiniBossCount = miniBossEnemies.Count;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRoomActive && !isRoomCleared)
        {
            if (IsPlayerFullyInside(other))
            {
                ActivateRoom();
            }
        }
    }

    private bool IsPlayerFullyInside(Collider2D playerCol)
    {
        Collider2D roomCol = GetComponent<Collider2D>();
        if (roomCol == null || playerCol == null) return false;

        Bounds roomBounds = roomCol.bounds;
        Bounds playerBounds = playerCol.bounds;

        // Cek apakah seluruh area bounds player berada di dalam bounds ruangan
        return playerBounds.min.x >= roomBounds.min.x &&
               playerBounds.max.x <= roomBounds.max.x &&
               playerBounds.min.y >= roomBounds.min.y &&
               playerBounds.max.y <= roomBounds.max.y;
    }

    private void ActivateRoom()
    {
        isRoomActive = true;
        if (entryDoor != null) entryDoor.SetActive(true);

        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null) enemy.SetActive(true);
        }

        if (splitEnemiesTracking)
        {
            foreach (GameObject enemy in miniBossEnemies)
            {
                if (enemy != null) enemy.SetActive(true);
            }
        }

        // Jalankan UI Counter Misi jika QuestManager adalah tipe Stage2, atau gunakan fallback string format
        if (QuestManager.Instance != null)
        {
            if (splitEnemiesTracking)
            {
                string progressText = $"{miniBossQuestLabel} (0/{initialMiniBossCount}) | {normalQuestLabel} (0/{initialEnemyCount})";
                QuestManager.Instance.SetObjective(progressText);
            }
            else
            {
                if (initialEnemyCount > 0)
                {
                    if (QuestManager.Instance is Stage2QuestManager s2Quest)
                    {
                        s2Quest.StartProgressObjective(questOnEnter, initialEnemyCount);
                    }
                    else
                    {
                        QuestManager.Instance.SetObjective($"{questOnEnter} (0/{initialEnemyCount})");
                    }
                }
                else
                {
                    QuestManager.Instance.SetObjective(questOnEnter);
                }
            }
        }
    }

    private void Update()
    {
        if (isRoomActive && !isRoomCleared)
        {
            // Hitung jumlah musuh yang masih hidup
            int currentAlive = 0;
            for (int i = enemiesInRoom.Count - 1; i >= 0; i--)
            {
                if (enemiesInRoom[i] != null) currentAlive++;
            }

            int deadCount = initialEnemyCount - currentAlive;

            if (splitEnemiesTracking)
            {
                int currentMiniBossAlive = 0;
                for (int i = miniBossEnemies.Count - 1; i >= 0; i--)
                {
                    if (miniBossEnemies[i] != null) currentMiniBossAlive++;
                }

                int deadMiniBoss = initialMiniBossCount - currentMiniBossAlive;

                // Update UI progress hanya saat ada perubahan angka
                if (deadCount != lastDeadCount || deadMiniBoss != lastMiniBossDeadCount)
                {
                    lastDeadCount = deadCount;
                    lastMiniBossDeadCount = deadMiniBoss;

                    if (QuestManager.Instance != null)
                    {
                        string progressText = $"{miniBossQuestLabel} ({deadMiniBoss}/{initialMiniBossCount}) | {normalQuestLabel} ({deadCount}/{initialEnemyCount})";
                        QuestManager.Instance.SetObjective(progressText);
                    }
                }

                if (currentAlive == 0 && currentMiniBossAlive == 0)
                {
                    RoomCleared();
                }
            }
            else
            {
                // Update UI progress hanya saat ada perubahan angka
                if (deadCount != lastDeadCount)
                {
                    lastDeadCount = deadCount;
                    if (QuestManager.Instance != null)
                    {
                        if (QuestManager.Instance is Stage2QuestManager s2Quest)
                        {
                            s2Quest.SetProgress(deadCount);
                        }
                        else if (initialEnemyCount > 0)
                        {
                            QuestManager.Instance.SetObjective($"{questOnEnter} ({deadCount}/{initialEnemyCount})");
                        }
                    }
                }

                if (currentAlive == 0)
                {
                    RoomCleared();
                }
            }
        }
    }

    private void RoomCleared()
    {
        isRoomCleared = true;
        isRoomActive = false;

        if (entryDoor != null) entryDoor.SetActive(false);

        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(true);
        }

        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(false);
            }
        }

        if (guideParticlePrefab != null)
        {
            SpawnGuideParticle();
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective(questOnClear);
        }
    }

    private void SpawnGuideParticle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform targetDoor = null;
        if (exitDoors != null && exitDoors.Length > 0 && exitDoors[0] != null)
        {
            targetDoor = exitDoors[0].transform;
        }
        else if (nextRoomBlockades != null && nextRoomBlockades.Length > 0 && nextRoomBlockades[0] != null)
        {
            targetDoor = nextRoomBlockades[0].transform;
        }

        if (targetDoor != null)
        {
            GameObject guide = Instantiate(guideParticlePrefab, player.transform.position, Quaternion.identity);
            GuideParticle script = guide.GetComponent<GuideParticle>();
            if (script != null)
            {
                script.Init(targetDoor);
            }
        }
    }
}
