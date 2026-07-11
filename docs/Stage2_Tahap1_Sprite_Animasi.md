# Panduan Tahap 1: Setup Sprite & Animasi di Unity Editor

Panduan langkah-per-langkah untuk menyiapkan sprite, animation clip, dan animator controller untuk monster Stage 2 (Dwarapala & Yaksa) di Unity Editor.

> **Referensi desain:** Lihat `docs/Stage2_Enemy_Design.md` untuk spesifikasi lengkap.

---

## Persiapan Awal

### Langkah 0: Buat Folder Kerja

1. Di **Project Window**, navigasi ke `Assets/Arts/Enemies/`
2. Klik kanan pada folder `Stage2-Dwarapala` → **Create** → **Folder** → beri nama `Animations`
3. Klik kanan pada folder `Stage2-MakaraOrYaksa` → **Create** → **Folder** → beri nama `Animations`

**Struktur folder yang diharapkan:**

```
Assets/Arts/Enemies/
├── Stage2-Dwarapala/
│   ├── Sprites/
│   │   ├── Idle.png
│   │   ├── Walk.png
│   │   └── AttackAndDeath.png
│   └── Animations/          ← BARU
│
└── Stage2-MakaraOrYaksa/
    ├── Sprites/
    │   └── Sprites.png
    └── Animations/           ← BARU
```

---

## Bagian A: Slice Spritesheet Dwarapala

---

### Langkah 1: Konfigurasi Import Idle.png

1. Di **Project Window**, klik file `Assets/Arts/Enemies/Stage2-Dwarapala/Sprites/Idle.png`
2. Di **Inspector**, pastikan pengaturan berikut:

| Pengaturan                | Nilai                                                                     |
| ------------------------- | ------------------------------------------------------------------------- |
| **Texture Type**    | Sprite (2D and UI)                                                        |
| **Sprite Mode**     | **Multiple**                                                        |
| **Pixels Per Unit** | **250** ← *disesuaikan agar ukuran sprite proporsional di scene* |
| **Filter Mode**     | Point (no filter) ←*jika pixel art*; atau Bilinear                     |
| **Compression**     | None*(untuk kualitas terbaik)*                                          |

3. Klik **Apply** di bawah Inspector

### Langkah 2: Slice Idle.png di Sprite Editor

1. Masih di Inspector `Idle.png`, klik tombol **Sprite Editor**
2. Jendela Sprite Editor terbuka, Anda akan melihat gambar 3 pose Dwarapala
3. Klik menu **Slice** (ikon gunting di toolbar atas)
4. Atur parameter slice:

| Parameter        | Nilai           |
| ---------------- | --------------- |
| **Type**   | Automatic       |
| **Pivot**  | Bottom          |
| **Method** | Delete Existing |

5. Klik tombol **Slice**
6. Unity akan otomatis membuat 3 kotak seleksi (rect) mengelilingi setiap pose

**Verifikasi hasil slice** — pastikan ada tepat **3 rect** (bukan lebih):

| # | Nama       | Deskripsi Visual                                                              |
| - | ---------- | ----------------------------------------------------------------------------- |
| 0 | `Idle_0` | Dwarapala menghadap**depan** (muka terlihat, pegang gada horizontal)    |
| 1 | `Idle_1` | Dwarapala menghadap**belakang** (rambut/mahkota terlihat dari belakang) |
| 2 | `Idle_2` | Dwarapala menghadap**samping kanan** (profil samping, gada di tangan)   |

7. **Rename sprite** (opsional tapi disarankan):

   - Klik rect pertama → di panel bawah, ubah **Name** menjadi `Idle_Front`
   - Klik rect kedua → **Name**: `Idle_Back`
   - Klik rect ketiga → **Name**: `Idle_Side`
8. Pastikan **Pivot** setiap rect = **Bottom** (agar kaki karakter jadi titik jangkar)
9. Klik **Apply** (pojok kanan atas Sprite Editor) lalu tutup Sprite Editor

---

### Langkah 3: Slice Walk.png di Sprite Editor

1. Klik file `Walk.png` di Project Window
2. Di Inspector, set **Sprite Mode** = **Multiple** → klik **Apply**
3. Klik **Sprite Editor**
4. Klik **Slice** → Type: **Automatic**, Pivot: **Bottom** → klik **Slice**
5. Pastikan ada tepat **12 rect** (3 baris × 4 kolom)

**Verifikasi & Rename:**

| Baris                  | Rect    | Nama Baru       | Arah                              |
| ---------------------- | ------- | --------------- | --------------------------------- |
| **Baris Atas**   | Walk_0  | `WalkFront_0` | Jalan menghadap depan, frame 1    |
|                        | Walk_1  | `WalkFront_1` | Jalan menghadap depan, frame 2    |
|                        | Walk_2  | `WalkFront_2` | Jalan menghadap depan, frame 3    |
|                        | Walk_3  | `WalkFront_3` | Jalan menghadap depan, frame 4    |
| **Baris Tengah** | Walk_4  | `WalkBack_0`  | Jalan menghadap belakang, frame 1 |
|                        | Walk_5  | `WalkBack_1`  | Jalan menghadap belakang, frame 2 |
|                        | Walk_6  | `WalkBack_2`  | Jalan menghadap belakang, frame 3 |
|                        | Walk_7  | `WalkBack_3`  | Jalan menghadap belakang, frame 4 |
| **Baris Bawah**  | Walk_8  | `WalkSide_0`  | Jalan ke samping, frame 1         |
|                        | Walk_9  | `WalkSide_1`  | Jalan ke samping, frame 2         |
|                        | Walk_10 | `WalkSide_2`  | Jalan ke samping, frame 3         |
|                        | Walk_11 | `WalkSide_3`  | Jalan ke samping, frame 4         |

> **Catatan**: Baris bawah menampilkan jalan ke arah **kiri**. Untuk jalan ke kanan, sprite yang sama akan di-flip (`flipX`) melalui script nanti.

6. Pastikan **Pivot** semua rect = **Bottom**
7. Klik **Apply**, tutup Sprite Editor

---

### Langkah 4: Slice AttackAndDeath.png (⚠️ Perlu Perhatian Khusus)

Spritesheet ini lebih rumit karena mengandung **frame noise kecil** yang harus dihapus manual.

1. Klik file `AttackAndDeath.png` di Project Window
2. Di Inspector, set **Sprite Mode** = **Multiple** → klik **Apply**
3. Klik **Sprite Editor**
4. Klik **Slice** → Type: **Automatic**, Pivot: **Bottom**, **Minimum Size**: isi **100** (untuk memfilter noise kecil) → klik **Slice**
5. **Hapus rect noise yang tersisa:**

   - Lihat semua rect yang muncul
   - Jika masih ada rect kecil (< 100 pixel width/height, bukan karakter utama), klik rect tersebut lalu tekan tombol **Delete**
   - Hanya sisakan rect yang berisi **sprite karakter utama**
6. **Pastikan tersisa 10 rect frame utama** dengan urutan dan rename berikut:

**Baris Atas — Death (4 frame):**

| Rect | Nama Baru   | Deskripsi Visual                                           |
| ---- | ----------- | ---------------------------------------------------------- |
| #0   | `Death_0` | Dwarapala terhuyung, posisi duduk, retak biru mulai muncul |
| #1   | `Death_1` | Jatuh membungkuk, retakan melebar                          |
| #2   | `Death_2` | Ambruk telungkup, badan retak parah                        |
| #3   | `Death_3` | Hancur total di tanah, tersisa puing                       |

**Baris Tengah — Attack Front (3 frame):**

| Rect | Nama Baru      | Deskripsi Visual                                                   |
| ---- | -------------- | ------------------------------------------------------------------ |
| #4   | `AtkFront_0` | Berdiri tegap menghadap depan, gada siap horizontal                |
| #5   | `AtkFront_1` | Gada diangkat ke atas (posisi 3/4 samping)                         |
| #6   | `AtkFront_2` | Gada dihantamkan ke tanah, efek retakan (menghadap depan, condong) |

**Baris Bawah — Attack Side (3 frame):**

| Rect | Nama Baru     | Deskripsi Visual                                         |
| ---- | ------------- | -------------------------------------------------------- |
| #7   | `AtkSide_0` | Gada diangkat tinggi ke atas (posisi depan, badan lebar) |
| #8   | `AtkSide_1` | Gada diayunkan ke bawah (posisi samping, efek gerakan)   |
| #9   | `AtkSide_2` | Gada menghantam tanah, efek impact biru bercahaya        |

7. Pastikan **Pivot** semua rect = **Bottom**
8. Klik **Apply**, tutup Sprite Editor

---

## Bagian B: Slice Spritesheet Yaksa

---

### Langkah 5: Slice Sprites.png (Yaksa)

1. Klik file `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Sprites/Sprites.png`
2. Di Inspector, set **Sprite Mode** = **Multiple** → klik **Apply**
3. Klik **Sprite Editor**
4. Klik **Slice** → Type: **Automatic**, Pivot: **Bottom**, **Minimum Size**: **80** → klik **Slice**
5. Hapus rect noise (jika ada rect yang hanya berisi panah/garis tanpa karakter, hapus — kecuali sprite panah energi yang akan kita gunakan)
6. **Identifikasi dan rename rect berdasarkan posisi visual:**

**Baris Atas — Walk / Semua Arah (8 frame):**

Seluruh baris atas adalah animasi **walk** untuk 3 arah. Untuk **idle**, gunakan clip walk yang sama (frame pertama atau FPS lebih lambat).

| Grup | Rect | Nama Baru | Deskripsi |
|---|---|---|---|
| **Grup Kiri** (2 frame) | #0 | `Yaksa_WalkFront_0` | Jalan menghadap depan, sayap terbuka, melayang |
| | #1 | `Yaksa_WalkFront_1` | Jalan menghadap depan, variasi posisi |
| **Grup Tengah** (4 frame) | #2 | `Yaksa_WalkSide_0` | Jalan menghadap samping, melayang |
| | #3 | `Yaksa_WalkSide_1` | Jalan menghadap samping, variasi |
| | #4 | `Yaksa_WalkSide_2` | Jalan menghadap samping, variasi |
| | #5 | `Yaksa_WalkSide_3` | Jalan menghadap samping, variasi |
| **Grup Kanan** (3 frame) | #6 | `Yaksa_WalkBack_0` | Jalan membelakangi kamera (tampak belakang) |
| | #7 | `Yaksa_WalkBack_1` | Jalan membelakangi kamera, variasi |
| | #8 | `Yaksa_WalkBack_2` | Jalan membelakangi kamera, variasi |

> **Catatan Idle**: Tidak ada sprite idle terpisah. Di Animator Controller, state Idle akan menggunakan clip Walk yang sama dengan **Speed parameter = 0** (script mengatur FPS lebih lambat atau hanya menampilkan frame pertama).

**Baris Tengah — Shoot (7 frame):**

| Rect | Nama Baru         | Deskripsi                                                                    |
| ---- | ----------------- | ---------------------------------------------------------------------------- |
| #8   | `Yaksa_Shoot_0` | Busur terangkat, panah energi mulai terbentuk                                |
| #9   | `Yaksa_Shoot_1` | Tali busur ditarik, energi mengumpul                                         |
| #10  | `Yaksa_Shoot_2` | Busur tertarik penuh                                                         |
| #11  | `Yaksa_Arrow`   | **Sprite panah energi saja** (proyektil yang terbang) — pisahkan ini! |
| #12  | `Yaksa_Shoot_3` | Follow-through setelah lepas panah (badan besar)                             |
| #13  | `Yaksa_Shoot_4` | Recovery posisi 1                                                            |
| #14  | `Yaksa_Shoot_5` | Recovery posisi 2                                                            |

> **Penting**: Frame panah energi panjang (#11 `Yaksa_Arrow`) akan digunakan terpisah sebagai sprite untuk prefab `EnergyArrow`. Jangan campur ke Animation Clip Shoot.

**Baris Bawah — Death / Defeated (6 frame):**

| Rect | Nama Baru         | Deskripsi                               |
| ---- | ----------------- | --------------------------------------- |
| #15  | `Yaksa_Death_0` | Yaksa terhuyung, kristal energi meredup |
| #16  | `Yaksa_Death_1` | Terhuyung lebih parah                   |
| #17  | `Yaksa_Death_2` | Tubuh mulai hancur dari bawah           |
| #18  | `Yaksa_Death_3` | Ledakan partikel energi, tubuh retak    |
| #19  | `Yaksa_Death_4` | Partikel menghilang, sisa energi        |
| #20  | `Yaksa_Death_5` | Hilang total, hanya jejak cahaya biru   |

7. Pastikan **Pivot** semua rect = **Bottom**
8. Klik **Apply**, tutup Sprite Editor

**Hasil akhir setelah slice:**

- Dari `Idle.png`: 3 sprite
- Dari `Walk.png`: 12 sprite
- Dari `AttackAndDeath.png`: 10 sprite
- Dari `Sprites.png` (Yaksa): ~21 sprite (termasuk `Yaksa_Arrow`)

---

## Bagian C: Membuat Animation Clips Dwarapala

---

### Langkah 6: Buat GameObject Sementara untuk Membuat Anim

1. Buka scene apa saja (atau buat scene baru sementara: **File** → **New Scene**)
2. Di **Hierarchy**, klik kanan → **Create Empty** → beri nama `DwarapalaTemp`
3. Pilih `DwarapalaTemp`, di Inspector klik **Add Component** → **Sprite Renderer**
4. Set sprite ke `Idle_Front` (dari Idle.png)
5. Buka **Window** → **Animation** → **Animation** (bukan Animator!)

> Jendela Animation akan muncul di bawah. Pastikan `DwarapalaTemp` masih terpilih di Hierarchy.

### Langkah 7: Buat Clip `Dwarapala_IdleFront.anim`

1. Di jendela Animation, klik dropdown **"Create"** (atau tombol Create jika belum ada clip)
2. Simpan sebagai: `Assets/Arts/Enemies/Stage2-Dwarapala/Animations/Dwarapala_IdleFront.anim`
3. Unity otomatis membuat **Animator Controller** juga — ini nanti akan kita ganti, jadi biarkan dulu
4. Di timeline Animation, klik **Add Property** → **Sprite Renderer** → **Sprite** → klik **+**
5. Pada frame `0:00`, sprite seharusnya sudah `Idle_Front`
6. Karena ini idle 1 frame (diam), **tidak perlu menambah keyframe lain**
7. Set **Samples** (FPS) di pojok kiri atas timeline = **1**
8. Pastikan **Loop Time** aktif:

   - Di Project Window, klik file `Dwarapala_IdleFront.anim`
   - Di Inspector, centang ✅ **Loop Time**

### Langkah 8: Buat Clip `Dwarapala_IdleBack.anim`

1. Di jendela Animation (dengan `DwarapalaTemp` masih terpilih), klik **dropdown nama clip** (yang bertuliskan "Dwarapala_IdleFront") → klik **Create New Clip...**
2. Simpan sebagai: `Dwarapala_IdleBack.anim`
3. **Add Property** → **Sprite Renderer** → **Sprite**
4. Pada frame `0:00`, drag sprite `Idle_Back` (dari Project window) ke value Sprite di keyframe
5. Set Samples = **1**, centang **Loop Time**

### Langkah 9: Buat Clip `Dwarapala_IdleRight.anim` dan `Dwarapala_IdleLeft.anim`

Dwarapala default menghadap ke **Kanan** pada sprite `Idle_Side`. Kita akan membuat dua clip terpisah:

**`Dwarapala_IdleRight.anim` (Arah Kanan):**
1. **Create New Clip** → simpan `Dwarapala_IdleRight.anim`
2. Set sprite = `Idle_Side` pada frame 0
3. Samples = **1**, Loop Time = ✅

**`Dwarapala_IdleLeft.anim` (Arah Kiri — Menggunakan Flip X):**
4. **Create New Clip** → simpan `Dwarapala_IdleLeft.anim`
5. Set sprite = `Idle_Side` pada frame 0
6. Klik tombol **Add Property** di jendela Animation → **Sprite Renderer** → **Flip X** → klik **+**
7. Di frame `0:00`, di jendela Inspector Sprite Renderer, centang/aktifkan ✅ **Flip X**
8. Samples = **1**, Loop Time = ✅

> **Tip Penting `flipX` di Animasi:** Unity mungkin akan otomatis membuat keyframe kedua di akhir timeline. Pastikan nilai `flipX` di keyframe kedua juga tercentang, atau hapus keyframe kedua tersebut agar status Flip X tetap aktif sepanjang durasi clip.

### Langkah 10: Buat Clip `Dwarapala_WalkFront.anim`

1. **Create New Clip** → simpan `Dwarapala_WalkFront.anim`
2. **Add Property** → **Sprite Renderer** → **Sprite**
3. Set **Samples** = **8** (fps)
4. Masukkan 4 keyframe:

| Frame | Waktu | Sprite          |
| ----- | ----- | --------------- |
| 0     | 0:00  | `WalkFront_0` |
| 1     | 0:01  | `WalkFront_1` |
| 2     | 0:02  | `WalkFront_2` |
| 3     | 0:03  | `WalkFront_3` |

> **Cara memasukkan keyframe**: Klik posisi frame di timeline → di baris "Sprite", klik bulatan keyframe → ganti sprite di value field. **ATAU** expand sprite property di Project Window dan drag langsung ke timeline.

> **Cara cepat**: Pilih keempat sprite (`WalkFront_0` sampai `WalkFront_3`) di Project Window sekaligus (Shift+klik), lalu drag langsung ke jendela Animation timeline.

5. Centang ✅ **Loop Time** di Inspector clip

### Langkah 11: Buat Clip Walk untuk Arah Lainnya

Ulangi Langkah 10 untuk:

**`Dwarapala_WalkBack.anim`** (Samples: 8, Loop: ✅)

| Frame | Sprite         |
| ----- | -------------- |
| 0     | `WalkBack_0` |
| 1     | `WalkBack_1` |
| 2     | `WalkBack_2` |
| 3     | `WalkBack_3` |

**`Dwarapala_WalkRight.anim`** (Samples: 8, Loop: ✅)

| Frame | Sprite         |
| ----- | -------------- |
| 0     | `WalkSide_0` |
| 1     | `WalkSide_1` |
| 2     | `WalkSide_2` |
| 3     | `WalkSide_3` |

**`Dwarapala_WalkLeft.anim`** (Samples: 8, Loop: ✅ — Menggunakan Flip X)

1. Buat clip baru `Dwarapala_WalkLeft.anim`
2. Drag sprite `WalkSide_0` sampai `WalkSide_3` seperti clip WalkRight
3. Klik **Add Property** → **Sprite Renderer** → **Flip X** → klik **+**
4. Pada frame `0:00`, centang/aktifkan ✅ **Flip X** di Inspector
5. Pastikan status **Flip X** tetap aktif sepanjang timeline (centang di seluruh keyframe)

### Langkah 12: Buat Clip `Dwarapala_AttackFront.anim`

1. **Create New Clip** → simpan `Dwarapala_AttackFront.anim`
2. **Add Property** → **Sprite Renderer** → **Sprite**
3. Set **Samples** = **8**
4. Masukkan 3 keyframe:

| Frame | Sprite         | Deskripsi                    |
| ----- | -------------- | ---------------------------- |
| 0     | `AtkFront_0` | Posisi siap, gada horizontal |
| 1     | `AtkFront_1` | Gada diangkat                |
| 2     | `AtkFront_2` | Hantam tanah                 |

5. **JANGAN centang Loop Time** (Loop: ❌) — serangan hanya main sekali
6. **Tambahkan Animation Event** (sangat penting untuk gameplay!):

   - Pindahkan playhead ke **frame 2** (frame terakhir, saat gada menyentuh tanah)
   - Klik kanan pada timeline di posisi playhead → **Add Animation Event** (atau klik ikon event di toolbar)
   - Di Inspector event yang muncul, set **Function** = `OnAttackHit`
   - Tambah event lagi di posisi setelah frame 2 (atau di frame 2 juga) → Function = `OnAttackEnd`

> **Catatan Animation Event:**
>
> - `OnAttackHit` → saat dipanggil, script `Stage2AnimationRelay` akan menjalankan logika damage AoE
> - `OnAttackEnd` → memberitahu script bahwa animasi selesai, kembalikan state ke Movement

### Langkah 13: Buat Clip `Dwarapala_AttackRight.anim` dan `Dwarapala_AttackLeft.anim`

**`Dwarapala_AttackRight.anim`** (Samples: 8, Loop: ❌)
1. **Create New Clip** → simpan `Dwarapala_AttackRight.anim`
2. Masukkan keyframe:

| Frame | Sprite        |
| ----- | ------------- |
| 0     | `AtkSide_0` |
| 1     | `AtkSide_1` |
| 2     | `AtkSide_2` |

3. **Animation Events**:
   - Frame 2 → `OnAttackHit`
   - Frame 2 (setelah hit) → `OnAttackEnd`

**`Dwarapala_AttackLeft.anim`** (Samples: 8, Loop: ❌ — Menggunakan Flip X)
4. **Create New Clip** → simpan `Dwarapala_AttackLeft.anim`
5. Masukkan keyframe `AtkSide_0` sampai `AtkSide_2`
6. Klik **Add Property** → **Sprite Renderer** → **Flip X**
7. Pada frame `0:00`, centang/aktifkan ✅ **Flip X** di Inspector
8. **Animation Events** (sama seperti AttackRight):
   - Frame 2 → `OnAttackHit`
   - Frame 2 (setelah hit) → `OnAttackEnd`

### Langkah 14: Buat Clip `Dwarapala_Death.anim`

1. **Create New Clip** → simpan `Dwarapala_Death.anim`
2. Samples = **6** (lebih lambat, dramatis), Loop = ❌
3. Masukkan 4 keyframe:

| Frame | Sprite      |
| ----- | ----------- |
| 0     | `Death_0` |
| 1     | `Death_1` |
| 2     | `Death_2` |
| 3     | `Death_3` |

4. **Animation Event** di frame terakhir (frame 3):
   - Function = `OnDeathEnd`

> Clip ini tidak perlu `OnAttackHit`. Event `OnDeathEnd` akan memberitahu `Stage2EnemyStats` untuk menghancurkan (Destroy) GameObject.

### Langkah 15: Verifikasi Semua Clip Dwarapala

Di Project Window, navigasi ke `Assets/Arts/Enemies/Stage2-Dwarapala/Animations/`.

Pastikan Anda memiliki **12 file `.anim`**:

| # | File | Loop | FPS | Keterangan |
|---|---|---|---|---|
| 1 | `Dwarapala_IdleFront.anim` | ✅ | 1 | |
| 2 | `Dwarapala_IdleBack.anim` | ✅ | 1 | |
| 3 | `Dwarapala_IdleLeft.anim` | ✅ | 1 | flipX = ✅ |
| 4 | `Dwarapala_IdleRight.anim` | ✅ | 1 | flipX = ❌ |
| 5 | `Dwarapala_WalkFront.anim` | ✅ | 8 | |
| 6 | `Dwarapala_WalkBack.anim` | ✅ | 8 | |
| 7 | `Dwarapala_WalkLeft.anim` | ✅ | 8 | flipX = ✅ |
| 8 | `Dwarapala_WalkRight.anim` | ✅ | 8 | flipX = ❌ |
| 9 | `Dwarapala_AttackFront.anim` | ❌ | 8 | event hit/end |
| 10| `Dwarapala_AttackLeft.anim` | ❌ | 8 | flipX = ✅, event hit/end |
| 11| `Dwarapala_AttackRight.anim`| ❌ | 8 | flipX = ❌, event hit/end |
| 12| `Dwarapala_Death.anim` | ❌ | 6 | event death end |

**Untuk memverifikasi clip secara visual:**

1. Klik satu file `.anim` di Project Window
2. Di Inspector, Anda bisa melihat preview animasi
3. Atau pilih `DwarapalaTemp` di Hierarchy, buka Animation window, dan pilih clip dari dropdown untuk preview

---

## Bagian D: Membuat Animation Clips Yaksa

---

### Langkah 16: Buat GameObject Sementara Yaksa

1. Di Hierarchy, klik kanan → **Create Empty** → beri nama `YaksaTemp`
2. **Add Component** → **Sprite Renderer** → set sprite ke `Yaksa_WalkFront_0`
3. Pastikan `YaksaTemp` terpilih, buka jendela Animation

### Langkah 17: Buat Clip Walk Yaksa (4 Arah)

Semua sprite baris atas adalah **walk**. Buat 4 clip walk:

**`Yaksa_WalkFront.anim`** (Samples: 10, Loop: ✅)

| Frame | Sprite                |
| ----- | --------------------- |
| 0     | `Yaksa_WalkFront_0` |
| 1     | `Yaksa_WalkFront_1` |

**`Yaksa_WalkBack.anim`** (Samples: 10, Loop: ✅)

| Frame | Sprite               |
| ----- | -------------------- |
| 0     | `Yaksa_WalkBack_0` |
| 1     | `Yaksa_WalkBack_1` |
| 2     | `Yaksa_WalkBack_2` |

**`Yaksa_WalkLeft.anim`** (Samples: 10, Loop: ✅ — Default Arah Kiri)

| Frame | Sprite               |
| ----- | -------------------- |
| 0     | `Yaksa_WalkSide_0` |
| 1     | `Yaksa_WalkSide_1` |
| 2     | `Yaksa_WalkSide_2` |
| 3     | `Yaksa_WalkSide_3` |

**`Yaksa_WalkRight.anim`** (Samples: 10, Loop: ✅ — Menggunakan Flip X)
1. Buat clip baru `Yaksa_WalkRight.anim`
2. Drag sprite `Yaksa_WalkSide_0` sampai `_3` seperti WalkLeft
3. Klik **Add Property** → **Sprite Renderer** → **Flip X**
4. Di frame `0:00`, centang/aktifkan ✅ **Flip X** di Inspector

---

### Langkah 17b: Idle Yaksa — Reuse Clip Walk

Yaksa **tidak memiliki sprite idle terpisah**. Untuk idle, gunakan clip walk yang sama:

**`Yaksa_IdleFront.anim`** (Samples: **4** ← lebih lambat dari walk, Loop: ✅)

| Frame | Sprite                |
| ----- | --------------------- |
| 0     | `Yaksa_WalkFront_0` |
| 1     | `Yaksa_WalkFront_1` |

**`Yaksa_IdleBack.anim`** (Samples: **4**, Loop: ✅)

| Frame | Sprite               |
| ----- | -------------------- |
| 0     | `Yaksa_WalkBack_0` |
| 1     | `Yaksa_WalkBack_1` |
| 2     | `Yaksa_WalkBack_2` |

**`Yaksa_IdleLeft.anim`** (Samples: **4**, Loop: ✅ — Default Arah Kiri)

| Frame | Sprite               |
| ----- | -------------------- |
| 0     | `Yaksa_WalkSide_0` |
| 1     | `Yaksa_WalkSide_1` |
| 2     | `Yaksa_WalkSide_2` |
| 3     | `Yaksa_WalkSide_3` |

**`Yaksa_IdleRight.anim`** (Samples: **4**, Loop: ✅ — Menggunakan Flip X)
1. Buat clip baru `Yaksa_IdleRight.anim`
2. Drag sprite `Yaksa_WalkSide_0` sampai `_3` seperti IdleLeft
3. Klik **Add Property** → **Sprite Renderer** → **Flip X**
4. Di frame `0:00`, centang/aktifkan ✅ **Flip X** di Inspector

### Langkah 18: Buat Clip Shoot Yaksa (Kiri dan Kanan)

Yaksa default menghadap ke **Kiri** saat menembak. Kita buat dua clip terpisah:

**`Yaksa_ShootLeft.anim`** (Samples: 10, Loop: ❌ — Default Kiri)

| Frame | Sprite            | Keterangan |
| ----- | ----------------- | ---------- |
| 0     | `Yaksa_Shoot_0` | Angkat busur |
| 1     | `Yaksa_Shoot_1` | Tarik busur |
| 2     | `Yaksa_Shoot_3` | Tarik busur penuh |
| 3     | `Yaksa_Shoot_4` | Lepas panah |
| 4     | `Yaksa_Shoot_5` | Recovery 1 |
| 5     | `Yaksa_Shoot_6` | Recovery 2 |

4. **Animation Events:**
   - **Frame 3** (saat panah dilepas) → Function: `OnShootProjectile`
   - **Frame 5** (akhir recovery) → Function: `OnAttackEnd`

**`Yaksa_ShootRight.anim`** (Samples: 10, Loop: ❌ — Menggunakan Flip X)
5. Buat clip baru `Yaksa_ShootRight.anim`
6. Masukkan keyframe `Yaksa_Shoot_0` sampai `_6` (tanpa frame _2/Arrow) seperti ShootLeft
7. Klik **Add Property** → **Sprite Renderer** → **Flip X**
8. Di frame `0:00`, centang/aktifkan ✅ **Flip X** di Inspector
9. **Animation Events** (sama seperti ShootLeft):
   - **Frame 3** (saat panah dilepas) → Function: `OnShootProjectile`
   - **Frame 5** (akhir recovery) → Function: `OnAttackEnd`

### Langkah 19: Buat Clip `Yaksa_Death.anim`

1. **Create New Clip** → `Yaksa_Death.anim`
2. Samples = **8**, Loop = ❌
3. Keyframe:

| Frame | Sprite            |
| ----- | ----------------- |
| 0     | `Yaksa_Death_0` |
| 1     | `Yaksa_Death_1` |
| 2     | `Yaksa_Death_2` |
| 3     | `Yaksa_Death_3` |
| 4     | `Yaksa_Death_4` |
| 5     | `Yaksa_Death_5` |

4. **Animation Event** di frame 5 → Function: `OnDeathEnd`

### Langkah 20: Verifikasi Semua Clip Yaksa

Di `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Animations/`:

Pastikan Anda memiliki **11 file `.anim`**:

| # | File | Loop | FPS | Keterangan |
|---|---|---|---|---|
| 1 | `Yaksa_WalkFront.anim` | ✅ | 10 | 2 frame |
| 2 | `Yaksa_WalkBack.anim` | ✅ | 10 | 3 frame |
| 3 | `Yaksa_WalkLeft.anim` | ✅ | 10 | 4 frame, flipX = ❌ |
| 4 | `Yaksa_WalkRight.anim` | ✅ | 10 | 4 frame, flipX = ✅ |
| 5 | `Yaksa_IdleFront.anim` | ✅ | 4 | Reuse WalkFront, FPS lambat |
| 6 | `Yaksa_IdleBack.anim` | ✅ | 4 | Reuse WalkBack, FPS lambat |
| 7 | `Yaksa_IdleLeft.anim` | ✅ | 4 | Reuse WalkSide, FPS lambat, flipX = ❌ |
| 8 | `Yaksa_IdleRight.anim` | ✅ | 4 | Reuse WalkSide, FPS lambat, flipX = ✅ |
| 9 | `Yaksa_ShootLeft.anim` | ❌ | 10 | event shoot/end, flipX = ❌ |
| 10| `Yaksa_ShootRight.anim`| ❌ | 10 | event shoot/end, flipX = ✅ |
| 11| `Yaksa_Death.anim` | ❌ | 8 | event death end |

---

## Bagian E: Membuat Animator Controller Dwarapala

---

### Langkah 21: Buat Animator Controller Baru

1. Di Project Window, navigasi ke `Assets/Arts/Enemies/Stage2-Dwarapala/Animations/`
2. Klik kanan → **Create** → **Animator Controller** → beri nama `DwarapalaController`
3. **Hapus** Animator Controller yang otomatis dibuat saat langkah 7 (biasanya bernama `DwarapalaTemp`) jika ada
4. **Double-click** `DwarapalaController` untuk membuka jendela **Animator**

### Langkah 22: Tambah Parameter

Di jendela Animator, klik tab **Parameters** (pojok kiri):

1. Klik **+** → **Int** → beri nama `Direction`
2. Klik **+** → **Float** → beri nama `Speed`
3. Klik **+** → **Trigger** → beri nama `Attack`
4. Klik **+** → **Trigger** → beri nama `Die`

### Langkah 23: Buat State — Idle per Arah

1. Klik kanan pada area kosong Animator → **Create State** → **Empty** → beri nama `Idle_Down`
2. Klik state `Idle_Down`:
   - Di Inspector, set **Motion** = drag `Dwarapala_IdleFront.anim` dari Project Window
3. Klik kanan lagi → **Create State** → **Empty** → `Idle_Up`
   - Motion = `Dwarapala_IdleBack.anim`
4. Buat state `Idle_Left` → Motion = `Dwarapala_IdleSide.anim`
5. Buat state `Idle_Right` → Motion = `Dwarapala_IdleSide.anim`

> `Idle_Left` dan `Idle_Right` menggunakan clip yang sama. Pembedaan kiri/kanan dilakukan via `flipX` di script `Stage2EnemyAnimator`.

### Langkah 24: Buat State — Walk per Arah

6. **Create State** → `Walk_Down` → Motion = `Dwarapala_WalkFront.anim`
7. **Create State** → `Walk_Up` → Motion = `Dwarapala_WalkBack.anim`
8. **Create State** → `Walk_Left` → Motion = `Dwarapala_WalkSide.anim`
9. **Create State** → `Walk_Right` → Motion = `Dwarapala_WalkSide.anim`

### Langkah 25: Buat State — Attack & Death

10. **Create State** → `Attack_Front` → Motion = `Dwarapala_AttackFront.anim`
11. **Create State** → `Attack_Side` → Motion = `Dwarapala_AttackSide.anim`
12. **Create State** → `Death` → Motion = `Dwarapala_Death.anim`

### Langkah 26: Set Default State

1. Klik kanan pada state `Idle_Down` → **Set as Layer Default State**
   - State ini akan berubah menjadi **oranye** (warna default)

### Langkah 27: Buat Transition — Idle ↔ Walk (per Arah)

Untuk **setiap pasangan** Idle/Walk per arah, buat transition bolak-balik:

**Arah Down (Direction == 0):**

1. Klik kanan state `Idle_Down` → **Make Transition** → tarik ke `Walk_Down`

   - Klik panah transition → di Inspector:
     - **Has Exit Time**: ❌ (uncheck)
     - **Transition Duration**: 0
     - **Conditions**: `Speed` → `Greater` → `0.1`
2. Klik kanan `Walk_Down` → **Make Transition** → tarik ke `Idle_Down`

   - **Has Exit Time**: ❌
   - **Transition Duration**: 0
   - **Conditions**: `Speed` → `Less` → `0.1`

**Ulangi untuk arah lain** (Up/Left/Right) dengan pola yang sama tetapi menambahkan condition `Direction`:

**Arah Up (Direction == 1):**

- `Idle_Up` → `Walk_Up`: Conditions: `Speed Greater 0.1` + `Direction Equals 1`
- `Walk_Up` → `Idle_Up`: Conditions: `Speed Less 0.1`

**Arah Left (Direction == 2):**

- `Idle_Left` → `Walk_Left`: Conditions: `Speed Greater 0.1` + `Direction Equals 2`
- `Walk_Left` → `Idle_Left`: Conditions: `Speed Less 0.1`

**Arah Right (Direction == 3):**

- `Idle_Right` → `Walk_Right`: Conditions: `Speed Greater 0.1` + `Direction Equals 3`
- `Walk_Right` → `Idle_Right`: Conditions: `Speed Less 0.1`

### Langkah 28: Buat Transition — Antar Arah (Idle → Idle)

Saat enemy berubah arah tanpa bergerak, perlu transition antar idle state:

| Dari           | Ke             | Condition              |
| -------------- | -------------- | ---------------------- |
| `Idle_Down`  | `Idle_Up`    | `Direction Equals 1` |
| `Idle_Down`  | `Idle_Left`  | `Direction Equals 2` |
| `Idle_Down`  | `Idle_Right` | `Direction Equals 3` |
| `Idle_Up`    | `Idle_Down`  | `Direction Equals 0` |
| `Idle_Up`    | `Idle_Left`  | `Direction Equals 2` |
| `Idle_Up`    | `Idle_Right` | `Direction Equals 3` |
| `Idle_Left`  | `Idle_Down`  | `Direction Equals 0` |
| `Idle_Left`  | `Idle_Up`    | `Direction Equals 1` |
| `Idle_Left`  | `Idle_Right` | `Direction Equals 3` |
| `Idle_Right` | `Idle_Down`  | `Direction Equals 0` |
| `Idle_Right` | `Idle_Up`    | `Direction Equals 1` |
| `Idle_Right` | `Idle_Left`  | `Direction Equals 2` |

Semua transition: **Has Exit Time** = ❌, **Transition Duration** = 0

> **Tip**: Ini banyak transition. Cara cepat: pilih beberapa transition sekaligus dan set properties di Inspector batch.

Ulangi pola serupa untuk transition antar Walk state (Walk_Down ↔ Walk_Up ↔ Walk_Left ↔ Walk_Right) jika ingin smooth. Atau biarkan — script `Stage2EnemyAnimator` akan handle perpindahan dengan mengatur Direction.

### Langkah 29: Buat Transition — Attack

**Dari Any State ke Attack:**

1. Klik kanan pada **Any State** (state abu-abu) → **Make Transition** → `Attack_Front`

   - Conditions: `Attack` (trigger) + `Direction Equals 0`
   - Has Exit Time: ❌, Transition Duration: 0
   - **Can Transition To Self**: ❌ (uncheck)
2. **Any State** → `Attack_Front`:

   - Conditions: `Attack` + `Direction Equals 1`
3. **Any State** → `Attack_Side`:

   - Conditions: `Attack` + `Direction Equals 2`
4. **Any State** → `Attack_Side`:

   - Conditions: `Attack` + `Direction Equals 3`

**Dari Attack kembali ke Idle:**

5. `Attack_Front` → `Idle_Down`:

   - **Has Exit Time**: ✅ (centang), Exit Time: 1 (setelah animasi selesai)
   - Transition Duration: 0
   - Conditions: (kosong / atau tambahkan `Direction Equals 0`)
6. `Attack_Side` → `Idle_Left`:

   - Has Exit Time: ✅, Exit Time: 1
   - Conditions: `Direction Equals 2`
7. `Attack_Side` → `Idle_Right`:

   - Has Exit Time: ✅, Exit Time: 1
   - Conditions: `Direction Equals 3`

> **Alternatif lebih simpel**: Buat return transition dari Attack langsung ke `Idle_Down` saja, lalu biarkan script handle perpindahan ke arah yang benar via set Direction.

### Langkah 30: Buat Transition — Death

1. **Any State** → `Death`:
   - Conditions: `Die` (trigger)
   - Has Exit Time: ❌, Transition Duration: 0
   - **Can Transition To Self**: ❌

> Death tidak perlu return transition — setelah animasi Death selesai, GameObject akan di-destroy oleh script.

---

## Bagian F: Membuat Animator Controller Yaksa

---

### Langkah 31: Buat `YaksaController.controller`

1. Di `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Animations/`, klik kanan → **Create** → **Animator Controller** → `YaksaController`
2. Double-click untuk membuka Animator

### Langkah 32: Tambah Parameter

| Parameter     | Tipe    |
| ------------- | ------- |
| `Direction` | Int     |
| `Speed`     | Float   |
| `Shoot`     | Trigger |
| `Die`       | Trigger |

### Langkah 33: Buat State

Buat state sama seperti Dwarapala, tetapi dengan clip Yaksa:

**Idle:**

- `Idle_Down` → `Yaksa_IdleFront.anim`
- `Idle_Up` → `Yaksa_WalkBack.anim` *(frame pertama sebagai idle)*
- `Idle_Left` → `Yaksa_IdleSide.anim`
- `Idle_Right` → `Yaksa_IdleSide.anim`

**Walk:**

- `Walk_Down` → `Yaksa_WalkFront.anim`
- `Walk_Up` → `Yaksa_WalkBack.anim`
- `Walk_Left` → `Yaksa_WalkSide.anim`
- `Walk_Right` → `Yaksa_WalkSide.anim`

**Shoot:**

- `Shoot` → `Yaksa_Shoot.anim`

**Death:**

- `Death` → `Yaksa_Death.anim`

### Langkah 34: Buat Transition

Ikuti pola yang sama dengan DwarapalaController (Langkah 27-30), dengan perbedaan:

- Ganti trigger `Attack` menjadi `Shoot` pada transition ke state `Shoot`
- Shoot tidak perlu split per arah (satu animasi untuk semua arah — Yaksa selalu menghadap pemain saat menembak)
- Transition dari **Any State** → `Shoot`: Condition = `Shoot` trigger saja (tanpa Direction)
- Transition dari `Shoot` → `Idle_Down`: Has Exit Time = ✅, Exit Time = 1

### Langkah 35: Set Default State

- Klik kanan `Idle_Down` → **Set as Layer Default State**

---

## Bagian G: Assign Controller & Cleanup

---

### Langkah 36: Assign Controller ke GameObject Sementara

1. Pilih `DwarapalaTemp` di Hierarchy
2. Di Inspector, pada komponen **Animator**:

   - Set **Controller** = `DwarapalaController`
3. Cek preview:

   - Buka Animation window, pilih clip dari dropdown, klik **Play** untuk preview
4. Ulangi untuk `YaksaTemp`:

   - Set **Controller** = `YaksaController`

### Langkah 37: Test Preview Animasi

1. Pilih `DwarapalaTemp`, buka Animation window
2. Dari dropdown clip, pilih masing-masing clip dan klik **Play**:
  - ✅ IdleFront, IdleBack, IdleLeft, IdleRight — sprite berubah sesuai arah dan flip
  - ✅ WalkFront/Back/Left/Right — sprite beranimasi jalan ke arah masing-masing
  - ✅ AttackFront/Left/Right — gerakan hantam gada ke arah masing-masing
  - ✅ Death — jatuh dan hancur
3. Ulangi untuk `YaksaTemp`

### Langkah 38: Cleanup

1. **Hapus Animator Controller otomatis** yang tidak terpakai (biasanya bernama `DwarapalaTemp.controller` atau `YaksaTemp.controller`) dari folder Animations
2. **Hapus GameObject sementara** dari Hierarchy:
   - Klik kanan `DwarapalaTemp` → Delete
   - Klik kanan `YaksaTemp` → Delete
3. **Jangan hapus** `DwarapalaController.controller` dan `YaksaController.controller` yang sudah dikonfigurasi

---

## Checklist Akhir Tahap 1

Centang setiap item setelah selesai:

### Sprite Slice

- [ ] `Idle.png` — 3 sprite (Front, Back, Side)
- [ ] `Walk.png` — 12 sprite (4 per arah × 3 arah)
- [ ] `AttackAndDeath.png` — 10 sprite utama (noise dihapus)
- [ ] `Sprites.png` (Yaksa) — ~21 sprite + 1 sprite `Yaksa_Arrow`

### Animation Clips Dwarapala (12 clip)

- [ ] `Dwarapala_IdleFront.anim` (loop, 1 fps)
- [ ] `Dwarapala_IdleBack.anim` (loop, 1 fps)
- [ ] `Dwarapala_IdleLeft.anim` (loop, 1 fps, flipX = ✅)
- [ ] `Dwarapala_IdleRight.anim` (loop, 1 fps, flipX = ❌)
- [ ] `Dwarapala_WalkFront.anim` (loop, 8 fps, 4 frame)
- [ ] `Dwarapala_WalkBack.anim` (loop, 8 fps, 4 frame)
- [ ] `Dwarapala_WalkLeft.anim` (loop, 8 fps, 4 frame, flipX = ✅)
- [ ] `Dwarapala_WalkRight.anim` (loop, 8 fps, 4 frame, flipX = ❌)
- [ ] `Dwarapala_AttackFront.anim` (no loop, 8 fps, 3 frame, events: OnAttackHit + OnAttackEnd)
- [ ] `Dwarapala_AttackLeft.anim` (no loop, 8 fps, 3 frame, flipX = ✅, events: OnAttackHit + OnAttackEnd)
- [ ] `Dwarapala_AttackRight.anim` (no loop, 8 fps, 3 frame, flipX = ❌, events: OnAttackHit + OnAttackEnd)
- [ ] `Dwarapala_Death.anim` (no loop, 6 fps, 4 frame, event: OnDeathEnd)

### Animation Clips Yaksa (11 clip)

- [ ] `Yaksa_WalkFront.anim` (loop, 10 fps, 2 frame)
- [ ] `Yaksa_WalkBack.anim` (loop, 10 fps, 3 frame)
- [ ] `Yaksa_WalkLeft.anim` (loop, 10 fps, 4 frame, flipX = ❌)
- [ ] `Yaksa_WalkRight.anim` (loop, 10 fps, 4 frame, flipX = ✅)
- [ ] `Yaksa_IdleFront.anim` (loop, 4 fps, reuse sprite WalkFront)
- [ ] `Yaksa_IdleBack.anim` (loop, 4 fps, reuse sprite WalkBack)
- [ ] `Yaksa_IdleLeft.anim` (loop, 4 fps, reuse sprite WalkSide, flipX = ❌)
- [ ] `Yaksa_IdleRight.anim` (loop, 4 fps, reuse sprite WalkSide, flipX = ✅)
- [ ] `Yaksa_ShootLeft.anim` (no loop, 10 fps, 6 frame, events: OnShootProjectile + OnAttackEnd, flipX = ❌)
- [ ] `Yaksa_ShootRight.anim` (no loop, 10 fps, 6 frame, events: OnShootProjectile + OnAttackEnd, flipX = ✅)
- [ ] `Yaksa_Death.anim` (no loop, 8 fps, 6 frame, event: OnDeathEnd)

### Animator Controller

- [ ] `DwarapalaController.controller` — 4 parameter, 13 state, transition lengkap
- [ ] `YaksaController.controller` — 4 parameter, 11 state, transition lengkap

### Cleanup

- [ ] Hapus Animator Controller otomatis yang tidak terpakai
- [ ] Hapus GameObject sementara (DwarapalaTemp, YaksaTemp)
- [ ] Verifikasi semua clip bisa di-preview tanpa error

---

> **Setelah Tahap 1 selesai**, lanjutkan ke **Tahap 2: Coding Script C#** — lihat `docs/Stage2_Enemy_Design.md` bagian 6 untuk daftar script yang perlu dibuat.

