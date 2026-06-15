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
        // Check if player entered the portal and we haven't already triggered it (avoid double loads)
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"EndPortal triggered! Traveling to: {targetSceneName}");

            if (travelEffect != null)
            {
                Instantiate(travelEffect, transform.position, Quaternion.identity);
            }

            // Load the next scene
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
