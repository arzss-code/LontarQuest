using System.Collections;
using UnityEngine;

/// <summary>
/// Mengelola urutan intro saat Scene Stage 2 dimulai.
/// Pola mengikuti Stage1IntroStarter: Fade In → Auto-walk (opsional) → Dialog Monolog Saka → Set Quest Awal.
/// </summary>
public class Stage2IntroStarter : MonoBehaviour
{
    [Header("Dialog Settings")]
    [Tooltip("Komponen IntroDialogue yang mengatur UI dialog intro di Stage 2")]
    [SerializeField] private IntroDialogue introDialogue;

    [Header("Cutscene Movement")]
    [Tooltip("Pemain akan dipindahkan ke titik ini di awal (opsional — kosongkan jika tidak perlu teleport)")]
    [SerializeField] private Transform portalSpawnPoint;

    [Tooltip("Titik tujuan pemain berjalan sebelum dialog dimulai (opsional — kosongkan jika tidak perlu auto-walk)")]
    [SerializeField] private Transform walkDestination;

    [Tooltip("Kecepatan jalan karakter saat cutscene auto-walk")]
    [SerializeField] private float walkSpeed = 3f;

    [Header("Cutscene Settings")]
    [Tooltip("Jeda waktu sebelum cutscene dimulai (detik)")]
    [SerializeField] private float startDelay = 0.5f;

    [Header("Quest Awal")]
    [Tooltip("Teks quest pertama yang muncul setelah dialog intro selesai")]
    [SerializeField] private string initialQuest = "Jelajahi Perpustakaan Melayang";

    private PlayerController playerController;
    private Animator playerAnimator;

    private void Start()
    {
        // Cari Player di scene dengan mencari component PlayerController
        playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            GameObject playerObj = playerController.gameObject;

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

        if (introDialogue != null)
        {
            introDialogue.OnDialogueFinished += OnIntroFinished;
            StartCoroutine(IntroSequence());
        }
        else
        {
            Debug.LogWarning("[Stage2IntroStarter] IntroDialogue belum di-assign di Inspector!");
            // Tanpa dialog, langsung set quest awal
            SetInitialQuest();
        }
    }

    private void OnDestroy()
    {
        // Lepas event binding untuk menghindari memory leak
        if (introDialogue != null)
        {
            introDialogue.OnDialogueFinished -= OnIntroFinished;
        }
    }

    private void OnIntroFinished()
    {
        if (introDialogue != null)
        {
            introDialogue.OnDialogueFinished -= OnIntroFinished;
        }

        // Set quest awal setelah dialog selesai
        SetInitialQuest();
    }

    private void SetInitialQuest()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective(initialQuest);
        }
    }

    private IEnumerator IntroSequence()
    {
        Debug.Log("[Stage2IntroStarter] IntroSequence dimulai.");
        // 1. Tunggu efek layar dari hitam menjadi terang
        if (FadeManager.Instance != null)
        {
            Debug.Log("[Stage2IntroStarter] Menunggu FadeIn...");
            yield return StartCoroutine(FadeManager.Instance.FadeIn());
            Debug.Log("[Stage2IntroStarter] FadeIn Selesai.");
        }
        else
        {
            Debug.Log("[Stage2IntroStarter] FadeManager.Instance tidak ditemukan.");
        }

        // 2. Jeda tambahan sebelum cutscene mulai
        yield return new WaitForSeconds(startDelay);

        Debug.Log($"[Stage2IntroStarter] Mencoba auto-walk. PlayerController: {(playerController != null ? "Ada" : "NULL")}, WalkDestination: {(walkDestination != null ? "Ada" : "NULL")}");

        // 3. Auto-walk jika walkDestination diatur
        if (playerController != null && walkDestination != null)
        {
            // Kunci kontrol pemain
            playerController.SetCanMove(false);
            playerController.isAutoWalking = true;

            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            Debug.Log($"[Stage2IntroStarter] Rigidbody2D: {(rb != null ? "Ada" : "NULL")}");

            if (rb != null)
            {
                // Teleport ke spawn point jika diatur
                if (portalSpawnPoint != null)
                {
                    Vector2 newSpawnPos = portalSpawnPoint.position;
                    Debug.Log($"[Stage2IntroStarter] Teleport player dari {rb.position} ke {newSpawnPos}");
                    rb.position = newSpawnPos;
                    playerController.transform.position = portalSpawnPoint.position;
                    Physics2D.SyncTransforms(); // Sinkronkan transform agar posisi baru terbaca oleh fisika
                }

                Vector2 startPos = rb.position;
                Vector2 endPos = walkDestination.position;
                Vector2 direction = (endPos - startPos).normalized;
                Debug.Log($"[Stage2IntroStarter] Start Pos: {startPos}, End Pos: {endPos}, Arah: {direction}");

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

                int loopCount = 0;
                // Gerakkan player menggunakan sistem Fisika
                while (Vector2.Distance(rb.position, endPos) > 0.05f)
                {
                    loopCount++;
                    float speed = walkSpeed > 0 ? walkSpeed : 3f;
                    Vector2 nextPos = Vector2.MoveTowards(rb.position, endPos, speed * Time.fixedDeltaTime);
                    rb.MovePosition(nextPos);

                    if (loopCount % 30 == 0 || loopCount < 5)
                    {
                        Debug.Log($"[Stage2IntroStarter] Loop #{loopCount}: Posisi sekarang: {rb.position}, Target: {endPos}, Sisa Jarak: {Vector2.Distance(rb.position, endPos)}");
                    }

                    yield return new WaitForFixedUpdate();

                    // Pengaman jika macet/stuck di dinding agar loop tidak selamanya
                    if (loopCount > 300)
                    {
                        Debug.LogWarning("[Stage2IntroStarter] Auto-walk memakan waktu terlalu lama (> 300 frame fisika). Memaksa selesai agar game tidak hang.");
                        break;
                    }
                }

                Debug.Log($"[Stage2IntroStarter] Auto-walk selesai setelah {loopCount} loop. Posisi akhir: {rb.position}");

                // Paskan posisi akhir
                rb.position = endPos;
                playerController.transform.position = new Vector3(endPos.x, endPos.y, playerController.transform.position.z);
                Physics2D.SyncTransforms();
            }

            // Hentikan animasi
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("Speed", 0f);
            }

            // Pastikan player terkunci selama dialog
            playerController.SetCanMove(false);
            playerController.isAutoWalking = false;
        }
        else if (playerController != null)
        {
            // Tidak ada auto-walk, tapi tetap kunci player selama dialog
            playerController.SetCanMove(false);
        }

        // 4. Mulai dialog intro
        Debug.Log("[Stage2IntroStarter] Memulai dialog monolog intro...");
        introDialogue.StartDialogue();
    }
}
