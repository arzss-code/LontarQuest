using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public enum AttackState
    {
        Waiting,
        Preparing,
        Attacking,
        Recovering
    }

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float prepareDuration = 1.0f;
    [SerializeField] private float cooldownDuration = 2.0f;

    [Header("References")]
    [SerializeField] private BossController bossController;
    [SerializeField] private BossMovement bossMovement;
    [SerializeField] private GameObject aoeIndicator;
    [SerializeField] private Animator aoeAnimator;

    private Transform player;

    private AttackState currentState = AttackState.Waiting;

    public bool IsBusy => currentState != AttackState.Waiting;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player tidak ditemukan!");
        }

        if (aoeIndicator != null)
            aoeIndicator.SetActive(false);
    }

    private void Update()
    {
        if (bossController == null)
            return;

        if (!bossController.IsAwaken)
            return;

        if (player == null)
            return;

        if (currentState != AttackState.Waiting)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance <= attackRange)
        {
            AttackRoutine();
        }
    }

    private void AttackRoutine()
    {
        currentState = AttackState.Preparing;

        bossMovement.SetCanMove(false);

        if (aoeIndicator != null)
        {
            aoeIndicator.SetActive(true);
            aoeAnimator.Play("AOEFill", 0, 0f);
        }

        // Tidak memanggil StartSlam()
        // Animation Event AOEFill yang akan memanggil BossController.StartSlam()
    }

    /// <summary>
    /// Dipanggil dari Animation Event pada frame terakhir BossSlam
    /// </summary>
    public void FinishAttack()
    {
        StartCoroutine(RecoverRoutine());
    }

    private IEnumerator RecoverRoutine()
    {
        currentState = AttackState.Recovering;

        if (aoeIndicator != null)
            aoeIndicator.SetActive(false);

        yield return new WaitForSeconds(cooldownDuration);

        bossMovement.SetCanMove(true);

        currentState = AttackState.Waiting;
    }
}