using UnityEngine;
using UnityEngine.UI;

public class FatherRescueInteract : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private Image progressFill;

    [Header("Settings")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float holdDuration = 2f;

    [Header("Sequence")]
    [SerializeField] private FatherRescueSequence rescueSequence;

    private bool playerInside;
    private bool completed;
    private bool interactionEnabled = false;

    private float holdTimer;

    //-----------------------------------------------------

    private void Start()
    {
        if (canvasUI != null)
            canvasUI.SetActive(false);

        if (progressFill != null)
            progressFill.fillAmount = 0;
    }

    //-----------------------------------------------------

    private void Update()
    {   
        if (!interactionEnabled)
        return;

        if (completed)
            return;

        if (!playerInside)
            return;

        //---------------------------------------
        // Hold E
        //---------------------------------------

        if (Input.GetKey(interactKey))
        {
            Debug.Log("Holding E");

            holdTimer += Time.deltaTime;

            progressFill.fillAmount =
                Mathf.Clamp01(holdTimer / holdDuration);

            if (holdTimer >= holdDuration)
            {
                CompleteInteraction();
            }
        }
        else
        {
            ResetProgress();
        }
    }

    //-----------------------------------------------------

    void CompleteInteraction()
    {
        completed = true;

        playerInside = false;

        if (canvasUI != null)
            canvasUI.SetActive(false);

        progressFill.fillAmount = 1;

        Debug.Log("Father Rescue Triggered");

        Destroy(gameObject);

        if (rescueSequence != null)
            rescueSequence.Play();
    }

    //-----------------------------------------------------

    void ResetProgress()
    {
        holdTimer = 0;

        if (progressFill != null)
            progressFill.fillAmount = 0;
    }

    //-----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!interactionEnabled)
            return;

        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        if (canvasUI != null)
            canvasUI.SetActive(true);
    }

    //-----------------------------------------------------

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        ResetProgress();

        if (canvasUI != null)
            canvasUI.SetActive(false);
    }

    public void EnableInteraction()
    {
        interactionEnabled = true;

        Debug.Log("Father Interaction Enabled");

        // Jika player sudah berada di area
        Collider2D player = Physics2D.OverlapBox(
            GetComponent<BoxCollider2D>().bounds.center,
            GetComponent<BoxCollider2D>().bounds.size,
            0f,
            LayerMask.GetMask("Player"));

        if (player != null)
        {
            playerInside = true;

            if (canvasUI != null)
                canvasUI.SetActive(true);
        }
    }
}