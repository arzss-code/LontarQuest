using UnityEngine;

public class PrasastiAwal : MonoBehaviour
{
    bool playerNear;

    void Update()
    {
        if(playerNear &&
           Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Mulai Puzzle");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            playerNear = false;
    }
}