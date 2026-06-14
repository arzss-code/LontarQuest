using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class JeepInteract : MonoBehaviour
{
    [SerializeField] private GameObject promptCanvas;

    private bool playerInside;

    private void Start()
    {
        promptCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        promptCanvas.SetActive(true);

        Debug.Log("PLAYER DEKAT JEEP");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        promptCanvas.SetActive(false);

        Debug.Log("PLAYER MENJAUH DARI JEEP");
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterJeep();
        }
    }

    private void EnterJeep()
    {
        StartCoroutine(EnterJeepRoutine());
    }

    private IEnumerator EnterJeepRoutine()
    {
        Debug.Log("FADE OUT DIMULAI");

        yield return StartCoroutine(
            FadeManager.Instance.FadeOut()
        );

        Debug.Log("LOAD CUTSCENE");

        SceneManager.LoadScene("Cutscene_to_1-1");
    }
}