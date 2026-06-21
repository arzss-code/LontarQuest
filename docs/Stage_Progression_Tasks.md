# Roguelike Stage Progression Implementation Tasks

Dokumen ini merincikan urutan implementasi _content_ yang telah disesuaikan dengan **Sistem Fokus 3 Zona Utama** pada update GDD terbaru.

---

## 🟢 STAGE 1: Reruntuhan Candi

_Tema Lingkungan: Lorong batu candi kuno, relief bercahaya Aksara Kawi, pintu batu._

### 1. Environment & Level Design (Stage 1)

- [x] Buat `Tilemap_Stage1` (Wall, Floor, Obstacle).
- [x] Rancang variasi `Room_Template_Stage1` untuk pengenalan jebakan lantai dasar.
- [x] Pasang _Lontar Pickup_ sebagai pengganti Boon Doors dengan drop stat dinamis.

### 2. Implementasi Musuh (Stage 1)

- [x] **Musuh: Kepala Kala (Banaspati - Chaser)**
  - [ ] Buat Prefab `Enemy_KepalaKala` dengan komponen AI (Mengejar dan meledak di jarak dekat).
  - [ ] Masukkan ke daftar _spawn_ ruangan Stage 1.

### 3. Lore & Jurnal (Stage 1)

- [ ] Buat Data ScriptableObject `Lore_KepalaKala`.
- [] Tempatkan `Prasasti_KepalaKala` di ruangan aman pertama.

---

## 🟡 STAGE 2: Perpustakaan Melayang

_Tema Lingkungan: Rak buku kayu melayang di ruang hampa, lontar beterbangan, aura magis._

### 1. Environment & Level Design (Stage 2)

- [ ] Buat `Tilemap_Stage2` (Platform kayu, jurang hampa).
- [ ] Desain tantangan _Platforming_ melompat antar rak.
- [ ] Pasang _Boon Doors_ dengan fokus **Aksara Lontara** & **Aksara Kawi**.

### 2. Implementasi Musuh (Stage 2)

- [ ] **Musuh: Yaksa (Leak - Ranged)**
  - [ ] Buat Prefab `Enemy_Yaksa` dengan serangan proyektil dan _teleport_.
- [ ] **Mini-Boss: Dwarapala (Buta Ijo - Tanker)**
  - [ ] Buat Prefab `MiniBoss_Dwarapala`. Lambat tapi sangat mematikan (menghancurkan lantai/AoE).
  - [ ] Tempatkan Dwarapala sebagai Mini-Boss penutup Stage 2.

### 3. Lore & Jurnal (Stage 2)

- [ ] Buat Data `Lore_Yaksa` dan `Lore_Dwarapala`.
- [ ] Pemain menemukan Jurnal Fisik Ayah (Prof. Arya) yang berisi rahasia Dimensi Lontar.

---

## 🔴 STAGE 3: Ruang Inti Dimensi (Konklusi)

_Tema Lingkungan: Arena kosmik luas, lantai kristal transparan, pusaran energi Bhatara Kala._

### 1. Environment & Level Design (Stage 3)

- [ ] Buat `Room_Boss_BhataraKala`.
- [ ] Kunci pemain di dalam arena luas tanpa jalan keluar hingga _combat_ selesai.

### 2. Final Boss: Bhatara Kala

- [ ] Buat Prefab `Boss_BhataraKala`.
- [ ] **Fase 1:** Mengeluarkan serangan kombinasi dari Kepala Kala, Yaksa, dan Dwarapala (Summon Phase).
- [ ] **Fase 2:** Serangan _Area of Effect_ kosmik besar menggunakan Pusaka Lontar.
- [ ] **Pelarian:** Memicu _timer_ pelarian ketika Boss kalah.

### 3. Ending Sequence

- [ ] Siapkan `EndingManager.cs` untuk memutar transisi _cutscene_ Saka mengambil alih posisi sebagai Juru Kunci dan membebaskan ayahnya.
