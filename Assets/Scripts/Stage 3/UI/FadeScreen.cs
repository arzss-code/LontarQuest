using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Dialogue")]
    [SerializeField] private IntroDialogue introDialogue;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(holdTime);

        Color color = fadeImage.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / fadeDuration
            );

            fadeImage.color = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            yield return null;
        }

        fadeImage.color = new Color(
            color.r,
            color.g,
            color.b,
            0f
        );

        fadeImage.gameObject.SetActive(false);

        // Jalankan dialog setelah fade selesai
        if (introDialogue != null)
        {
            introDialogue.StartDialogue();
        }
        else
        {
            Debug.LogWarning("IntroDialogue belum di-assign pada FadeScreen!");
        }
    }
}