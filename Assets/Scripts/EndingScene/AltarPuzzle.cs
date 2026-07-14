using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AltarPuzzle : MonoBehaviour
{
    [Header("Altar")]
    [SerializeField] private int altarID;

    [Header("Components")]
    [SerializeField] private GameObject interactionArea;
    [SerializeField] private Light2D spotLight;

    [SerializeField] private Color correctColor = Color.cyan;

    [SerializeField] private Color wrongColor = Color.red;

    

    private AltarPuzzleManager puzzleManager;

    public int AltarID => altarID;

    //------------------------------------------------

    private void Awake()
    {
        if (interactionArea != null)
            interactionArea.SetActive(false);

        if (spotLight != null)
            spotLight.enabled = false;

        puzzleManager = FindFirstObjectByType<AltarPuzzleManager>();
    }

    //------------------------------------------------

    public void EnableInteraction(bool value)
    {
        if (interactionArea != null)
            interactionArea.SetActive(value);
    }

    //------------------------------------------------

    public void Interact()
    {
        Debug.Log($"Altar {altarID} Interacted");

        if (puzzleManager != null)
            puzzleManager.TryActivate(this);
    }

    //------------------------------------------------

    public void ActivateCorrect()
    {
        if (spotLight == null)
            return;

        spotLight.enabled = true;
        spotLight.color = correctColor;
    }

    public void ActivateWrong()
    {
        if (spotLight == null)
            return;

        spotLight.enabled = true;
        spotLight.color = wrongColor;
    }

    public void TurnOff()
    {
        if (spotLight == null)
            return;

        spotLight.enabled = false;
    }
}