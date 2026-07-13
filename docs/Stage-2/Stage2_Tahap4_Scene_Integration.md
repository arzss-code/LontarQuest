# Panduan Stage 2 — Tahap 4: Scene Integration

Dokumen ini memandu Anda langkah-demi-langkah di Unity Editor untuk melakukan integrasi level pada scene `Stage2.unity`. Tahap ini melanjutkan **Tahap 3: Prefab Assembly** dengan merakit Room Manager, penempatan musuh (Dwarapala & MiniBoss), Boss Arena (Yaksa), pembuatan data Lore (ScriptableObjects), penempatan Prasasti Lore, serta Portal Transisi.

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
   * **Weakness Hint**: 
     > *"Gerakan Dwarapala sangat lambat dan dapat dibaca. Saka dapat memancing serangannya, melakukan dash untuk menghindar dari area hantaman (AoE), lalu menyerang balik saat gada musuh tertancap di tanah."*

5. Klik kanan lagi di folder `Assets/Data/Lores/` → **Create** → **LontarQuest** → **Lore Data**.
6. Beri nama file baru tersebut **`Lore_Yaksa.asset`**.
7. Di Inspector, isi field berikut:
   * **Lore ID**: `lore_yaksa`
   * **Monster Name**: `Yaksa`
   * **Monster Sprite**: Pilih sprite `Yaksa_IdleFront_0` (dari folder `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Sprites/`).
   * **Mythology Description**:
     > *"Makhluk gaib pelindung perpustakaan kuno yang melayang di udara. Memiliki sayap kristal energi biru yang memancarkan aura magis. Yaksa menyerang menggunakan busur mistis yang menembakkan panah energi pencari sasaran."*
   * **Weakness Hint**:
     > *"Yaksa akan selalu terbang mundur untuk menjaga jarak. Gunakan pilar perpustakaan untuk menghalangi tembakan panah energinya, lalu serang dengan cepat saat ia melakukan jeda cooldown serangan."*

---

## Bagian B: Setup Room 1 s/d Room 4 di Scene `Stage2.unity`

Kita akan menyusun logika ruangan bertarung dan menempatkan musuh Dwarapala secara dinamis.

### Langkah 2: Setup Room 1 (Dungeon Tutorial)
1. Buka scene `Stage2` di Hierarchy.
2. Cari area **Room 1** (pintu masuk pertama). Buat GameObject kosong baru → beri nama `Room_1_Manager`.
3. Tambahkan komponen **BoxCollider2D** pada `Room_1_Manager`:
   * Centang **Is Trigger** = ✅.
   * Atur posisi dan ukurannya (Size) agar mencakup seluruh lantai area pertempuran Room 1.
4. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * **Entry Door**: Seret objek pintu batu penutup pintu masuk ke slot ini.
   * **Exit Doors**: Seret objek `BoonDoor` (pintu keluar Room 1) ke list ini.
   * **Next Room Blockades**: Seret jeruji/tembok pembatas jalan ke Room 2 ke list ini.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab` dari `Assets/Prefabs/Stage1/`.
   * **Quest On Enter**: `"Bersihkan Koridor Candi"`
   * **Quest On Clear**: `"Selidiki Lebih Dalam"`
5. Drag prefab **`Enemy_Dwarapala`** (yang dibuat pada Tahap 3) sebanyak **3 buah** ke dalam area Room 1 di scene. Posisikan di koordinat yang menyebar secara taktis.
6. Pilih ketiga objek `Enemy_Dwarapala` tersebut, lalu **Nonaktifkan** (uncheck kotak aktif GameObject di pojok kiri atas Inspector) agar musuh tersembunyi secara default.
7. Pilih kembali `Room_1_Manager`, lalu seret ketiga objek `Enemy_Dwarapala` (yang nonaktif tadi) ke dalam list **Enemies in Room**.

### Langkah 3: Setup Room 2 (Dungeon Escalate)
1. Buat GameObject kosong baru di area Room 2 → beri nama `Room_2_Manager`.
2. Terapkan langkah collider trigger yang sama seperti Room 1 (Is Trigger = ✅).
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * Hubungkan pintu masuk (`Entry Door`), pintu keluar/hadiah (`Exit Doors`), dan blockade jalan (`Next Room Blockades`).
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Kalahkan Dwarapala Penjaga"`
   * **Quest On Clear**: `"Telusuri Ruang Buku"`
4. Drag prefab **`Enemy_Dwarapala`** sebanyak **4 buah** ke dalam area Room 2.
5. Nonaktifkan keempat objek musuh tersebut di Inspector.
6. Daftarkan keempat objek musuh tersebut ke list **Enemies in Room** pada `Room_2_Manager`.

### Langkah 4: Setup Room 3 (Mini Boss Room)
1. Buat GameObject kosong baru di area Room 3 → beri nama `Room_3_Manager`.
2. Tambahkan komponen **BoxCollider2D** (Is Trigger = ✅) menutupi area bertarung Room 3.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * Hubungkan pintu dan blockade yang sesuai untuk Room 3.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Kalahkan Dwarapala Raksasa!"`
   * **Quest On Clear**: `"Akses Koridor Utama Terbuka"`
4. Drag **1 buah** prefab **`MiniBoss_Dwarapala`** (skala 1.5x) dan **2 buah** prefab **`Enemy_Dwarapala`** biasa ke dalam Room 3.
5. Nonaktifkan ketiga objek musuh tersebut di Inspector.
6. Daftarkan ketiga objek musuh tersebut ke list **Enemies in Room** pada `Room_3_Manager`.

### Langkah 5: Setup Room 4 (Gauntlet Room)
1. Buat GameObject kosong baru di area Room 4 → beri nama `Room_4_Manager`.
2. Tambahkan komponen **BoxCollider2D** (Is Trigger = ✅) menutupi area bertarung Room 4.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   * Hubungkan pintu dan blockade yang sesuai untuk Room 4.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Quest On Enter**: `"Bersihkan Rintangan Terakhir"`
   * **Quest On Clear**: `"Mendekati Ruang Boss"`
4. Drag prefab **`Enemy_Dwarapala`** sebanyak **5 buah** ke dalam area Room 4.
5. Nonaktifkan kelima objek musuh tersebut di Inspector.
6. Daftarkan kelima objek musuh tersebut ke list **Enemies in Room** pada `Room_4_Manager`.

---

## Bagian C: Integrasi Boss Arena & Yaksa

Setup ini mengintegrasikan bos Yaksa agar pertarungan Boss Fight dapat terpicu secara otomatis.

### Langkah 6: Konfigurasi Stage2BossArena
1. Di Hierarchy scene `Stage2`, buat GameObject kosong di gerbang masuk arena bos → beri nama `BossArena_Yaksa`.
2. Tambahkan komponen **BoxCollider2D**:
   * Centang **Is Trigger** = ✅.
   * Sesuaikan ukuran collider agar melintangi jalan masuk arena (sehingga player pasti memicu trigger ini saat masuk).
3. Tambahkan komponen **`Stage2BossArena.cs`**:
   * **Entry Door**: Seret pintu masuk arena yang akan menutup saat bos aktif.
   * **Next Room Blockades**: Seret portal transisi/pembatas jalan keluar di belakang bos ke list ini.
   * **Guide Particle Prefab**: Tarik prefab `Guide.prefab`.
   * **Boss Object**: Drag prefab instance **`Boss_Yaksa`** yang diletakkan di tengah arena ke slot ini. Pastikan prefab instance di scene diset **Nonaktif (disabled)** secara default.
   * **Boss AI**: Tarik script `Stage2EnemyMovement` yang menempel pada objek `Boss_Yaksa` ke slot ini.
   * **Boss Health UI**: Seret panel UI **`BossHealthPanel`** dari Canvas prefab ke slot ini.
   * **Boss Health Slider**: Seret slider HP bos yang ada di dalam panel tersebut ke slot ini.
   * **Lontar Reward Prefab**: Tarik prefab **`LontarBossDrop.prefab`** dari folder `Assets/Prefab/` ke slot ini.
   * **Lontar Spawn Point**: Buat child object kosong di tengah arena bos (beri nama `LontarSpawnPoint`), lalu seret objek ini ke slot `Lontar Spawn Point` di script.
   * **Quest On Boss**: `"Kalahkan Yaksa"`
   * **Quest On Clear**: `"Ambil Kitab Lontar"`

---

## Bagian D: Penempatan Prasasti Lore & Portal Akhir

Untuk melengkapi eksplorasi roguelite, kita menempatkan Prasasti Lore di area aman dan Portal Keluar di belakang arena bos.

### Langkah 7: Pasang Prasasti Kuno (`LoreInteractable`)
Kita akan meletakkan dua buah prasasti di ruangan aman untuk memberikan lore dan hint kelemahan musuh.

1. **Prasasti Dwarapala (Safe Room 1 - Setelah Room 2)**:
   * Drag prefab **`Prasasti_KepalaKala`** dari `Assets/Prefabs/Stage1/` ke ruangan aman pertama di Stage 2. Rename menjadi `Prasasti_Dwarapala`.
   * Hapus/ganti komponen interactable jika masih mengarah ke data lama. Pastikan memiliki komponen **`LoreInteractable.cs`**.
   * Di Inspector komponen `LoreInteractable`:
     * **Lore Data To Unlock**: Seret aset ScriptableObject **`Lore_Dwarapala`** yang dibuat di Bagian A ke slot ini.
     * **Interact Key**: `E`
2. **Prasasti Yaksa (Safe Room 2 - Setelah Room 4)**:
   * Drag prefab **`Prasasti_KepalaKala`** ke ruangan aman kedua (sebelum Boss Arena). Rename menjadi `Prasasti_Yaksa`.
   * Di Inspector komponen `LoreInteractable`:
     * **Lore Data To Unlock**: Seret aset ScriptableObject **`Lore_Yaksa`** yang dibuat di Bagian A ke slot ini.
     * **Interact Key**: `E`

### Langkah 8: Pasang Portal Transisi Level (`EndPortal`)
1. Drag prefab **`EndPortal.prefab`** (dari folder `Assets/Prefab/`) ke area di belakang arena Boss Yaksa (dekat dengan Lontar Spawn Point).
2. Di Inspector komponen **`EndPortal`**:
   * Pastikan **Is Trigger** pada BoxCollider2D tercentang ✅.
   * **Target Scene Name**: Isi dengan nama scene berikutnya, yaitu **`Stage3`** (atau scene tujuan selanjutnya sesuai Build Settings).
3. Secara default di Inspector, klik centang aktif pada GameObject `EndPortal` agar **Nonaktif (disabled)** saat permainan dimulai. Portal ini akan diaktifkan secara otomatis setelah Lontar jatuh dan diambil oleh Saka.

---

## Bagian E: Progress Tracking Checklist (Stage 2 Scene Integration)

Gunakan checklist di bawah ini untuk memantau progress pengerjaan di Unity Editor. Beri tanda centang `[x]` pada langkah-langkah yang telah selesai disiapkan.

- [ ] **Data Setup**: Buat `Lore_Dwarapala.asset` di folder `Assets/Data/Lores/` (Pastikan ID = `lore_dwarapala`)
- [ ] **Data Setup**: Buat `Lore_Yaksa.asset` di folder `Assets/Data/Lores/` (Pastikan ID = `lore_yaksa`)
- [ ] **HUD Integration**: Ganti `QuestManager` lama di Canvas `Stage2` dengan script `Stage2QuestManager` dan hubungkan referensi UI-nya
- [ ] **Room 1 Setup**: Konfigurasi `Room_1_Manager` dengan trigger collider dan daftarkan 3 prefab `Enemy_Dwarapala` (dinonaktifkan secara default)
- [ ] **Room 2 Setup**: Konfigurasi `Room_2_Manager` dengan trigger collider dan daftarkan 4 prefab `Enemy_Dwarapala` (dinonaktifkan secara default)
- [ ] **Room 3 Setup**: Konfigurasi `Room_3_Manager` dengan trigger collider dan daftarkan 1 prefab `MiniBoss_Dwarapala` + 2 prefab `Enemy_Dwarapala` (dinonaktifkan secara default)
- [ ] **Room 4 Setup**: Konfigurasi `Room_4_Manager` dengan trigger collider dan daftarkan 5 prefab `Enemy_Dwarapala` (dinonaktifkan secara default)
- [ ] **Boss Arena Setup**: Konfigurasi trigger `BossArena_Yaksa` dengan script `Stage2BossArena` dan hubungkan prefab instance `Boss_Yaksa` (dinonaktifkan secara default)
- [ ] **Boss UI Setup**: Hubungkan panel Canvas `BossHealthPanel` dan `BossHealthSlider` ke komponen `Stage2BossArena`
- [ ] **Boss Reward**: Assign prefab `LontarBossDrop` dan spawn point di komponen `Stage2BossArena`
- [ ] **Lore Interaction**: Tempatkan `Prasasti_Dwarapala` di Safe Room 1 dengan script `LoreInteractable` terhubung ke `Lore_Dwarapala`
- [ ] **Lore Interaction**: Tempatkan `Prasasti_Yaksa` di Safe Room 2 dengan script `LoreInteractable` terhubung ke `Lore_Yaksa`
- [ ] **End Transition**: Pasang `EndPortal` di belakang arena bos (dinonaktifkan secara default) dengan target scene `Stage3`
