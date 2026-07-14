using UnityEngine;
using System.Collections;
using UnityEngine.Video;

public class FatherCutscene : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;

    [SerializeField] private Transform interactionPoint;

    [SerializeField] private float walkSpeed = 2.2f;

    [SerializeField] private float stopDistance = 0.05f;

    [SerializeField] private FlashbackPlayer flashbackPlayer;
    [Header("Flashback")]
    [SerializeField] private VideoClip videoClip1;



    [Header("Dialogue")]
    [SerializeField] private IntroDialogue dialogueE;
    [SerializeField] private IntroDialogue dialogueF;

   [SerializeField] private FatherRescueInteract fatherInteract;
    private Animator playerAnimator;

    private bool playing;

    //--------------------------------------------------

    private void Awake()
    {
        playerAnimator =
            player.GetComponentInChildren<Animator>();
    }

    //--------------------------------------------------

    public void Play()
    {
        if (playing)
            return;

        StartCoroutine(CutsceneRoutine());
    }

    //--------------------------------------------------

    IEnumerator CutsceneRoutine()
    {
        playing = true;

        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Jalan menuju Ayah
        //----------------------------------

        while (Vector2.Distance(
            player.transform.position,
            interactionPoint.position) > stopDistance)
        {
            player.transform.position =
                Vector2.MoveTowards(
                    player.transform.position,
                    interactionPoint.position,
                    walkSpeed * Time.deltaTime);

            //----------------------------------
            // Walk Up
            //----------------------------------

            playerAnimator.SetFloat("MoveX", 0);
            playerAnimator.SetFloat("MoveY", 1);

            playerAnimator.SetFloat("LastMoveX", 0);
            playerAnimator.SetFloat("LastMoveY", 1);

            playerAnimator.SetFloat("Speed", 1);

            yield return null;
        }

        //----------------------------------
        // Idle Up
        //----------------------------------

        playerAnimator.SetFloat("Speed", 0);

        playerAnimator.SetFloat("MoveX", 0);
        playerAnimator.SetFloat("MoveY", 0);

        playerAnimator.SetFloat("LastMoveX", 0);
        playerAnimator.SetFloat("LastMoveY", 1);

        //----------------------------------
        // Dialogue
        //----------------------------------

        //----------------------------------
        // Dialogue E
        //----------------------------------

        dialogueE.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueE.HasFinished());

        //----------------------------------
        // Flashback
        //----------------------------------

        yield return StartCoroutine(
            flashbackPlayer.PlayVideo(videoClip1));

        //----------------------------------
        // Dialogue F
        //----------------------------------

        dialogueF.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueF.HasFinished());

        //----------------------------------
        // Enable Interaction
        //----------------------------------

        fatherInteract.EnableInteraction();

        //----------------------------------
        // Player bebas bergerak
        //----------------------------------

        player.SetCanMove(true);

        playing = false;
    }
}