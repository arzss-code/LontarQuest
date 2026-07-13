using UnityEngine;
using System.Collections;

public class EndingSceneSequence : MonoBehaviour
{

    
    [Header("Narration")]
    [SerializeField] private EndingNarration narration;

    [Header("Dialogues")]
    [SerializeField] private IntroDialogue dialogueA;
    [SerializeField] private IntroDialogue dialogueB;
    [SerializeField] private IntroDialogue dialogueC;
    


    [SerializeField] private FireflyGuide fireflyGuide;
    

    [Header("Fade")]

    [SerializeField] private float illustrationFadeDuration = 0.5f;
    

    [Header("Illustration")]
    [SerializeField] private CanvasGroup illustration01;

    private void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        //----------------------------------
        // Tunggu Narration selesai
        //----------------------------------

        yield return new WaitUntil(() => narration.Finished);

        //----------------------------------
        // Dialogue A
        //----------------------------------

        dialogueA.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueA.HasFinished());

        //----------------------------------
        // Show Illustration
        //----------------------------------

        yield return FadeIllustration(0, 1);

        dialogueB.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueB.HasFinished());

        yield return FadeIllustration(1, 0);

        //----------------------------------
        // Dialogue C
        //----------------------------------

        dialogueC.StartDialogue();

        yield return new WaitForSeconds(0.8f);

        fireflyGuide.BeginGuide();

        yield return new WaitUntil(() =>
            dialogueC.HasFinished());

        //----------------------------------
        // Sequence selesai
        //----------------------------------

        Debug.Log("Ending Scene Finished");
    }

    IEnumerator FadeIllustration(float from, float to)
    {
        float timer = 0;

        illustration01.alpha = from;

        while (timer < illustrationFadeDuration)
        {
            timer += Time.deltaTime;

            illustration01.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    timer / illustrationFadeDuration);

            yield return null;
        }

        illustration01.alpha = to;
    }
}