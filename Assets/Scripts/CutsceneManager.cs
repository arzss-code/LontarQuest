using System.Collections;
using TMPro;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;

    [Header("Dialog UI")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TMP_Text dialogText;

    [Header("Objective")]
    [SerializeField] private GameObject doorMarker;
    [SerializeField] private DoorTrigger doorTrigger;

    [Header("Dialog Lines")]
    [TextArea(2, 4)]
    [SerializeField] private string[] lines;

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool cutsceneActive = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        StartIntroCutscene();
    }

    private void Update()
    {
        if (!cutsceneActive)
            return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    private void StartIntroCutscene()
    {
        cutsceneActive = true;

        if (doorMarker != null)
            doorMarker.SetActive(false);

        if (playerController != null)
            playerController.SetCanMove(false);

        dialogPanel.SetActive(true);

        currentLineIndex = 0;
        ShowLine(lines[currentLineIndex]);
    }

    private void ShowLine(string line)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in line)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogText.text = lines[currentLineIndex];
        isTyping = false;
    }

    private void ShowNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= lines.Length)
        {
            EndCutscene();
            return;
        }

        ShowLine(lines[currentLineIndex]);
    }

    private void EndCutscene()
    {
        Debug.Log("CUTSCENE SELESAI - PLAYER BISA GERAK");

        cutsceneActive = false;
        dialogPanel.SetActive(false);

        if (playerController != null)
        {
            playerController.SetCanMove(true);
        }
        else
        {
            Debug.LogError("PlayerController belum diisi di CutsceneManager!");
        }

        if (doorMarker != null)
        {
            doorMarker.SetActive(true);
        }

        if (doorTrigger != null)
        {
            doorTrigger.EnableDoor();
        }
    }
}