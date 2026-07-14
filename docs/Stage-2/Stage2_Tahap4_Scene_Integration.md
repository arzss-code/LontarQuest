# Panduan Stage 2 — Tahap 4: Scene Integration

Dokumen ini memandu Anda langkah-demi-langkah di Unity Editor untuk melakukan integrasi level pada scene `Stage2.unity`. Tahap ini melanjutkan **Tahap 3: Prefab Assembly** dengan merakit Room Manager, penempatan musuh (Dwarapala & MiniBoss), Boss Arena (Yaksa), sistem Quest, Dialog Intro & Pasca-Boss, pembuatan data Lore, penempatan Prasasti Lore, serta Portal Transisi.

## Struktur Room Stage 2

Berdasarkan hierarki scene yang sudah disusun, alur Stage 2 adalah:

```
[Intro Spawn] → [Library_Enemies] → [MiniBoss_Room] → [Coridor_Enemies] → [Boss_Room] → [EndPortal → Stage3]
```

| # | Hierarchy Name | Isi | Peran Naratif |
|---|---|---|---|
| 1 | `Library_Enemies` | 11× Enemy_Dwarapala | **Perpustakaan** — ruang utama bertarung. Semua spawn langsung. |
| 2 | `MiniBoss_Room` | 1× MiniBos_Dwarapala + 3× Enemy_Dwarapala | **Area Mini Boss** — ditemukan saat menjelajahi perpustakaan. Harus dikalahkan untuk membuka jalan ke koridor. |
| 3 | `Coridor_Enemies` | 3× Enemy_Dwarapala | **Lorong menuju Bos** — semua musuh harus dikalahkan untuk membuka area boss. |
| 4 | `Boss_Room` | 1× Boss_Yaksa | **Arena Boss Yaksa** — pintu masuk terkunci saat player masuk (tidak bisa mundur). Kalahkan Yaksa untuk membuka gerbang keluar. |

**Alur Gameplay:**
1. Saka memasuki Perpustakaan Melayang, melihat monolog intro
2. Diberi quest untuk **menjelajahi perpustakaan** dan mencari jalan keluar
3. Saat menjelajah, Saka memasuki area perpustakaan (`Library_Enemies`) — harus membersihkan 11 Dwarapala
4. Setelah bersih, Saka menemukan jalan ke **area Mini Boss** (`MiniBoss_Room`) — harus mengalahkan MiniBoss + 3 Dwarapala untuk membuka lorong
5. Saka memasuki **Koridor** (`Coridor_Enemies`) — 3 Dwarapala terakhir menghalangi pintu area boss
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

### Langkah 2: Pasang Script Intro (`Stage2IntroStarter`)

Script `Stage2IntroStarter.cs` mengatur urutan intro ketika scene `Stage2` pertama kali dimuat: layar fade-in dari hitam, Saka berjalan otomatis (opsional), lalu monolog dialog Saka dimainkan.

1. Buat GameObject kosong di awal scene (area spawn player) → beri nama **`Stage2_IntroManager`**.
2. Tambahkan komponen **`Stage2IntroStarter`** pada `Stage2_IntroManager`.
3. Di Inspector `Stage2IntroStarter`:
   * **Start Delay**: `0.5` (jeda sebelum cutscene mulai)
   * **Walk Speed**: `3` (kecepatan auto-walk, jika digunakan)
   * **Portal Spawn Point**: (Opsional) Buat child Transform bernama `SpawnPoint` jika ingin teleport Saka ke posisi awal tertentu.
   * **Walk Destination**: (Opsional) Buat child Transform bernama `WalkTarget` jika ingin Saka berjalan otomatis ke suatu titik sebelum dialog.
   * **Initial Quest**: `"Jelajahi Perpustakaan Melayang"`

### Langkah 3: Konfigurasi Dialog Intro (Monolog Saka)

1. Pada GameObject **`Stage2_IntroManager`**, tambahkan komponen **`IntroDialogue`**.
2. Di Inspector `IntroDialogue`:
   * **Dialogue Panel**: Seret panel UI dialog dari Canvas (panel yang sama yang dipakai untuk semua dialog di game).
   * **Name Text**: Seret komponen TMP nama speaker.
   * **Dialogue Text**: Seret komponen TMP isi dialog.
   * **Freeze Player**: ✅ (centang)
   * **Unfreeze Player When Finished**: ✅ (centang)
   * **Dialogues** (Array, 3 elemen):

     | Index | Speaker | Text |
     |-------|---------|------|
     | 0 | `Saka` | `"Tempat apa ini...? Rak-rak buku melayang di tengah kekosongan. Seperti perpustakaan yang ditinggalkan oleh waktu."` |
     | 1 | `Saka` | `"Lontar-lontar berterbangan di udara... Pasti ada sesuatu yang penting tersembunyi di sini."` |
     | 2 | `Saka` | `"Tapi batu-batu penjaga itu masih bergerak. Aku harus tetap waspada."` |

3. Kembali ke komponen `Stage2IntroStarter`, seret komponen `IntroDialogue` yang baru ditambahkan ke slot **Intro Dialogue**.

**Catatan:** Setelah dialog selesai, quest otomatis diset ke `"Jelajahi Perpustakaan Melayang"` dan kontrol Saka dilepas.

---

## Bagian C: Setup Room Bertarung di Scene `Stage2.unity`

Kita akan menyusun logika ruangan bertarung untuk ketiga area menggunakan `Stage2RoomManager`. Pastikan komponen `Stage2QuestManager` sudah terpasang di Canvas scene (menggantikan `QuestManager` biasa) agar progress counter `(X/Y)` otomatis tampil.

### Langkah 4: Setup Library_Enemies (Perpustakaan Utama)

Area pertempuran utama pertama. 11 Dwarapala sudah diposisikan di dalam perpustakaan.

1. Buat GameObject kosong di area perpustakaan → beri nama `Library_RoomManager`.
2. Tambahkan komponen **BoxCollider2D**:
   * Centang **Is Trigger** = ✅.
   * Atur posisi dan ukurannya agar mencakup seluruh area lantai perpustakaan.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * **Entry Door**: Seret objek pintu masuk perpustakaan yang akan menutup saat player masuk.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Seret penghalang/jeruji yang menutup jalan menuju `MiniBoss_Room`. Ini akan terbuka otomatis saat semua musuh di perpustakaan mati.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab` dari `Assets/Prefabs/Stage1/`.
   * **Quest On Enter**: `"Bersihkan Perpustakaan dari Penjaga"`
   * **Quest On Clear**: `"Cari Jalan Keluar Perpustakaan"`
4. Pilih **semua 11 objek** `Enemy_Dwarapala` yang ada di bawah `Library_Enemies` di Hierarchy.
5. Pastikan semua 11 objek diset **Nonaktif** (uncheck kotak aktif di Inspector).
6. Pilih `Library_RoomManager`, lalu seret ke-11 objek `Enemy_Dwarapala` ke list **Enemies in Room**.

> **Progress UI**: Saat Saka masuk perpustakaan, quest akan menampilkan `"Bersihkan Perpustakaan dari Penjaga (0/11)"` dan terupdate otomatis saat musuh mati.

### Langkah 5: Setup MiniBoss_Room (Area Mini Boss)

Area mini boss ditemukan saat Saka menjelajahi perpustakaan. Harus mengalahkan semua musuh untuk membuka lorong ke koridor.

1. Buat GameObject kosong di area mini boss → beri nama `MiniBoss_RoomManager`.
2. Tambahkan komponen **BoxCollider2D** (Is Trigger = ✅) menutupi area pertempuran mini boss.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * **Entry Door**: Seret pintu masuk area mini boss.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Seret penghalang/jeruji yang menutup jalan menuju `Coridor_Enemies`.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Kalahkan Dwarapala Raksasa!"`
   * **Quest On Clear**: `"Lorong Menuju Area Baru Terbuka"`
4. Pilih **1 objek** `MiniBos_Dwarapala` dan **3 objek** `Enemy_Dwarapala` di bawah `MiniBoss_Room`.
5. Pastikan keempat objek diset **Nonaktif** di Inspector.
6. Daftarkan keempat objek ke list **Enemies in Room** pada `MiniBoss_RoomManager`.

> **Progress UI**: `"Kalahkan Dwarapala Raksasa! (0/4)"`

### Langkah 6: Setup Coridor_Enemies (Lorong Menuju Boss)

Lorong transisi pendek sebelum arena boss. Semua musuh harus mati agar pintu arena boss terbuka.

1. Buat GameObject kosong di area koridor → beri nama `Coridor_RoomManager`.
2. Tambahkan komponen **BoxCollider2D** (Is Trigger = ✅) menutupi area koridor.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * **Entry Door**: Seret pintu masuk koridor.
   * **Exit Doors**: (Kosongkan jika tidak ada BoonDoor)
   * **Next Room Blockades**: Seret penghalang/jeruji yang menutup jalan menuju `Boss_Room`. Ini akan terbuka otomatis saat semua musuh di koridor mati.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Terobos Penjaga Lorong"`
   * **Quest On Clear**: `"Mendekati Ruang Sang Yaksa"`
4. Pilih **3 objek** `Enemy_Dwarapala` di bawah `Coridor_Enemies`.
5. Pastikan ketiga objek diset **Nonaktif** di Inspector.
6. Daftarkan ketiga objek ke list **Enemies in Room** pada `Coridor_RoomManager`.

> **Progress UI**: `"Terobos Penjaga Lorong (0/3)"`

---

## Bagian D: Integrasi Boss Arena & Yaksa

### Langkah 7: Konfigurasi Stage2BossArena

Area di luar perpustakaan tempat Boss Yaksa menunggu. Saka **tidak bisa mundur** begitu masuk — pintu entry terkunci.

1. Di Hierarchy scene `Stage2`, buat GameObject kosong di gerbang masuk arena bos → beri nama `BossArena_Yaksa`.
2. Tambahkan komponen **BoxCollider2D**:
   * Centang **Is Trigger** = ✅.
   * Sesuaikan ukuran collider agar melintangi pintu masuk arena (sehingga player pasti memicu trigger ini saat masuk).
3. Tambahkan komponen **`Stage2BossArena.cs`**:
   * **Entry Door**: Seret pintu/dinding masuk arena yang akan **menutup dan mengunci** saat Saka masuk (pemain tidak bisa mundur ke koridor).
   * **Next Room Blockades**: Seret `ExitWall_Boss` (dinding di belakang arena yang menghalangi portal keluar). Ini terbuka otomatis saat Yaksa mati.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Boss Object**: Drag objek `Boss_Yaksa` dari `Boss_Room` di Hierarchy ke slot ini. Pastikan `Boss_Yaksa` diset **Nonaktif (disabled)** secara default.
   * **Boss AI**: Tarik script `Stage2EnemyMovement` yang menempel pada `Boss_Yaksa` ke slot ini.
   * **Boss Health UI**: Seret panel UI **`BossHealthPanel`** dari Canvas.
   * **Boss Health Slider**: Seret slider HP bos dari panel tersebut.
   * **Lontar Reward Prefab**: Tarik prefab **`LontarBossDrop.prefab`** dari folder `Assets/Prefab/`.
   * **Lontar Spawn Point**: Buat child object kosong di tengah arena bos (beri nama `LontarSpawnPoint`), seret ke slot ini.
   * **Post Boss Delay**: `1.5` (detik)
   * **Quest On Boss**: `"Kalahkan Yaksa"`
   * **Quest On Clear**: `"Ambil Kitab Lontar"`

### Langkah 8: Konfigurasi Dialog Pasca-Boss (Monolog Saka setelah Yaksa kalah)

Setelah Yaksa dikalahkan, monolog Saka akan dimainkan sebelum Lontar jatuh (konsisten dengan pola Stage 1 `BossArenaController.postBossDialogue`).

1. Pada GameObject **`BossArena_Yaksa`**, tambahkan komponen **`IntroDialogue`**.
2. Di Inspector `IntroDialogue`:
   * **Dialogue Panel**: Seret panel UI dialog yang sama dari Canvas.
   * **Name Text**: Seret komponen TMP nama speaker.
   * **Dialogue Text**: Seret komponen TMP isi dialog.
   * **Freeze Player**: ✅ (centang)
   * **Unfreeze Player When Finished**: ✅ (centang)
   * **Dialogues** (Array, 3 elemen):

     | Index | Speaker | Text |
     |-------|---------|------|
     | 0 | `Saka` | `"Makhluk itu... panah energinya bisa membunuhku kalau aku lengah sedikit saja."` |
     | 1 | `Saka` | `"Perpustakaan ini menyimpan sesuatu. Kenapa ada roh penjaga sekuat ini di tempat yang seharusnya sudah lama mati?"` |
     | 2 | `Saka` | `"Lontar itu bersinar... Sepertinya ada kekuatan kuno lagi yang bisa kuserap. Aku harus mengambilnya."` |

3. Kembali ke komponen `Stage2BossArena`, seret komponen `IntroDialogue` yang baru ditambahkan ke slot **Post Boss Dialogue**.

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

1. Buat GameObject dinding/geruji penghalang jalan keluar di belakang arena boss Yaksa, beri nama **`ExitWall_Boss`**.
2. Seret `ExitWall_Boss` ke dalam list **`Next Room Blockades`** pada komponen `Stage2BossArena` di GameObject `BossArena_Yaksa`. Dengan begitu, gerbang ini akan otomatis menghalangi jalan saat permainan dimulai dan terbuka saat Yaksa dikalahkan.
3. Drag prefab **`EndPortal.prefab`** (dari folder `Assets/Prefab/`) dan posisikan **di belakang** `ExitWall_Boss`.
4. Di Inspector komponen **`EndPortal`**:
   * Pastikan GameObject tetap **Aktif** (`m_IsActive = true`) ✅ agar bisa mendeteksi trigger pemain setelah gerbang blockade terbuka.
   * Pastikan **Is Trigger** pada BoxCollider2D tercentang ✅.
   * **Target Scene Name**: Isi dengan **`Stage3`**.

### Langkah 10: Pasang Prasasti Kuno (`LoreInteractable`)

Tempatkan dua prasasti di area aman untuk memberikan lore dan hint kelemahan musuh.

1. **Prasasti Dwarapala (Area Aman Setelah Perpustakaan)**:
   * Drag prefab **`Prasasti_KepalaKala`** dari `Assets/Prefabs/Stage1/` ke area aman setelah Library bersih. Rename menjadi `Prasasti_Dwarapala`.
   * Pastikan memiliki komponen **`LoreInteractable.cs`**.
   * Di Inspector:
     * **Lore Data To Unlock**: Seret `Lore_Dwarapala.asset`.
     * **Interact Key**: `E`
2. **Prasasti Yaksa (Area Aman Setelah MiniBoss)**:
   * Drag prefab **`Prasasti_KepalaKala`** ke area aman setelah MiniBoss Room. Rename menjadi `Prasasti_Yaksa`.
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

### Data & Assets
- [ ] Buat `Lore_Dwarapala.asset` di folder `Assets/Data/Lores/` (ID = `lore_dwarapala`)
- [ ] Buat `Lore_Yaksa.asset` di folder `Assets/Data/Lores/` (ID = `lore_yaksa`)

### Intro Sequence
- [ ] Buat GameObject `Stage2_IntroManager` dengan komponen `Stage2IntroStarter`
- [ ] Tambahkan komponen `IntroDialogue` pada `Stage2_IntroManager` dan isi 3 baris monolog intro Saka
- [ ] Hubungkan `IntroDialogue` ke slot `Intro Dialogue` di `Stage2IntroStarter`
- [ ] Set field `Initial Quest` = `"Jelajahi Perpustakaan Melayang"`

### HUD Integration
- [ ] Ganti `QuestManager` lama di Canvas `Stage2` dengan script `Stage2QuestManager` dan hubungkan referensi UI-nya

### Room Setup
- [ ] **Library**: Konfigurasi `Library_RoomManager` dengan trigger collider dan daftarkan 11 `Enemy_Dwarapala` (nonaktif default). Quest: `"Bersihkan Perpustakaan dari Penjaga"` / `"Cari Jalan Keluar Perpustakaan"`
- [ ] **MiniBoss**: Konfigurasi `MiniBoss_RoomManager` dengan trigger collider dan daftarkan 1 `MiniBos_Dwarapala` + 3 `Enemy_Dwarapala` (nonaktif default). Quest: `"Kalahkan Dwarapala Raksasa!"` / `"Lorong Menuju Area Baru Terbuka"`
- [ ] **Coridor**: Konfigurasi `Coridor_RoomManager` dengan trigger collider dan daftarkan 3 `Enemy_Dwarapala` (nonaktif default). Quest: `"Terobos Penjaga Lorong"` / `"Mendekati Ruang Sang Yaksa"`

### Boss Arena
- [ ] Konfigurasi trigger `BossArena_Yaksa` dengan script `Stage2BossArena` dan hubungkan `Boss_Yaksa` (nonaktif default)
- [ ] Hubungkan `BossHealthPanel` dan `BossHealthSlider` ke `Stage2BossArena`
- [ ] Assign prefab `LontarBossDrop` dan `LontarSpawnPoint` di `Stage2BossArena`
- [ ] Tambahkan komponen `IntroDialogue` pada `BossArena_Yaksa` dan isi 3 baris monolog pasca-boss Saka
- [ ] Hubungkan `IntroDialogue` ke slot `Post Boss Dialogue` di `Stage2BossArena`

### Lore & Portal
- [ ] Tempatkan `Prasasti_Dwarapala` di area aman setelah Library dengan `LoreInteractable` → `Lore_Dwarapala`
- [ ] Tempatkan `Prasasti_Yaksa` di area aman setelah MiniBoss Room dengan `LoreInteractable` → `Lore_Yaksa`
- [ ] Buat `ExitWall_Boss` sebagai blockade, lalu pasang `EndPortal` (selalu aktif) di belakangnya dengan target scene `Stage3`
