using UnityEngine;

public class Animation02Relay : MonoBehaviour
{
    [SerializeField] private ProjectileIndicator indicator;

    private void Awake()
    {
        if (indicator == null)
            indicator = GetComponent<ProjectileIndicator>();
    }

    public void DestroyIndicator()
    {
        indicator.DestroySelf();
    }
}