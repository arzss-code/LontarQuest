using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CandlePuzzleManager : MonoBehaviour
{
    public static CandlePuzzleManager Instance;

    [Header("Puzzle State")]
    public bool leftCandleLit;
    public bool rightCandleLit;

    [Header("Cutscene")]
    [SerializeField] private PlayableDirector doorCutsceneDirector;

    [SerializeField] private IntroDialogue postCompletionDialogue;

    private bool cutscenePlayed = false;

    private PlayerController playerController;

    public bool PuzzleCompleted =>
        leftCandleLit && rightCandleLit;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (cutscenePlayed)
            return;

        if (PuzzleCompleted)
        {
            cutscenePlayed = true;

            Debug.Log("PUZZLE COMPLETE");

            StartCoroutine(PuzzleCompleteRoutine());
        }
    }

    private IEnumerator PuzzleCompleteRoutine()
    {
        if (playerController != null)
            playerController.SetCanMove(false);

        if (doorCutsceneDirector != null)
        {
            doorCutsceneDirector.Play();

            yield return new WaitForSeconds(
                (float)doorCutsceneDirector.duration
            );
        }

        if (postCompletionDialogue != null)
        {
            postCompletionDialogue.StartDialogue();

            yield return new WaitUntil(() =>
                !postCompletionDialogue.IsPlaying
            );
        }

        if (playerController != null)
            playerController.SetCanMove(true);
    }
}