using UnityEngine;
using UnityEngine.EventSystems;

public class MenuHoverButton :
    MonoBehaviour,
    IPointerEnterHandler
{
    public enum MenuType
    {
        Story,
        Settings,
        Credits,
        Quit
    }

    public MenuType menuType;

    public MenuContentController contentController;

    public void OnPointerEnter(
    PointerEventData eventData
    )
    {
        Debug.Log(
            "Hover : " + menuType
        );

        switch(menuType)
        {
            case MenuType.Story:
                contentController.ShowSynopsis();
                break;

            case MenuType.Settings:
                contentController.ShowSettings();
                break;

            case MenuType.Credits:
                contentController.ShowCredits();
                break;

            case MenuType.Quit:
                contentController.ShowQuit();
                break;
        }
    }
}