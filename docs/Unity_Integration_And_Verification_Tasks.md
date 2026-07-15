# Panduan Integrasi Unity & Verifikasi Sistem Game LontarQuest

Berdasarkan hasil audit sistem file proyek (`StartMenu.unity`, `TagManager.asset`, dan `EditorBuildSettings.asset`), **seluruh konfigurasi Inspector dan Editor Build Settings telah terpasang secara otomatis**. Anda tidak perlu melakukan drag-and-drop atau konfigurasi tag/layer dari awal.

Berikut adalah laporan sinkronisasi aset proyek dan petunjuk verifikasi sistem:

---

## 1. Hasil Audit Sinkronisasi Proyek (Project Audit & Synchronization)

* **Build Settings Scene Registry (`EditorBuildSettings.asset`)**: **Telah Terdaftar**. 
  - Scene pembuka `StartMenu.unity` berada pada Indeks 0.
  - Scene `Story-1.unity`, `SafeHub.unity`, dan seluruh scene stage telah terdaftar dan aktif.
* **Tag & Layer (`TagManager.asset`)**: **Telah Terkonfigurasi**.
  - Tag `"Wall"` (indeks 2) dan Layer `"Wall"` (indeks 12) sudah terdaftar dalam proyek. Objek pembatas map di stage game sudah otomatis terhubung dengan layer/tag ini.
* **Inspector Bindings di Scene `StartMenu.unity`**: **Telah Terhubung**.
  - Script `StartMenuManager` sudah terhubung secara benar dengan GameObject `AnyKeyText` dan CanvasGroup `FadePanel`.
  - Prefab `SaveManager` dan `PauseManager` sudah diletakkan di dalam scene pembuka.

---

## 2. Lembar Verifikasi Fungsionalitas Sistem (System Verification Checklist)

Karena semua pengaturan Editor telah selaras, Anda hanya perlu membuka scene **`Assets/Scenes/Tristan/StartMenu.unity`** di Editor Unity, menekan tombol **Play**, dan menjalankan pengujian berikut untuk memverifikasi fungsionalitas sistem:

### A. Uji Fungsionalitas Main Menu & Save/Load
* [ ] **Splash Screen Transition**: Saat game pertama kali dibuka, layar menampilkan teks petunjuk pembuka. Menekan tombol apa saja berhasil menyembunyikan teks tersebut dan memunculkan panel Main Menu secara halus.
* [ ] **Save File Detection**: Jika file simpanan game tidak ada, tombol **Lanjutkan** berwarna abu-abu redup (*grayed out*) dan tidak bisa diklik.
* [ ] **New Game Flow**: Mengklik **Game Baru** berhasil menghapus data save lama, memudarkan layar, dan memuat scene `"Story-1"` untuk memulai cerita dari awal.
* [ ] **Continue Run Flow**: Setelah progres game tersimpan (masuk ke Stage 1/2/3), keluar ke menu utama. Tombol **Lanjutkan** kini harus aktif secara visual. Mengklik **Lanjutkan** berhasil memuat scene `"SafeHub"` secara langsung.
* [ ] **SafeHub Portal Backtrack**: Masuk ke SafeHub setelah mencapai Stage 2 atau 3. Pastikan portal ke stage sebelumnya (mis. Stage 1) tetap terbuka secara visual dan dapat dimasuki kembali.

### B. Uji Navigasi & Pause Menu
* [ ] **Quit Button**: Mengklik tombol **Keluar** pada Main Menu berhasil menutup game (atau menghentikan mode Play di Unity Editor).
* [ ] **Main Menu Return**: Saat berada di dalam game (Play Mode), tekan tombol **ESC** untuk membuka Pause Menu. Klik tombol **Main Menu**. Pastikan game menutup menu pause secara bersih dan memuat scene `"StartMenu"`.
* [ ] **ESC Prevention in StartMenu**: Saat berada di scene Main Menu, tekan tombol **ESC** berkali-kali. Pastikan UI Pause Menu tidak muncul sama sekali.
* [ ] **Journal Pause Stability**: Buka jurnal dengan menekan tombol **Tab** (game akan di-pause). Matikan karakter Saka atau picu transisi scene. Pastikan di scene baru jurnal otomatis tertutup dan game dapat dimainkan dengan lancar (kecepatan waktu `Time.timeScale = 1.0f`).

### C. Uji Fitur Gameplay & Combat
* [ ] **Wall Collision Arrow**: Tembakkan panah Saka ke arah dinding/tembok pembatas. Pastikan objek panah hancur bertabrakan seketika dan tidak terbang menembus tembok.
* [ ] **Saka Animator Lock**: Buka jurnal, buka teka-teki altar, atau picu dialog intro. Pastikan pergerakan Saka terkunci dan animasinya langsung diam di tempat (tidak tampak berlari di tempat).
* [ ] **Burn Effect Stacking**: Serang musuh menggunakan elemen api secara berturut-turut. Pastikan warna musuh berkedip merah dan kembali ke warna awal yang asli secara sempurna setelah efek terbakar 4 detik selesai (tidak menjadi merah permanen).
* [ ] **Enemy State Combat Stage 3**: Dekati musuh di Stage 3 hingga terdeteksi. Serang musuh tersebut. Pastikan kecerdasan buatan musuh terus menyerang Saka tanpa kembali ke state patroli (berputar-putar/diam) di tengah-tengah combat.
* [ ] **Null Safety Test (Enemy HP Bar)**: Hapus bar darah (`healthBar`) dari salah satu musuh di editor Inspector. Serang musuh tersebut. Pastikan game tidak mengalami crash (`NullReferenceException`) dan damage popup tetap muncul di layar.
