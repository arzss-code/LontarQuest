using UnityEngine;

public class Stage2AnimationRelay : MonoBehaviour
{
    private Stage2EnemyAttack attack;
    private Stage2EnemyStats stats;

    private void Awake()
    {
        // Mencari komponen di objek yang sama atau objek parent
        // (menampung case jika Animator dan Relay diletakkan di visual parent / child)
        attack = GetComponent<Stage2EnemyAttack>();
        if (attack == null) attack = GetComponentInParent<Stage2EnemyAttack>();

        stats = GetComponent<Stage2EnemyStats>();
        if (stats == null) stats = GetComponentInParent<Stage2EnemyStats>();
    }

    // Fungsi event untuk Dwarapala (Melee)
    public void OnAttackHit()
    {
        if (attack != null)
        {
            attack.OnAttackHit();
        }
        else
        {
            Debug.LogWarning($"[Stage2AnimationRelay] OnAttackHit dipanggil tapi Stage2EnemyAttack tidak ditemukan di {gameObject.name}!");
        }
    }

    public void OnAttackEnd()
    {
        if (attack != null)
        {
            attack.OnAttackEnd();
        }
        else
        {
            Debug.LogWarning($"[Stage2AnimationRelay] OnAttackEnd dipanggil tapi Stage2EnemyAttack tidak ditemukan di {gameObject.name}!");
        }
    }

    // Fungsi event untuk Yaksa (Ranged)
    public void OnShootProjectile()
    {
        if (attack != null)
        {
            attack.OnShootProjectile();
        }
        else
        {
            Debug.LogWarning($"[Stage2AnimationRelay] OnShootProjectile dipanggil tapi Stage2EnemyAttack tidak ditemukan di {gameObject.name}!");
        }
    }

    // Fungsi event untuk Kematian (Dwarapala & Yaksa)
    public void OnDeathEnd()
    {
        if (stats != null)
        {
            stats.OnDeathEnd();
        }
        else
        {
            Debug.LogWarning($"[Stage2AnimationRelay] OnDeathEnd dipanggil tapi Stage2EnemyStats tidak ditemukan di {gameObject.name}!");
        }
    }
}
