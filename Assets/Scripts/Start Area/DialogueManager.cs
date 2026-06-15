using TMPro;
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    [HideInInspector]
    public bool dialogFinished;

    private bool isTyping;
    private string currentText;

    private Coroutine typingCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDialogue(string text)
    {
        dialogFinished = false;

        currentText = text;

        dialoguePanel.SetActive(true);

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Kalau masih mengetik → tampilkan langsung semua teks
            if (isTyping)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

                dialogueText.text = currentText;

                isTyping = false;

                return;
            }

            // Kalau sudah selesai mengetik → tutup dialog
            dialoguePanel.SetActive(false);

            dialogFinished = true;
        }
    }
}