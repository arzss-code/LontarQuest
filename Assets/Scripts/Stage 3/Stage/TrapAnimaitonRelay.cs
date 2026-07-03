using UnityEngine;

public class TrapAnimationRelay : MonoBehaviour
{
    [SerializeField] private TrapController trapController;

    // Animation Event
    public void EnableDamage()
    {
        if (trapController != null)
            trapController.EnableDamage();
    }

    // Animation Event
    public void DisableDamage()
    {
        if (trapController != null)
            trapController.DisableDamage();
    }
}