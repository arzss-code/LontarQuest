using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    [SerializeField] private TreasureChest chest;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        chest.PlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        chest.PlayerExit();
    }
}