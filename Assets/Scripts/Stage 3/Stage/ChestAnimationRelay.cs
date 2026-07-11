using UnityEngine;

public class ChestAnimationRelay : MonoBehaviour
{
    [SerializeField] private TreasureChest treasureChest;

    public void SpawnLoot()
    {
        if (treasureChest != null)
            treasureChest.SpawnLoot();
    }

    public void DisableChest()
    {
        if (treasureChest != null)
            treasureChest.DisableChest();
    }
}