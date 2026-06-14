using UnityEngine;

public class TestReveal : MonoBehaviour
{
    [SerializeField] private JeepReveal jeepReveal;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            jeepReveal.Reveal();
        }
    }
}