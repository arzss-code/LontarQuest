using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    [SerializeField] private Image flashImage;

    public IEnumerator Flash(
        float fadeIn,
        float hold,
        float fadeOut)
    {
        //--------------------------------
        // Fade In
        //--------------------------------

        float t = 0;

        while (t < fadeIn)
        {
            t += Time.deltaTime;

            Color c = flashImage.color;
            c.a = Mathf.Lerp(0, 1, t / fadeIn);

            flashImage.color = c;

            yield return null;
        }

        //--------------------------------
        // Hold
        //--------------------------------

        yield return new WaitForSeconds(hold);

        //--------------------------------
        // Fade Out
        //--------------------------------

        t = 0;

        while (t < fadeOut)
        {
            t += Time.deltaTime;

            Color c = flashImage.color;
            c.a = Mathf.Lerp(1, 0, t / fadeOut);

            flashImage.color = c;

            yield return null;
        }

        Color end = flashImage.color;
        end.a = 0;

        flashImage.color = end;
    }
}