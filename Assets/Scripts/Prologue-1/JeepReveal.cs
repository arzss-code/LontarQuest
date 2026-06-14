using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JeepReveal : MonoBehaviour
{
    [SerializeField] private float revealDuration = 2f;

    private Image jeepImage;

    private void Awake()
    {
        jeepImage = GetComponent<Image>();

        SetHidden();
    }

    public void SetHidden()
    {
        if (jeepImage == null)
            return;

        jeepImage.color = Color.black;
    }

    public void Reveal()
    {
        StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        float timer = 0f;

        while (timer < revealDuration)
        {
            timer += Time.deltaTime;

            jeepImage.color = Color.Lerp(
                Color.black,
                Color.white,
                timer / revealDuration
            );

            yield return null;
        }

        jeepImage.color = Color.white;
    }
}