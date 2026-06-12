using UnityEngine;

public class BowIndicatorSort : MonoBehaviour
{
    SpriteRenderer indicatorRenderer;

    SpriteRenderer playerRenderer;

    void Awake()
    {
        indicatorRenderer =
        GetComponent<SpriteRenderer>();

        playerRenderer =
        GetComponentInParent<PlayerController>()
        .GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if(playerRenderer == null)
            return;

        // selalu dibawah player
        indicatorRenderer.sortingOrder =
        playerRenderer.sortingOrder - 1;
    }
}