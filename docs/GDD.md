# Game Design Document: LontarQuest: The Lost Expedition

## 👥 Tim Pengembang (Kelompok 2)
- Atsiila Arya Nabiih (4.33.23.1.04)
- Azani Fathur Fadhika (4.33.23.1.05)
- Tristan Eka Wiranata (4.33.23.1.24)
- Warseno Bambang Setyono (4.33.23.1.25)

---

## 1. Game Specification
- **Genre:** 2.5D Top Down Action RPG
- **Tema:** Eksplorasi Arkeologi, Dark Fantasy, Mitologi & Budaya Nusantara
- **Platform:** PC (Windows)
- **Rating:** Teen (13+)
- **Jumlah Pemain:** 1 Pemain (Single-player)
- **Kontrol Utama:** Keyboard & Mouse (WASD)
- **Target Pengembangan:** 1 Bulan (MVP Focus)

## 2. Sinopsis Game
Saka, seorang penjelajah muda, nekat menelusuri sebuah candi kuno demi mencari ayahnya—seorang arkeolog yang hilang 10 tahun lalu. Di sana, ia memecahkan sandi Aksara Kawi yang tanpa sengaja membawanya masuk ke "Dimensi Lontar", sebuah alam gaib tak stabil tempat rahasia semesta Nusantara disimpan. Saka harus bertarung melawan manifestasi mitologi lokal, menyerap kekuatan dari aksara-aksara kuno, dan mengungkap kenyataan pahit bahwa ayahnya telah mengorbankan diri menjadi Juru Kunci dimensi tersebut. Saka harus menyelamatkan peninggalan ayahnya dan keluar sebelum dimensi itu menelan dirinya.

---

## 3. Cerita (Storyline)

### Babak 1: Setup (Pengenalan & Insiden Pemantik)
**Dunia Nyata & Titik Awal Perjalanan**
Saka bertekad mencari ayahnya, Profesor Arya. Saka nekat pergi sendirian menggunakan Jeep tua ayahnya ke sebuah candi terlupakan di pedalaman Nusantara. Di sana, ia menemukan ruang bawah tanah tak masuk akal dengan gravitasi nol. Perjalanan Saka resmi dimulai ketika ia berhasil memecahkan sandi Aksara Kawi dan menyalakan obor ritual *Sedulur Papat Limo Pancer* yang menarik kesadarannya menembus ruang dan waktu.

### Babak 2: Conflict (Konflik & Rintangan)
**Bertahan Hidup di Dimensi Lontar**
Saka terbangun di Dimensi Lontar. Ia harus bertahan dari manifestasi energi pelindung candi kuno, mencatat mitologi musuh di jurnal, dan menyerap kekuatan dari pilar Aksara Nusantara.

### Babak 3: Climax (Puncak Ketegangan)
**Kebangkitan Sang Penelan Waktu & Pilihan Sulit**
Saka tiba di inti dimensi dan menemukan Prof. Arya yang tubuhnya menua drastis. Prof. Arya mengorbankan diri menjadi segel untuk menahan **Bhatara Kala**. Kini segel itu melemah. Prof. Arya menyerahkan Pusaka Lontar kepada Saka dan menyuruhnya lari.

### Babak 4: Falling Action (Penurunan Ketegangan)
**Pertukaran Posisi & Jalan Pulang**
Saka menolak kabur. Ia menggenggam Pusaka Lontar, menyatu dengan aliran informasi dimensi, menciptakan celah portal, dan melemparkan Prof. Arya keluar. Saka kemudian berhasil menenangkan amukan Bhatara Kala.

### Babak 5: Resolution (Penyelesaian Akhir)
**Juru Kunci Baru & Kembalinya Sang Ayah**
Prof. Arya terbangun dengan selamat di dunia nyata. Namun Saka kini berdiri abadi sebagai Juru Kunci yang baru di dalam Dimensi Lontar, menjaga keseimbangan semesta.

---

## 4. Game Elements

### A. Karakter
| Peran | Nama | Detail & Latar Belakang |
|---|---|---|
| **Main Character** | Saka Wiryawan (24) | Skeptis, cerdas, keras kepala. Menjadi penjelajah independen untuk mencari ayahnya. |
| **NPC / Guardian** | Prof. Arya Wiryawan (56) | Idealis, rela berkorban. Mengorbankan fisik dan jiwanya menjadi segel hidup (Juru Kunci) di Dimensi Lontar. |

### B. Musuh & Penjaga (Bestiary)
| Nama Penjaga | Tipe Tempur | Asal | Wujud & Sifat | Mekanisme Ancaman |
|---|---|---|---|---|
| **Kepala Kala** *(Banaspati)* | Chaser | Candi Hindu-Buddha | Kepala raksasa batu melayang tanpa rahang bawah. | Agresif, mendeteksi pergerakan/suara, meledakkan energi di jarak dekat. |
| **Dwarapala** *(Buta Ijo)* | Tanker | Hindu-Buddha | Raksasa batu berwajah menyeramkan, membawa Gada. | Sangat teritorial, lambat namun serangannya menghancurkan arena & memberi stun. |
| **Yaksa** *(Leak)* | Ranged | Roh Alam | Roh alam setengah dewa/iblis yang melayang. | Menjaga jarak, menembakkan proyektil energi, memaksa pemain menghindar. |
| **Bhatara Kala** | Final Boss | Hindu | Entitas kosmik pusaran waktu tanpa bentuk permanen. | Absolut. Memiliki fase serangan kombinasi musuh dan fase AoE besar (Pusaka Lontar). |

---

## 5. Gameplay & Mekanik (Sistem Fokus 3 Zona)

Permainan menggunakan sistem Roguelite dengan Level Tetap (Fixed), berfokus pada **3 Zona Eksplorasi Manual (Hand-crafted)**.

### Sistem *Quest* & *Condition*
- **Main Quest:** Menembus 3 Zona Spesifik di Dimensi Lontar secara berurutan.
- **Looping Quest:** Setiap jalan *(run)* dimulai dari *Basecamp* (Hub). Jika mati, HP jadi 0, kembali ke Hub namun menyimpan **Pecahan Relik**.
- **Side Quest:** Mengumpulkan entri lengkap "Buku Panduan Ekologi Gaib" (Jurnal Nusantara).
- **Win Condition:** Menyelesaikan Zona Terakhir, mengalahkan Final Boss, dan kabur ke titik keluar.

### Player Control
- `W, A, S, D` : Pergerakan
- `Shift` / `Space` : *Dodge Roll*
- `Klik Kiri` : *Light Melee*
- `Klik Kanan` : *Special Attack*
- `E` / `F` : Interaksi objek/Jurnal
- `Tab` : Membuka Jurnal Nusantara

### Fitur Kunci
1. **Sistem Pertarungan Aksara (Boons):** Power-up per *run*.
   - *Aksara Lontara* = Kecepatan (Dash/Attack Speed).
   - *Aksara Batak* = Ketahanan (Damage Reduction/HP Reg).
   - *Aksara Kawi* = Kekuatan Spesial (Elemental/Stamina).
2. **Buku Panduan Ekologi Gaib:** Jurnal sejarah dan mitologi budaya Nusantara.

---

## 6. Level Design (3 Zona Utama)

| Zona / Stage | Lingkungan & Visual | Musuh & Tantangan |
|---|---|---|
| **Hub: Suaka Lontar** *(Basecamp)* | Pelataran batu melayang, api unggun biru, kubah energi aksara. | Tempat *respawn* & upgrade permanen dengan Pecahan Relik. |
| **Stage 1: Reruntuhan Candi** | Lorong batu candi Javanese kuno, relief Aksara Kawi, pintu masuk batu kuno. | Pengenalan mekanik dasar. Musuh: **Kepala Kala** *(Chaser)*. Fokus Aksara Batak. |
| **Stage 2: Perpustakaan Melayang** | Rak buku kayu yang melayang di ruang hampa, lontar beterbangan, cahaya biru magis. | Tantangan platforming & navigasi. Musuh: **Yaksa** *(Ranged)* dan **Dwarapala** *(Tanker/Mini Boss)*. Fokus Aksara Lontara & Kawi. |
| **Stage 3: Ruang Inti Dimensi** | Arena kosmik luas dengan lantai kristal transparan, pusaran energi kosmik ungu/emas. | **Final Boss Fight:** Bhatara Kala (Multiple Phases). Fase pelarian waktu menuju ending. |
