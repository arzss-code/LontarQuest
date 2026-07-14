using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomManager : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    [Tooltip("Pintu tempat Saka masuk (akan memblokir jalan saat Saka masuk)")]
    public GameObject entryDoor;
    
    [Tooltip("Pintu-pintu hadiah (BoonDoor) yang akan MUNCUL saat ruangan bersih")]
    public GameObject[] exitDoors;

    [Tooltip("Pintu fisik (tembok/jeruji) yang memblokir jalan ke ruangan selanjutnya. Akan TERBUKA saat ruangan bersih.")]
    public GameObject[] nextRoomBlockades;

    [Header("Petunjuk Arah Visual")]
    [Tooltip("Prefab Partikel Kunang-kunang untuk menunjuk jalan keluar saat ruangan dibersihkan")]
    public GameObject guideParticlePrefab;

    [Header("Pengaturan Musuh")]
    [Tooltip("Daftar musuh di dalam ruangan ini")]
    public List<GameObject> enemiesInRoom = new List<GameObject>();

    [Header("Status Ruangan (Read Only)")]
    [SerializeField] private bool isRoomActive = false;
    [SerializeField] private bool isRoomCleared = false;

    [Header("Quest Updates")]
    [Tooltip("Misi yang muncul saat Saka masuk ke ruangan ini")]
    public string questOnEnter;
    
    [Tooltip("Misi yang muncul setelah ruangan ini berhasil dibersihkan")]
    public string questOnClear;

    private void Start()
    {
        // Pastikan BoxCollider diset ke Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // 1. Sembunyikan pintu keluar (Boon Doors) di awal
        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(false);
        }

        // 1b. Pastikan blokade fisik jalan ke depan TERTUTUP (Menghalangi)
        if (nextRoomBlockades != null && nextRoomBlockades.Length > 0)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("⚠️ ROOM MANAGER: Kolom 'Next Room Blockades' kosong! Pintu keluar tidak akan ditutup di awal.");
        }

        // (Dihapus: Membuka entryDoor di Start). Pintu masuk biarkan diatur oleh RoomManager sebelumnya,
        // agar tidak 'menabrak' (clashing) dengan perintah SetActive(true) dari ruangan sebelumnya.

        // 3. Matikan musuh sementara agar mereka 'tidur' sebelum Saka masuk
        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null) enemy.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Jika Saka menginjak lantai ruangan, dan ruangan belum aktif
        if (other.CompareTag("Player") && !isRoomActive && !isRoomCleared)
        {
            if (IsPlayerFullyInside(other))
            {
                ActivateRoom();
            }
        }
    }

    private bool IsPlayerFullyInside(Collider2D playerCol)
    {
        Collider2D roomCol = GetComponent<Collider2D>();
        if (roomCol == null || playerCol == null) return false;

        Bounds roomBounds = roomCol.bounds;
        Bounds playerBounds = playerCol.bounds;

        // Cek apakah seluruh area bounds player berada di dalam bounds ruangan
        return playerBounds.min.x >= roomBounds.min.x &&
               playerBounds.max.x <= roomBounds.max.x &&
               playerBounds.min.y >= roomBounds.min.y &&
               playerBounds.max.y <= roomBounds.max.y;
    }

    private void ActivateRoom()
    {
        isRoomActive = true;
        Debug.Log("Saka memasuki ruangan! Pintu terkunci!");

        // 1. Kunci pintu masuk (munculkan jeruji batu)
        if (entryDoor != null) entryDoor.SetActive(true);

        // 2. Bangunkan semua musuh
        if (enemiesInRoom == null || enemiesInRoom.Count == 0)
        {
            Debug.LogWarning("⚠️ ROOM MANAGER: Kolom 'Enemies In Room' KOSONG! Karena tidak ada musuh, ruangan akan LANGSUNG terbuka.");
        }
        else
        {
            foreach (GameObject enemy in enemiesInRoom)
            {
                if (enemy != null) enemy.SetActive(true);
            }
        }

        // 3. Perbarui Quest di layar
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(questOnEnter))
        {
            QuestManager.Instance.SetObjective(questOnEnter);
        }
    }

    private void Update()
    {
        // Hanya pantau kematian musuh jika ruangan sedang aktif pertarungan
        if (isRoomActive && !isRoomCleared)
        {
            // Bersihkan list dari musuh yang sudah mati (Game Object-nya hancur / null)
            enemiesInRoom.RemoveAll(enemy => enemy == null);

            // Jika musuh habis
            if (enemiesInRoom.Count == 0)
            {
                RoomCleared();
            }
        }
    }

    private void RoomCleared()
    {
        isRoomCleared = true;
        isRoomActive = false;
        Debug.Log("Ruangan Bersih! Pintu Hadiah Terbuka!");

        // 1. Buka kembali pintu masuk agar Saka bisa mundur (opsional)
        if (entryDoor != null) entryDoor.SetActive(false);

        // 2. Buka pintu keluar (Boon Doors) agar Saka bisa memilih hadiah
        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(true);
        }

        // 3. Buka blokade fisik (tembok) agar Saka bisa lanjut jalan
        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(false);
            }
        }

        // 4. Munculkan Kunang-Kunang Petunjuk Jalan
        if (guideParticlePrefab != null)
        {
            SpawnGuideParticle();
        }

        // 5. Perbarui Quest di layar
        if (QuestManager.Instance != null && !string.IsNullOrEmpty(questOnClear))
        {
            QuestManager.Instance.SetObjective(questOnClear);
        }
    }

    private void SpawnGuideParticle()
    {
        // Cari posisi pemain saat ini
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform targetDoor = null;

        // Prioritas pertama: Tunjuk ke arah Pintu Hadiah (BoonDoor)
        if (exitDoors != null && exitDoors.Length > 0 && exitDoors[0] != null)
        {
            targetDoor = exitDoors[0].transform;
        }
        // Prioritas kedua: Jika tidak ada hadiah, tunjuk ke arah Tembok Lorong yang baru terbuka
        else if (nextRoomBlockades != null && nextRoomBlockades.Length > 0 && nextRoomBlockades[0] != null)
        {
            targetDoor = nextRoomBlockades[0].transform;
        }

        // Jika ada target yang bisa dituju
        if (targetDoor != null)
        {
            // Buat kunang-kunang di posisi Saka
            GameObject guide = Instantiate(guideParticlePrefab, player.transform.position, Quaternion.identity);
            GuideParticle script = guide.GetComponent<GuideParticle>();
            if (script != null)
            {
                // Perintahkan terbang ke target
                script.Init(targetDoor);
            }
        }
    }
}
