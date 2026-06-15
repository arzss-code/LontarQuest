using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleInteract : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TMP_Text actionText;

    [Header("Light")]
    [SerializeField] private Light2D candleLight;
    [SerializeField] private bool isLeftCandle;

    private bool playerInRange;
    private bool isLit;

    private void Start()
    {
        promptPanel.SetActive(false);

        if (actionText != null)
            actionText.text = "Light";

        if (candleLight != null)
            candleLight.enabled = false;
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (isLit)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            LightCandle();
        }
    }

    private void LightCandle()
    {
        isLit = true;

        if (candleLight != null)
            candleLight.enabled = true;

        if (isLeftCandle)
        {
            CandlePuzzleManager.Instance.leftCandleLit = true;
        }
        else
        {
            CandlePuzzleManager.Instance.rightCandleLit = true;
        }

        promptPanel.SetActive(false);

        Debug.Log("CANDLE LIT");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isLit)
            return;

        playerInRange = true;

        promptPanel.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        promptPanel.SetActive(false);
    }
}