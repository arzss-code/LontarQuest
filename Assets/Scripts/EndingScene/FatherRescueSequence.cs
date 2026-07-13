using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Unity.Cinemachine;

public class FatherRescueSequence : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Fade")]
    [SerializeField] private CanvasGroup blackFade;

    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Animation")]
    [SerializeField] private Animator trapAnimator;
    [SerializeField] private Animator fatherAnimator;

    [SerializeField] private float trapBreakDuration = 2.2f;
    [SerializeField] private float fatherCollapseDuration = 1.5f;
    [SerializeField] private float secondFadeDelay = 0.3f;

    [Header("Dialogue")]
    [SerializeField] private IntroDialogue dialogueG;

    [SerializeField] private IntroDialogue dialogueH01;
    [SerializeField] private IntroDialogue dialogueH02;
    [SerializeField] private IntroDialogue dialogueH03;
    [SerializeField] private IntroDialogue dialogueI01;
    [SerializeField] private IntroDialogue dialogueI02;
    [SerializeField] private IntroDialogue dialogueI03;
    [SerializeField] private IntroDialogue dialogueI04;
    [SerializeField] private IntroDialogue dialogueI05;
    [SerializeField] private IntroDialogue dialogueI06;
    [SerializeField] private IntroDialogue dialogueI07;
    

    [Header("Flashback")]
    [SerializeField] private FlashbackPlayer flashbackPlayer;

    [SerializeField] private VideoClip videoClip1;
    [SerializeField] private VideoClip videoClip2;
    [SerializeField] private VideoClip videoClip3;

    [Header("Firefly")]
    [SerializeField] private FireflyController fireflyController;

    [SerializeField]
    private int fatherWaypointIndex = 6;

    [Header("Camera")]

    [SerializeField] private CinemachineCamera cmPlayer;

    [SerializeField] private float zoomOutSize = 14.5f;

    [SerializeField] private float zoomDuration = 1f;

    private float defaultCameraSize;

    //--------------------------------------------------

    public void Play()
    {
        StartCoroutine(RescueRoutine());
    }

    private void Awake()
    {
        defaultCameraSize =
            cmPlayer.Lens.OrthographicSize;
    }

    //--------------------------------------------------

    IEnumerator RescueRoutine()
    {
        Debug.Log("Father Rescue Start");

        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Fade Black
        //----------------------------------

        yield return Fade(0, 1);

        yield return new WaitForSeconds(0.2f);

        //----------------------------------
        // Trap Break
        //----------------------------------

        trapAnimator.SetTrigger("Break");

        Debug.Log("Trap Break");

        yield return new WaitForSeconds(trapBreakDuration);

        //----------------------------------
        // Father Collapse
        //----------------------------------

        fatherAnimator.SetTrigger("Collapse");

        Debug.Log("Father Collapse");

        yield return new WaitForSeconds(fatherCollapseDuration);

        //----------------------------------
        // Diam sebentar
        //----------------------------------

        yield return new WaitForSeconds(0.3f);

        //----------------------------------
        // Fade kembali
        //----------------------------------

        yield return Fade(1,0);

        //----------------------------------
        // Dialogue G
        //----------------------------------

        dialogueG.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueG.HasFinished());


        //----------------------------------
        // Fade Black Lagi
        //----------------------------------

        yield return Fade(0, 1);

        yield return new WaitForSeconds(secondFadeDelay);


        //----------------------------------
        // Ayah Bangkit
        //----------------------------------

        fatherAnimator.SetTrigger("Idle");

        yield return new WaitForSeconds(0.3f);


        //----------------------------------
        // Fade Kembali
        //----------------------------------

        yield return Fade(1, 0);

        //----------------------------------
        // Dialogue H01
        //----------------------------------

        dialogueH01.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueH01.HasFinished());

         //----------------------------------
        // Video Clip 2
        //----------------------------------

        yield return StartCoroutine(
            flashbackPlayer.PlayVideo(videoClip2));


        //----------------------------------
        // Dialogue H02
        //----------------------------------

        dialogueH02.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueH02.HasFinished());


         //----------------------------------
        // Video Clip 3
        //----------------------------------

        yield return StartCoroutine(
            flashbackPlayer.PlayVideo(videoClip3));


        //----------------------------------
        // Dialogue H03
        //----------------------------------

        dialogueH03.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueH03.HasFinished());


        //----------------------------------
        // Dialogue I01
        //----------------------------------

        dialogueI01.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI01.HasFinished());

        //----------------------------------
        // Reveal Firefly
        //----------------------------------

        yield return new WaitForSeconds(0.4f);

        fireflyController.Reveal(fatherWaypointIndex);

        yield return new WaitUntil(() =>
            !fireflyController.IsMoving());

        //----------------------------------
        // Dialogue I02
        //----------------------------------

        dialogueI02.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI02.HasFinished());

        //----------------------------------
        // Dialogue I03
        //----------------------------------

        dialogueI03.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI03.HasFinished());

        //----------------------------------
        // Dialogue I04
        //----------------------------------

        dialogueI04.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI04.HasFinished());


        //----------------------------------
        // Pathfinder Nod
        //----------------------------------

        yield return StartCoroutine(
            fireflyController.Nod());


        //----------------------------------
        // Dialogue I05
        //----------------------------------

        dialogueI05.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI05.HasFinished());


        //----------------------------------
        // Pathfinder Nod
        //----------------------------------

        yield return StartCoroutine(
            fireflyController.Nod());


        //----------------------------------
        // Dialogue I06
        //----------------------------------

        dialogueI06.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI06.HasFinished());


        //----------------------------------
        // Pathfinder Nod
        //----------------------------------

        yield return StartCoroutine(
            fireflyController.Nod());


        //----------------------------------
        // Dialogue I07
        //----------------------------------

        dialogueI07.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueI07.HasFinished());
        
        //----------------------------------
        // Pathfinder Nod
        //----------------------------------

        yield return StartCoroutine(
            fireflyController.Nod());

        yield return new WaitForSeconds(0.5f);


        //----------------------------------
        // Zoom Out
        //----------------------------------

        yield return StartCoroutine(
            ZoomCamera(zoomOutSize));


        //----------------------------------
        // Show Altar Order
        //----------------------------------

        yield return StartCoroutine(
            fireflyController.ShowAltarOrder());


        //----------------------------------
        // Zoom In
        //----------------------------------

        yield return StartCoroutine(
            ZoomCamera(defaultCameraSize));


        //----------------------------------
        // Player bebas bergerak
        //----------------------------------

        player.SetCanMove(true);


        //----------------------------------
        // Sementara selesai
        //----------------------------------

        player.SetCanMove(true);

        Debug.Log("Father Rescue Finish");
    }


    
    //--------------------------------------------------

    IEnumerator Fade(float from, float to)
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            blackFade.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    timer / fadeDuration);

            yield return null;
        }

        blackFade.alpha = to;
    }

    IEnumerator ZoomCamera(float targetSize)
    {
        float timer = 0f;

        float startSize =
            cmPlayer.Lens.OrthographicSize;

        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;

            var lens = cmPlayer.Lens;

            lens.OrthographicSize =
                Mathf.Lerp(
                    startSize,
                    targetSize,
                    timer / zoomDuration);

            cmPlayer.Lens = lens;

            yield return null;
        }

        var finalLens = cmPlayer.Lens;

        finalLens.OrthographicSize = targetSize;

        cmPlayer.Lens = finalLens;
    }
}