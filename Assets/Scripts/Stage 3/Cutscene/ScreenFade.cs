using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image fadeImage;

    public IEnumerator FadeIn(float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            Color c = fadeImage.color;
            c.a = Mathf.Lerp(0, 1, t / duration);

            fadeImage.color = c;

            yield return null;
        }

        Color end = fadeImage.color;
        end.a = 1;
        fadeImage.color = end;
    }

    public IEnumerator FadeOut(float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            Color c = fadeImage.color;
            c.a = Mathf.Lerp(1, 0, t / duration);

            fadeImage.color = c;

            yield return null;
        }

        Color end = fadeImage.color;
        end.a = 0;
        fadeImage.color = end;
    }
}