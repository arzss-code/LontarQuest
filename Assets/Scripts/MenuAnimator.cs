using UnityEngine;
using System.Collections;

public class MenuAnimator : MonoBehaviour
{
    [SerializeField]
    Animator menuAnimator;

    bool isOpen;

    void Update()
    {
        if(isOpen &&
           Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        isOpen = true;

        menuAnimator.SetTrigger(
            "Open"
        );

        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        isOpen = false;

        menuAnimator.SetTrigger(
            "Close"
        );

        StartCoroutine(
            CloseRoutine()
        );
    }

    IEnumerator CloseRoutine()
    {
        yield return new WaitForSecondsRealtime(
            0.3f
        );

        Time.timeScale = 1f;
    }
}