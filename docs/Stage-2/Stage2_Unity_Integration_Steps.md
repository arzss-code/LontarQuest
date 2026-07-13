# Panduan Integrasi Unity Stage 2: Misi, UI, & Level Setup

Dokumen ini berisi panduan langkah-demi-langkah di Unity Editor untuk merakit sistem Misi, UI, Rintangan, dan transisi Portal di scene `Stage2.unity` berdasarkan dokumen `Stage2_Enemy_Design.md` dan `Stage2_Quest_Task_Implementation.md`.

---

## Bagian 1: Setup HUD & Sistem Misi Hibrida

### Langkah 1: Pasang HUD Canvas
1. Buka scene `Stage2` di Unity Editor.
2. Di Project Window, temukan prefab **`Canvas.prefab`** (terletak di folder `Assets/Prefab/`).
3. Drag `Canvas.prefab` ke Hierarchy scene `Stage2`.
4. Buka struktur anak (*child*) Canvas tersebut, pastikan terdapat:
   - Panel HUD Player (Darah & Stamina).
   - **`QuestPanel`** (berisi teks objektif misi).
   - **`BossHealthPanel`** (berisi slider darah bos).

### Langkah 2: Setup Quest Manager Stage 2
1. Di Hierarchy, klik kanan → **Create Empty** → beri nama `QuestManager`.
2. Tambahkan komponen **`Stage2QuestManager.cs`** ke objek tersebut (Remove component `QuestManager` bawaan jika ada).
3. Di Inspector `Stage2QuestManager`, hubungkan referensi dari Canvas:
   - Drag object `QuestPanel` ke slot **`Quest Panel`**.
   - Drag object text TMP di dalam QuestPanel ke slot **`Objective Text`**.

---

## Bagian 2: Perakitan Kamar Dungeon (Room Setup)

Setiap ruangan bertarung di Stage 2 harus dipasangi trigger pembatas agar pertarungan berjalan otomatis.

### Langkah 3: Setup Trigger Kamar (`Stage2RoomManager`)
1. Buat GameObject kosong baru di tengah ruangan → beri nama `Room_Trigger_1` (sesuaikan nomor ruangan).
2. Tambahkan komponen **BoxCollider2D**:
   - Centang **Is Trigger** = ✅.
   - Atur ukurannya agar mencakup seluruh lantai ruangan yang akan menjadi area tempur.
3. Tambahkan komponen **`Stage2RoomManager.cs`**:
   - **`Entry Door`**: Seret batu/jeruji penutup pintu masuk ke slot ini.
   - **`Exit Doors`**: Seret objek pintu keluar / `BoonDoor` (jika ada) ke dalam list ini.
   - **`Next Room Blockades`**: Seret jeruji/tembok pembatas jalan ke ruangan berikutnya ke list ini.
   - **`Guide Particle Prefab`**: Masukkan prefab `Guide.prefab` (dari `Assets/Prefabs/Stage1/`).
   - **`Enemies in Room`**: Seret semua objek musuh (Dwarapala/MiniBoss) yang ada di dalam ruangan ini ke dalam list.
   - **`Quest On Enter`**: Isi instruksi masuk (misal: *"Bersihkan Koridor Candi"*).
   - **`Quest On Clear`**: Isi instruksi selesai (misal: *"Telusuri Lebih Dalam"*).

---

## Bagian 3: Perakitan Boss Arena & Yaksa

### Langkah 4: Setup Pintu & Trigger Boss Arena
1. Buat GameObject kosong di gerbang arena bos → beri nama `Stage2BossArena`.
2. Tambahkan komponen **BoxCollider2D**:
   - Centang **Is Trigger** = ✅.
   - Sesuaikan ukuran collider agar melintangi jalan masuk arena (sehingga player pasti melewatinya).
3. Tambahkan komponen **`Stage2BossArena.cs`**:
   - **`Entry Door`**: Seret pintu masuk yang akan menutup saat bos aktif.
   - **`Next Room Blockades`**: Seret pembatas jalan keluar di belakang bos.
   - **`Guide Particle Prefab`**: Masukkan prefab `Guide.prefab`.
   - **`Boss Object`**: Seret prefab **`Boss_Yaksa`** Anda di scene ke slot ini.
   - **`Boss AI`**: Seret script `Stage2EnemyMovement` yang menempel pada objek `Boss_Yaksa` ke slot ini.
   - **`UI Setup`**:
     - **`Boss Health UI`**: Seret panel `BossHealthPanel` dari Canvas ke slot ini.
     - **`Boss Health Slider`**: Seret slider HP bos yang ada di dalam panel tersebut ke slot ini.
   - **`Lontar Reward Prefab`**: Masukkan prefab `LontarBossDrop.prefab` (dari `Assets/Prefab/`).
   - **`Lontar Spawn Point`**: Buat child object kosong di tengah arena, seret ke slot ini untuk menentukan titik Lontar terjatuh setelah bos mati.

---

## Bagian 4: Prasasti Lore & Portal Transisi Keluar

### Langkah 5: Pasang Prasasti Kuno (`LoreInteractable`)
1. Di folder `Assets/Prefabs/Stage1/`, drag prefab **`Prasasti_KepalaKala`** (atau arca interaktif serupa) ke sudut ruangan aman Stage 2.
2. Di Inspector komponen **`LoreInteractable`**:
   - Atur data **`Lore Data`** ke ScriptableObject Boons/Lore Stage 2 yang ingin dibuka saat dibaca oleh player Saka.

### Langkah 6: Pasang Portal Transisi Level (`EndPortal`)
1. Drag prefab **`EndPortal.prefab`** (dari folder `Assets/Prefabs/Stage1/` atau `Assets/Prefab/`) ke area di belakang arena Boss Yaksa.
2. Di Inspector komponen **`EndPortal`**:
   - Pastikan **Is Trigger** pada collider-nya tercentang ✅.
   - Set target scene yang dimuat (misal: `InsideTemple` atau `Stage3`).
3. Nonaktifkan GameObject Portal ini di awal (`SetActive(false)`). Portal ini akan otomatis diaktifkan setelah Lontar Bos Yaksa diambil oleh player.

---

## Bagian 5: Checklist Progress Tracking (Stage 2 Integration)

Gunakan checklist ini untuk melacak penyelesaian setup Editor Unity:

- [ ] `Canvas.prefab` terpasang di Hierarchy scene Stage 2.
- [ ] Objek `QuestManager` dibuat dengan script `Stage2QuestManager` ter-assign ke panel Canvas.
- [ ] Trigger Kamar menggunakan `Stage2RoomManager` dengan list musuh terdaftar lengkap.
- [ ] Pintu masuk dan pembatas jalan keluar terhubung ke masing-masing Room Manager.
- [ ] Objek `Boss_Yaksa` dipasang di arena (kondisi default: Nonaktif / Deactivated).
- [ ] Script AI Bos Yaksa terhubung ke slot `Boss AI` di komponen `Stage2BossArena`.
- [ ] UI Boss Health Bar & Slider di Canvas terhubung ke komponen `Stage2BossArena`.
- [ ] Prefab Lontar drop dan spawn point telah di-assign di Boss Arena.
- [ ] Prasasti Lore interaktif diletakkan di area eksplorasi.
- [ ] Portal transisi `EndPortal` dipasang di belakang arena bos (kondisi default: Nonaktif).
- [ ] Pengaturan BGM pertarungan bos telah dikonfigurasi.
