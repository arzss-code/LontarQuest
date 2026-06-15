using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionController : MonoBehaviour
{
    [Header("Sections")]
    [SerializeField] private GameObject section1;
    [SerializeField] private GameObject section2;

    [Header("Jeep")]
    [SerializeField] private JeepMoveTest jeep;

    [Header("Section 2 Points")]
    [SerializeField] private Transform section2StartPoint;
    [SerializeField] private Transform section2EndPoint;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName = "Cutscene";

    private bool section1Finished;
    private bool section2Finished;

    private void Update()
    {
        // SECTION 1 SELESAI
        if (!section1Finished && jeep.HasReachedEnd)
        {
            section1Finished = true;

            StartCoroutine(GoToSection2());
        }
    }

    private IEnumerator GoToSection2()
    {
        yield return StartCoroutine(
            FadeManager.Instance.FadeOut()
        );

        Debug.Log("PINDAH KE SECTION 2");

        section1.SetActive(false);
        section2.SetActive(true);

        jeep.ResetMovement(
            section2StartPoint,
            section2EndPoint
        );

        yield return StartCoroutine(
            FadeManager.Instance.FadeIn()
        );

        StartCoroutine(CheckSection2End());
    }

    private IEnumerator CheckSection2End()
    {
        while (!jeep.HasReachedEnd)
        {
            yield return null;
        }

        if (section2Finished)
            yield break;

        section2Finished = true;

        yield return StartCoroutine(
            FadeManager.Instance.FadeOut()
        );

        Debug.Log("LOAD SCENE CUTSCENE");

        SceneManager.LoadScene(nextSceneName);
    }
}