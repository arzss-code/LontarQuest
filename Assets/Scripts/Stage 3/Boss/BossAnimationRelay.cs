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
}