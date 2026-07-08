using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class BossEndingCutscene : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cmPlayer;
    [SerializeField] private CinemachineCamera cmArtifact;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Artifact")]
    [SerializeField] private GameObject artifact;
    [SerializeField] private Animator artifactAnimator;

    [Header("Positions")]
    [SerializeField] private Transform playerTeleportPoint;
    [SerializeField] private Transform artifactTargetPoint;

    [Header("Timing")]
    [SerializeField] private float artifactAppearDelay = 0.5f;
    [SerializeField] private float playerTeleportDelay = 1f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float stopDistance = 0.08f;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject playerHudBar;
    [SerializeField] private GameObject questCanvas;

    [SerializeField] private Animator playerAnimator;

    

    //------------------------------------------------------------

    private void Awake()
    {
        if (artifact != null)
            artifact.SetActive(false);
    }

    //------------------------------------------------------------

    public void Play()
    {
        StartCoroutine(Sequence());
    }

    //------------------------------------------------------------

    private IEnumerator Sequence()
    {
        Debug.Log("===== Ending Sequence =====");

        //----------------------------------
        // Hide Gameplay UI
        //----------------------------------

        if (playerHudBar != null)
            playerHudBar.SetActive(false);

        if (questCanvas != null)
            questCanvas.SetActive(false);

        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Kamera ke Artifact
        //----------------------------------

        cmPlayer.Priority = 5;
        cmArtifact.Priority = 20;

        //----------------------------------
        // Tunggu
        //----------------------------------

        yield return new WaitForSeconds(artifactAppearDelay);

        //----------------------------------
        // Munculkan Artifact
        //----------------------------------

        artifact.SetActive(true);

        if (artifactAnimator != null)
        {
            artifactAnimator.Rebind();
            artifactAnimator.Update(0f);
        }

        //----------------------------------
        // Tunggu lagi
        //----------------------------------

        yield return new WaitForSeconds(playerTeleportDelay);

        //----------------------------------
        // Teleport Player
        //----------------------------------

        player.transform.position =
            playerTeleportPoint.position;

        //----------------------------------
        // Jalan menuju Artifact
        //----------------------------------

        yield return WalkToArtifact();

        Debug.Log("Player sampai di Artifact");
    }

    //------------------------------------------------------------

    private IEnumerator WalkToArtifact()
    {
        while (Vector2.Distance(
            player.transform.position,
            artifactTargetPoint.position) > stopDistance)
        {
            //----------------------------------
            // Gerakkan Player
            //----------------------------------

            player.transform.position =
                Vector2.MoveTowards(
                    player.transform.position,
                    artifactTargetPoint.position,
                    walkSpeed * Time.deltaTime);

            //----------------------------------
            // Animasi Walk Up
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

        playerAnimator.SetFloat("MoveX", 0);
        playerAnimator.SetFloat("MoveY", 0);

        playerAnimator.SetFloat("LastMoveX", 0);
        playerAnimator.SetFloat("LastMoveY", 1);

        playerAnimator.SetFloat("Speed", 0);
    }
}