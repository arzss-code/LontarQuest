using UnityEngine;

public class MenuContentController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    GameObject synopsisPanel;

    [SerializeField]
    GameObject settingsPanel;

    [SerializeField]
    GameObject creditPanel;

    [SerializeField]
    GameObject quitPanel;
    [SerializeField]
    GameObject storySelectionPanel;


    void Start()
    {
        ShowSynopsis();
    }

    void HideAllPanels()
    {
        synopsisPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditPanel.SetActive(false);
        quitPanel.SetActive(false);
        storySelectionPanel.SetActive(false);
    }

    public void ShowSynopsis()
    {
        HideAllPanels();
        synopsisPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void ShowCredits()
    {
        HideAllPanels();
        creditPanel.SetActive(true);
    }

    public void ShowQuit()
    {
        HideAllPanels();
        quitPanel.SetActive(true);
    }

    public void ShowStorySelection()
    {
        HideAllPanels();

        storySelectionPanel.SetActive(
            true
        );
    }
}