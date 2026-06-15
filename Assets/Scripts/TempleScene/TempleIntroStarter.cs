using UnityEngine;

public class TempleIntroStarter : MonoBehaviour
{
    [SerializeField] private IntroDialogue introDialogue;

    private void Start()
    {
        introDialogue.StartDialogue();
    }
}