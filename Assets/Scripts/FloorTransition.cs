using UnityEngine;

public class FloorTransition : MonoBehaviour
{
    [Header("Floor")]
    [SerializeField]
    GameObject currentFloor;

    [SerializeField]
    GameObject targetFloor;

    [Header("Spawn")]
    [SerializeField]
    Transform targetSpawn;

    private void OnTriggerEnter2D(
    Collider2D other)
    {
        if(!other.CompareTag(
            "Player"))
        {
            return;
        }

        // aktif/nonaktif floor
        currentFloor.SetActive(
            false
        );

        targetFloor.SetActive(
            true
        );

        // pindahkan player
        other.transform.position =
        targetSpawn.position;
    }
}