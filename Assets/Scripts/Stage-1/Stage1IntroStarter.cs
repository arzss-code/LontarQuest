using System.Collections;
using UnityEngine;

public class Stage1IntroStarter : MonoBehaviour
{
    [Header("Dialog Settings")]
    [Tooltip("Masukkan komponen IntroDialogue yang mengatur UI dialog di Stage 1")]
    [SerializeField] private IntroDialogue introDialogue;

    [Header("Cutscene Movement")]
    [Tooltip("Pemain akan dipindahkan ke titik ini di awal (opsional)")]
    [SerializeField] private Transform portalSpawnPoint;
    
    [Tooltip("Titik tujuan pemain berjalan sebelum dialog dimulai")]
    [SerializeField] private Transform walkDestination;
    
    [Tooltip("Kecepatan jalan karakter saat cutscene")]
    [SerializeField] private float walkSpeed = 3f;

    [Header("Cutscene Settings")]
    [Tooltip("Jeda waktu sebelum animasi dimulai")]
    [SerializeField] private float startDelay = 0.5f;

    [Header("Mechanic Tips")]
    [Tooltip("Panel UI untuk menampilkan tips mekanik dasar")]
    [SerializeField] private GameObject mechanicTipsPanel;

    private PlayerController playerController;
    private Animator playerAnimator;
    private bool isShowingTips = false;

    private void Start()
    {
        // Cari Player di scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
            
            // Cari Animator yang memiliki controller (menghindari error jika ada animator kosong)
            Animator[] animators = playerObj.GetComponentsInChildren<Animator>();
            foreach (Animator anim in animators)
            {
                if (anim.runtimeAnimatorController != null)
                {
                    playerAnimator = anim;
                    break;
                }
            }
        }

        if (mechanicTipsPanel != null)
        {
            mechanicTipsPanel.SetActive(false);
        }

        if (introDialogue != null)
        {
            introDialogue.OnDialogueFinished += OnIntroFinished;
            StartCoroutine(IntroSequence());
        }
        else
        {
            Debug.LogWarning("Stage1IntroStarter: IntroDialogue belum di-assign di Inspector!");
        }
    }

    private void OnIntroFinished()
    {
        if (introDialogue != null)
        {
            introDialogue.OnDialogueFinished -= OnIntroFinished;
        }

        if (mechanicTipsPanel != null)
        {
            StartCoroutine(ShowTipsRoutine());
        }
    }

    private IEnumerator ShowTipsRoutine()
    {
        isShowingTips = true;
        mechanicTipsPanel.SetActive(true);

        if (playerController != null)
            playerController.SetCanMove(false);

        // Tunggu satu frame agar spasi dari menutup dialog tidak langsung menutup tips
        yield return null;

        // Tunggu sampai pemain menekan Spasi atau Klik Kiri
        while (!Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        mechanicTipsPanel.SetActive(false);
        isShowingTips = false;

        if (playerController != null)
            playerController.SetCanMove(true);
    }

    private IEnumerator IntroSequence()
    {
        // Tunggu efek layar dari hitam menjadi terang
        if (FadeManager.Instance != null)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
        }

        // Jeda tambahan (jika diperlukan) sebelum Saka mulai bergerak
        yield return new WaitForSeconds(startDelay);

        if (playerController != null && walkDestination != null)
        {
            // Kunci kontrol pemain
            playerController.SetCanMove(false);
            
            // Dapatkan Rigidbody2D player karena Player adalah objek fisika (Dynamic)
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Pindahkan player ke titik awal portal jika ada
                if (portalSpawnPoint != null)
                {
                    Vector2 newSpawnPos = portalSpawnPoint.position;
                    rb.position = newSpawnPos;
                    playerController.transform.position = portalSpawnPoint.position;
                }

                Vector2 startPos = rb.position;
                Vector2 endPos = walkDestination.position;
                Vector2 direction = (endPos - startPos).normalized;

                // Set animasi berjalan
                if (playerAnimator != null)
                {
                    playerAnimator.SetFloat("MoveX", direction.x);
                    playerAnimator.SetFloat("MoveY", direction.y);
                    
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        playerAnimator.SetFloat("LastMoveX", Mathf.Sign(direction.x));
                        playerAnimator.SetFloat("LastMoveY", 0);
                    }
                    else
                    {
                        playerAnimator.SetFloat("LastMoveX", 0);
                        playerAnimator.SetFloat("LastMoveY", Mathf.Sign(direction.y));
                    }

                    playerAnimator.SetFloat("Speed", 1f);
                }

                // Gerakkan player menggunakan sistem Fisika (FixedUpdate)
                while (Vector2.Distance(rb.position, endPos) > 0.05f)
                {
                    float speed = walkSpeed > 0 ? walkSpeed : 3f;
                    Vector2 nextPos = Vector2.MoveTowards(rb.position, endPos, speed * Time.fixedDeltaTime);
                    rb.MovePosition(nextPos);
                    
                    yield return new WaitForFixedUpdate();
                }

                // Paskan posisi
                rb.position = endPos;
                playerController.transform.position = new Vector3(endPos.x, endPos.y, playerController.transform.position.z);
            }

            // Hentikan animasi
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Speed", 0f);
            }

            // Pastikan SetCanMove(false) agar tidak bisa digerakkan saat dialog
            playerController.SetCanMove(false);
        }

        Debug.Log("STAGE 1 CUTSCENE MULAI");
        introDialogue.StartDialogue();

        // Update Quest pertama kali
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective("Jelajahi Reruntuhan Candi");
        }
    }
}
