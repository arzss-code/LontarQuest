using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private BossManager bossManager;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Ada yang masuk Trigger : " + other.name);

        if (triggered)
            return;

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Bukan Player");
            return;
        }

        Debug.Log("Player Masuk Trigger");

        triggered = true;

        if (bossManager != null)
        {
            Debug.Log("BossManager ditemukan");
            bossManager.StartBossFight();
        }
        else
        {
            Debug.LogError("BossManager BELUM di Assign!");
        }
    }
}