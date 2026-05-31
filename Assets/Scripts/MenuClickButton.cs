using UnityEngine;

public class MenuClickButton : MonoBehaviour
{
    public enum MenuType
    {
        Story,
        Quit
    }

    public MenuType menuType;

    public MenuContentController contentController;

    public void OnClick()
    {
        switch(menuType)
        {
            case MenuType.Story:

                contentController.ShowStorySelection();

                break;

            case MenuType.Quit:

                Application.Quit();

                break;
        }
    }
}