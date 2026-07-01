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
Saka bertekad mencari ayahnya, Profesor Arya. Saka nekat pergi sendirian menggunakan Jeep tua ayahnya ke sebuah candi terlupakan di pedalaman Nusantara. Di sana, ia menemukan ruang bawah tanah tak masuk akal dengan gravitasi nol. Perjalanan Saka resmi dimulai ketika ia berhasil memecahkan sandi Aksara Kawi dan menyalakan obor ritual _Sedulur Papat Limo Pancer_ yang menarik kesadarannya menembus ruang dan waktu.

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

| Peran              | Nama                     | Detail & Latar Belakang                                                                                     |
| ------------------ | ------------------------ | ----------------------------------------------------------------------------------------------------------- |
| **Main Character** | Saka Wiryawan (24)       | Skeptis, cerdas, keras kepala. Menjadi penjelajah independen untuk mencari ayahnya.                         |
| **NPC / Guardian** | Prof. Arya Wiryawan (56) | Idealis, rela berkorban. Mengorbankan fisik dan jiwanya menjadi segel hidup (Juru Kunci) di Dimensi Lontar. |

### B. Musuh & Penjaga (Bestiary)

| Nama Penjaga                            | Tipe Tempur | Asal               | Wujud & Sifat                                       | Mekanisme Ancaman                                                                   |
| --------------------------------------- | ----------- | ------------------ | --------------------------------------------------- | ----------------------------------------------------------------------------------- |
| **Arca Gana**                           | Melee       | Candi Hindu-Buddha | Patung batu penjaga yang hidup kembali.             | Lambat namun ulet, menyerang dalam jarak dekat saat pemain lengah.                  |
| **Kepala Kala** _Referensi (Banaspati)_ | Boss / Chaser| Candi Hindu-Buddha | Kepala raksasa batu melayang tanpa rahang bawah.    | Agresif, mendeteksi pergerakan/suara, meledakkan energi di jarak dekat.             |
| **Dwarapala** _Referensi (Buta Ijo)_    | Tanker      | Hindu-Buddha       | Raksasa batu berwajah menyeramkan, membawa Gada.    | Sangat teritorial, lambat namun serangannya menghancurkan arena & memberi stun.     |
| **Yaksa** _Referensi (Leak)_            | Boss / Ranged| Roh Alam           | Roh alam setengah dewa/iblis yang melayang.         | Menjaga jarak, menembakkan proyektil energi, memaksa pemain menghindar.             |
| **Nisakala**                            | Assasin     | Entitas Kosmik     | Bayangan tanpa wujud fisik yang jelas.              | Sangat cepat, dapat menghilang sementara dan menyerang secara mengejutkan.          |
| **Bhatara Kala**                        | Final Boss  | Hindu              | Entitas kosmik pusaran waktu tanpa bentuk permanen. | Absolut. Memiliki fase serangan kombinasi musuh dan fase AoE besar (Pusaka Lontar). |

---

## 5. Gameplay & Mekanik (Sistem Fokus 3 Zona)

Permainan menggunakan sistem Roguelite dengan Level Tetap (Fixed), berfokus pada **3 Zona Eksplorasi Manual (Hand-crafted)**.

### Sistem _Quest_ & _Condition_

- **Main Quest:** Menembus 3 Zona Spesifik di Dimensi Lontar secara berurutan.
- **Looping Quest:** Setiap jalan _(run)_ dimulai dari _Basecamp_ (Hub). Jika mati, HP jadi 0, kembali ke Hub namun menyimpan **Pecahan Relik**.
- **Side Quest:** Mengumpulkan entri lengkap "Buku Panduan Ekologi Gaib" (Jurnal Nusantara).
- **Win Condition:** Menyelesaikan Zona Terakhir, mengalahkan Final Boss, dan kabur ke titik keluar.

### Player Control

- `W, A, S, D` : Pergerakan
- `Space` : _Dodge Roll_
- `Klik Kiri` / `J` : _Light Melee_ (Sabetan Pedang)
- `Klik Kanan` / `K` : _Special Attack_ (Panah)
- `E` : Interaksi objek/Jurnal
- `Tab` : Membuka Jurnal Nusantara

### Fitur Kunci

1. **Sistem Pertarungan Aksara (Boons - Slot & Level-Up):** Power-up ala *Roguelike (Hades Style)*.
   - Pemain memiliki 4 Slot kemampuan: **Melee**, **Bow**, **Dash**, dan **Passive**.
   - Setiap slot hanya bisa diisi oleh 1 *Boon*. Jika pemain mengambil *Boon* berbeda untuk slot yang sama, *Boon* lama akan **tergantikan (Overwrite)**.
   - Jika pemain mengambil *Boon* yang sama, *Boon* tersebut akan **naik level (Upgrade)**.
   - **Tipe Elemen:**
     - *Aksara Lontara* = Kecepatan (Kec. Serang/Lari).
     - *Aksara Batak* = Ketahanan (Damage Reduction/Shield).
     - *Aksara Kawi* = Kekuatan Spesial (Damage over Time/Burn).
2. **Buku Panduan Ekologi Gaib:** Jurnal sejarah dan mitologi budaya Nusantara.

---

## 6. Level Design & Progression (3 Zona Utama)

Permainan dirancang menggunakan sistem progresi linear dengan elemen *Roguelite*. Pemain harus melewati 3 Zona Utama (Stage) yang telah didesain secara manual (*hand-crafted*) secara berurutan. Jika pemain mati di tengah perjalanan, mereka akan kembali ke *Hub (Suaka Lontar)*, mereset semua *Boon* yang didapat pada *run* tersebut, namun tetap mempertahankan beberapa progres permanen (seperti Jurnal).

Setiap zona memiliki mekanik ruang (*Room System*) sebagai berikut:
- **Room Manager:** Setiap ruangan tempur dikendalikan oleh sistem yang mengunci pintu masuk saat pemain masuk, lalu memunculkan musuh.
- **Combat Phase:** Pemain harus mengalahkan semua musuh di dalam ruangan tersebut untuk membuka jalur (barikade/pintu) menuju ruangan berikutnya.
- **Reward Phase:** Setelah ruangan berhasil dibersihkan, pemain akan diberikan *reward* berupa *Boon* (melalui interaksi *Lontar Pickup* atau melewati *Boon Door*) untuk memperkuat stat karakter sebelum melanjutkan. Partikel penunjuk jalan (*Guide Particle*) juga akan muncul untuk mengarahkan pemain ke rute berikutnya.
- **Lore & Eksplorasi:** Di sela-sela ruangan tempur, terdapat *Safe Room* atau area transisi di mana pemain dapat menemukan objek jurnal (Prasasti/Lontar Fisik) untuk memperluas Lore.

### Tingkatan dari Stage ke Stage (Progresi Perjalanan)

Perjalanan Saka dibagi menjadi beberapa tingkatan yang semakin sulit. Setiap transisi *stage* membawa tantangan musuh dan lingkungan yang baru:

1. **Stage 1 (Reruntuhan Candi):** Tingkat awal yang berfokus pada pengenalan mekanik ruang (*Room System*) dan menghindar. Pemain akan diberi sebuah **Quest** khusus untuk terus memburu dan mengalahkan musuh dasar **Arca Gana** di sepanjang lorong reruntuhan. Klimaks dari *Stage 1* adalah pertarungan melawan *Boss* **Kepala Kala**.
2. **Stage 2 (Perpustakaan Melayang):** Setelah mengalahkan Kepala Kala, tingkat kesulitan naik dengan adanya bahaya lingkungan berupa jurang (*void*) dan platform melayang. Musuh yang dihadapi kini lebih tebal, yaitu **Dwarapala**, dan pemain harus mengakhiri level ini dengan melawan *Boss* **Yaksa**.
3. **Stage 3 (Ruang Inti Dimensi):** Tahap akhir dari permainan. Tingkatan ini bersifat lebih kosmik dan menekan batas kemampuan *Boon* yang telah dikumpulkan. Pemain akan diserang oleh entitas gaib bernama **Nisakala**, sebelum akhirnya menghadapi pertarungan klimaks melawan **Bhatara Kala** (atau wujud puncak dari **Kepala Kala**).

### Detail Zona / Stage

| Zona / Stage | Tema Lingkungan & Visual | Struktur Level & Tantangan | Musuh & Boss |
|---|---|---|---|
| **Hub: Suaka Lontar** *(Basecamp)* | Pelataran batu melayang, api unggun biru, kubah energi aksara. | Area aman tempat pemain *respawn*. Terdapat titik awal perjalanan (portal) dan fitur pengecekan progres Jurnal. | *(Tidak ada musuh)* |
| **Stage 1: Reruntuhan Candi** | Lorong batu candi kuno, relief bercahaya Aksara Kawi, pintu batu kuno. | **Pengenalan Quest & Mekanik.** Pemain harus mengikuti alur *quest* mengalahkan **Arca Gana** secara bertahap. Terdapat *Lontar Pickup* sebagai *Boon*. | Musuh: **Arca Gana**<br>Boss: **Kepala Kala** |
| **Stage 2: Perpustakaan Melayang** | Rak buku kayu yang melayang di ruang hampa, lontar beterbangan, cahaya magis. | **Tantangan Platforming.** Medan pertempuran dipenuhi jurang hampa. Menuntut kelincahan bermanuver menghindari *damage* selagi melawan musuh berdarah tebal. | Musuh: **Dwarapala**<br>Boss: **Yaksa** |
| **Stage 3: Ruang Inti Dimensi** | Arena kosmik luas dengan lantai kristal transparan, pusaran energi ungu/emas. | **Arena Final.** Ruangan tertutup berskala besar (*lock-in arena*). Memiliki fase pertempuran puncak yang menentukan *ending* permainan. | Musuh: **Nisakala**<br>Boss Final: **Bhatara Kala / Kepala Kala** |
