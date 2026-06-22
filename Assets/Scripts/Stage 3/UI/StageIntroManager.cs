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
    [SerializeField] private float flashDuration = 0.2f;

    private void OnEnable()
    {
        if (introDialogueA != null)
        {
            introDialogueA.OnDialogueFinished += HandleDialogueAFinished;
        }
    }

    private void OnDisable()
    {
        if (introDialogueA != null)
        {
            introDialogueA.OnDialogueFinished -= HandleDialogueAFinished;
        }
    }

    private void HandleDialogueAFinished()
    {
        StartCoroutine(FlashSequence());
    }

    private IEnumerator FlashSequence()
    {
        Color c = whiteFlash.color;

        // Langsung putih penuh
        float maxAlpha = 0.55f;

        c.a = maxAlpha;
        whiteFlash.color = c;

        // Tahan putih sebentar
        yield return new WaitForSeconds(0.35f);

        // Baru fade keluar
        float t = 0f;
        float fadeOutDuration = 0.6f;

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

        if (introDialogueB != null)
        {
            introDialogueB.StartDialogue();
        }
    }
}