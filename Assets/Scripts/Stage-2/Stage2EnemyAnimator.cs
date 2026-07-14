using UnityEngine;

public class Stage2EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    // Cached Animator Parameter Hashes for performance (Solusi B: 2D Simple Directional)
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int DieHash = Animator.StringToHash("Die");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning($"[Stage2EnemyAnimator] Animator tidak ditemukan di {gameObject.name}!");
        }
    }

    /// <summary>
    /// Update parameter Speed di Animator
    /// </summary>
    public void SetSpeed(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat(SpeedHash, speed);
        }
    }

    /// <summary>
    /// Mengirimkan koordinat arah vector2 langsung ke parameter Float MoveX & MoveY di Animator (2D Blend Tree)
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (animator == null) return;

        // Jika tidak ada pergerakan / magnitude mendekati 0, jangan ubah nilai terakhir
        if (direction.sqrMagnitude < 0.001f) return;

        animator.SetFloat(MoveXHash, direction.x);
        animator.SetFloat(MoveYHash, direction.y);
    }

    /// <summary>
    /// Memicu trigger Attack (Dwarapala)
    /// </summary>
    public void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }
    }

    /// <summary>
    /// Memicu trigger Shoot (Yaksa)
    /// </summary>
    public void TriggerShoot()
    {
        if (animator != null)
        {
            animator.SetTrigger(ShootHash);
        }
    }

    /// <summary>
    /// Memicu trigger Die
    /// </summary>
    public void TriggerDie()
    {
        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }
    }
}
