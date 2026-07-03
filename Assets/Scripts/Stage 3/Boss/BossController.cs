using UnityEngine;
using System.Collections;

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
    [SerializeField] private ParticleSystem shieldBreakParticle;

    [Header("Hit Flash")]
    private Material hitMaterial;
    private static readonly int FlashID = Shader.PropertyToID("_Flash");
    private Coroutine flashRoutine;

    [Header("Slam Attack")]
    [SerializeField] private float slamRadius = 2.5f;
    [SerializeField] private int slamDamage = 25;
    [SerializeField] private LayerMask playerLayer;
    [Header("Shockwave")]
    [SerializeField] private float shockwaveRadius = 3.5f;
    [SerializeField] private float shockwaveForce = 10f;
    [SerializeField] private BossStats bossStats;
    [SerializeField] private BossShieldBreakUI shieldBreakUI;

    

    private Transform player;
    private Vector3 impactPoint;

    // Boss belum aktif saat awal scene
    private bool isAwaken = false;
    public bool IsAwaken => isAwaken;

    private void Start()
    {
        hitMaterial = spriteRenderer.material;
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

    public void PlayHitFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        hitMaterial.SetFloat(FlashID, 1f);

        yield return new WaitForSeconds(0.08f);

        hitMaterial.SetFloat(FlashID, 0f);

        flashRoutine = null;
    }

    /// <summary>
    /// Digunakan saat Boss mati atau arena di-reset.
    /// </summary>
    
    public void StartSlam()
    {
        animator.SetTrigger("Slam");
    }
    public void SetImpactPoint(Vector3 point)
    {
        impactPoint = point;
    }

    public void SlamImpact()
    {
        Debug.Log("SLAM IMPACT");

        // Pindahkan Dust ke titik impact
        if (slamDust != null)
        {
            slamDust.transform.position = impactPoint;
            slamDust.Play();
        }

        DoSlamDamage();

        DoShockwave();

        if (cameraShake != null)
        {
            cameraShake.Shake(
                3f,
                4f,
                0.18f);
        }
    }
    
    private void DoSlamDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            impactPoint,
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
            impactPoint,
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
            (Vector2)impactPoint).normalized;

        playerController.ApplyKnockback(knockDirection);
    }
    
    public void OnSlamFinished()
    {
        Debug.Log("SLAM FINISHED");

        if (bossAttack != null)
            bossAttack.FinishAttack();
    }

    public void PlayShieldBreakEffect()
    {
        Debug.Log("Shield Break Effect");

        //----------------------------------------
        // Particle
        //----------------------------------------

        if (shieldBreakParticle != null)
        {
            shieldBreakParticle.transform.position = transform.position;
            shieldBreakParticle.Play();
        }

        //----------------------------------------
        // Camera Shake
        //----------------------------------------

        if (cameraShake != null)
        {
            cameraShake.Shake(
                1.5f,
                2f,
                0.15f);
        }
    }

    private void OnEnable()
    {
        if (bossStats != null)
        {
            bossStats.OnShieldBroken += OnShieldBroken;
            bossStats.OnShieldRecovered += OnShieldRecovered;
        }
    }

    private void OnDisable()
    {
        if (bossStats != null)
        {
            bossStats.OnShieldBroken -= OnShieldBroken;
            bossStats.OnShieldRecovered -= OnShieldRecovered;
        }
    }
    
    public void Sleep()
    {
        isAwaken = false;

        animator.enabled = false;
        spriteRenderer.enabled = false;

        if (bossCollider != null)
            bossCollider.enabled = false;
    }

    private void OnShieldBroken()
    {
        Debug.Log("BossController : Shield Broken");

        PlayShieldBreakEffect();

        if (shieldBreakUI != null)
            shieldBreakUI.Show();
    }

    private void OnShieldRecovered()
    {
        Debug.Log("BossController : Shield Recovered");

        if (shieldBreakUI != null)
            shieldBreakUI.Hide();
    }

    

    private void OnDrawGizmosSelected()
    {
        // Slam Radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            impactPoint,
            slamRadius);

        // Shockwave Radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            impactPoint,
            shockwaveRadius);

        // Titik Impact
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(
            impactPoint,
            0.15f);
    }
}