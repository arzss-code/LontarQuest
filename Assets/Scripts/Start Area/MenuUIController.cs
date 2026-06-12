using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    [SerializeField]
    GameObject menuPanel;

    bool isOpen;

    void Start()
    {
        menuPanel.SetActive(false);
    }

    public bool IsOpen()
    {
        return isOpen;
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);

        isOpen = true;

        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);

        isOpen = false;

        Time.timeScale = 1f;
    }

    void Update()
    {
        if(!isOpen)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }
}