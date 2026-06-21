using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoonDoor : MonoBehaviour
{
    [Header("Tipe Hadiah Lorong Ini")]
    public BoonType doorRewardType;

    [Header("Lorong Lain (Opsional)")]
    [Tooltip("Pintu lorong lain yang harus ditutup/diblokir jika pemain memilih lorong ini")]
    public GameObject[] otherDoorsToLock;

    [Header("Blokade Ruangan Berikutnya")]
    [Tooltip("Jeruji/Tembok fisik yang menghalangi jalan ke arena bawah. Akan TERBUKA (hilang) setelah Saka menyentuh Boon ini.")]
    public GameObject[] nextRoomBlockades;

    private bool isUsed = false;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed) return;

        if (other.CompareTag("Player"))
        {
            isUsed = true;
            Debug.Log($"Saka Memilih Jalur Boon: {doorRewardType}");

            // 1. Kunci pintu lorong alternatif lain (jika ada)
            LockOtherDoors();

            // 2. BUKA blokade menuju ruangan selanjutnya (Arena Bawah)
            UnlockNextRooms();

            // 3. Tampilkan UI Pemilihan Boon (3 Pilihan Boost)
            if (BoonUIManager.Instance != null)
            {
                BoonUIManager.Instance.ShowBoonSelection();
            }

            // 4. Hilangkan objek pintu Boon ini agar seolah-olah jalan terbuka
            gameObject.SetActive(false);
        }
    }

    private void LockOtherDoors()
    {
        if (otherDoorsToLock == null) return;
        
        foreach (GameObject door in otherDoorsToLock)
        {
            if (door != null)
            {
                // Sembunyikan pintu alternatif agar tidak bisa dipilih lagi
                door.SetActive(false);
            }
        }
    }

    private void UnlockNextRooms()
    {
        if (nextRoomBlockades == null) return;

        // Buka tembok penghalang (dengan cara menyembunyikannya) agar jalan tembus
        foreach (GameObject blockade in nextRoomBlockades)
        {
            if (blockade != null)
            {
                blockade.SetActive(false);
            }
        }
    }
}
