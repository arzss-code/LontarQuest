using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StageIntroManager : MonoBehaviour
{
    [Header("Dialogues")]
    [SerializeField] private IntroDialogue introDialogueA;
    [SerializeField] private IntroDialogue introDialogueB;

    [Header("Flash")]
    [SerializeField] private Image whiteFlash;
    [SerializeField] private float maxAlpha = 0.55f;
    [SerializeField] private float holdDuration = 0.35f;
    [SerializeField] private float fadeOutDuration = 0.6f;

    [Header("Shake")]
    [SerializeField] private CameraShake cameraShake;

    private void OnEnable()
    {
        if (introDialogueA != null)
        {
            introDialogueA.OnDialogueFinished += HandleDialogueAFinished;
        }

        if (introDialogueB != null)
        {
            introDialogueB.OnDialogueFinished += HandleDialogueBFinished;
        }
    }

    private void OnDisable()
    {
        if (introDialogueA != null)
        {
            introDialogueA.OnDialogueFinished -= HandleDialogueAFinished;
        }

        if (introDialogueB != null)
        {
            introDialogueB.OnDialogueFinished -= HandleDialogueBFinished;
        }
    }

    private void HandleDialogueAFinished()
    {
        StartCoroutine(FlashSequence());
    }

    private void HandleDialogueBFinished()
    {
        if (cameraShake != null)
        {
            cameraShake.StopShake();
        }
    }

    private IEnumerator FlashSequence()
    {
        Color c = whiteFlash.color;

        // White Flash
        c.a = maxAlpha;
        whiteFlash.color = c;

        // Mulai Gempa
        if (cameraShake != null)
        {
            cameraShake.StartShake(
                1f, // Amplitude
                1f  // Frequency
            );
        }

        // Tahan Flash
        yield return new WaitForSeconds(holdDuration);

        // Fade Out Flash
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(
                maxAlpha,
                0f,
                t / fadeOutDuration
            );

            whiteFlash.color = c;

            yield return null;
        }

        c.a = 0f;
        whiteFlash.color = c;

        // Munculkan Dialog B
        if (introDialogueB != null)
        {
            introDialogueB.StartDialogue();
        }
    }
}