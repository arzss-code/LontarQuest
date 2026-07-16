# Laporan Audit & Dokumentasi Pembaruan Sistem Game LontarQuest

Dokumen ini mencatat hasil audit komprehensif serta status perbaikan bug-bug kritis, celah eksploitasi, dan optimasi arsitektural yang telah diimplementasikan dalam proyek game **LontarQuest** ("Dimensi Lontar").

---

## 1. Daftar Perbaikan Sistem & Bug Kritis (Status: FIXED)

Pembaruan besar pada kode telah berhasil diintegrasikan melalui repositori (khususnya pada commit `1e014a413ccc3af6d24d5f9f4bfd43fb6127f6f8`). Berikut rincian perubahan yang telah dilakukan untuk masing-masing berkas:

### A. Penanganan Pembekuan Game (Pause & Scene Transition)
* **Temuan**: Saat UI Jurnal dibuka (menekan **Tab**), game di-pause menggunakan `Time.timeScale = 0f`. Jika terjadi perpindahan scene (mati/respawn) saat jurnal terbuka, game membeku selamanya di scene baru.
* **Perbaikan**: Di [JournalManager.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Systems/JournalManager.cs), ditambahkan callback `OnSceneLoaded`:
  ```csharp
  private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
  {
      if (journalUIPanel != null)
      {
          journalUIPanel.SetActive(false);
      }
      Time.timeScale = 1f; // Paksa kembalikan waktu normal saat ganti scene
  }
  ```

### B. Perbaikan Logika State Combat AI Musuh (Stage 3)
* **Temuan**: Di status patroli, musuh memanggil `StartCoroutine(WaitRoutine())` tanpa menyimpan referensinya. Saat beralih ke mode `Combat`, coroutine lama tidak bisa dihentikan karena variabel referensi `waitRoutine` selalu `null`. Akibatnya, AI musuh sering ter-reset secara paksa kembali ke status patroli di tengah perkelahian.
* **Perbaikan**: Di [EnemyMovement.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyMovement.cs), coroutine direkam secara benar dan dibersihkan saat selesai:
  ```csharp
  // Saat memanggil coroutine patroli/wait:
  waitRoutine = StartCoroutine(WaitRoutine());

  // Di akhir coroutine WaitRoutine():
  currentState = MovementState.Patrol;
  waitRoutine = null;
  ```

### C. Keamanan Pengecekan Referensi Null (Null Safety)
* **Temuan 1**: Di [EnemyStats.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyStats.cs), pemanggilan `healthBar.Initialize()` dan `healthBar.SetHealth()` memicu crash jika komponen bar darah tidak dipasang di Inspector.
* **Temuan 2**: Di [JournalTempleManager.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/TempleScene/JournalTempleManager.cs), pemanggilan `PuzzleManager.Instance.IsPuzzleOpen` memicu error jika instance puzzle manager kosong di scene.
* **Perbaikan**: Menambahkan null-checks yang ketat:
  - **EnemyStats**: `if (healthBar != null) { healthBar.Initialize(maxHP); }`
  - **JournalTempleManager**: `if(PuzzleManager.Instance != null && PuzzleManager.Instance.IsPuzzleOpen)`

### D. Perbaikan Eksploitasi Tembakan Panah (Arrow Wall Penetration)
* **Temuan**: Proyektil panah Saka tidak hancur saat menabrak dinding map, sehingga pemain bisa melakukan *safespot sniping* (menembak musuh dari balik dinding).
* **Perbaikan**: Di [ArrowProjectile.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Start%20Area/ArrowProjectile.cs), ditambahkan deteksi tumbukan dengan layer tembok:
  ```csharp
  if (other.gameObject.layer == LayerMask.NameToLayer("Wall") || other.CompareTag("Wall") || other.gameObject.name.Contains("Wall"))
  {
      Destroy(gameObject);
  }
  ```

### E. Perbaikan Inkonsistensi Visual Warna & Durasi Tick (Burn Effect)
* **Temuan 1**: Status terbakar yang bertumpuk (*stacking*) mengunci warna sprite musuh menjadi merah permanen karena skrip menyimpan warna dasar (`sr.color`) yang sedang berubah merah dari skrip sebelumnya.
* **Temuan 2**: Loop jeda coroutine memakan waktu rill 1.1 detik (1.0s + 0.1s), memperlambat DPS sebesar 10%.
* **Perbaikan**: Di [BurnEffect.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Systems/BurnEffect.cs), warna asli dicache sekali di `Awake` dan dipulihkan sebelum coroutine baru atau saat objek dihancurkan. Durasi tick disesuaikan tepat 1.0 detik (`0.15s` flash merah + `0.85s` jeda sisa).

### F. Karakter Berlari di Tempat Saat Cutscene (Animator Lock)
* **Temuan**: Logika `canMove == false` diletakkan di paling atas `Update` sehingga kode reset pergerakan di bawahnya menjadi tidak pernah tercapai (*unreachable code*). Saka tampak terus berlari di tempat saat gerakan dimatikan eksternal.
* **Perbaikan**: Di [PlayerController.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Start%20Area/PlayerController.cs), logika reset pergerakan dipindahkan ke baris teratas blok `canMove` check:
  ```csharp
  if (!canMove)
  {
      movement = Vector2.zero;
      if (animator != null) animator.SetFloat("Speed", 0);
      return;
  }
  ```

### G. Akses Balik Stage Sebelumnya di Portal SafeHub
* **Temuan**: Syarat portal terbuka menggunakan perbandingan mutlak `stageNumber == lastStage`, mengunci kembali stage sebelumnya saat progres bertambah.
* **Perbaikan**: Di [SafeHubPortal.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/SafeHub/SafeHubPortal.cs), diubah menjadi `stageNumber <= lastStage` agar pemain bebas memainkan ulang stage lama.

### H. Pembersihan Kode Mati (Dead Code Cleanup)
* **Perbaikan**: Berkas `Stage3PlayerController.cs` dan berkas metadatanya telah **dihapus secara permanen** dari proyek karena semua scene kini telah menggunakan controller utama [PlayerController.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Start%20Area/PlayerController.cs).

---

## 2. Sinkronisasi Estetika & Catatan Desain (Design Sync)

* **Sistem Slot Boon**: Berdasarkan audit kode [BoonData.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/ScriptableObjects/BoonData.cs), rencana 4 slot terpisah (*Melee*, *Bow*, *Dash*, *Passive*) disederhanakan menjadi **2 slot pasif bebas** dengan mekanisme overwrite pada indeks 0 saat slot penuh.
* **Pause UI**: Tampilan pause saat ini masih dirancang prosedural via C# di [PauseManager.cs](file:///home/fadhika/Documents/GitHub/LontarQuest/Assets/Scripts/Systems/PauseManager.cs). Rekomendasi perbaikan berikutnya adalah memigrasikannya ke sistem Prefab UI Editor Unity.
