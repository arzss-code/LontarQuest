using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class GarageCutsceneTest : MonoBehaviour
{
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera garageCamera;
    [SerializeField] private JeepReveal jeepReveal;
    [SerializeField] private IntroDialogue garageDialogue;

    private PlayerController playerController;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
    }

    public void StartCutscene()
    {
        StartCoroutine(CutsceneRoutine());
    }

    private IEnumerator CutsceneRoutine()
    {
        playerController.SetCanMove(false);

        garageCamera.Priority = 20;
        playerCamera.Priority = 10;

        jeepReveal.Reveal();

        yield return new WaitForSeconds(3f);

        garageCamera.Priority = 5;
        playerCamera.Priority = 10;

        yield return new WaitForSeconds(0.5f);

        garageDialogue.StartDialogue();

        while (garageDialogue.IsPlaying)
        {
            yield return null;
        }

        playerController.SetCanMove(true);
    }
}