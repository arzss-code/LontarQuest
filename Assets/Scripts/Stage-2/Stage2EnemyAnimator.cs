using UnityEngine;

public class Stage2EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private int lastDirection = 0; // Default to Down (0)

    // Cached Animator Parameter Hashes for performance
    private static readonly int DirectionHash = Animator.StringToHash("Direction");
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
    /// Mengonversi arah Vector2 ke integer (0=Down, 1=Up, 2=Left, 3=Right)
    /// </summary>
    public void SetDirection(Vector2 direction)
    {
        if (animator == null) return;

        // Jika tidak ada pergerakan / magnitude mendekati 0, pertahankan arah terakhir
        if (direction.sqrMagnitude < 0.001f)
        {
            animator.SetInteger(DirectionHash, lastDirection);
            return;
        }

        int targetDir = lastDirection;

        // Tentukan sumbu yang dominan (horizontal vs vertikal)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Dominan Horizontal
            targetDir = direction.x > 0 ? 3 : 2; // 3 = Right, 2 = Left
        }
        else
        {
            // Dominan Vertikal
            targetDir = direction.y > 0 ? 1 : 0; // 1 = Up, 0 = Down
        }

        lastDirection = targetDir;
        animator.SetInteger(DirectionHash, targetDir);
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
