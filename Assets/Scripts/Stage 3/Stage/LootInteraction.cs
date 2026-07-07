using UnityEngine;

public class LootInteraction : MonoBehaviour
{
    [SerializeField] private LootPickup loot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        loot.PlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        loot.PlayerExit();
    }
}