using UnityEngine;

public class Area01IntroStarter : MonoBehaviour
{
    [SerializeField] private IntroDialogue introDialogue;

    private void Start()
    {
        Debug.Log("AREA01 START TERPANGGIL");

        introDialogue.StartDialogue();
    }
}