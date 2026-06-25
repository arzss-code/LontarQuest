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
        if (entranceBarrier != null)
        {
            entranceCollider = entranceBarrier.GetComponent<BoxCollider2D>();
            entranceSprite = entranceBarrier.GetComponent<SpriteRenderer>();

            if (entranceCollider != null)
                entranceCollider.enabled = false;

            if (entranceSprite != null)
                entranceSprite.enabled = false;
        }

        // Boss HP UI awalnya disembunyikan
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

        if (entranceCollider != null)
            entranceCollider.enabled = false;

        if (entranceSprite != null)
            entranceSprite.enabled = false;
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
        // Tunggu kamera
        //--------------------------------------------------

        yield return new WaitForSeconds(cameraFocusDelay);

        //--------------------------------------------------
        // Munculkan Boss HP UI
        //--------------------------------------------------

        if (bossUIAnimator != null)
        {
            bossUIAnimator.gameObject.SetActive(true);
            bossUIAnimator.SetTrigger("Show");
        }

        //--------------------------------------------------
        // Tunggu animasi UI selesai
        //--------------------------------------------------

        yield return new WaitForSeconds(uiAnimationDelay);

        //--------------------------------------------------
        // Spawn Boss
        //--------------------------------------------------

        if (bossController != null)
            bossController.ShowBoss();

        //--------------------------------------------------
        // Tunggu animasi Boss Spawn selesai
        //--------------------------------------------------

        yield return new WaitUntil(() => bossController != null && bossController.IsAwaken);

        //--------------------------------------------------
        // Boss diam sebentar
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
        // Tunggu kamera kembali
        //--------------------------------------------------

        yield return new WaitForSeconds(cameraReturnDelay);

        //--------------------------------------------------
        // Battle dimulai
        //--------------------------------------------------

        if (playerController != null)
            playerController.SetCanMove(true);

        Debug.Log("=== Battle Start ===");
    }
}