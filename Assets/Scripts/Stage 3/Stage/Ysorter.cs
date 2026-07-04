using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSorter : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField]
    private int offset = 0;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        sr.sortingOrder =
            Mathf.RoundToInt(-transform.position.y * 100) + offset;
    }
}