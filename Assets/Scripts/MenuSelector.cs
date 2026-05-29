using UnityEngine;

public class MenuSelector : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField]
    RectTransform firstButton;

    [SerializeField]
    float offsetX = -40f;

    void Awake()
    {
        rectTransform =
        GetComponent<RectTransform>();
    }

    void Start()
    {
        MoveToButton(
            firstButton
        );
    }

    public void MoveToButton(
        RectTransform button)
    {
        rectTransform.position =
        button.position +
        new Vector3(
            offsetX,
            0,
            0
        );
    }
}