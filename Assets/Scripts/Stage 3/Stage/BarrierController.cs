using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BarrierController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D barrierCollider;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.8f;

    private bool isOpen;

    //------------------------------------------------

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (barrierCollider == null)
            barrierCollider = GetComponent<Collider2D>();
    }

    //------------------------------------------------

    public void Open()
    {
        if (isOpen)
            return;

        isOpen = true;

        StartCoroutine(OpenRoutine());
    }

    //------------------------------------------------

    private IEnumerator OpenRoutine()
    {
        //----------------------------------
        // Disable Collider
        //----------------------------------

        barrierCollider.enabled = false;

        //----------------------------------
        // Fade Sprite
        //----------------------------------

        Color color = spriteRenderer.color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(
                1f,
                0f,
                timer / fadeDuration);

            spriteRenderer.color = color;

            yield return null;
        }

        //----------------------------------

        gameObject.SetActive(false);
    }
}