using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D bossCollider;

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
    public void Sleep()
    {
        isAwaken = false;

        animator.enabled = false;
        spriteRenderer.enabled = false;

        if (bossCollider != null)
            bossCollider.enabled = false;
    }
}