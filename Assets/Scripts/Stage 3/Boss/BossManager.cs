using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class BossManager : MonoBehaviour
{
    [Header("Arena")]
    [SerializeField] private GameObject entranceBarrier;
    [SerializeField] private BossController bossController;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cmPlayer;
    [SerializeField] private CinemachineCamera cmBoss;

    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [Header("Boss UI")]
    [SerializeField] private Animator bossUIAnimator;

    [Header("Shield Break UI")]
    [SerializeField] private BossShieldBreakUI shieldBreakUI;

    [Header("Cutscene Timing")]
    [SerializeField] private float cameraFocusDelay = 1.2f;
    [SerializeField] private float uiAnimationDelay = 1.2f;
    [SerializeField] private float bossRevealDelay = 2f;
    [SerializeField] private float cameraReturnDelay = 0.8f;

    [Header("UI")]
    [SerializeField] private float hideUIAfter = 0.8f;

    private BoxCollider2D entranceCollider;
    private SpriteRenderer entranceSprite;

    private void Awake()
    {
        //--------------------------------------------------
        // Barrier
        //--------------------------------------------------

        if (entranceBarrier != null)
        {
            entranceCollider = entranceBarrier.GetComponent<BoxCollider2D>();
            entranceSprite = entranceBarrier.GetComponent<SpriteRenderer>();

            if (entranceCollider != null)
                entranceCollider.enabled = false;

            if (entranceSprite != null)
                entranceSprite.enabled = false;
        }

        //--------------------------------------------------
        // Boss UI
        //--------------------------------------------------

        if (bossUIAnimator != null)
            bossUIAnimator.gameObject.SetActive(false);

        //--------------------------------------------------
        // Shield Break UI
        //--------------------------------------------------

        if (shieldBreakUI != null)
            shieldBreakUI.Hide();
    }

    public void StartBossFight()
    {
        StartCoroutine(BossIntroSequence());
    }

    public void EndBossFight()
    {
        Debug.Log("=== Boss Fight Finished ===");

        //--------------------------------------------------
        // Barrier dibuka kembali
        //--------------------------------------------------

        if (entranceCollider != null)
            entranceCollider.enabled = false;

        if (entranceSprite != null)
            entranceSprite.enabled = false;

        //--------------------------------------------------
        // Hide Boss UI
        //--------------------------------------------------

        HideBossUI();

        //--------------------------------------------------
        // Hide Shield Break Text
        //--------------------------------------------------

        if (shieldBreakUI != null)
            shieldBreakUI.Hide();
    }

    private IEnumerator BossIntroSequence()
    {
        Debug.Log("=== Boss Intro ===");

        //--------------------------------------------------
        // Freeze Player
        //--------------------------------------------------

        if (playerController != null)
            playerController.SetCanMove(false);

        //--------------------------------------------------
        // Tutup Barrier
        //--------------------------------------------------

        if (entranceCollider != null)
            entranceCollider.enabled = true;

        if (entranceSprite != null)
            entranceSprite.enabled = true;

        //--------------------------------------------------
        // Kamera ke Boss
        //--------------------------------------------------

        if (cmPlayer != null)
            cmPlayer.Priority = 5;

        if (cmBoss != null)
            cmBoss.Priority = 20;

        //--------------------------------------------------
        // Tunggu Kamera
        //--------------------------------------------------

        yield return new WaitForSeconds(cameraFocusDelay);

        //--------------------------------------------------
        // Boss UI Muncul
        //--------------------------------------------------

        ShowBossUI();

        //--------------------------------------------------
        // Tunggu Animasi UI
        //--------------------------------------------------

        yield return new WaitForSeconds(uiAnimationDelay);

        //--------------------------------------------------
        // Spawn Boss
        //--------------------------------------------------

        if (bossController != null)
            bossController.ShowBoss();

        //--------------------------------------------------
        // Tunggu Boss Bangun
        //--------------------------------------------------

        yield return new WaitUntil(() =>
            bossController != null &&
            bossController.IsAwaken);

        //--------------------------------------------------
        // Boss Diam Sebentar
        //--------------------------------------------------

        yield return new WaitForSeconds(bossRevealDelay);

        //--------------------------------------------------
        // Kamera kembali ke Player
        //--------------------------------------------------

        if (cmPlayer != null)
            cmPlayer.Priority = 20;

        if (cmBoss != null)
            cmBoss.Priority = 5;

        //--------------------------------------------------
        // Tunggu Kamera
        //--------------------------------------------------

        yield return new WaitForSeconds(cameraReturnDelay);

        //--------------------------------------------------
        // Battle Start
        //--------------------------------------------------

        if (playerController != null)
            playerController.SetCanMove(true);

        Debug.Log("=== Battle Start ===");
    }

    /// <summary>
    /// Menampilkan Boss UI
    /// </summary>
    public void ShowBossUI()
    {
        if (bossUIAnimator == null)
            return;

        bossUIAnimator.gameObject.SetActive(true);

        bossUIAnimator.ResetTrigger("Hide");
        bossUIAnimator.SetTrigger("Show");
    }

    /// <summary>
    /// Menyembunyikan Boss UI
    /// </summary>
    public void HideBossUI()
    {
        if (bossUIAnimator == null)
            return;

        bossUIAnimator.ResetTrigger("Show");
        bossUIAnimator.SetTrigger("Hide");

        StartCoroutine(DisableBossUIRoutine());
    }

    private IEnumerator DisableBossUIRoutine()
    {
        yield return new WaitForSeconds(hideUIAfter);

        if (bossUIAnimator != null)
            bossUIAnimator.gameObject.SetActive(false);
    }
}