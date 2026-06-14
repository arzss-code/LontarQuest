using System.Collections;
using UnityEngine;
using TMPro;

public class ExitDoor : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptCanvas;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text actionText;

    [Header("Teleport")]
    [SerializeField] private Transform area02Spawn;

    [Header("Cutscene")]
    [SerializeField] private GarageCutsceneTest garageCutscene;

    private bool playerInside;
    private bool isEntering;

    private void Start()
    {
        promptCanvas.SetActive(false);

        // Pintu baru aktif setelah kunci ditemukan
        GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!GameManager.Instance.HasKey)
            return;

        playerInside = true;

        promptCanvas.SetActive(true);

        keyText.text = "E";
        actionText.text = "Enter";

        Debug.Log("Player dapat masuk pintu.");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        promptCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (isEntering)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EnterDoorRoutine());
        }
    }

    private IEnumerator EnterDoorRoutine()
    {
        isEntering = true;
        playerInside = false;

        promptCanvas.SetActive(false);

        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        player.SetCanMove(false);

        // Fade Out
        yield return StartCoroutine(
            FadeManager.Instance.FadeOut()
        );

        // Teleport ke Area02
        if (area02Spawn != null)
        {
            player.transform.position =
                area02Spawn.position;
        }
        else
        {
            Debug.LogError("Area02Spawn belum di-assign!");
        }

        // Fade In
        yield return StartCoroutine(
            FadeManager.Instance.FadeIn()
        );

        // Jalankan cutscene garasi
        if (garageCutscene != null)
        {
            garageCutscene.StartCutscene();
        }
        else
        {
            Debug.LogError("GarageCutscene belum di-assign!");
            player.SetCanMove(true);
        }
    }
}