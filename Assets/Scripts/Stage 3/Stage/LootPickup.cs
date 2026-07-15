using System.Collections;
using UnityEngine;

public class LootPickup : MonoBehaviour
{
    [Header("Pop Animation")]
    [SerializeField] private float popHeight = 0.7f;
    [SerializeField] private float popDistance = 0.5f;
    [SerializeField] private float popDuration = 0.35f;

    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject pickupPrompt;

    private bool playerInside;
    private bool pickedUp;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);

        StartCoroutine(PopRoutine());
    }

    private IEnumerator PopRoutine()
    {
        Vector3 endPos =
            startPos +
            new Vector3(Random.Range(-popDistance, popDistance), 0, 0);

        float timer = 0;

        while (timer < popDuration)
        {
            timer += Time.deltaTime;

            float t = timer / popDuration;

            Vector3 pos =
                Vector3.Lerp(startPos, endPos, t);

            // Lengkungan parabola
            pos.y += Mathf.Sin(t * Mathf.PI) * popHeight;

            transform.position = pos;

            yield return null;
        }

        transform.position = endPos;
    }

    private void Update()
    {
        if (!playerInside)
            return;

        if (pickedUp)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            PickUp();
        }
    }

    public void PlayerEnter()
    {
        playerInside = true;

        if (pickupPrompt != null)
            pickupPrompt.SetActive(true);
    }

    public void PlayerExit()
    {
        playerInside = false;

        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    private void PickUp()
    {
        pickedUp = true;

        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);

        Debug.Log("Loot Picked!");

        //----------------------------------
        // Show Reward UI
        //----------------------------------

        if (BoonUIManager.Instance != null)
        {
            BoonUIManager.Instance.ShowBoonSelection();
        }

        //----------------------------------
        // Destroy Loot
        //----------------------------------

        Destroy(gameObject);
    }
}