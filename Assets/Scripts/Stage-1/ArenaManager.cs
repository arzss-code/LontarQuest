using UnityEngine;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour
{
    [Header("Arena Configuration")]
    [Tooltip("The door GameObject that blocks progression when activated")]
    [SerializeField] private GameObject door;

    [Tooltip("List of enemies that must be defeated to unlock the door")]
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    [Header("Status Indicators (Read Only)")]
    [SerializeField] private bool isActivated = false;
    [SerializeField] private bool isCleared = false;

    private void Start()
    {
        // Ensure the door is inactive at the start of the level so the player can enter
        if (door != null)
        {
            door.SetActive(false);
        }
    }

    private void Update()
    {
        // Only monitor enemy status if the arena lock is currently active
        if (isActivated && !isCleared)
        {
            if (AreAllEnemiesDefeated())
            {
                UnlockArena();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Trigger the lock when the player enters the room
        if (other.CompareTag("Player") && !isActivated && !isCleared)
        {
            LockArena();
        }
    }

    private void LockArena()
    {
        isActivated = true;
        
        if (door != null)
        {
            door.SetActive(true);
            Debug.Log("Arena Locked! Defeat all enemies to proceed.");
        }
        else
        {
            Debug.LogWarning("Arena door is not assigned on " + gameObject.name);
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        // Check if any enemy is still alive
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                return false; // Found at least one enemy still alive
            }
        }
        return true; // All elements in list are null (destroyed)
    }

    private void UnlockArena()
    {
        isCleared = true;
        isActivated = false;

        if (door != null)
        {
            door.SetActive(false);
            Debug.Log("Arena Cleared! Door unlocked.");
        }
    }

    // Helper method to dynamically add enemies if they are spawned at runtime
    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }
}
