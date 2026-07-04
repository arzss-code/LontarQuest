using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 20;

    private bool hasDamagedPlayer = false;

    public void ResetDamageCycle()
    {
        hasDamagedPlayer = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //----------------------------------------
        // Hanya Player
        //----------------------------------------

        if (!other.CompareTag("Player"))
            return;

        //----------------------------------------
        // Sudah terkena pada siklus ini?
        //----------------------------------------

        if (hasDamagedPlayer)
            return;

        PlayerStats playerStats =
            other.GetComponent<PlayerStats>();

        if (playerStats == null)
            playerStats =
                other.GetComponentInParent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);

            hasDamagedPlayer = true;
        }
    }
}