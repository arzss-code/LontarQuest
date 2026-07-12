using UnityEngine;
using TMPro;
using System.Collections;

[System.Serializable]
public class NarrationLine
{
    [TextArea(2,5)]
    public string text;

    [Tooltip("Berapa lama teks diam setelah selesai diketik")]
    public float stayDuration = 3f;
}

public class EndingNarration : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private TMP_Text narrationText;

    [Header("Narration")]
    [SerializeField] private NarrationLine[] narrations;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float fadeDuration = 0.6f;

    [SerializeField] private CanvasGroup whiteFlash;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [SerializeField] private Animator playerAnimator;


    

    public bool Finished { get; private set; }

    private void Start()
    {
        whiteFlash.alpha = 0;

        StartCoroutine(PlayNarration());
    }

    private IEnumerator PlayNarration()
    {
        //----------------------------------------
        // Freeze Player
        //----------------------------------------

        if (player != null)
            player.SetCanMove(false);

        Finished = false;

        blackScreen.SetActive(true);

        narrationText.text = "";
        narrationText.alpha = 1;

        foreach (NarrationLine line in narrations)
        {
            yield return StartCoroutine(TypeText(line.text));

            yield return new WaitForSeconds(line.stayDuration);

            yield return StartCoroutine(FadeOutText());
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(DipToWhite());

        yield return StartCoroutine(RevealScene());

        Finished = true;
    }

    IEnumerator TypeText(string text)
    {
        narrationText.text = "";
        narrationText.alpha = 1;

        foreach(char c in text)
        {
            narrationText.text += c;

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator FadeOutText()
    {
        float timer = 0;

        while(timer < fadeDuration)
        {
            timer += Time.deltaTime;

            narrationText.alpha =
                Mathf.Lerp(1,0,timer/fadeDuration);

            yield return null;
        }

        narrationText.text = "";
    }

    IEnumerator DipToWhite()
    {
        if (whiteFlash == null)
            yield break;

        float duration = 2f;

        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(0, 1, timer / duration);

            yield return null;
        }

        whiteFlash.alpha = 1;
    }

    IEnumerator RevealScene()
    {
        //----------------------------------------
        // Tahan layar putih sebentar
        //----------------------------------------

        // yield return new WaitForSeconds(0.5f);

        //----------------------------------------
        // Black Screen tidak diperlukan lagi
        //----------------------------------------

        if (blackScreen != null)
            blackScreen.SetActive(false);

        //----------------------------------------
        // Mulai animasi bangun
        //----------------------------------------

        if (playerAnimator != null)
            playerAnimator.SetTrigger("WakeUp");

        //----------------------------------------
        // White Fade Out
        //----------------------------------------

        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            whiteFlash.alpha =
                Mathf.Lerp(1f, 0f, timer / duration);

            yield return null;
        }

        whiteFlash.alpha = 0f;
    }
}