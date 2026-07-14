using System.Collections;
using UnityEngine;

public class AltarPuzzleManager : MonoBehaviour
{
    [Header("Altars")]
    [SerializeField] private AltarPuzzle[] altars;

    [Header("Ending")]
    [SerializeField] private EndingPortalSequence endingPortalSequence;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip correctSFX;

    [SerializeField] private AudioClip wrongSFX;

    private bool isResetting = false;

    private int currentStep = 0;
    private readonly int[] correctOrder =
    {
        1,
        2,
        3,
        4,
        5
    };

    //--------------------------------------------------



    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        DisablePuzzle();
    }

    //--------------------------------------------------

    public void BeginPuzzle()
    {
        Debug.Log("Puzzle Started");

        foreach (AltarPuzzle altar in altars)
        {
            altar.EnableInteraction(true);
        }
    }

    //--------------------------------------------------

    public void DisablePuzzle()
    {
        foreach (AltarPuzzle altar in altars)
        {
            altar.EnableInteraction(false);
        }
    }

    public void TryActivate(AltarPuzzle altar)
    {
        if (isResetting)
            return;


        Debug.Log($"Current Step : {currentStep}");

        Debug.Log($"Player memilih : {altar.AltarID}");

        if (altar.AltarID == correctOrder[currentStep])
        {
            Debug.Log("BENAR");

            altar.ActivateCorrect();
            PlayCorrectSFX();

            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                Debug.Log("PUZZLE COMPLETE");

                DisablePuzzle();

                if (endingPortalSequence != null)
                    endingPortalSequence.Play();
            }
        }
        else
        {
            Debug.Log("SALAH");

            StartCoroutine(WrongRoutine());
        }
    }

    //--------------------------------------------------

    private IEnumerator WrongRoutine()
    {
        isResetting = true;

        foreach (AltarPuzzle altar in altars)
        {
            altar.ActivateWrong();
        }
        PlayWrongSFX();

        yield return new WaitForSeconds(0.8f);

        foreach (AltarPuzzle altar in altars)
        {
            altar.TurnOff();
        }

        currentStep = 0;

        isResetting = false;

        Debug.Log("Puzzle Reset");
    }

    private void PlayCorrectSFX()
    {
        if (audioSource == null || correctSFX == null)
            return;

        audioSource.PlayOneShot(correctSFX);
    }

    private void PlayWrongSFX()
    {
        if (audioSource == null || wrongSFX == null)
            return;

        audioSource.PlayOneShot(wrongSFX);
    }
}