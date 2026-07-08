using UnityEngine;
using System.Collections;

public class ExitBarrier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;

    [Header("Barrier")]
    [SerializeField] private SpriteRenderer barrierRenderer;
    [SerializeField] private Collider2D barrierCollider;

    [SerializeField] private float fadeDuration = 1f;

private bool opened = false;

    private bool playerInside;

    void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenBarrier();
        }
    }

    public void PlayerEnter()
    {
        playerInside = true;

        if (interactionUI != null)
            interactionUI.SetActive(true);
    }

    public void PlayerExit()
    {
        playerInside = false;

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void OpenBarrier()
    {
        if (opened)
            return;

        opened = true;

        playerInside = false;

        if (interactionUI != null)
            interactionUI.SetActive(false);

        StartCoroutine(FadeBarrier());
    }

    System.Collections.IEnumerator FadeBarrier()
    {
        Color start = barrierRenderer.color;

        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            Color c = start;
            c.a = Mathf.Lerp(1f, 0f, t);

            barrierRenderer.color = c;

            yield return null;
        }

        barrierCollider.enabled = false;
    }
}