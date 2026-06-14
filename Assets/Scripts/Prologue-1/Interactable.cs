using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class Interactable : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptCanvas;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text actionText;

    [Header("Progress Bar")]
    [SerializeField] private GameObject progressBarBG;
    [SerializeField] private Image fillImage;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 2f;
    [SerializeField] private float resultDuration = 1f; // No Key tampil 2 detik

    [Header("Interaction Type")]
    [SerializeField] private bool containsKey = false;

    [Header("Door")]
    [SerializeField] private Collider2D exitDoorTrigger;
    [SerializeField] private PlayableDirector doorRevealDirector;

    private bool playerInside = false;
    private bool isSearching = false;
    private bool hasBeenChecked = false;

    private PlayerController playerController;
    private Coroutine searchingAnimationCoroutine;

    private void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        if (promptCanvas != null)
            promptCanvas.SetActive(false);

        if (progressBarBG != null)
            progressBarBG.SetActive(false);

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (hasBeenChecked)
            return;

        playerInside = true;

        promptCanvas.SetActive(true);

        keyText.gameObject.SetActive(true);
        actionText.text = "Check";

        progressBarBG.SetActive(false);
        fillImage.fillAmount = 0f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        if (promptCanvas != null)
            promptCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (hasBeenChecked)
            return;

        if (isSearching)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SearchRoutine());
        }
    }

    private IEnumerator SearchRoutine()
    {
        isSearching = true;

        playerController.SetCanMove(false);

        // Hide [E]
        keyText.gameObject.SetActive(false);

        // Show progress bar
        progressBarBG.SetActive(true);
        fillImage.fillAmount = 0f;

        // Start animated text
        searchingAnimationCoroutine = StartCoroutine(AnimateSearching());

        float timer = 0f;

        while (timer < searchDuration)
        {
            timer += Time.deltaTime;

            fillImage.fillAmount = timer / searchDuration;

            yield return null;
        }

        fillImage.fillAmount = 1f;

        // Stop searching animation
        isSearching = false;

        if (searchingAnimationCoroutine != null)
        {
            StopCoroutine(searchingAnimationCoroutine);
            searchingAnimationCoroutine = null;
        }

        // Small delay biar progress bar sempat penuh
        yield return new WaitForSeconds(0.2f);

        progressBarBG.SetActive(false);

        // Result
        if (containsKey)
        {
            actionText.text = "Key Found!";

            GameManager.Instance.HasKey = true;

            yield return new WaitForSeconds(1f);

            if (doorRevealDirector != null)
            {
                doorRevealDirector.Play();

                // tunggu Timeline selesai
                yield return new WaitForSeconds(
                    (float)doorRevealDirector.duration
                );
            }

            if (exitDoorTrigger != null)
                exitDoorTrigger.enabled = true;
        }
        else
        {
            actionText.text = "No Key";
        }

        // Delay tambahan sebelum panel hilang
        yield return new WaitForSeconds(resultDuration);

        hasBeenChecked = true;

        promptCanvas.SetActive(false);

        playerController.SetCanMove(true);
    }

    private IEnumerator AnimateSearching()
    {
        while (isSearching)
        {
            actionText.text = "Searching.";
            yield return new WaitForSeconds(0.25f);

            if (!isSearching) yield break;

            actionText.text = "Searching..";
            yield return new WaitForSeconds(0.25f);

            if (!isSearching) yield break;

            actionText.text = "Searching...";
            yield return new WaitForSeconds(0.25f);
        }
    }
}