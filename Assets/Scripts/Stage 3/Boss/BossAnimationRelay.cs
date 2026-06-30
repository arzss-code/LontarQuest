using UnityEngine;

public class BossAnimationRelay : MonoBehaviour
{
    [SerializeField] private BossController bossController;

    public void OnSpawnFinished()
    {
        if (bossController != null)
        {
            bossController.OnSpawnFinished();
        }
    }

    public void SlamImpact()
    {
        if (bossController != null)
        {
            bossController.SlamImpact();
        }
    }

    public void OnSlamFinished()
    {
        if (bossController != null)
        {
            bossController.OnSlamFinished();
        }
    }
}