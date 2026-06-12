using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour,
    IPointerEnterHandler
{
    public MenuSelector menuSelector;

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        menuSelector.MoveToButton(
            transform as RectTransform
        );
    }
}