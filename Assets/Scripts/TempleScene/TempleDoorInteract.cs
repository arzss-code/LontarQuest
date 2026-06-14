using System.Collections;
using TMPro;
using UnityEngine;

public class TempleDoorInteract : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text actionText;

    [Header("Dialogues")]
    [SerializeField] private IntroDialogue lockedDialogue;
    [SerializeField] private IntroDialogue openedDialogue;

    private bool playerInRange = false;
    private bool isOpeningSequenceStarted = false;

    private void Start()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);

        if (actionText != null)
            actionText.text = "Inspect";
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleDoorInteraction();
        }
    }

    private void HandleDoorInteraction()
    {
        if (isOpeningSequenceStarted)
            return;

        promptPanel.SetActive(false);

        // Puzzle belum selesai
        if (!CandlePuzzleManager.Instance.PuzzleCompleted)
        {
            if (lockedDialogue != null)
            {
                lockedDialogue.StartDialogue();
            }

            return;
        }

        // Puzzle selesai
        isOpeningSequenceStarted = true;

        StartCoroutine(OpenDoorSequence());
    }

    private IEnumerator OpenDoorSequence()
    {
        // Mainkan dialog pintu terbuka
        if (openedDialogue != null)
        {
            openedDialogue.StartDialogue();

            yield return new WaitUntil(() =>
                !openedDialogue.IsPlaying
            );
        }

        // Fade ke hitam
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(
                FadeManager.Instance.FadeOut()
            );
        }

        Debug.Log("FADE COMPLETE");

        // BESOK:
        // SceneManager.LoadScene("NextScene");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (promptPanel != null)
            promptPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}