using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public enum AttackState
    {
        Waiting,
        Preparing,
        Recovering
    }

    [Header("Attack")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float cooldownDuration = 2f;

    [Header("References")]
    [SerializeField] private BossController bossController;
    [SerializeField] private BossMovement bossMovement;
    [SerializeField] private GameObject aoeIndicator;
    [SerializeField] private Animator aoeAnimator;
    [SerializeField] private GravityPullController gravityPull;

    [Header("Effects")]
    [SerializeField] private ParticleSystem gravityParticles;

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

        float distance = Vector2.Distance(
            transform.position,
            player.position);

        if (distance <= attackRange)
        {
            BeginAttack();
        }
    }

    private void BeginAttack()
    {
        currentState = AttackState.Preparing;

        bossMovement.SetCanMove(false);

        gravityPull.StartPull();

        if (gravityParticles != null)
        {
            gravityParticles.Play();
        }

        if (aoeIndicator != null)
        {
            aoeIndicator.SetActive(true);
            aoeAnimator.Play("AOEFill", 0, 0f);
        }

        // Boss Slam akan dipanggil dari Animation Event
        // pada akhir animasi AOEFill.
    }

    /// <summary>
    /// Dipanggil oleh BossController saat Animation Event
    /// OnSlamFinished().
    /// </summary>
    public void FinishAttack()
    {
        gravityPull.StopPull();

        if (gravityParticles != null)
        {
            gravityParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting);
        }

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