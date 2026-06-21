using TMPro;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public bool IsPuzzleOpen =>
        puzzlePanel.activeSelf;

    [Header("Altar Fires")]
    public GameObject[] altarFires;

    [Header("Portal")]
    public GameObject portal;

    [Header("Panel")]
    public GameObject puzzlePanel;

    [Header("Question UI")]
    public TMP_Text aksaraText;
    public TMP_Text questionText;
    public TMP_Text resultText;

    [Header("Choice Text")]
    public TMP_Text choiceAText;
    public TMP_Text choiceBText;
    public TMP_Text choiceCText;

    [Header("Choice Buttons")]
    public GameObject choiceA;
    public GameObject choiceB;
    public GameObject choiceC;

    [Header("Player")]
    public PlayerController playerController;

    [Header("Puzzle Data")]
    public PuzzleData[] puzzles;

    private int currentPuzzleID;

    private bool[] solvedPuzzles;

    private void Awake()
    {
        Instance = this;

        solvedPuzzles =
            new bool[puzzles.Length];
    }

    private void Start()
    {
        puzzlePanel.SetActive(false);
        portal.SetActive(false);

        foreach(GameObject fire in altarFires)
        {
            fire.SetActive(false);
        }
    }

    private bool AllPuzzlesSolved()
    {
        foreach(bool solved in solvedPuzzles)
        {
            if(!solved)
                return false;
        }

        return true;
    }

    public void OpenPuzzle(int puzzleID)
    {
        currentPuzzleID = puzzleID;
        choiceA.SetActive(true);
        choiceB.SetActive(true);
        choiceC.SetActive(true);

        PuzzleData puzzle =
            puzzles[puzzleID];

        aksaraText.text =
            puzzle.aksaraText;

        questionText.text =
            puzzle.question;

        choiceAText.text =
            puzzle.choiceA;

        choiceBText.text =
            puzzle.choiceB;

        choiceCText.text =
            puzzle.choiceC;

        resultText.text = "";

        puzzlePanel.SetActive(true);

        playerController.canMove = false;
    }

    public void SelectAnswer(int answerIndex)
    {
        PuzzleData puzzle =
            puzzles[currentPuzzleID];

        if(answerIndex ==
           puzzle.correctAnswer)
        {
            AnswerCorrect();
        }
        else
        {
            AnswerWrong();
        }
    }

    void AnswerCorrect()
    {
        if (solvedPuzzles[currentPuzzleID])
            return;

        solvedPuzzles[currentPuzzleID] = true;

        aksaraText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);

        choiceA.SetActive(false);
        choiceB.SetActive(false);
        choiceC.SetActive(false);

        int totalSolved = 0;

        for(int i = 0; i < solvedPuzzles.Length; i++)
        {
            if(solvedPuzzles[i])
                totalSolved++;
        }

        resultText.text =
            "Pesan berhasil diterjemahkan!\n\n" +
            "Obor " + totalSolved +
            " dari 5 menyala.";

        if(AllPuzzlesSolved())
        {
            portal.SetActive(true);

            resultText.text =
                "Kelima prasasti berhasil diterjemahkan!\n\n" +
                "Semua obor menyala.\n" +
                "Portal menuju Dimensi Lontar terbuka!";
        }

        if(currentPuzzleID <
        altarFires.Length)
        {
            altarFires[currentPuzzleID]
                .SetActive(true);
        }

        Debug.Log(
            "Puzzle " +
            currentPuzzleID +
            " solved!"
        );

        Invoke(nameof(ClosePuzzle), 2f);
    }

    void AnswerWrong()
    {
        aksaraText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);
        choiceA.SetActive(false);
        choiceB.SetActive(false);
        choiceC.SetActive(false);

        resultText.text =
            "Jawaban salah.\nCoba lagi.";

        Invoke(
            nameof(ResetQuestion),
            2f
        );
    }

    void ResetQuestion()
    {
        resultText.text = "";

        aksaraText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);
        choiceA.SetActive(true);
        choiceB.SetActive(true);
        choiceC.SetActive(true);
    }

    public void ClosePuzzle()
    {
        puzzlePanel.SetActive(false);

        playerController.canMove = true;

        aksaraText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);

        choiceA.SetActive(true);
        choiceB.SetActive(true);
        choiceC.SetActive(true);

        resultText.text = "";
    }

    public bool IsSolved(
        int puzzleID)
    {
        return solvedPuzzles[puzzleID];
    }
}