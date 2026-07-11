using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossEndingCutscene : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cmPlayer;
    [SerializeField] private CinemachineCamera cmArtifact;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Artifact")]
    [SerializeField] private GameObject artifact;
    [SerializeField] private Animator artifactAnimator;

    [Header("Dialogue")]
    [SerializeField] private IntroDialogue firstDialogue;
    [SerializeField] private IntroDialogue secondDialogue;

    [Header("Positions")]
    [SerializeField] private Transform playerTeleportPoint;
    [SerializeField] private Transform artifactTargetPoint;

    [Header("Timing")]
    [SerializeField] private float artifactAppearDelay = 0.5f;
    [SerializeField] private float playerTeleportDelay = 1f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float stopDistance = 0.08f;
    [SerializeField] private Transform exitWalkPoint;
    [SerializeField]private float walkDownSpeed = 1.6f;
    

    [Header("Gameplay UI")]
    [SerializeField] private GameObject playerHudBar;
    [SerializeField] private GameObject questCanvas;

    [SerializeField] private Animator playerAnimator;

    [Header("Effects")]
    [SerializeField] private ScreenFlash flash;

    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    [Header("Screen Fade")]
    [SerializeField] private ScreenFade blackFade;

    [Header("Fall")]

    [SerializeField] private Transform holeCenter;

    [SerializeField] private float fallDuration = 1.2f;

    [SerializeField] private float rotateSpeed = 720f;

    [SerializeField] private float shrinkScale = 0.15f;

    [Header("Arena Collapse")]

    [SerializeField] private GameObject crackedFloor;

    [SerializeField] private GameObject crackedFloor2ndPhase;

    [SerializeField] private GameObject hole;

    [Header("Ending")]

    [SerializeField] private float cameraZoomSize = 1.2f;

    [SerializeField] private float cameraZoomDuration = 2f;

    [SerializeField] private string nextSceneName = "EndingScene";


    

    //------------------------------------------------------------

    private SpriteRenderer playerSprite;

    private void Awake()
    
    {
        
        if (artifact != null)
            artifact.SetActive(false);

        if (crackedFloor != null)
        crackedFloor.SetActive(false);

        if (crackedFloor2ndPhase != null)
            crackedFloor2ndPhase.SetActive(false);

        if (hole != null)
            hole.SetActive(false);

        playerAnimator =
            player.GetComponentInChildren<Animator>();

        playerSprite =
            player.GetComponentInChildren<SpriteRenderer>();
        
    }

    //------------------------------------------------------------

    public void Play()
    {
        StartCoroutine(Sequence());
    }

    //------------------------------------------------------------

    private IEnumerator Sequence()
    {
        Debug.Log("===== Ending Sequence =====");

        //----------------------------------
        // Hide Gameplay UI
        //----------------------------------

        if (playerHudBar != null)
            playerHudBar.SetActive(false);

        if (questCanvas != null)
            questCanvas.SetActive(false);

        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Fade To Black
        //----------------------------------

        yield return blackFade.FadeIn(0.5f);

        yield return new WaitForSeconds(0.5f);

        //
        // Semua perubahan dilakukan
        // ketika layar hitam
        //

        cmPlayer.Priority = 5;
        cmArtifact.Priority = 20;

        player.transform.position =
            playerTeleportPoint.position;

        artifact.SetActive(true);

        if (artifactAnimator != null)
        {
            artifactAnimator.Rebind();
            artifactAnimator.Update(0f);
        }

        //----------------------------------
        // Diam sejenak saat layar hitam
        //----------------------------------

        yield return new WaitForSeconds(0.8f);

        //----------------------------------
        // Fade Out perlahan
        //----------------------------------

        yield return blackFade.FadeOut(1f);

        //----------------------------------
        // Establishing Shot
        //----------------------------------

        yield return new WaitForSeconds(0.7f);

        //----------------------------------
        // Jalan menuju Artifact
        //----------------------------------

        yield return WalkToArtifact();

        Debug.Log("Player sampai di Artifact");

        //----------------------------------
        // Dialogue Pertama
        //----------------------------------

        firstDialogue.StartDialogue();

        yield return new WaitUntil(() =>
            firstDialogue.HasFinished());

        Debug.Log("Dialogue Pertama selesai");

        //----------------------------------
        // Pickup Artifact
        //----------------------------------

        Debug.Log("PLAY PICKUP");
        artifactAnimator.SetTrigger("Pickup");

        // Tunggu animasi pickup selesai
        yield return new WaitForSeconds(3.3f);

        // Hilangkan Artifact
        artifact.SetActive(false);

        Debug.Log("Artifact Picked Up");

        //----------------------------------
        // Dialogue Kedua
        //----------------------------------

        secondDialogue.StartDialogue();

        yield return new WaitUntil(() =>
            secondDialogue.HasFinished());

        Debug.Log("Dialogue Kedua selesai");

        // Diam sebentar
        yield return new WaitForSeconds(0.5f);

        // Player mulai berjalan sambil arena runtuh
        yield return WalkDownWithCollapse();

    }

    //------------------------------------------------------------

    private IEnumerator WalkToArtifact()
    {
        while (Vector2.Distance(
            player.transform.position,
            artifactTargetPoint.position) > stopDistance)
        {
            //----------------------------------
            // Gerakkan Player
            //----------------------------------

            player.transform.position =
                Vector2.MoveTowards(
                    player.transform.position,
                    artifactTargetPoint.position,
                    walkSpeed * Time.deltaTime);

            //----------------------------------
            // Animasi Walk Up
            //----------------------------------

            playerAnimator.SetFloat("MoveX", 0);
            playerAnimator.SetFloat("MoveY", 1);

            playerAnimator.SetFloat("LastMoveX", 0);
            playerAnimator.SetFloat("LastMoveY", 1);

            playerAnimator.SetFloat("Speed", 1);

            yield return null;
        }

        //----------------------------------
        // Idle Up
        //----------------------------------

        playerAnimator.SetFloat("MoveX", 0);
        playerAnimator.SetFloat("MoveY", 0);

        playerAnimator.SetFloat("LastMoveX", 0);
        playerAnimator.SetFloat("LastMoveY", 1);

        playerAnimator.SetFloat("Speed", 0);
    }


    private IEnumerator WalkDownWithCollapse()
    {
        float timer = 0f;

        bool flash1 = false;
        bool flash2 = false;
        bool flash3 = false;

        while (Vector2.Distance(
            player.transform.position,
            exitWalkPoint.position) > stopDistance)
        {
            //--------------------------------
            // Timer
            //--------------------------------

            timer += Time.deltaTime;

            //--------------------------------
            // Gerakkan Player
            //--------------------------------

            player.transform.position =
                Vector2.MoveTowards(
                    player.transform.position,
                    exitWalkPoint.position,
                    walkDownSpeed * Time.deltaTime);

            //--------------------------------
            // Animasi Walk Down
            //--------------------------------

            playerAnimator.SetFloat("MoveX", 0);
            playerAnimator.SetFloat("MoveY", -1);

            playerAnimator.SetFloat("LastMoveX", 0);
            playerAnimator.SetFloat("LastMoveY", -1);

            playerAnimator.SetFloat("Speed", 1);

            //--------------------------------
            // Flash Pertama
            //--------------------------------

            if (!flash1 && timer >= 0.7f)
            {
                flash1 = true;

                yield return flash.Flash(
                    0.05f,
                    0.05f,
                    0.2f);

                yield return CameraShake(
                    0.8f,
                    2f,
                    0.25f);
                //--------------------------------
                // Phase 1 Retakan
                //--------------------------------

                if (crackedFloor != null)
                {
                    crackedFloor.SetActive(true);

                    Debug.Log("Cracked Floor Phase 1");
                }
            }

            //--------------------------------
            // Flash Kedua
            //--------------------------------

            if (!flash2 && timer >= 1.6f)
            {
                flash2 = true;

                yield return flash.Flash(
                    0.05f,
                    0.05f,
                    0.2f);

                yield return CameraShake(
                    1.8f,
                    3f,
                    0.35f);

                //--------------------------------
                // Phase 2 Retakan
                //--------------------------------

                if (crackedFloor2ndPhase != null)
                {
                    crackedFloor2ndPhase.SetActive(true);

                    Debug.Log("Cracked Floor Phase 2");
                }
            }

            //--------------------------------
            // Flash Ketiga
            //--------------------------------

            if (!flash3 && timer >= 2.5f)
            {
                flash3 = true;

                yield return flash.Flash(
                    0.05f,
                    0.08f,
                    0.25f);

                yield return CameraShake(
                    3.8f,
                    5f,
                    0.8f);

                //--------------------------------
                // Lubang terbuka
                //--------------------------------

                if (hole != null)
                {
                    hole.SetActive(true);

                    Debug.Log("Hole Open");
                }

                break;
                }

            yield return null;
        }

        //--------------------------------
        // Stop berjalan
        //--------------------------------

        playerAnimator.SetFloat("Speed",0);

        yield return new WaitForSeconds(0.35f);

        yield return PlayerFallSequence();

        yield return new WaitForSeconds(0.45f);

        yield return HoleConsumeScreen();
    }


    private IEnumerator ShowDialogue(string text)
    {
        DialogueManager.Instance.ShowDialogue(text);

        yield return new WaitUntil(() =>
            DialogueManager.Instance.dialogFinished);
    }

    private IEnumerator PlayerFallSequence()
    {
        Debug.Log("Player Fall");

        Transform visual =
            player.transform.Find("Visual");

        if (visual == null)
            yield break;

        SpriteRenderer sr =
            visual.GetComponent<SpriteRenderer>();

        if (sr == null)
            yield break;

        Vector3 startPosition =
            player.transform.position;

        Vector3 startScale =
            visual.localScale;

        Color startColor =
            sr.color;

        float currentRotation =
            visual.eulerAngles.z;

        float timer = 0;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fallDuration;

            //--------------------------------
            // Bergerak menuju Hole
            //--------------------------------

            player.transform.position =
                Vector3.Lerp(
                    startPosition,
                    holeCenter.position,
                    t);

            //--------------------------------
            // Rotate Z
            //--------------------------------

            currentRotation +=
                rotateSpeed * Time.deltaTime;

            visual.rotation =
                Quaternion.Euler(
                    0,
                    0,
                    currentRotation);

            //--------------------------------
            // Shrink
            //--------------------------------

            visual.localScale =
                Vector3.Lerp(
                    startScale,
                    Vector3.one * shrinkScale,
                    t);

            //--------------------------------
            // Fade
            //--------------------------------

            Color c = startColor;

            c.a = Mathf.Lerp(1f, 0f, t);

            sr.color = c;

            yield return null;
        }

        Debug.Log("Player Hilang");

        player.gameObject.SetActive(false);
    }

    private IEnumerator CameraShake(float amplitude, float frequency, float duration)
    {
        noise.AmplitudeGain = amplitude;
        noise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }

    private IEnumerator HoleConsumeScreen()
    {
        Debug.Log("Hole Consume");

        Vector3 crack1 =
            crackedFloor.transform.localScale;

        Vector3 crack2 =
            crackedFloor2ndPhase.transform.localScale;

        Vector3 holeScale =
            hole.transform.localScale;

        float timer = 0;

        while(timer < cameraZoomDuration)
        {
            timer += Time.deltaTime;

            float t = timer / cameraZoomDuration;

            //--------------------------------
            // Smooth
            //--------------------------------

            float curve =
                Mathf.SmoothStep(0,1,t);

            //--------------------------------
            // Scale
            //--------------------------------

            crackedFloor.transform.localScale =
                Vector3.Lerp(
                    crack1,
                    crack1 * 2.2f,
                    curve);

            crackedFloor2ndPhase.transform.localScale =
                Vector3.Lerp(
                    crack2,
                    crack2 * 2.4f,
                    curve);

            hole.transform.localScale =
                Vector3.Lerp(
                    holeScale,
                    holeScale * 6f,
                    curve);

            yield return null;
        }

        yield return blackFade.FadeIn(0.8f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(nextSceneName);
    }
}