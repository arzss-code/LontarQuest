using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D bossCollider;
    [SerializeField] private GameObject aoeIndicator;
    [SerializeField] private BossAttack bossAttack;

    [Header("Effects")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private ParticleSystem slamDust;

    [Header("Slam Attack")]
    [SerializeField] private float slamRadius = 2.5f;
    [SerializeField] private int slamDamage = 25;
    [SerializeField] private LayerMask playerLayer;
    [Header("Shockwave")]
    [SerializeField] private float shockwaveRadius = 3.5f;
    [SerializeField] private float shockwaveForce = 10f;

    

    private Transform player;

    // Boss belum aktif saat awal scene
    private bool isAwaken = false;
    public bool IsAwaken => isAwaken;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Player dengan Tag 'Player' tidak ditemukan.");

        animator.enabled = false;
        spriteRenderer.enabled = false;

        if (bossCollider != null)
            bossCollider.enabled = false;
    }

    private void Update()
    {
        if (!isAwaken)
            return;

        if (player == null)
            return;

        UpdateDirection();
    }

    /// <summary>
    /// Dipanggil oleh BossManager untuk memunculkan Boss.
    /// </summary>
    public void ShowBoss()
    {
        isAwaken = false;

        spriteRenderer.enabled = true;

        if (bossCollider != null)
            bossCollider.enabled = true;

        animator.enabled = true;

        animator.Play("BossSpawn", 0, 0f);
    }

    /// <summary>
    /// Dipanggil oleh Animation Event pada frame terakhir BossSpawn.
    /// </summary>
    public void OnSpawnFinished()
    {
        Debug.Log("Boss Spawn Finished!");

        Awaken();
    }

    private void UpdateDirection()
    {
        Vector2 direction = player.position - transform.position;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetInteger("Direction", direction.x > 0 ? 3 : 2);
        }
        else
        {
            animator.SetInteger("Direction", direction.y > 0 ? 1 : 0);
        }
    }

    /// <summary>
    /// Boss mulai aktif dan bisa menghadap Player.
    /// </summary>
    private void Awaken()
    {
        isAwaken = true;

        Debug.Log("Bahtara Kala telah bangkit!");
    }

    /// <summary>
    /// Digunakan saat Boss mati atau arena di-reset.
    /// </summary>
    
    public void StartSlam()
    {
        animator.SetTrigger("Slam");
    }

    public void SlamImpact()
    {
        Debug.Log("SLAM IMPACT");

        DoSlamDamage();
        if (slamDust != null)
        {
            slamDust.Play();
        }

        DoShockwave();

        if (cameraShake != null)
        {
            cameraShake.Shake(
                3f,     // Amplitude
                4f,     // Frequency
                0.18f); // Duration
        }
    }
    
    private void DoSlamDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            slamRadius,
            playerLayer);

        if (hit == null)
            return;

        PlayerStats playerStats =
            hit.GetComponentInParent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.TakeDamage(slamDamage);

            Debug.Log("Player terkena Slam!");
        }
    }

    private void DoShockwave()
    {
        

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            shockwaveRadius,
            playerLayer);

        if (hit == null)
            return;

        PlayerController playerController =
            hit.GetComponentInParent<PlayerController>();

        if (playerController == null)
            return;

        Vector2 knockDirection =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        playerController.ApplyKnockback(knockDirection);

        
    }
    
    public void OnSlamFinished()
    {
        Debug.Log("SLAM FINISHED");

        if (bossAttack != null)
            bossAttack.FinishAttack();
    }
    
    public void Sleep()
    {
        isAwaken = false;

        animator.enabled = false;
        spriteRenderer.enabled = false;

        if (bossCollider != null)
            bossCollider.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Radius Damage
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            slamRadius);

        // Radius Shockwave
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            shockwaveRadius);
    }
}