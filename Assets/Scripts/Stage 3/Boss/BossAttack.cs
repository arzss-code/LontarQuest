using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    private enum AttackType
    {
        GravitySlam,
        LongRangeSlam
    }

    public enum AttackState
    {
        Waiting,
        Teleporting,
        Preparing,
        Recovering,
        Staggered
    }

    [Header("Attack")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float cooldownDuration = 2f;

    [Header("References")]
    [SerializeField] private BossController bossController;
    [SerializeField] private BossMovement bossMovement;
    [SerializeField] private BossStats bossStats;
    [SerializeField] private GameObject aoeIndicator;
    [SerializeField] private Animator aoeAnimator;
    [SerializeField] private GravityPullController gravityPull;
    [SerializeField] private BossTeleport bossTeleport;

    [Header("Effects")]
    [SerializeField] private ParticleSystem gravityParticles;

    [Header("Long Range")]
    [SerializeField] private float longRangeDelay = 3.5f;

    private float longRangeTimer;
    private int gravityAttackCount = 0;
    private float longRangeCooldownTimer = 0f;

    private Transform player;

    // Attack
    private AttackType currentAttack;
    private Vector3 lockedTargetPosition;

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

    private void OnEnable()
    {
        if (bossStats != null)
            bossStats.OnShieldBroken += CancelCurrentAttack;
    }

    private void OnDisable()
    {
        if (bossStats != null)
            bossStats.OnShieldBroken -= CancelCurrentAttack;
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

        if (longRangeCooldownTimer > 0f)
            longRangeCooldownTimer -= Time.deltaTime;

        float distance = Vector2.Distance(
            transform.position,
            player.position);

        //--------------------------------------------------
        // PLAYER DEKAT
        //--------------------------------------------------

        if (distance <= attackRange)
        {
            currentAttack = AttackType.GravitySlam;

            BeginAttack();

            return;
        }

        //--------------------------------------------------
        // PLAYER JAUH
        //--------------------------------------------------

        if (longRangeCooldownTimer <= 0f)
        {
            currentAttack = AttackType.LongRangeSlam;

            lockedTargetPosition = player.position;

            BeginAttack();

            longRangeCooldownTimer = longRangeDelay;
        }
    }

    private void BeginTeleport()
    {
        currentState = AttackState.Teleporting;

        bossMovement.SetCanMove(false);

        // Lock posisi Player
        lockedTargetPosition = player.position;

        if (bossTeleport != null)
        {
            bossTeleport.StartTeleport();
        }
    }

    public void FinishTeleport()
    {
        bossMovement.SetCanMove(true);

        currentState = AttackState.Waiting;
    }

    private void BeginAttack()
    {
        currentState = AttackState.Preparing;

        // Simpan posisi Player saat Boss mulai menyerang

        bossMovement.SetCanMove(false);

        // ==========================
        // GRAVITY SLAM (JARAK DEKAT)
        // ==========================
        if (currentAttack == AttackType.GravitySlam)
        {
            bossController.SetImpactPoint(transform.position);

            aoeIndicator.transform.position = transform.position;

            gravityPull.StartPull();

            if (gravityParticles != null)
                gravityParticles.Play();

            gravityAttackCount++;

            Debug.Log("Gravity Count : " + gravityAttackCount);
        }

        // ==========================
        // LONG RANGE SLAM
        // ==========================
        else
        {
            gravityAttackCount = 0;
            // Titik impact di posisi terakhir Player
            bossController.SetImpactPoint(lockedTargetPosition);

            // AOE muncul di posisi yang dikunci
            aoeIndicator.transform.position = lockedTargetPosition;

            // Tidak ada Gravity Pull
            // Tidak ada Gravity Particle

            Debug.Log("Long Range Slam");
        }

        aoeIndicator.SetActive(true);
        aoeAnimator.Play("AOEFill", 0, 0f);
    }

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

        //--------------------------------------------------
        // Setelah 2 Gravity Slam,
        // langsung Teleport + Long Range
        //--------------------------------------------------

        if (gravityAttackCount >= 2)
        {
            gravityAttackCount = 0;

            BeginTeleport();

            yield break;
        }

        //--------------------------------------------------
        // Kembali Chase
        //--------------------------------------------------

        bossMovement.SetCanMove(true);

        currentState = AttackState.Waiting;
    }

    public void CancelCurrentAttack()
    {
        Debug.Log("===== ATTACK INTERRUPTED =====");

        StopAllCoroutines();

        //--------------------------------------------------
        // Stop Gravity Pull
        //--------------------------------------------------

        if (gravityPull != null)
            gravityPull.StopPull();

        //--------------------------------------------------
        // Stop Gravity Particle
        //--------------------------------------------------

        if (gravityParticles != null)
            gravityParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear);

        //--------------------------------------------------
        // Hilangkan AOE
        //--------------------------------------------------

        if (aoeIndicator != null)
            aoeIndicator.SetActive(false);

        //--------------------------------------------------
        // Stop Movement
        //--------------------------------------------------

        if (bossMovement != null)
            bossMovement.SetCanMove(false);

        //--------------------------------------------------
        // Masuk Stagger
        //--------------------------------------------------

        currentState = AttackState.Staggered;

        StartCoroutine(StaggerRoutine());
    }

    public Vector3 GetLockedTargetPosition()
    {
        return lockedTargetPosition;
    }

    public bool IsLongRangeAttack()
    {
        return currentAttack == AttackType.LongRangeSlam;
    }

    private IEnumerator StaggerRoutine()
    {
        Debug.Log("===== STAGGER =====");

        yield return new WaitForSeconds(3f);

        Debug.Log("===== STAGGER END =====");

        bossMovement.SetCanMove(true);

        currentState = AttackState.Waiting;
    }
}