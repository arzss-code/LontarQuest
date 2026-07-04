using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDetection detection;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform projectileSpawn;

    [Header("Prefabs")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject projectilePrefab;

    [Header("Combat")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float attackCooldown = 2f;

    //------------------------------------------------

    private bool isAttacking;
    private float nextAttackTime;

    private Vector2 lockedPosition;
    public bool IsAttacking => isAttacking;


    public void TryAttack()
    {
        if (isAttacking)
            return;

        if (!detection.HasTarget)
            return;

        if (Time.time < nextAttackTime)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                detection.Target.position);

        if (distance > attackRange)
            return;

        //---------------------------------

        lockedPosition =
            detection.Target.position;

        isAttacking = true;

        nextAttackTime =
            Time.time + attackCooldown;

        animator.SetTrigger("Attack");
    }

    

    public void SpawnIndicator()
    {
        if (indicatorPrefab == null)
        {
            Debug.LogError("Indicator Prefab belum diisi.");
            return;
        }

        GameObject obj = Instantiate(
            indicatorPrefab,
            projectileSpawn.position,
            Quaternion.identity);

        ProjectileIndicator indicator =
            obj.GetComponent<ProjectileIndicator>();

        if (indicator == null)
        {
            Debug.LogError("ProjectileIndicator component tidak ditemukan.");
            return;
        }

        indicator.Initialize(
            projectileSpawn.position,
            lockedPosition);
    }

    public void SpawnProjectile()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab belum diisi.");
            return;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            projectileSpawn.position,
            Quaternion.identity);

        VoidShot shot = projectile.GetComponent<VoidShot>();

        if (shot == null)
        {
            Debug.LogError("VoidShot component tidak ditemukan.");
            return;
        }

        shot.Initialize(
            projectileSpawn.position,
            lockedPosition);

        Collider2D owner =
            GetComponent<Collider2D>();

        if(owner != null)
        {
            shot.SetOwner(owner);
        }
    }

    public void EndAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange);

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(
            lockedPosition,
            0.15f);
    }
}