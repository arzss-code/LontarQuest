# Panduan Stage 2 — Tahap 3: Prefab Assembly & Level Setup

Dokumen ini memandu Anda langkah-demi-langkah di Unity Editor untuk merakit Prefab musuh Dwarapala, MiniBoss, proyektil Panah Energi, Bos Yaksa, serta menyusun layout ruangan dan Boss Arena Stage 2 menggunakan script C# yang telah dibuat pada Tahap 2.

---

## Bagian A: Merakit Prefab Proyektil (EnergyArrow)

Buat proyektil laser homing Yaksa terlebih dahulu agar bisa dipasangkan di attack component Yaksa nanti.

### Langkah 1: Buat GameObject Panah
1. Di Hierarchy, klik kanan → **Create Empty** → beri nama `EnergyArrow`.
2. Di Inspector, tambahkan komponen **Sprite Renderer**:
   - **Sprite**: pilih `Yaksa_Arrow` (dari spritesheet Yaksa).
   - **Color**: ubah warnanya ke warna biru cyan terang (contoh: `R: 128, G: 230, B: 255, A: 255`).
   - **Sorting Layer**: `Default` atau layer gameplay Anda.
   - **Order in Layer**: `10` (agar selalu tampil di atas karakter/tembok).
3. Tambahkan komponen **Rigidbody2D**:
   - **Body Type**: `Kinematic` (tidak terpengaruh gravitasi).
   - **Collision Detection**: `Continuous` (mencegah peluru tembus dinding).
4. Tambahkan komponen **CircleCollider2D**:
   - **Is Trigger**: centang ✅.
   - **Offset**: `X: 0.99995, Y: 0.1538459`
   - **Radius**: `0.15`
5. Tambahkan komponen **Trail Renderer**:
   - **Time**: `0.3` (detik jejak bertahan).
   - **Min Vertex Distance**: `0.1`.
   - **Width**: Set nilai input angka ke `0.15`. Buka Curve Editor (klik grafik) lalu tarik keyframe kanan (titik di ujung 1.0) ke bawah hingga bernilai `0.0` agar mengecil lancip.
   - **Color**: Klik kolom warna untuk membuka Gradient Editor:
     - Pin bawah kiri (warna awal): Biru Cyan terang (R: 128, G: 230, B: 255).
     - Pin atas kanan (transparansi akhir): Set `Alpha` ke `0` agar memudar halus.
   - **Material**: Gunakan `Sprites-Default` (default).
   - **Cast Shadows** (di tab Lighting): Set ke `Off` (hemat performa).
   - **Order in Layer** (di tab Additional Settings): Set ke `9` (satu tingkat di bawah sprite panah yang bernilai 10).
6. Tambahkan komponen **EnergyArrow.cs**:
   - Drag komponen **Trail Renderer** yang baru dibuat ke slot `Trail Renderer` di script.
   - **Speed**: `8`.
   - **Lifetime**: `4`.
   - **Homing Strength**: `1.5`.
   - **Max Homing Angle**: `30`.
7. Seret GameObject `EnergyArrow` dari Hierarchy ke folder Project Anda (misal `Assets/Prefabs/`) untuk menjadikannya **Prefab**, lalu hapus objek tersebut dari Hierarchy.

---

## Bagian B: Merakit Prefab Dwarapala (Musuh Kroco & Mini-Boss)

### Langkah 2: Rakit Dwarapala Biasa (Kroco)
1. Drag sprite default Dwarapala (`Dwarapala_IdleFront_0`) ke dalam scene Hierarchy → beri nama `Enemy_Dwarapala`.
2. Atur **Tag** = `"Enemy"` dan **Layer** = `Enemy`.
3. Tambahkan komponen **Sprite Renderer**:
   - **Order in Layer**: `5`.
4. Tambahkan komponen **Animator**:
   - **Controller**: Pilih `DwarapalaController` yang telah dibuat di Tahap 1.
5. Tambahkan komponen **Rigidbody2D**:
   - **Body Type**: `Dynamic`.
   - **Mass**: `1000` (agar kokoh dan tidak mudah terdorong player).
   - **Linear Drag**: `0`.
   - **Angular Drag**: `0`.
   - **Gravity Scale**: `0` (wajib untuk Top-Down).
   - **Freeze Rotation Z**: centang ✅.
6. Tambahkan komponen **BoxCollider2D** (sebagai collider fisik kaki/tubuh):
   - Atur ukurannya agar mencakup area pijakan kaki Dwarapala (misal: `Size X: 0.8, Y: 0.4`, `Offset Y: 0.1`).
7. Tambahkan komponen **CircleCollider2D** (sebagai pemicu trigger deteksi/serangan):
   - **Is Trigger**: centang ✅.
   - **Radius**: `0.6` - `0.8`.
8. Tambahkan komponen logika C# baru:
   - **Stage2EnemyStats.cs**:
     - **Max HP**: `120` (untuk musuh kroco biasa).
     - **Sprite Renderer**: Drag SpriteRenderer musuh ke slot ini.
     - **Damage Color**: `Red` (berkedip merah saat terpukul).
     - **Flash Duration**: `0.15`.
   - **Stage2EnemyMovement.cs**:
     - **Mode**: `Chase`.
     - **Move Speed**: `2.2`.
     - **Detection Radius**: `8`.
     - **Leash Radius**: `12`.
     - **Stop Distance**: `1.2`.
   - **Stage2EnemyAttack.cs**:
     - **Attack Type**: `MeleeAoE`.
     - **Damage**: `15`.
     - **Attack Range**: `1.4`.
     - **Attack Cooldown**: `1.5`.
     - **Target Layer**: `Player` (layer tempat player berada).
     - **Melee Radius**: `1.5`.
     - **Melee Hitbox Offset**: `X: 0.8, Y: 0`.
     - **Attack VFX Prefab**: Pasang visual efek hantaman (debu/slash jika ada, opsional).
   - **Stage2EnemyAnimator.cs**
   - **Stage2AnimationRelay.cs**
9. Buat child GameObject kosong di dalam `Enemy_Dwarapala`:
   - Klik kanan `Enemy_Dwarapala` → **Create Empty** → beri nama `AimPoint`.
   - Geser posisi `AimPoint` ke dada musuh (posisi Y sekitar `0.5` - `0.7`). Ini penting agar panah Saka mengarah ke tengah tubuh musuh.
10. Seret `Enemy_Dwarapala` dari Hierarchy ke folder Project Anda untuk menjadikannya **Prefab**.

### Langkah 3: Buat Prefab MiniBoss Dwarapala
1. Drag prefab `Enemy_Dwarapala` yang baru dibuat ke scene Hierarchy → rename menjadi `MiniBoss_Dwarapala`.
2. Di Inspector, atur **Scale** transform musuh menjadi `1.5` kali lipat di sumbu X dan Y (skala `1.5, 1.5, 1`).
3. Sesuaikan komponen di **Stage2EnemyStats**:
   - **Max HP**: `300` (MiniBoss jauh lebih keras).
4. Sesuaikan komponen di **Stage2EnemyMovement**:
   - **Move Speed**: `1.8` (sedikit lebih lambat karena ukurannya yang besar).
   - **Leash Radius**: `18`.
5. Sesuaikan komponen di **Stage2EnemyAttack**:
   - **Damage**: `25` (hantaman gada MiniBoss sangat sakit!).
   - **Attack Range**: `1.8` (jangkauan lebih jauh karena ukuran gada membesar).
   - **Melee Radius**: `2.2`.
   - **Melee Hitbox Offset**: `X: 1.2, Y: 0`.
6. Simpan sebagai prefab terpisah (`MiniBoss_Dwarapala.prefab`) dengan menyeretnya ke folder Project, lalu hapus kedua objek Dwarapala di scene Hierarchy agar scene tetap bersih.

---

## Bagian C: Merakit Prefab Bos Yaksa (Leak)

### Langkah 4: Perakitan Objek Bos
1. Drag sprite default Yaksa (`Yaksa_IdleFront_0` / sesuai sheet) ke scene → beri nama `Boss_Yaksa`.
2. Atur **Tag** = `"Enemy"` dan **Layer** = `Enemy`.
3. Tambahkan komponen **Animator**:
   - **Controller**: Pilih `YaksaController` yang telah dibuat di Tahap 1.
4. Tambahkan komponen **Rigidbody2D**:
   - **Body Type**: `Dynamic`.
   - **Gravity Scale**: `0`.
   - **Freeze Rotation Z**: centang ✅.
5. Tambahkan komponen **BoxCollider2D** (collider fisik):
   - Sesuaikan dengan ukuran tubuh Yaksa.
6. Tambahkan komponen **CircleCollider2D** (trigger):
   - **Is Trigger**: centang ✅.
   - **Radius**: `0.7`.
7. Buat child GameObject kosong di dalam `Boss_Yaksa`:
   - Child 1: nama `AimPoint` (geser Y ke `0.6` untuk dada).
   - Child 2: nama `FirePoint` (geser ke arah kanan busurnya, misal `X: 0.5, Y: 0.3`).
8. Buat satu child GameObject lagi untuk mengelompokkan sprite visual:
   - Nama: `VisualParent`.
   - Pindahkan komponen `SpriteRenderer` dan `Animator` dari objek utama `Boss_Yaksa` ke dalam `VisualParent` ini (atau biarkan di objek utama tetapi pastikan pivot hover di-set dengan benar. Menggunakan `VisualParent` terpisah sangat direkomendasikan agar goyangan sinus tidak memindahkan posisi collider fisik musuh).
9. Tambahkan komponen logika C# baru ke root `Boss_Yaksa`:
   - **Stage2EnemyStats.cs**:
     - **Max HP**: `350` (HP Bos).
     - **Is Boss**: centang ✅.
     - **Sprite Renderer**: Drag SpriteRenderer (yang kini di visual child) ke slot ini.
   - **Stage2EnemyMovement.cs**:
     - **Mode**: `KeepDistance`.
     - **Move Speed**: `2.5`.
     - **Detection Radius**: `12`.
     - **Leash Radius**: `15`.
     - **Preferred Distance**: `6`.
     - **Retreat Distance**: `3`.
     - **Retreat Speed**: `3.0`.
     - **Visual Parent**: Drag objek `VisualParent` anak ke slot ini agar bos bergoyang naik-turun secara melayang.
     - **Sinusoidal Float Speed**: `2`.
     - **Sinusoidal Float Amount**: `0.15`.
   - **Stage2EnemyAttack.cs**:
     - **Attack Type**: `RangedProjectile`.
     - **Damage**: `15`.
     - **Attack Range**: `8` (jarak tembak maksimal).
     - **Attack Cooldown**: `1.8`.
     - **Target Layer**: `Player`.
     - **Projectile Prefab**: Tarik prefab `EnergyArrow` yang dibuat di Bagian A ke slot ini.
     - **Fire Point**: Tarik child object `FirePoint` ke slot ini.
   - **Stage2EnemyAnimator.cs**
   - **Stage2AnimationRelay.cs**
10. Simpan sebagai prefab `Boss_Yaksa.prefab` di folder project Anda, lalu hapus dari Hierarchy.

---

## Bagian D: Setup Level & Layout Ruangan Stage 2

### Langkah 5: Susun Kamar & Terapkan RoomManager
1. Buka scene Stage 2 Anda.
2. Di setiap ruangan arena pertarungan musuh biasa, pastikan terdapat objek pemicu **RoomManager**:
3. Tempatkan prefab musuh biasa di dalam ruangan tersebut:
   - Spawn beberapa `Enemy_Dwarapala` di titik-titik strategis (sesuai tabel sebaran musuh: Room 1 = 3 Dwarapala, Room 2 = 4 Dwarapala, Room 3 = 1 MiniBoss + 2 Dwarapala, dst).
4. Nonaktifkan musuh secara manual di Inspector (set checklist active GameObject ke ❌) agar mereka mati/nonaktif saat permainan dimulai.
5. Pada komponen **RoomManager** di ruangan tersebut:
   - Drag musuh-musuh yang dinonaktifkan tadi ke dalam list **Enemies In Room**.
   - Konfigurasikan pintu masuk (`entryDoor`), Boon Doors (`exitDoors`), blockade jalan (`nextRoomBlockades`), dan teks quest.
   - Saat Saka masuk ke dalam area trigger ruangan, musuh akan otomatis bangun (`SetActive(true)`) dan pintu terkunci.

---

## Bagian E: Setup Boss Arena Yaksa

### Langkah 6: Konfigurasi Stage2BossArena
1. Di scene Stage 2, temukan area terdalam yang menjadi ruangan Boss Arena.
2. Buat GameObject baru di area tersebut, beri nama `BossArena_Yaksa`.
3. Tambahkan komponen **BoxCollider2D**:
   - **Is Trigger**: centang ✅.
   - Atur ukurannya agar menutupi seluruh gerbang masuk arena.
4. Tambahkan script **Stage2BossArena.cs**:
   - **Entry Door**: Tarik pintu gerbang masuk arena ke slot ini (tembok/jeruji yang menutup jalan masuk).
   - **Next Room Blockades**: Tarik portal keluar / rute jalan ke Stage 3 ke dalam list ini.
   - **Boss Object**: Tarik prefab instance `Boss_Yaksa` yang diletakkan di dalam arena ke slot ini.
   - **Boss AI**: Tarik komponen `Stage2EnemyMovement` yang ada pada instance `Boss_Yaksa` tadi ke slot ini (agar AI bangun saat Saka masuk).
   - **Boss Health UI**: Tarik panel health bar bos (Canvas slider) di layar game ke slot ini.
   - **Boss Health Slider**: Tarik Slider UI-nya ke slot ini.
   - **Lontar Reward Prefab**: Tarik prefab Lontar Boss Drop ke slot ini.
   - **Lontar Spawn Point**: Pasang penanda posisi jatuhnya lontar setelah Yaksa kalah.
   - **Guide Particle Prefab**: Tarik prefab kunang-kunang penunjuk rute jalan.
5. Pastikan instance `Boss_Yaksa` di dalam arena di-set nonaktif (`SetActive(false)`) secara default di Inspector.
6. Simpan scene. Lakukan uji coba tekan **Play**!
