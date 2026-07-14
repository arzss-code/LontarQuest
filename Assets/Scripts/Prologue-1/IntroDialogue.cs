using System;
using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;

    [TextArea(3, 5)]
    public string text;
}

public class IntroDialogue : MonoBehaviour
{
    public bool IsPlaying { get; private set; }

    public event Action OnDialogueFinished;

    // BARU
    public event Action<int> OnLineChanged;

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Dialogue Data")]
    [SerializeField] private DialogueLine[] dialogues;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Behaviour")]
    [SerializeField] private bool freezePlayer = true;

    [SerializeField] private bool unfreezePlayerWhenFinished = true;

    [SerializeField] private FlashbackPlayer flashbackPlayer;

    private PlayerController playerController;

    private bool isTyping;
    private bool skipTyping;
    private bool nextDialogue;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        flashbackPlayer = FindFirstObjectByType<FlashbackPlayer>();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlaying)
            return;

        //----------------------------------
        // Lock Input Saat Video Diputar
        //----------------------------------

        if (flashbackPlayer != null &&
            flashbackPlayer.IsPlaying)
            return;

        //----------------------------------
        // Input
        //----------------------------------

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else
            {
                nextDialogue = true;
            }
        }
    }

    public void StartDialogue()
    {
        if (IsPlaying)
            return;

        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        IsPlaying = true;

        if (freezePlayer && playerController != null)
            playerController.SetCanMove(false);

        dialoguePanel.SetActive(true);

        for (int i = 0; i < dialogues.Length; i++)
        {
            DialogueLine dialogue = dialogues[i];

            // BARU
            OnLineChanged?.Invoke(i);

            nameText.text = dialogue.speaker;

            yield return StartCoroutine(TypeText(dialogue.text));

            nextDialogue = false;

            yield return new WaitUntil(() => nextDialogue);
        }

        dialoguePanel.SetActive(false);

        if (unfreezePlayerWhenFinished && playerController != null)
            playerController.SetCanMove(true);

        IsPlaying = false;

        OnDialogueFinished?.Invoke();
    }

    private IEnumerator TypeText(string textToType)
    {
        dialogueText.text = "";

        isTyping = true;
        skipTyping = false;

        foreach (char letter in textToType)
        {
            if (skipTyping)
            {
                dialogueText.text = textToType;
                break;
            }

            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public bool HasFinished()
    {
        return !IsPlaying && !dialoguePanel.activeSelf;
    }
}