using UnityEngine;
using System.Collections;
using UnityEngine.Video;

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

    [Header("Flashback")]
    [SerializeField] private FlashbackPlayer flashbackPlayer;

    [SerializeField] private VideoClip videoClip1;
    [SerializeField] private VideoClip videoClip2;
    [SerializeField] private VideoClip videoClip3;

    //--------------------------------------------------

    public void Play()
    {
        StartCoroutine(RescueRoutine());
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
        // Player bebas bergerak
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
}