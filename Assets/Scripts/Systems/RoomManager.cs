using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    [Header("Pengaturan Ruangan")]
    [Tooltip("Daftar musuh di dalam ruangan ini")]
    public List<GameObject> enemiesInRoom = new List<GameObject>();

    [Tooltip("Pintu-pintu hadiah (BoonDoor) yang akan terbuka saat ruangan bersih")]
    public GameObject[] exitDoors;

    private bool isRoomCleared = false;

    private void Start()
    {
        // Kunci/Sembunyikan semua pintu keluar saat awal
        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(false);
        }
    }

    private void Update()
    {
        if (isRoomCleared) return;

        // Bersihkan list dari musuh yang sudah mati (null)
        enemiesInRoom.RemoveAll(enemy => enemy == null);

        // Jika musuh habis
        if (enemiesInRoom.Count == 0)
        {
            RoomCleared();
        }
    }

    private void RoomCleared()
    {
        isRoomCleared = true;
        Debug.Log("Ruangan Bersih! Pintu Hadiah Terbuka!");

        // Buka pintu keluar
        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(true);
        }
    }
}
