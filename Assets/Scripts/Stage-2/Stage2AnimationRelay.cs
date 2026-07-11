using UnityEngine;

public class Stage2AnimationRelay : MonoBehaviour
{
    // Fungsi event untuk Dwarapala (Melee)
    public void OnAttackHit()
    {
        Debug.Log("Animation Event: OnAttackHit");
        // Nanti akan dihubungkan ke Stage2EnemyAttack untuk kalkulasi damage AoE
    }

    public void OnAttackEnd()
    {
        Debug.Log("Animation Event: OnAttackEnd");
        // Nanti akan dihubungkan ke Stage2EnemyAttack/Movement untuk mengembalikan state normal
    }

    // Fungsi event untuk Yaksa (Ranged)
    public void OnShootProjectile()
    {
        Debug.Log("Animation Event: OnShootProjectile");
        // Nanti akan dihubungkan ke Stage2EnemyAttack untuk spawn proyektil panah energi
    }

    // Fungsi event untuk Kematian (Dwarapala & Yaksa)
    public void OnDeathEnd()
    {
        Debug.Log("Animation Event: OnDeathEnd");
        // Nanti akan dihubungkan ke Stage2EnemyStats untuk men-destroy object
    }
}
