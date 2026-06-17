# Roguelike Stage Progression Implementation Tasks

Dokumen ini merincikan urutan implementasi *content* yang telah disesuaikan dengan **Sistem Fokus 3 Zona Utama** pada update GDD terbaru.

---

## 🟢 STAGE 1: Reruntuhan Candi
*Tema Lingkungan: Lorong batu candi kuno, relief bercahaya Aksara Kawi, pintu batu.*

### 1. Environment & Level Design (Stage 1)
- [ ] Buat `Tilemap_Stage1` (Wall, Floor, Obstacle).
- [ ] Rancang variasi `Room_Template_Stage1` untuk pengenalan jebakan lantai dasar.
- [ ] Pasang *Boon Doors* dengan fokus memunculkan hadiah **Aksara Batak** (Pertahanan).

### 2. Implementasi Musuh (Stage 1)
- [ ] **Musuh: Kepala Kala (Banaspati - Chaser)**
  - [ ] Buat Prefab `Enemy_KepalaKala` dengan komponen AI (Mengejar dan meledak di jarak dekat).
  - [ ] Masukkan ke daftar *spawn* ruangan Stage 1.

### 3. Lore & Jurnal (Stage 1)
- [ ] Buat Data ScriptableObject `Lore_KepalaKala`.
- [ ] Tempatkan `Prasasti_KepalaKala` di ruangan aman pertama.

---

## 🟡 STAGE 2: Perpustakaan Melayang
*Tema Lingkungan: Rak buku kayu melayang di ruang hampa, lontar beterbangan, aura magis.*

### 1. Environment & Level Design (Stage 2)
- [ ] Buat `Tilemap_Stage2` (Platform kayu, jurang hampa).
- [ ] Desain tantangan *Platforming* melompat antar rak.
- [ ] Pasang *Boon Doors* dengan fokus **Aksara Lontara** & **Aksara Kawi**.

### 2. Implementasi Musuh (Stage 2)
- [ ] **Musuh: Yaksa (Leak - Ranged)**
  - [ ] Buat Prefab `Enemy_Yaksa` dengan serangan proyektil dan *teleport*.
- [ ] **Mini-Boss: Dwarapala (Buta Ijo - Tanker)**
  - [ ] Buat Prefab `MiniBoss_Dwarapala`. Lambat tapi sangat mematikan (menghancurkan lantai/AoE).
  - [ ] Tempatkan Dwarapala sebagai Mini-Boss penutup Stage 2.

### 3. Lore & Jurnal (Stage 2)
- [ ] Buat Data `Lore_Yaksa` dan `Lore_Dwarapala`.
- [ ] Pemain menemukan Jurnal Fisik Ayah (Prof. Arya) yang berisi rahasia Dimensi Lontar.

---

## 🔴 STAGE 3: Ruang Inti Dimensi (Konklusi)
*Tema Lingkungan: Arena kosmik luas, lantai kristal transparan, pusaran energi Bhatara Kala.*

### 1. Environment & Level Design (Stage 3)
- [ ] Buat `Room_Boss_BhataraKala`.
- [ ] Kunci pemain di dalam arena luas tanpa jalan keluar hingga *combat* selesai.

### 2. Final Boss: Bhatara Kala
- [ ] Buat Prefab `Boss_BhataraKala`.
- [ ] **Fase 1:** Mengeluarkan serangan kombinasi dari Kepala Kala, Yaksa, dan Dwarapala (Summon Phase).
- [ ] **Fase 2:** Serangan *Area of Effect* kosmik besar menggunakan Pusaka Lontar.
- [ ] **Pelarian:** Memicu *timer* pelarian ketika Boss kalah.

### 3. Ending Sequence
- [ ] Siapkan `EndingManager.cs` untuk memutar transisi *cutscene* Saka mengambil alih posisi sebagai Juru Kunci dan membebaskan ayahnya.
