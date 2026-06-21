using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    public Transform waypoint1;
    public Transform waypoint2;

    public PlayerController playerController;
    public DialogueManager dialogueManager;
    public Animator animator;

    [Header("Objects")]
    // public GameObject prasastiHighlight;

    [Header("Settings")]
    public float walkSpeed = 2f;

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        // Lock movement
        playerController.canMove = false;

        //-----------------------
        // JALAN TURUN
        //-----------------------

        SetAnimationDirection(0, -1);

        yield return MoveTo(waypoint1);

        animator.SetFloat("Speed", 0);

        // lihat ke kanan
        animator.SetFloat("LastMoveX", 1);
        animator.SetFloat("LastMoveY", 0);

        yield return new WaitForSeconds(0.8f);

        // lihat ke bawah lagi
        animator.SetFloat("LastMoveX", 0);
        animator.SetFloat("LastMoveY", -1);

        yield return new WaitForSeconds(0.8f);

        //-----------------------
        // DIALOG AWAL
        //-----------------------

        yield return ShowDialogueAndWait(
            "Wah... tempat ini ternyata jauh lebih luas."
        );

        yield return ShowDialogueAndWait(
            "Dari luar cuma terlihat seperti reruntuhan biasa."
        );

        yield return ShowDialogueAndWait(
            "Tapi di dalamnya seperti menyimpan sesuatu..."
        );

        //-----------------------
        // MELIHAT PRASASTI
        //-----------------------

        animator.SetFloat("LastMoveX", -1);
        animator.SetFloat("LastMoveY", 0);

        yield return new WaitForSeconds(1f);

        yield return ShowDialogueAndWait(
            "Hm?"
        );

        yield return ShowDialogueAndWait(
            "Prasasti itu..."
        );

        yield return ShowDialogueAndWait(
            "Simbolnya mirip dengan yang pernah Ayah catat di jurnalnya."
        );

        //-----------------------
        // JALAN KE PRASASTI
        //-----------------------

        SetAnimationDirection(-1, 0);

        yield return MoveTo(waypoint2);

        animator.SetFloat("Speed", 0);

        animator.SetFloat("LastMoveX", -1);
        animator.SetFloat("LastMoveY", 0);

        yield return new WaitForSeconds(0.5f);

        //-----------------------
        // QUEST
        //-----------------------

        // prasastiHighlight.SetActive(true);

        yield return ShowDialogueAndWait(
            "Aku harus memeriksanya lebih dekat."
        );

        //-----------------------
        // PLAYER BISA GERAK
        //-----------------------

        playerController.canMove = true;
    }

    IEnumerator MoveTo(Transform target)
    {
        while (
            Vector3.Distance(
                player.position,
                target.position
            ) > 0.05f)
        {
            player.position =
                Vector3.MoveTowards(
                    player.position,
                    target.position,
                    walkSpeed * Time.deltaTime
                );

            yield return null;
        }
    }

    IEnumerator ShowDialogueAndWait(string text)
    {
        dialogueManager.ShowDialogue(text);

        yield return new WaitUntil(
            () => dialogueManager.dialogFinished
        );
    }

    void SetAnimationDirection(float x, float y)
    {
        animator.SetFloat("MoveX", x);
        animator.SetFloat("MoveY", y);

        animator.SetFloat("LastMoveX", x);
        animator.SetFloat("LastMoveY", y);

        animator.SetFloat("Speed", 1);
    }
}