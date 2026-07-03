using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    [SerializeField] private EnemyAttack attack;

    private void Awake()
    {
        if (attack == null)
            attack = GetComponentInParent<EnemyAttack>();
    }

    public void SpawnIndicator()
    {
        attack.SpawnIndicator();
    }

    public void SpawnProjectile()
    {
        attack.SpawnProjectile();
    }

    public void EndAttack()
    {
        attack.EndAttack();
    }
}