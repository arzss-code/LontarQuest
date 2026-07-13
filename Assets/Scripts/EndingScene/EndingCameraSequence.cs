using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class EndingCameraSequence : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera cmPlayer;
    [SerializeField] private CinemachineCamera cmFather;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private SpriteRenderer fatherSprite;

    [Header("Timing")]
    [SerializeField] private float revealDuration = 1.5f;
    [SerializeField] private float cameraStayTime = 2f;

    [Header("Dialogues")]
    [SerializeField] private IntroDialogue dialogueD;

    //--------------------------------------------------
    public void Play()
    {
        StartCoroutine(CameraRoutine());
    }

    //--------------------------------------------------

    private IEnumerator CameraRoutine()
    {
        Debug.Log("Camera Sequence Start");

        //----------------------------------
        // Freeze Player
        //----------------------------------

        player.SetCanMove(false);

        //----------------------------------
        // Kamera ke Ayah
        //----------------------------------

        cmPlayer.Priority = 5;
        cmFather.Priority = 20;

        // Tunggu kamera selesai berpindah
        yield return new WaitForSeconds(1f);

       //----------------------------------
        // Reveal Ayah
        //----------------------------------

        yield return StartCoroutine(RevealFather());

        yield return new WaitForSeconds(0.5f);

        dialogueD.StartDialogue();

        yield return new WaitUntil(() =>
            dialogueD.HasFinished());

        //----------------------------------
        // Diam sebentar
        //----------------------------------

        // yield return new WaitForSeconds(cameraStayTime);

        //----------------------------------
        // Kamera kembali
        //----------------------------------

        cmFather.Priority = 5;
        cmPlayer.Priority = 20;

        //----------------------------------
        // Player bisa bergerak lagi
        //----------------------------------

        player.SetCanMove(true);

        Debug.Log("Camera Sequence Finish");
    }

    //--------------------------------------------------

    private IEnumerator RevealFather()
    {
        Color startColor =
            Color.black;

        Color targetColor =
            new Color(
                115f / 255f,
                114f / 255f,
                114f / 255f,
                1f);

        fatherSprite.color = startColor;

        float timer = 0;

        while (timer < revealDuration)
        {
            timer += Time.deltaTime;

            fatherSprite.color =
                Color.Lerp(
                    startColor,
                    targetColor,
                    timer / revealDuration);

            yield return null;
        }

        fatherSprite.color = targetColor;
    }
}