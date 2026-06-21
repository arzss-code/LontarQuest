using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public GameObject panel;

    public void ClosePuzzle()
    {
        panel.SetActive(false);

        Time.timeScale = 1f;
    }
}