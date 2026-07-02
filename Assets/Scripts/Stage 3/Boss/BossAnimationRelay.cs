using UnityEngine;

public class BossAnimationRelay : MonoBehaviour
{
    [SerializeField] private BossController bossController;
    [SerializeField] private BossTeleport bossTeleport;
    [SerializeField] private BossAttack bossAttack;

    public void OnSpawnFinished()
    {
        if (bossController != null)
            bossController.OnSpawnFinished();
    }

    public void SlamImpact()
    {
        if (bossController != null)
            bossController.SlamImpact();
    }

    public void OnSlamFinished()
    {
        if (bossController != null)
            bossController.OnSlamFinished();
    }

    public void Teleport()
    {
        if (bossTeleport != null)
            bossTeleport.TeleportToFarthestPoint();
    }

    public void FinishTeleport()
    {
        if (bossAttack != null)
            bossAttack.FinishTeleport();
    }
}