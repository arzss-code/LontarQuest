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

    [Header("Cutscene Timing")]
    [SerializeField] private float cameraFocusDelay = 1.2f;
    [SerializeField] private float uiAnimationDelay = 1.2f;
    [SerializeField] private float bossRevealDelay = 2f;
    [SerializeField] private float cameraReturnDelay = 0.8f;

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
        {
            bossUIAnimator.gameObject.SetActive(false);
        }
    }

    public void StartBossFight()
    {
        StartCoroutine(BossIntroSequence());
    }

    public void EndBossFight()
    {
        Debug.Log("=== Boss Fight Finished ===");

        //--------------------------------------------------
        // Buka Barrier
        //--------------------------------------------------

        if (entranceCollider != null)
            entranceCollider.enabled = false;

        if (entranceSprite != null)
            entranceSprite.enabled = false;

        //--------------------------------------------------
        // Sembunyikan Boss UI
        //--------------------------------------------------

        HideBossUI();
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
        // Tampilkan Boss UI
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
        // Kamera Kembali ke Player
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
        // Battle Dimulai
        //--------------------------------------------------

        if (playerController != null)
            playerController.SetCanMove(true);

        Debug.Log("=== Battle Start ===");
    }

    /// <summary>
    /// Menampilkan Boss HP UI
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
    /// Menyembunyikan Boss HP UI
    /// </summary>
    public void HideBossUI()
    {
        if (bossUIAnimator == null)
            return;

        bossUIAnimator.ResetTrigger("Show");
        bossUIAnimator.SetTrigger("Hide");
    }
}