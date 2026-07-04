using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D damageCollider;
    [SerializeField] private TrapDamage trapDamage;

    private void Awake()
    {
        if (damageCollider != null)
            damageCollider.enabled = false;
    }

    //==========================
    // Animation Event
    //==========================

    public void EnableDamage()
    {

        if (damageCollider != null)
            damageCollider.enabled = true;

        if (trapDamage != null)
            trapDamage.ResetDamageCycle();
    }

    public void DisableDamage()
    {

        if (damageCollider != null)
            damageCollider.enabled = false;
    }
}