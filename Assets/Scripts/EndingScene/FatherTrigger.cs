using UnityEngine;

public class FatherTrigger : MonoBehaviour
{
    [SerializeField] private FatherCutscene fatherCutscene;

    [Header("Firefly")]
    [SerializeField] private GameObject fireflyObject;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        triggered = true;

        //----------------------------------
        // Hilangkan Firefly
        //----------------------------------

        if (fireflyObject != null)
            fireflyObject.SetActive(false);

        //----------------------------------
        // Mulai Cutscene
        //----------------------------------

        fatherCutscene.Play();

        gameObject.SetActive(false);
    }
}