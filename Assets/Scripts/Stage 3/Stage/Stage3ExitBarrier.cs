using UnityEngine;
using System.Collections;

public class ExitBarrier : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactionUI;

    [Header("Barrier")]
    [SerializeField] private SpriteRenderer barrierRenderer;
    [SerializeField] private Collider2D barrierCollider;

    [Header("Interaction")]
    [SerializeField] private Collider2D interactionTrigger;

    [SerializeField] private float fadeDuration = 1f;

    private bool opened = false;
    private bool playerInside;

    //----------------------------------------------------

    private void Start()
    {
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    //----------------------------------------------------

    private void Update()
    {
        if (opened)
            return;

        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            OpenBarrier();
        }
    }

    //----------------------------------------------------

    public void PlayerEnter()
    {
        if (opened)
            return;

        playerInside = true;

        if (interactionUI != null)
            interactionUI.SetActive(true);
    }

    //----------------------------------------------------

    public void PlayerExit()
    {
        if (opened)
            return;

        playerInside = false;

        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    //----------------------------------------------------

    private void OpenBarrier()
    {
        if (opened)
            return;

        opened = true;
        playerInside = false;

        if (interactionUI != null)
            interactionUI.SetActive(false);

        StartCoroutine(FadeBarrier());
    }

    //----------------------------------------------------

    private IEnumerator FadeBarrier()
    {
        Color startColor = barrierRenderer.color;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fadeDuration;

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);

            barrierRenderer.color = c;

            yield return null;
        }

        //----------------------------------------
        // Matikan Barrier
        //----------------------------------------

        if (barrierCollider != null)
            barrierCollider.enabled = false;

        //----------------------------------------
        // Matikan Trigger Interaksi
        //----------------------------------------

        if (interactionTrigger != null)
            interactionTrigger.enabled = false;

        //----------------------------------------
        // Sembunyikan Prompt
        //----------------------------------------

        if (interactionUI != null)
            interactionUI.SetActive(false);

        //----------------------------------------
        // Matikan Sprite
        //----------------------------------------

        if (barrierRenderer != null)
            barrierRenderer.enabled = false;
    }
}