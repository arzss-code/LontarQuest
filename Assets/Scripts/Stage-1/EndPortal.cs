using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPortal : MonoBehaviour
{
    [Header("Portal Settings")]
    [Tooltip("The name of the scene to load when the player enters the portal")]
    [SerializeField] private string targetSceneName = "Stage2";

    [Tooltip("Optional effect to instantiate when the portal is entered (e.g., transition particle, sound)")]
    [SerializeField] private GameObject travelEffect;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[EndPortal] OnTriggerEnter2D called with: {other.gameObject.name}. Root: {other.transform.root.name}");

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null)
        {
            Debug.Log($"[EndPortal] No PlayerController found on {other.gameObject.name} or its parents.");
            return;
        }

        if (!player.CompareTag("Player"))
        {
            Debug.LogWarning($"[EndPortal] Ignored {player.gameObject.name}: PlayerController found but tag is {player.tag}.");
            return;
        }

        if (!hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"EndPortal triggered! Traveling to: {targetSceneName}");

            if (travelEffect != null)
            {
                Instantiate(travelEffect, transform.position, Quaternion.identity);
            }

            // Save Checkpoint Boon dan HP sebelum pindah stage
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SaveCheckpoint();
            }

            // Load the next scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
