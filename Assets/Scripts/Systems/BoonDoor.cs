using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoonDoor : MonoBehaviour
{
    [Header("Tipe Hadiah Lorong Ini")]
    public BoonType doorRewardType;

    [Header("Lorong Lain (Opsional)")]
    [Tooltip("Pintu lorong lain yang harus ditutup/diblokir jika pemain memilih lorong ini")]
    public GameObject[] otherDoorsToLock;

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
            Debug.Log($"Saka Memilih Jalur: {doorRewardType}");

            // 1. Kunci pintu lorong lain
            LockOtherDoors();

            // 2. Tampilkan UI Pemilihan Boon sesuai tipe pintu
            if (BoonUIManager.Instance != null)
            {
                BoonUIManager.Instance.ShowBoonSelection(doorRewardType);
            }

            // 3. Matikan trigger ini agar tidak dobel
            this.enabled = false;
        }
    }

    private void LockOtherDoors()
    {
        foreach (GameObject door in otherDoorsToLock)
        {
            if (door != null)
            {
                // Disini Anda bisa mengubah Sprite pintu menjadi batu 
                // atau mengaktifkan Collider non-trigger agar jalan tertutup.
                door.SetActive(false); // Sederhananya, hilangkan saja pintunya
            }
        }
    }
}
