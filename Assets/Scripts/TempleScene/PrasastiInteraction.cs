using UnityEngine;

public class PrasastiInteraction : MonoBehaviour
{
    [Header("Puzzle")]
    public int puzzleID;

    [Header("UI")]
    public GameObject ePrompt;

    [Header("Visual")]
    // public GameObject highlight;

    private bool playerNearby;

    private void Start()
    {
        if (ePrompt != null)
            ePrompt.SetActive(false);

        // if (highlight != null)
        //     highlight.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby)
            return;

        // Kalau puzzle sudah selesai,
        // jangan bisa dibuka lagi
        if (PuzzleManager.Instance.IsSolved(puzzleID))
            return;

        if(JournalTempleManager.Instance.IsJournalOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PuzzleManager.Instance.OpenPuzzle(puzzleID);

            if (ePrompt != null)
                ePrompt.SetActive(false);
        }
    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Masuk Trigger : " + other.name);

        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        Debug.Log("PLAYER TERDETEKSI");

        if (PuzzleManager.Instance.IsSolved(puzzleID))
            return;

        playerNearby = true;

        if (ePrompt != null)
            ePrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        playerNearby = false;

        if (ePrompt != null)
            ePrompt.SetActive(false);

        // if (highlight != null)
        //     highlight.SetActive(false);
    }
}