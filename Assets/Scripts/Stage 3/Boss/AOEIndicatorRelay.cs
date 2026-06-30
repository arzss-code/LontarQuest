using UnityEngine;

public class AOEIndicatorRelay : MonoBehaviour
{
    [SerializeField] private BossController bossController;

    public void StartSlam()
    {
        if (bossController != null)
            bossController.StartSlam();
    }
}