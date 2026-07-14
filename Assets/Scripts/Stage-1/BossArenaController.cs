using UnityEngine;

public class BossArenaController : MonoBehaviour
{
    [Header("Pengaturan Pintu")]
    [Tooltip("Pintu tempat Saka masuk (akan memblokir jalan saat Saka masuk)")]
    public GameObject entryDoor;

    [Tooltip("Pintu fisik (tembok/jeruji) yang memblokir jalan ke ruangan selanjutnya. Akan TERBUKA saat Boss mati.")]
    public GameObject[] nextRoomBlockades;

    [Header("Petunjuk Arah Visual")]
    [Tooltip("Prefab Partikel Kunang-kunang untuk menunjuk jalan keluar saat Boss mati (Opsional)")]
    public GameObject guideParticlePrefab;

    [Header("UI Setup")]
    [Tooltip("UI Bar Darah Bos (Canvas / Panel) yang akan muncul saat arena terkunci")]
    public GameObject bossHealthUI;

    [Header("Boss Setup")]
    [Tooltip("Objek bos fisik yang akan dipantau kematiannya (Kala, Gana, dll)")]
    [SerializeField] private GameObject bossObject;
    
    [Tooltip("Script AI Bos (contoh: KalaAI / GanaAI) yang akan dibangunkan saat Saka masuk arena")]
    [SerializeField] private MonoBehaviour bossAI;

    [Header("Post-Boss Cutscene")]
    [Tooltip("Dialog yang akan muncul setelah Boss mati")]
    [SerializeField] private IntroDialogue postBossDialogue;
    
    [Tooltip("Waktu tunggu sebelum dialog muncul (agar player melihat bos hancur)")]
    [SerializeField] private float postBossDelay = 1.5f;

    [Header("Lontar Reward (Drop)")]
    [Tooltip("Prefab LontarPickup yang akan dijatuhkan bos saat mati")]
    public GameObject lontarRewardPrefab;
    
    [Tooltip("Titik kemunculan Lontar (opsional, jika kosong akan muncul di lokasi Trigger Boss)")]
    public Transform lontarSpawnPoint;

    private bool isArenaLocked = false;
    private Vector3 lastBossPosition;

    void Start()
    {
        // 1. Pastikan BoxCollider diset ke Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        // 2. FASE TIDUR: Dinding belakang tertutup, bos tertidur.
        // (entryDoor tidak di-SetActive(false) di sini agar tidak konflik dengan ruangan sebelumnya)
        
        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(true);
            }
        }

        if (bossAI != null)
        {
            bossAI.enabled = false;
        }

        // 3. Sembunyikan UI Darah Bos di awal
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(false);
        }
    }

    void Update()
    {
        // 3. FASE KEMENANGAN: Jika arena sedang terkunci, pantau terus objek Bos
        if (isArenaLocked)
        {
            if (bossObject != null)
            {
                lastBossPosition = bossObject.transform.position;
            }
            else // Jika objek bos dihancurkan (Destroy)
            {
                isArenaLocked = false; // Set false langsung agar coroutine tidak terpanggil dobel
                StartCoroutine(UnlockArenaSequence());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 2. FASE LOCK-IN: Jika Saka memasuki arena dan arena belum terkunci
        if (other.CompareTag("Player") && !isArenaLocked)
        {
            // Jangan mengunci arena jika bos ternyata sudah mati sebelumnya
            if (bossObject != null)
            {
                LockArena();
            }
        }
    }

    private void LockArena()
    {
        isArenaLocked = true;
        
        // Kunci pintu masuk
        if (entryDoor != null) entryDoor.SetActive(true);
        
        // Munculkan UI Darah Bos
        if (bossHealthUI != null) bossHealthUI.SetActive(true);
        
        if (bossAI != null)
        {
            bossAI.enabled = true; // Bangunkan bos
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective("Kalahkan Kepala Kala");
        }
        
        Debug.Log("Arena Terkunci! Pertarungan Bos Dimulai!");
    }

    private System.Collections.IEnumerator UnlockArenaSequence()
    {
        // Hilangkan UI Darah Bos secepatnya
        if (bossHealthUI != null) bossHealthUI.SetActive(false);

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.CompleteCurrentObjective();
        }

        // Beri jeda agar pemain melihat animasi bos mati / hancur
        yield return new WaitForSeconds(postBossDelay);

        // Jalankan dialog jika di-assign
        if (postBossDialogue != null)
        {
            postBossDialogue.StartDialogue();
            
            // Tunggu hingga dialog selesai dimainkan
            yield return new WaitUntil(() => !postBossDialogue.IsPlaying);
        }

        // Spawn Lontar Item
        if (lontarRewardPrefab != null)
        {
            Vector3 spawnPos = lontarSpawnPoint != null ? lontarSpawnPoint.position : lastBossPosition;
            Instantiate(lontarRewardPrefab, spawnPos, Quaternion.identity);

            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.SetObjective("Ambil Lontar Kuno yang Terjatuh");
            }
        }

        // Buka pintu masuk (opsional agar bisa mundur)
        if (entryDoor != null) entryDoor.SetActive(false);

        // Buka blokade ke ruangan selanjutnya / portal
        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(false);
            }
        }

        // Munculkan kunang-kunang jika ada
        if (guideParticlePrefab != null)
        {
            SpawnGuideParticle();
        }
        
        Debug.Log("Bos Dikalahkan! Arena Terbuka!");
        
        // Matikan komponen script ini agar tidak mengecek Update lagi, 
        // tapi JANGAN Destroy(gameObject) karena mungkin partikel masih butuh Transform ini.
        this.enabled = false; 
    }

    private void SpawnGuideParticle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform targetDoor = null;

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
