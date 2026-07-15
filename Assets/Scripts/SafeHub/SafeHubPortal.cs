using UnityEngine;
using UnityEngine.SceneManagement;

// Portal di SafeHub. Terbuka HANYA jika stage-nya = stage terakhir yang
// dilalui player (SaveManager.GetLastStageReached()). Kalau terkunci,
// menampilkan sprite gembok dan tidak bisa dimasuki.
[RequireComponent(typeof(SpriteRenderer))]
public class SafeHubPortal : MonoBehaviour
{
    [Header("Stage")]
    [Tooltip("Nomor stage yang dituju portal ini: 1, 2, atau 3")]
    [SerializeField] private int stageNumber = 1;

    [Tooltip("Nama scene yang dibuka saat portal ditekan")]
    [SerializeField] private string targetSceneName = "Stage1";

    [Header("Visual")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    [Tooltip("Object berisi Light 2D (dan efek lain) yang menyala hanya saat portal terbuka")]
    [SerializeField] private GameObject portalLight;

    [Header("UI")]
    [Tooltip("Prompt 'Tekan E' yang muncul saat player dekat (opsional)")]
    [SerializeField] private GameObject ePrompt;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked;
    private bool playerNearby;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (ePrompt != null)
            ePrompt.SetActive(false);

        // Pastikan SaveManager ada supaya nilai progress terbaca dari file save
        // (mis. saat scene SafeHub dites langsung tanpa lewat StartMenu).
        if (SaveManager.Instance == null)
        {
            new GameObject("SaveManager (Auto)").AddComponent<SaveManager>();
        }

        int lastStage = SaveManager.Instance.GetLastStageReached();

        isUnlocked = (stageNumber <= lastStage);

        // Ganti sprite sesuai status kunci
        spriteRenderer.sprite = isUnlocked ? unlockedSprite : lockedSprite;

        // Light hanya menyala saat portal terbuka
        if (portalLight != null)
            portalLight.SetActive(isUnlocked);
    }

    private void Update()
    {
        if (!isUnlocked || !playerNearby)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            EnterPortal();
        }
    }

    private void EnterPortal()
    {
        Debug.Log($"[SafeHubPortal] Masuk ke stage {stageNumber}: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        // Prompt hanya muncul kalau portal terbuka
        if (isUnlocked && ePrompt != null)
            ePrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (ePrompt != null)
            ePrompt.SetActive(false);
    }
}
