# Panduan Stage 2 — Tahap 3: Prefab Assembly

Dokumen ini memandu Anda langkah-demi-langkah di Unity Editor untuk merakit Prefab musuh Dwarapala, MiniBoss, proyektil Panah Energi, dan Bos Yaksa menggunakan script C# yang telah dibuat pada Tahap 2.

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
7. Seret GameObject `EnergyArrow` dari Hierarchy ke folder Project Anda (misal `Assets/Prefab/`) untuk menjadikannya **Prefab**, lalu hapus objek tersebut dari Hierarchy.

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
     - **Leash Type**: Pilih tipe pembatas (`Radius` atau `Box`).
     - **Leash Radius**: `12` (jika menggunakan `Radius`).
     - **Leash Box Size**: `X: 10, Y: 10` (jika menggunakan `Box`).
     - **Leash Offset**: `X: 0, Y: 0` (untuk menggeser posisi pusat pembatas jika diperlukan).
     - **Stop Distance**: `X: 1.2, Y: 1.8` (jarak berhenti disesuaikan per sumbu untuk mengatasi hambatan fisik).
   - **Stage2EnemyAttack.cs**:
     - **Attack Type**: `MeleeAoE`.
     - **Damage**: `15`.
     - **Attack Range**: `X: 1.5, Y: 2.0` (jangkauan serang per sumbu).
     - **Attack Cooldown**: `1.5`.
     - **Target Layer**: `Player` (layer tempat player berada).
     - **Melee Radius**: `1.5`.
     - **Melee Hitbox Offset**: `X: 0.8, Y: 0.5`.
     - **Body Center Offset**: `X: 0, Y: 0.7` (pusat badan untuk perhitungan offset).
     - **Use Heavy Knockback**: centang ✅ (menggunakan knockback berat khusus).
     - **Heavy Knockback Force**: `22` (dorongan kuat).
     - **Heavy Knockback Duration**: `0.35` (durasi pentalan).
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
4. Sesuaikan komponen **Stage2EnemyMovement.cs**:
   - **Move Speed**: `1.8` (sedikit lebih lambat karena ukurannya yang besar).
   - **Stop Distance**: `X: 1.8, Y: 2.7` (di-scale 1.5x dari Dwarapala biasa).
   - **Leash Type**: Pilih tipe pembatas (`Radius` atau `Box`).
   - **Leash Radius**: `18` (jika menggunakan `Radius`).
   - **Leash Box Size**: `X: 15, Y: 15` (jika menggunakan `Box`).
   - **Leash Offset**: `X: 0, Y: 0`.
5. Sesuaikan komponen di **Stage2EnemyAttack**:
   - **Damage**: `25` (hantaman gada MiniBoss sangat sakit!).
   - **Attack Range**: `X: 2.2, Y: 3.0` (di-scale 1.5x).
   - **Melee Radius**: `2.2`.
   - **Melee Hitbox Offset**: `X: 1.2, Y: 0.75` (di-scale 1.5x).
   - **Body Center Offset**: `X: 0, Y: 1.05` (di-scale 1.5x).
   - **Heavy Knockback Force**: `28` (terpental sangat jauh).
   - **Heavy Knockback Duration**: `0.45` (durasi terpental lebih lama).
6. Simpan sebagai prefab terpisah (`MiniBoss_Dwarapala.prefab`) dengan menyeretnya ke folder Project, lalu hapus kedua objek Dwarapala di scene Hierarchy agar scene tetap bersih.

### Langkah 3a: Pengaturan Batas Pergerakan (Leash Boundary & Offset) di Inspector

Anda dapat mengatur bagaimana musuh membatasi area pengejaran/patrolinya untuk mencegah musuh keluar terlalu jauh:

1. Cari parameter **Leash Settings (Batas Pergerakan)** di komponen **Stage2EnemyMovement.cs**.
2. Atur opsi pada **Leash Type**:
   - **Radius**: Musuh dibatasi oleh lingkaran imajiner dengan radius **Leash Radius**. Pilihan ini sangat cocok untuk ruangan terbuka luas.
   - **Box**: Musuh dibatasi oleh kotak imajiner dengan ukuran **Leash Box Size** (lebar X, tinggi Y). Sangat cocok untuk ruangan kamar tertutup agar musuh tidak tembus/keluar dari koridor atau pintu ruangan.
3. Atur nilai **Leash Offset** (Vector2) jika pusat area batas pergerakan musuh tidak berada tepat pada posisi spawn awal musuh (misal: jika musuh spawn di pojok kamar tapi harus menjaga seluruh ruangan).
4. Di Scene View Unity, saat objek musuh dipilih, batas pergerakan visual berwarna **Hijau** akan ditampilkan secara otomatis (lingkaran jika memilih Radius, kotak jika memilih Box) dan bergeser secara akurat mengikuti nilai **Leash Offset** untuk memudahkan penyesuaian.

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
     - **Leash Type**: Pilih tipe pembatas (`Radius` or `Box`).
     - **Leash Radius**: `15` (jika menggunakan `Radius`).
     - **Leash Box Size**: `X: 12, Y: 12` (jika menggunakan `Box`).
     - **Leash Offset**: `X: 0, Y: 0`.
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

## Bagian D: Checklist Progress Tracking (Prefab Assembly)

Gunakan checklist ini untuk melacak penyelesaian perakitan prefab di Unity Editor:

- [X] Prefab `EnergyArrow` berhasil dirakit dengan SpriteRenderer, Rigidbody2D, CircleCollider2D, TrailRenderer, dan script `EnergyArrow.cs` terhubung.
- [X] Prefab `Enemy_Dwarapala` berhasil dirakit dengan SpriteRenderer, Animator, Rigidbody2D, BoxCollider2D, CircleCollider2D, AimPoint child, dan stats/movement/attack/animator/relay script terhubung.
- [X] Prefab `MiniBoss_Dwarapala` berhasil dibuat dari duplikasi `Enemy_Dwarapala` dengan skala 1.5x, HP 300, dan stats yang disesuaikan.
- [X] Prefab `Boss_Yaksa` berhasil dirakit dengan visual terpisah pada `VisualParent`, Animator, Rigidbody2D, colliders, AimPoint & FirePoint child, serta script status melayang/stats/attack terhubung.
