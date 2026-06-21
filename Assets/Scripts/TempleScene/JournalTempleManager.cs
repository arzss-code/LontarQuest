using UnityEngine;

public class JournalTempleManager : MonoBehaviour
{
    public static JournalTempleManager Instance;

    public bool IsJournalOpen =>
        journalUI.activeSelf;

    [Header("UI")]
    public GameObject journalUI;

    public GameObject page1;
    public GameObject page2;

    [Header("Player")]
    public PlayerController playerController;

    private bool isOpen;

    private int currentPage = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        journalUI.SetActive(false);

        page1.SetActive(true);
        page2.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(PuzzleManager.Instance.IsPuzzleOpen)
                return;

            ToggleJournal();
        }
    }

    public void ToggleJournal()
    {
        isOpen = !isOpen;

        journalUI.SetActive(isOpen);

        playerController.canMove = !isOpen;

        if(isOpen)
        {
            ShowPage(1);
        }
    }

    public void CloseJournal()
    {
        isOpen = false;

        journalUI.SetActive(false);

        playerController.canMove = true;
    }

    public void NextPage()
    {
        if(currentPage == 1)
        {
            ShowPage(2);
        }
    }

    public void PrevPage()
    {
        if(currentPage == 2)
        {
            ShowPage(1);
        }
    }

    void ShowPage(int page)
    {
        currentPage = page;

        page1.SetActive(page == 1);
        page2.SetActive(page == 2);
    }
}