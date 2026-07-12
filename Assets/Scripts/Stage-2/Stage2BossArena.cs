using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Stage2BossArena : MonoBehaviour
{
    [Header("Door Configuration")]
    [Tooltip("Pintu masuk yang akan memblokir jalan saat player masuk")]
    [SerializeField] private GameObject entryDoor;
    [Tooltip("Dinding/jeruji yang memblokir jalan ke ruangan berikutnya. Terbuka saat boss mati.")]
    [SerializeField] private GameObject[] nextRoomBlockades;

    [Header("Visual Direction Guide")]
    [SerializeField] private GameObject guideParticlePrefab;

    [Header("UI Setup")]
    [SerializeField] private GameObject bossHealthUI;
    [SerializeField] private Slider bossHealthSlider;

    [Header("Boss Setup")]
    [SerializeField] private GameObject bossObject;
    [SerializeField] private MonoBehaviour bossAI; // Script AI (Stage2EnemyMovement)
    [SerializeField] private float postBossDelay = 1.5f;

    [Header("Lontar Reward (Drop)")]
    [SerializeField] private GameObject lontarRewardPrefab;
    [SerializeField] private Transform lontarSpawnPoint;

    [Header("Quest Configuration")]
    [SerializeField] private string questOnBoss = "Kalahkan Yaksa";
    [SerializeField] private string questOnClear = "Lanjutkan ke Ruang Inti";

    private Stage2EnemyStats bossStats;
    private bool isArenaLocked = false;

    private void Start()
    {
        // Pastikan BoxCollider diset ke Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // FASE AWAL: Pintu keluar tertutup, AI bos tertidur, UI darah mati
        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(true);
            }
        }

        if (bossObject != null)
        {
            bossStats = bossObject.GetComponent<Stage2EnemyStats>();
            // Nonaktifkan GameObject bos di awal agar tidak bergerak sebelum trigger terpicu
            bossObject.SetActive(false);
        }

        if (bossAI != null)
        {
            bossAI.enabled = false;
        }

        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isArenaLocked)
        {
            if (bossObject != null && bossStats != null)
            {
                // Update nilai HP slider secara smooth
                bossHealthSlider.value = Mathf.MoveTowards(
                    bossHealthSlider.value, 
                    bossStats.CurrentHP, 
                    Time.deltaTime * bossHealthSlider.maxValue * 2f
                );
            }
            else
            {
                // Boss mati (GameObject hancur)
                isArenaLocked = false;
                StartCoroutine(UnlockArenaSequence());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isArenaLocked)
        {
            if (bossObject != null)
            {
                LockArena();
            }
        }
    }

    private void LockArena()
    {
        isArenaLocked = true;

        // Tutup pintu masuk
        if (entryDoor != null) entryDoor.SetActive(true);

        // Bangunkan bos dan AI-nya
        if (bossObject != null)
        {
            bossObject.SetActive(true);
        }
        if (bossAI != null)
        {
            bossAI.enabled = true;
        }

        // Tampilkan UI Darah Bos
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(true);
            if (bossStats != null && bossHealthSlider != null)
            {
                bossHealthSlider.maxValue = bossStats.MaxHP;
                bossHealthSlider.value = bossStats.MaxHP;
            }
        }

        // Jalankan Quest
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective(questOnBoss);
        }

        Debug.Log("[Boss Arena] Arena terkunci! Pertarungan Yaksa dimulai.");
    }

    private IEnumerator UnlockArenaSequence()
    {
        // Sembunyikan UI darah bos
        if (bossHealthUI != null) bossHealthUI.SetActive(false);

        // Selesaikan objektif bertarung
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CompleteCurrentObjective();
        }

        // Tunggu boss mati berkeping-keping
        yield return new WaitForSeconds(postBossDelay);

        // Spawn Lontar drop
        if (lontarRewardPrefab != null)
        {
            Vector3 spawnPos = lontarSpawnPoint != null ? lontarSpawnPoint.position : transform.position;
            Instantiate(lontarRewardPrefab, spawnPos, Quaternion.identity);

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SetObjective("Ambil Lontar Kuno yang Terjatuh");
            }
        }
        else
        {
            // Jika tidak ada drop, langsung set quest berikutnya
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SetObjective(questOnClear);
            }
        }

        // Buka kembali semua pintu
        if (entryDoor != null) entryDoor.SetActive(false);

        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(false);
            }
        }

        // Spawn visual kunang-kunang untuk menunjukkan jalan
        if (guideParticlePrefab != null)
        {
            SpawnGuideParticle();
        }

        Debug.Log("[Boss Arena] Bos kalah! Arena terbuka.");

        // Nonaktifkan script ini agar tidak lagi mengupdate loop
        this.enabled = false;
    }

    private void SpawnGuideParticle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform targetDoor = null;

        // Arahkan ke rute terbuka berikutnya
        if (nextRoomBlockades != null && nextRoomBlockades.Length > 0 && nextRoomBlockades[0] != null)
        {
            targetDoor = nextRoomBlockades[0].transform;
        }

        if (targetDoor != null)
        {
            GameObject guide = Instantiate(guideParticlePrefab, player.transform.position, Quaternion.identity);
            GuideParticle script = guide.GetComponent<GuideParticle>();
            if (script != null)
            {
                script.Init(targetDoor);
            }
        }
    }
}
