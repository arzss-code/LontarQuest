# Rencana Implementasi: Sistem Boon Slot & Level-Up (Hades Style)

Dokumen ini merincikan rencana teknis untuk mengubah sistem pengambilan *Boon* yang tadinya menumpuk tak terbatas, menjadi sistem berbasis **Slot (Tipe Kemampuan)** dan **Level-Up**, demi menjaga keseimbangan (balance) permainan *LontarQuest*.

## Konsep Utama
Membatasi pemain pada 4 Slot *Boon* utama:
1. **Melee Slot**: Modifikasi sabetan pedang (Light Attack).
2. **Bow Slot**: Modifikasi tembakan panah (Special Attack).
3. **Dash Slot**: Modifikasi efek menghindar (Dodge Roll).
4. **Passive Slot**: Status latar belakang (HP, Damage Reduction, Base Move Speed).

Jika pemain mengambil Lontar dengan elemen yang sama pada slot yang sudah terisi, *Boon* tersebut akan **naik level** (Maksimal Level 2). Jika mengambil elemen berbeda, elemen lama di slot tersebut akan **tergantikan (replace)**.

---

## Detail Perubahan Skrip

### 1. ScriptableObjects / Data Layer (`BoonData.cs`)
Struktur data perlu diubah agar memiliki properti slot dan tingkatan level.

- Tambahkan `enum BoonSlot { Melee, Bow, Dash, Passive }`.
- Tambahkan parameter `public BoonSlot slot;` untuk menentukan *Boon* ini masuk ke *slot* mana.
- Tambahkan parameter `public int level = 1;` untuk penanda level *Boon*.
- Tambahkan parameter `public BoonData nextLevelBoon;` (referensi ScriptableObject untuk versi *upgrade*-nya. Jika kosong, berarti sudah Level Max).

### 2. Player Stats Layer (`PlayerModifier.cs`)
Ubah logika penyimpanan *Boon* di sisi karakter pemain.

- Ubah variabel `List<BoonData> activeBoons` menjadi struktur koleksi yang dibatasi berdasar `BoonSlot` (contoh: `Dictionary<BoonSlot, BoonData> activeBoons`).
- Ubah logika `AddBoon()`: 
  - Jika slot masih kosong, tambahkan.
  - Jika slot sudah terisi (baik dengan *Boon* yang sama dengan level lebih tinggi, atau *Boon* yang berbeda), **ganti** (replace) *Boon* lama dengan *Boon* baru.
  - Pemanggilan `RecalculateModifiers()` harus membuang status dari *Boon* lama yang tergantikan dan menghitung ulang dari isi *Dictionary* saat ini.

### 3. UI & Selection Logic (`BoonUIManager.cs`)
Pastikan daftar pilihan *Boon* yang muncul menyesuaikan dengan apa yang sudah dimiliki Saka.

- Dalam fungsi `ShowBoonSelection()`, saring daftar `allAvailableBoons` sebelum dikocok.
- **Logika Penyaringan Cerdas:** 
  - Periksa *Dictionary* di `PlayerModifier`.
  - Jika pemain sudah punya *Boon A (Level 1)* di Slot Melee, hapus *Boon A (Level 1)* dari *pool* acakan, dan ganti dengan *Boon A (Level 2)* (dari referensi `nextLevelBoon`).
  - Jika pemain sudah punya *Boon* dengan *Level Max* (tidak ada referensi `nextLevelBoon`), jangan masukkan *Boon* tersebut ke dalam *pool* acakan sama sekali.
  - Sisa *pool* yang valid baru dikocok (Fisher-Yates) dan diambil 3 teratas untuk ditampilkan ke UI.

---

## Verifikasi Implementasi
Setelah diimplementasikan, uji coba harus memastikan 2 skenario ini berjalan sempurna:
1. **Skenario Overwrite:** Pungut "Kecepatan (Lontara)" di Slot Melee -> Pungut "Api (Kawi)" di Slot Melee. Hasil: Kecepatan hilang, digantikan oleh Api. Karakter tidak bisa memukul cepat sekaligus berapi-api.
2. **Skenario Upgrade:** Pungut "Api (Kawi) Lv 1" di Slot Melee -> Buka *Boon UI* lagi, yang muncul di UI seharusnya adalah opsi "Api (Kawi) Lv 2", bukan Lv 1. Saat dipilih, status apinya bertambah kuat tanpa menduplikasi data.
