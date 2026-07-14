# Panduan Stage 2 — Tahap 4: Scene Integration

Dokumen ini memandu Anda langkah-demi-langkah di Unity Editor untuk melakukan integrasi level pada scene `Stage2.unity`. Tahap ini melanjutkan **Tahap 3: Prefab Assembly** dengan merakit Room Manager, penempatan musuh (Dwarapala & MiniBoss), Boss Arena (Yaksa), sistem Quest, Dialog Intro & Pasca-Boss, pembuatan data Lore, penempatan Prasasti Lore, serta Portal Transisi.

## Struktur Room Stage 2

Berdasarkan rekomendasi hierarki standar, level diatur dengan mengelompokkan area bermain ke bawah objek induk `Rooms` agar hierarki scene bersih dan terstruktur. Alur Stage 2 adalah:

```
[Intro Spawn] → [Library_Enemies] → [MiniBoss_Room] → [Coridor_Enemies] → [Boss_Room] → [EndPortal → Stage3]
```

| # | Nama Objek Induk (Rooms/) | Sub-Hierarki Utama | Peran Naratif |
|---|---|---|---|
| 1 | `1_Library_Room` | `Library_RoomManager`, `Library_Enemies` (11× Dwarapala), `Prasasti_Dwarapala` | **Perpustakaan** — Ruang utama bertarung. Semua musuh spawn langsung saat masuk trigger area. |
| 2 | `2_MiniBoss_Room` | `MiniBoss_RoomManager`, `MiniBoss_Enemies` (1× MiniBos + 3× Dwarapala), `Prasasti_Yaksa` | **Area Mini Boss** — Ditemukan saat menjelajah. Harus dikalahkan untuk membuka jalan/gerbang ke koridor. |
| 3 | `3_Coridor_Room` | `Coridor_RoomManager`, `Coridor_Enemies` (3× Dwarapala) | **Lorong menuju Bos** — Semua musuh harus dikalahkan untuk membuka pintu ke Boss Room. |
| 4 | `4_Boss_Room` | `BossArena_Yaksa`, `Boss_Yaksa` (1× Yaksa), `LontarSpawnPoint`, `ExitWall_Boss`, `EndPortal` | **Arena Boss Yaksa** — Pintu masuk mengunci saat player masuk. Kalahkan Yaksa untuk mendapatkan Lontar, membuka gerbang keluar, dan memicu EndPortal. |

**Alur Gameplay:**
1. Saka memasuki Perpustakaan Melayang, melihat monolog intro
2. Diberi quest untuk **menjelajahi perpustakaan** dan mencari jalan keluar
3. Saat menjelajah, Saka memasuki area perpustakaan (`Library_Enemies` di bawah `1_Library_Room`) — harus membersihkan 11 Dwarapala
4. Setelah bersih, Saka menemukan jalan ke **area Mini Boss** (`MiniBoss_Enemies` di bawah `2_MiniBoss_Room`) — harus mengalahkan MiniBoss + 3 Dwarapala untuk membuka lorong
5. Saka memasuki **Koridor** (`Coridor_Enemies` di bawah `3_Coridor_Room`) — 3 Dwarapala terakhir menghalangi pintu area boss
6. Setelah koridor bersih, pintu menuju **Boss Room** terbuka
7. Saka memasuki arena Yaksa — pintu di belakang tertutup (tidak bisa mundur). Kalahkan Yaksa → monolog pasca-boss → Lontar jatuh → gerbang keluar terbuka → portal ke Stage 3

---

## Bagian A: Pembuatan Data Lore & Boon (ScriptableObjects)

Sebelum menempatkan prasasti di scene, kita harus membuat aset data pendukungnya terlebih dahulu.

### Langkah 1: Buat Aset LoreData Baru

1. Buka Unity Editor, arahkan fokus ke Project Window di folder `Assets/Data/Lores/`.
2. Klik kanan di folder kosong tersebut → **Create** → **LontarQuest** → **Lore Data**.
3. Beri nama file baru tersebut **`Lore_Dwarapala.asset`**.
4. Di Inspector, isi field berikut:

   * **Lore ID**: `lore_dwarapala` (ID unik sistem jurnal/save).
   * **Monster Name**: `Dwarapala`
   * **Monster Sprite**: Pilih sprite `Dwarapala_IdleFront_0` (dari folder `Assets/Arts/Enemies/Stage2-Dwarapala/Sprites/`).
   * **Mythology Description**:
     > *"Penjaga pintu gerbang candi dalam mitologi kuno Nusantara. Berwujud raksasa batu berwajah menyeramkan dengan taring mencuat, membawa senjata gada besi besar untuk menghantam penyusup berkeping-keping. Bersifat sangat protektif terhadap wilayahnya."*
     >
   * **Weakness Hint**:
     > *"Gerakan Dwarapala sangat lambat dan dapat dibaca. Saka dapat memancing serangannya, melakukan dash untuk menghindar dari area hantaman (AoE), lalu menyerang balik saat gada musuh tertancap di tanah."*
     >
5. Klik kanan lagi di folder `Assets/Data/Lores/` → **Create** → **LontarQuest** → **Lore Data**.
6. Beri nama file baru tersebut **`Lore_Yaksa.asset`**.
7. Di Inspector, isi field berikut:

   * **Lore ID**: `lore_yaksa`
   * **Monster Name**: `Yaksa`
   * **Monster Sprite**: Pilih sprite `Yaksa_IdleFront_0` (dari folder `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Sprites/`).
   * **Mythology Description**:
     > *"Makhluk gaib pelindung perpustakaan kuno yang melayang di udara. Memiliki sayap kristal energi biru yang memancarkan aura magis. Yaksa menyerang menggunakan busur mistis yang menembakkan panah energi pencari sasaran."*
     >
   * **Weakness Hint**:
     > *"Yaksa akan selalu terbang mundur untuk menjaga jarak. Gunakan pilar perpustakaan untuk menghalangi tembakan panah energinya, lalu serang dengan cepat saat ia melakukan jeda cooldown serangan."*
     >

---

## Bagian B: Setup Intro Sequence & Dialog Masuk Stage 2

> [!TIP]
> **Cara Cepat (Otomatis):** Anda dapat mengonfigurasi seluruh **Bagian B** dan monolog pasca-boss secara otomatis dengan membuka scene `Stage2` di Unity Editor, lalu mengeklik menu **`LontarQuest` -> `Setup Stage 2 (Bagian B)`** (atau klik tombol Setup di menu `Window > LontarQuest Setup`). Setup tool akan otomatis mengaktifkan GameObject, memasang script, dan mengisi seluruh monolog Saka secara presisi.

Jika Anda ingin melakukannya secara manual, ikuti langkah-langkah berikut:

### Langkah 2: Pasang Script Intro (`Stage2IntroStarter`)

Script `Stage2IntroStarter.cs` mengatur urutan intro ketika scene `Stage2` pertama kali dimuat: layar fade-in dari hitam, Saka berjalan otomatis (opsional), lalu monolog dialog Saka dimainkan.

1. Pindahkan GameObject **`IntroStart`** dan **`IntroWalk`** ke luar dari parent `Managers` dan diletakkan di bawah parent baru bernama **`Environment & Setup`** (jika belum ada, silakan buat GameObject kosong tersebut). Ini penting untuk menjaga agar parent `Managers` hanya berisi script/controller logika murni.
2. Di Hierarchy scene `Stage2`, cari GameObject **`IntroManager`** di bawah parent **`Managers`** (pastikan dicentang **Active** ✅).
3. Jika ada komponen `Stage1IntroStarter` lama pada `IntroManager`, klik kanan komponen tersebut lalu pilih **Remove Component**.
4. Tambahkan komponen **`Stage2IntroStarter`** pada `IntroManager`.
5. Di Inspector `Stage2IntroStarter`:
   * **Start Delay**: `0.5` (jeda sebelum cutscene mulai)
   * **Walk Speed**: `3` (kecepatan auto-walk, jika digunakan)
   * **Portal Spawn Point**: Seret GameObject **`IntroStart`** (yang berada di bawah `Environment & Setup`).
   * **Walk Destination**: Seret GameObject **`IntroWalk`** (yang berada di bawah `Environment & Setup`).
   * **Initial Quest**: `"Jelajahi Perpustakaan Melayang"`

### Langkah 3: Konfigurasi Dialog Intro (Monolog Saka)

1. Di Hierarchy scene `Stage2`, cari GameObject **`IntroDialogueManager`** di bawah parent **`Managers`** (pastikan dicentang **Active** ✅). GameObject ini sudah memiliki komponen **`IntroDialogue`** dengan referensi UI Canvas terpasang.
2. Di Inspector komponen **`IntroDialogue`** pada `IntroDialogueManager`:
   * **Freeze Player**: ✅ (centang)
   * **Unfreeze Player When Finished**: ✅ (centang)
   * **Dialogues** (Array, 3 elemen):

     | Index | Speaker | Text |
     |-------|---------|------|
     | 0 | `Saka` | `"Tempat apa ini...? Rak-rak buku melayang di tengah kekosongan. Seperti perpustakaan yang ditinggalkan oleh waktu."` |
     | 1 | `Saka` | `"Lontar-lontar berterbangan di udara... Pasti ada sesuatu yang penting tersembunyi di sini."` |
     | 2 | `Saka` | `"Tapi batu-batu penjaga itu masih bergerak. Aku harus tetap waspada."` |

3. Kembali ke komponen `Stage2IntroStarter` pada `IntroManager`, seret GameObject **`IntroDialogueManager`** ke slot **Intro Dialogue**.

**Catatan:** Setelah dialog selesai, quest otomatis diset ke `"Jelajahi Perpustakaan Melayang"` dan kontrol Saka dilepas.

---

## Bagian C: Setup Room Bertarung di Scene `Stage2.unity`

Kita akan menyusun logika ruangan bertarung untuk ketiga area menggunakan `Stage2RoomManager`.

> [!IMPORTANT]
> **Audit & Restrukturisasi UI Canvas Sebelum Memulai:**
> 1. **Ganti QuestManager**: Pilih GameObject **`QuestManager`** di bawah folder `Managers`. Klik kanan komponen `QuestManager` bawaan lama lalu pilih **Remove Component**. Tambahkan komponen baru **`Stage2QuestManager`**.
> 2. **Restrukturisasi UI Canvas (Audit Tangkapan Layar)**:
>    * **Hapus `QuestPanel` Kosong**: GameObject bernama `QuestPanel` yang berada langsung di bawah parent `UI` saat ini kosong (tanpa anak). Hapus objek kosong ini.
>    * **Buat `QuestPanel` Baru di bawah Canvas**: Buat GameObject kosong baru di bawah **`Canvas`** utama (beri nama **`QuestPanel`**). Pindahkan objek **`Panel`** (background quest), **`Judul`** (teks "Misi"), dan **`ObjectiveText`** (teks TMP isi quest) yang saat ini melayang langsung di bawah `Canvas` agar menjadi anak (children) dari `QuestPanel` baru ini.
>    * **Hubungkan Referensi Misi**: Di komponen `Stage2QuestManager`, seret GameObject **`QuestPanel`** baru ke slot **Quest Panel**, dan **`ObjectiveText`** ke slot **Objective Text**.
>    * **Pindahkan `JournalUI` & `EPrompt` ke Canvas**: Pindahkan GameObject **`JournalUI`** dan **`EPrompt`** agar berada di bawah **`Canvas`** utama (atau jika `EPrompt` merupakan World Space UI, pastikan ia memiliki komponen Canvas sendiri dengan Render Mode set ke *World Space*). Hubungkan `JournalUI` ke slot **Journal UI Panel** pada script `JournalManager`.
>    * **Verifikasi `DialoguePanel`**: `DialoguePanel` saat ini sudah memiliki nested `Canvas` (`DialoguePanel > Canvas`), hal ini sudah benar agar monolog Saka ter-render secara terpisah.
> 3. **Buat Folder Induk Ruangan**: Buat GameObject kosong baru di root hierarki bernama **`Rooms`** untuk mengelompokkan semua objek bertarung agar rapi.

### Panduan Pembuatan & Konfigurasi Pintu Misi (Entry Door & Next Room Blockades)

Sebelum merakit ruangan, Anda harus menyiapkan objek fisik pintu/penghalang di scene agar dapat dipasangkan pada script `Stage2RoomManager` masing-masing ruangan. Ikuti aturan konfigurasi standar berikut:

#### 1. Komponen Fisik Pintu
*   Setiap pintu (baik pintu masuk maupun blokade jalan) harus berupa GameObject yang memiliki **Sprite Renderer** (visual pintu/jeruji) atau menggunakan layer khusus Tilemap.
*   Wajib memiliki komponen **`BoxCollider2D`** (atau collider fisik lain) dengan ketentuan:
    *   **Is Trigger** = ❌ (Jangan dicentang!). Pintu harus memblokir pergerakan fisik pemain/musuh secara nyata.
    *   Sesuaikan ukuran collider agar menutupi seluruh lebar celah/lorong jalan secara rapat.

#### 2. Konfigurasi State Awal di Inspector (SANGAT KRITIKAL ⚠️)
*   **`Entry Door` (Pintu Masuk)**:
    *   **State Awal**: Harus diset **Nonaktif / Disabled** di Inspector (hilangkan centang pada nama GameObject di pojok kiri atas Inspector).
    *   *Alasan*: Jika aktif sejak awal, Saka akan terhalang dan tidak bisa masuk ke dalam ruangan. Script `Stage2RoomManager` akan otomatis mengaktifkannya (`SetActive(true)`) untuk mengunci Saka saat trigger ruangan pertama kali terinjak.
*   **`Next Room Blockades` (Blokade Jalan Keluar)**:
    *   **State Awal**: Harus diset **Aktif / Enabled** di Inspector (centang nama GameObject).
    *   *Alasan*: Blokade harus menghalangi jalan Saka sejak awal sebelum seluruh musuh di ruangan tersebut dikalahkan. Script `Stage2RoomManager` akan otomatis menonaktifkannya (`SetActive(false)`) untuk membuka jalan setelah ruangan bersih (`RoomCleared`).

---

Di bawah ini adalah langkah-langkah penyiapan per ruangan:

### Langkah 4: Setup Library_Enemies (Perpustakaan Utama)

Area pertempuran utama pertama. 11 Dwarapala sudah diposisikan di dalam perpustakaan.

1. Di bawah GameObject **`Rooms`**, buat GameObject kosong induk baru bernama **`1_Library_Room`**.
2. Pindahkan GameObject **`Library_Enemies`** (yang berisi 11 Dwarapala) agar menjadi anak dari **`1_Library_Room`**.
3. Buat GameObject kosong bernama **`Library_Doors`** di bawah **`1_Library_Room`**. Di dalamnya, buat dua GameObject pintu/penghalang:
   * **`Library_EntryDoor`** (Pintu Masuk): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set NONAKTIF (Disabled) di Inspector.**
   * **`Library_NextBlockade`** (Blokade ke MiniBoss): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set AKTIF (Enabled) di Inspector.**
4. Buat GameObject kosong di bawah **`1_Library_Room`** → beri nama **`Library_RoomManager`**.
5. Tambahkan komponen **BoxCollider2D** pada `Library_RoomManager`:
   * Centang **Is Trigger** = ✅.
   * Atur posisi dan ukurannya agar mencakup seluruh area lantai perpustakaan utama.
6. Tambahkan komponen **`Stage2RoomManager.cs`** pada `Library_RoomManager`:
   * **Entry Door**: Seret **`Library_EntryDoor`**.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Tambah 1 elemen di list, seret **`Library_NextBlockade`**.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab` dari `Assets/Prefabs/Stage1/`.
   * **Quest On Enter**: `"Bersihkan Perpustakaan dari Penjaga"`
   * **Quest On Clear**: `"Cari Jalan Keluar Perpustakaan"`
6. Pilih **semua 11 objek** `Enemy_Dwarapala` yang ada di bawah `Library_Enemies` di Hierarchy.
7. Pastikan semua 11 objek diset **Nonaktif** (uncheck kotak aktif di Inspector) secara default agar mereka baru muncul saat player masuk ruangan.
8. Pilih `Library_RoomManager`, lalu seret ke-11 objek `Enemy_Dwarapala` ke list **Enemies in Room**.

> **Progress UI**: Saat Saka masuk perpustakaan, quest akan menampilkan `"Bersihkan Perpustakaan dari Penjaga (0/11)"` dan terupdate otomatis saat musuh mati.

### Langkah 5: Setup MiniBoss_Room (Area Mini Boss)

Area mini boss ditemukan saat Saka menjelajahi perpustakaan. Harus mengalahkan semua musuh untuk membuka lorong ke koridor.

1. Di bawah GameObject **`Rooms`**, buat GameObject kosong induk baru bernama **`2_MiniBoss_Room`**.
2. Pindahkan GameObject **`MiniBoss_Room`** (yang berisi 1 MiniBoss dan 3 Dwarapala) agar menjadi anak dari **`2_MiniBoss_Room`**. Opsional: rename `MiniBoss_Room` (container musuh) menjadi **`MiniBoss_Enemies`** untuk menghindari kebingungan nama dengan induknya.
3. Buat GameObject kosong bernama **`MiniBoss_Doors`** di bawah **`2_MiniBoss_Room`**. Di dalamnya, buat dua GameObject pintu/penghalang:
   * **`MiniBoss_EntryDoor`** (Pintu Masuk): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set NONAKTIF (Disabled) di Inspector.**
   * **`MiniBoss_NextBlockade`** (Blokade ke Lorong): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set AKTIF (Enabled) di Inspector.**
4. Buat GameObject kosong di bawah **`2_MiniBoss_Room`** → beri nama **`MiniBoss_RoomManager`**.
5. Tambahkan komponen **BoxCollider2D** pada `MiniBoss_RoomManager` (Is Trigger = ✅) menutupi area pertempuran mini boss.
6. Tambahkan komponen **`Stage2RoomManager.cs`** pada `MiniBoss_RoomManager`:
   * **Entry Door**: Seret **`MiniBoss_EntryDoor`**.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Tambah 1 elemen di list, seret **`MiniBoss_NextBlockade`**.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Kalahkan Dwarapala Raksasa!"`
   * **Quest On Clear**: `"Lorong Menuju Area Baru Terbuka"`
6. Pilih **1 objek** `MiniBos_Dwarapala` dan **3 objek** `Enemy_Dwarapala` di bawah container musuh.
7. Pastikan keempat objek diset **Nonaktif** di Inspector secara default.
8. Daftarkan keempat objek ke list **Enemies in Room** pada `MiniBoss_RoomManager`.

> **Progress UI**: `"Kalahkan Dwarapala Raksasa! (0/4)"`

### Langkah 6: Setup Coridor_Enemies (Lorong Menuju Boss)

Lorong transisi pendek sebelum arena boss. Semua musuh harus mati agar pintu arena boss terbuka.

1. Di bawah GameObject **`Rooms`**, buat GameObject kosong induk baru bernama **`3_Coridor_Room`**.
2. Pindahkan GameObject **`Coridor_Enemies`** (yang berisi 3 Dwarapala) agar menjadi anak dari **`3_Coridor_Room`**.
3. Buat GameObject kosong bernama **`Coridor_Doors`** di bawah **`3_Coridor_Room`**. Di dalamnya, buat dua GameObject pintu/penghalang:
   * **`Coridor_EntryDoor`** (Pintu Masuk): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set NONAKTIF (Disabled) di Inspector.**
   * **`Coridor_NextBlockade`** (Blokade ke Arena Boss): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set AKTIF (Enabled) di Inspector.**
4. Buat GameObject kosong di bawah **`3_Coridor_Room`** → beri nama **`Coridor_RoomManager`**.
5. Tambahkan komponen **BoxCollider2D** pada `Coridor_RoomManager` (Is Trigger = ✅) menutupi area koridor.
6. Tambahkan komponen **`Stage2RoomManager.cs`** pada `Coridor_RoomManager`:
   * **Entry Door**: Seret **`Coridor_EntryDoor`**.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Tambah 1 elemen di list, seret **`Coridor_NextBlockade`**.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Terobos Penjaga Lorong"`
   * **Quest On Clear**: `"Mendekati Ruang Sang Yaksa"`
6. Pilih **3 objek** `Enemy_Dwarapala` di bawah `Coridor_Enemies`.
7. Pastikan ketiga objek diset **Nonaktif** di Inspector secara default.
8. Daftarkan ketiga objek ke list **Enemies in Room** pada `Coridor_RoomManager`.

> **Progress UI**: `"Terobos Penjaga Lorong (0/3)"`

---

## Bagian D: Integrasi Boss Arena & Yaksa

### Langkah 7: Konfigurasi Stage2BossArena

Area di luar perpustakaan tempat Boss Yaksa menunggu. Saka **tidak bisa mundur** begitu masuk — pintu entry terkunci.

1. Di bawah GameObject **`Rooms`**, buat GameObject kosong induk baru bernama **`4_Boss_Room`**.
2. Pindahkan container GameObject **`Boss_Room`** (yang berisi `Boss_Yaksa`) ke bawah **`4_Boss_Room`**.
3. Buat GameObject kosong bernama **`Boss_Doors`** di bawah **`4_Boss_Room`**. Di dalamnya, buat pintu/penghalang:
   * **`Boss_EntryDoor`** (Pintu Masuk Arena): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set NONAKTIF (Disabled) di Inspector.**
   * **`ExitWall_Boss`** (Dinding Pembatas Keluar): Pasang komponen `Sprite Renderer` dan `BoxCollider2D` (Is Trigger = ❌). **Set AKTIF (Enabled) di Inspector.**
4. Buat GameObject kosong di gerbang masuk arena bos di bawah **`4_Boss_Room`** → beri nama **`BossArena_Yaksa`**.
5. Tambahkan komponen **BoxCollider2D** pada `BossArena_Yaksa`:
   * Centang **Is Trigger** = ✅.
   * Sesuaikan ukuran collider agar melintangi pintu masuk arena (sehingga player pasti memicu trigger ini saat masuk).
6. Tambahkan komponen **`Stage2BossArena.cs`** pada `BossArena_Yaksa`:
   * **Entry Door**: Seret **`Boss_EntryDoor`**.
   * **Next Room Blockades**: Seret **`ExitWall_Boss`**.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Boss Object**: Drag objek `Boss_Yaksa` dari `Boss_Room` di Hierarchy ke slot ini. Pastikan `Boss_Yaksa` diset **Nonaktif (disabled)** secara default.
   * **Boss AI**: Tarik script `Stage2EnemyMovement` yang menempel pada `Boss_Yaksa` ke slot ini.
   * **Boss Health UI**: Seret panel UI **`BossHealthPanel`** dari Canvas (pastikan berada di bawah child `Canvas` utama di parent `UI`).
   * **Boss Health Slider**: Seret slider HP bos dari panel tersebut.
   * **Lontar Reward Prefab**: Tarik prefab **`LontarBossDrop.prefab`** dari folder `Assets/Prefab/`.
   * **Lontar Spawn Point**: Buat child object kosong di tengah arena bos di bawah **`4_Boss_Room`** (beri nama **`LontarSpawnPoint`**), seret ke slot ini.
   * **Post Boss Delay**: `1.5` (detik)
   * **Quest On Boss**: `"Kalahkan Yaksa"`
   * **Quest On Clear**: `"Ambil Kitab Lontar"`

### Langkah 8: Konfigurasi Dialog Pasca-Boss (Monolog Saka setelah Yaksa kalah)

Setelah Yaksa dikalahkan, monolog Saka akan dimainkan sebelum Lontar jatuh (konsisten dengan pola Stage 1 `BossArenaController.postBossDialogue`).

1. Di Hierarchy scene `Stage2`, cari GameObject **`PostBossDialogueManager`** di bawah parent **`Managers`** (pastikan dicentang **Active** ✅). GameObject ini sudah memiliki komponen **`IntroDialogue`** dengan referensi UI Canvas terpasang.
2. Di Inspector komponen **`IntroDialogue`** pada `PostBossDialogueManager`:
   * **Freeze Player**: ✅ (centang)
   * **Unfreeze Player When Finished**: ✅ (centang)
   * **Dialogues** (Array, 3 elemen):

     | Index | Speaker | Text |
     |-------|---------|------|
     | 0 | `Saka` | `"Makhluk itu... panah energinya bisa membunuhku kalau aku lengah sedikit saja."` |
     | 1 | `Saka` | `"Perpustakaan ini menyimpan sesuatu. Kenapa ada roh penjaga sekuat ini di tempat yang seharusnya sudah lama mati?"` |
     | 2 | `Saka` | `"Lontar itu bersinar... Sepertinya ada kekuatan kuno lagi yang bisa kuserap. Aku harus mengambilnya."` |

3. Kembali ke komponen `Stage2BossArena` pada GameObject **`BossArena_Yaksa`**, seret GameObject **`PostBossDialogueManager`** ke slot **Post Boss Dialogue**.

**Alur otomatis setelah Yaksa mati:**
1. UI HP bos disembunyikan
2. Quest `"Kalahkan Yaksa"` selesai
3. Jeda 1.5 detik (animasi kematian bos)
4. **Monolog Saka dimainkan** (3 baris dialog di atas)
5. Lontar drop muncul → Quest `"Ambil Lontar Kuno yang Terjatuh"`
6. Dinding `ExitWall_Boss` terbuka → Portal keluar terlihat
7. Setelah Lontar diambil → Boon selection → Portal ke `Stage3`

---

## Bagian E: Setup Portal Akhir & Prasasti Lore

### Langkah 9: Pasang Portal Transisi Level (`EndPortal`)

Agar konsisten dengan arsitektur Stage 1 (Kepala Kala) dan menghindari bug portal tidak bisa dipicu:

1. Buat GameObject dinding/geruji penghalang jalan keluar di belakang arena boss Yaksa di bawah parent **`4_Boss_Room`**, beri nama **`ExitWall_Boss`**.
2. Seret `ExitWall_Boss` ke dalam list **`Next Room Blockades`** pada komponen `Stage2BossArena` di GameObject `BossArena_Yaksa`. Dengan begitu, gerbang ini akan otomatis menghalangi jalan saat permainan dimulai dan terbuka saat Yaksa dikalahkan.
3. Drag prefab **`EndPortal.prefab`** (dari folder `Assets/Prefab/`) dan posisikan **di belakang** `ExitWall_Boss` sebagai anak dari **`4_Boss_Room`**.
4. Di Inspector komponen **`EndPortal`**:
   * Pastikan GameObject tetap **Aktif** (`m_IsActive = true`) ✅ agar bisa mendeteksi trigger pemain setelah gerbang blockade terbuka.
   * Pastikan **Is Trigger** pada BoxCollider2D tercentang ✅.
   * **Target Scene Name**: Isi dengan **`Stage3`**.

### Langkah 10: Pasang Prasasti Kuno (`LoreInteractable`)

Tempatkan dua prasasti di area aman untuk memberikan lore dan hint kelemahan musuh.

1. **Prasasti Dwarapala (Area Aman Setelah Perpustakaan)**:
   * Drag prefab **`Prasasti_KepalaKala`** dari `Assets/Prefabs/Stage1/` ke area aman setelah Library bersih (posisikan di bawah **`1_Library_Room`**). Rename menjadi `Prasasti_Dwarapala`.
   * Pastikan memiliki komponen **`LoreInteractable.cs`**.
   * Di Inspector:
     * **Lore Data To Unlock**: Seret `Lore_Dwarapala.asset`.
     * **Interact Key**: `E`
2. **Prasasti Yaksa (Area Aman Setelah MiniBoss)**:
   * Drag prefab **`Prasasti_KepalaKala`** ke area aman setelah MiniBoss Room (posisikan di bawah **`2_MiniBoss_Room`**). Rename menjadi `Prasasti_Yaksa`.
   * Di Inspector:
     * **Lore Data To Unlock**: Seret `Lore_Yaksa.asset`.
     * **Interact Key**: `E`

---

## Bagian F: Ringkasan Quest Flow

```
┌──────────────────────────────────────────────────────────────────────────────┐
│  INTRO                                                                       │
│  [Monolog Saka — 3 baris]                                                    │
│  Quest: "Jelajahi Perpustakaan Melayang"                                     │
├──────────────────────────────────────────────────────────────────────────────┤
│  LIBRARY_ENEMIES (11 Dwarapala)                                              │
│  Enter → "Bersihkan Perpustakaan dari Penjaga (0/11)"                        │
│  Clear → "Cari Jalan Keluar Perpustakaan"                                    │
│          → Blockade ke MiniBoss_Room terbuka                                 │
├──────────────────────────────────────────────────────────────────────────────┤
│  MINIBOSS_ROOM (1 MiniBoss + 3 Dwarapala)                                    │
│  Enter → "Kalahkan Dwarapala Raksasa! (0/4)"                                │
│  Clear → "Lorong Menuju Area Baru Terbuka"                                   │
│          → Blockade ke Coridor terbuka                                       │
├──────────────────────────────────────────────────────────────────────────────┤
│  CORIDOR_ENEMIES (3 Dwarapala)                                               │
│  Enter → "Terobos Penjaga Lorong (0/3)"                                      │
│  Clear → "Mendekati Ruang Sang Yaksa"                                        │
│          → Blockade ke Boss_Room terbuka                                     │
├──────────────────────────────────────────────────────────────────────────────┤
│  BOSS_ROOM (Boss Yaksa)                                                      │
│  Enter → Pintu belakang TERKUNCI (tidak bisa mundur)                         │
│          Quest: "Kalahkan Yaksa"                                             │
│  Clear → [Monolog Saka — 3 baris]                                            │
│          Lontar drop → "Ambil Lontar Kuno yang Terjatuh"                     │
│          ExitWall terbuka → EndPortal terlihat                               │
│          → Boon Selection → "Masuki Portal" → Stage3                         │
└──────────────────────────────────────────────────────────────────────────────┘
```

---

## Bagian G: Progress Tracking Checklist

Gunakan checklist di bawah ini untuk memantau progress pengerjaan di Unity Editor. Beri tanda centang `[x]` pada langkah-langkah yang telah selesai disiapkan.

### Audit & Penyesuaian Hierarki (Penting!)
- [ ] Buat GameObject kosong induk **`Rooms`** di root hierarki.
- [ ] Buat sub-folder kosong **`1_Library_Room`**, **`2_MiniBoss_Room`**, **`3_Coridor_Room`**, dan **`4_Boss_Room`** di bawah `Rooms`.
- [ ] Pindahkan container musuh (`Library_Enemies`, `MiniBoss_Room` [rename ke `MiniBoss_Enemies`], `Coridor_Enemies`, `Boss_Room`) ke sub-folder masing-masing.
- [ ] Pindahkan `IntroStart` dan `IntroWalk` dari `Managers` ke parent baru bernama **`Environment & Setup`** di root.
- [ ] Hapus GameObject `QuestPanel` kosong di root `UI`.
- [ ] Buat GameObject kosong `QuestPanel` baru di bawah `Canvas` utama, lalu masukkan `Panel`, `Judul`, dan `ObjectiveText` ke dalamnya.
- [ ] Pindahkan `JournalUI` dan `EPrompt` ke bawah `Canvas` utama agar ter-render dengan benar.
- [ ] Hubungkan referensi UI Baru: `QuestPanel` baru dan `ObjectiveText` ke `Stage2QuestManager`, serta `JournalUI` ke `JournalManager`.

### Data & Assets
- [ ] Buat `Lore_Dwarapala.asset` di folder `Assets/Data/Lores/` (ID = `lore_dwarapala`)
- [ ] Buat `Lore_Yaksa.asset` di folder `Assets/Data/Lores/` (ID = `lore_yaksa`)

### Intro Sequence
- [ ] Pastikan GameObject `IntroManager` aktif dan terpasang komponen `Stage2IntroStarter`
- [ ] Pastikan GameObject `IntroDialogueManager` aktif dan isi 3 baris monolog intro Saka
- [ ] Hubungkan `IntroDialogue` (pada `IntroDialogueManager`) ke slot `introDialogue` di `Stage2IntroStarter`
- [ ] Hubungkan `IntroStart` dan `IntroWalk` dari `Environment & Setup` ke slot spawn point & destination di `Stage2IntroStarter`
- [ ] Set field `Initial Quest` = `"Jelajahi Perpustakaan Melayang"`

### HUD Integration
- [ ] Ganti `QuestManager` lama di Canvas `Stage2` dengan script `Stage2QuestManager` dan hubungkan referensi UI-nya (Quest Panel & Objective Text)

### Room Setup
- [ ] **Library**: Konfigurasi `Library_RoomManager` di bawah `1_Library_Room` dengan trigger collider dan daftarkan 11 `Enemy_Dwarapala` (nonaktif default). Quest: `"Bersihkan Perpustakaan dari Penjaga"` / `"Cari Jalan Keluar Perpustakaan"`
- [ ] **MiniBoss**: Konfigurasi `MiniBoss_RoomManager` di bawah `2_MiniBoss_Room` dengan trigger collider dan daftarkan 1 `MiniBos_Dwarapala` + 3 `Enemy_Dwarapala` (nonaktif default). Quest: `"Kalahkan Dwarapala Raksasa!"` / `"Lorong Menuju Area Baru Terbuka"`
- [ ] **Coridor**: Konfigurasi `Coridor_RoomManager` di bawah `3_Coridor_Room` dengan trigger collider dan daftarkan 3 `Enemy_Dwarapala` (nonaktif default). Quest: `"Terobos Penjaga Lorong"` / `"Mendekati Ruang Sang Yaksa"`

### Boss Arena
- [ ] Konfigurasi trigger `BossArena_Yaksa` di bawah `4_Boss_Room` dengan script `Stage2BossArena` dan hubungkan `Boss_Yaksa` (nonaktif default)
- [ ] Hubungkan `BossHealthPanel` dan `BossHealthSlider` (dari Canvas utama) ke `Stage2BossArena`
- [ ] Assign prefab `LontarBossDrop` dan buat `LontarSpawnPoint` di bawah `4_Boss_Room` untuk dihubungkan ke `Stage2BossArena`
- [ ] Pastikan GameObject `PostBossDialogueManager` aktif dan isi 3 baris monolog pasca-boss Saka
- [ ] Hubungkan `IntroDialogue` (pada `PostBossDialogueManager`) ke slot `Post Boss Dialogue` di `Stage2BossArena`

### Lore & Portal
- [ ] Tempatkan `Prasasti_Dwarapala` di area aman setelah Library (di bawah `1_Library_Room`) dengan `LoreInteractable` → `Lore_Dwarapala`
- [ ] Tempatkan `Prasasti_Yaksa` di area aman setelah MiniBoss Room (di bawah `2_MiniBoss_Room`) dengan `LoreInteractable` → `Lore_Yaksa`
- [ ] Buat `ExitWall_Boss` (di bawah `4_Boss_Room`) sebagai blockade, lalu pasang `EndPortal` (di bawah `4_Boss_Room`, selalu aktif) di belakangnya dengan target scene `Stage3`
