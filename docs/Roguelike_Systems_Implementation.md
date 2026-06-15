# Panduan Implementasi Sistem Roguelike & Jurnal LontarQuest

Dokumen ini berisi panduan teknis dan urutan kerja (Workflow) untuk memasang sistem *Boon* (Hadiah Ruangan) dan *Jurnal Nusantara* (Edukasi Mitologi) ke dalam Unity Editor Anda.

---

## Urutan Implementasi (Wajib Berurutan)

Untuk menghindari error referensi kosong (Missing Reference) di Unity, harap lakukan perakitan dengan urutan berikut:

1. **Buat Data Master Dulu:** Buat setidaknya 1 `Boon Data` dan 1 `Lore Data` menggunakan menu *ScriptableObject* (Klik Kanan -> Create -> LontarQuest).
2. **Pasang Skrip ke Saka:** Pasang `PlayerModifier` ke karakter utama (Saka).
3. **Bangun Ruangan (Room):** Pasang `RoomManager` di tengah ruangan, dan masukkan musuh-musuhnya ke dalam list.
4. **Pasang Pintu (Boon Door):** Pasang `BoonDoor` di pintu keluar ruangan tersebut, lalu hubungkan pintu ini ke `RoomManager` di langkah 3.
5. **Pasang Pemicu Cerita (Lore):** Pasang `LoreInteractable` pada patung/arca yang ada di ruangan tersebut, dan masukkan `Lore Data` dari langkah 1 ke dalamnya.
6. **Pasang Manajer Global:** Buat objek kosong `GameManager` dan pasang `JournalManager` dan `BoonUIManager`.

---

## Detail Langkah Eksekusi di Unity

### A. Membuat Data Aksara & Mitologi (Tanpa Coding)
Sistem ini menggunakan *ScriptableObject*. Anda bertindak sebagai Game Designer di sini.
1. Buka folder `Assets/` di tab Project, lalu buat folder baru bernama `Data` (atau bebas).
2. Di dalam folder `Data`, klik kanan area kosong -> **Create -> LontarQuest**.
3. Pilih **Boon Data** untuk membuat *template* kekuatan baru. Di Inspector, isi nama, tipe Aksara (Lontara/Batak/Kawi), ikon, dan jumlah status bonusnya (misal: kecepatan lari, tambahan damage).
4. Pilih **Lore Data** untuk membuat *template* buku mitologi monster. Isi ID, nama monster, gambar, dan sejarah mitologinya.

### B. Setup Player Saka
Saka membutuhkan sistem untuk menerima *buff* tersebut.
1. Klik *Prefab* **Saka**.
2. Tarik skrip `PlayerModifier.cs` (ada di `Assets/Scripts/Systems/`) ke Saka. Pastikan posisinya berdampingan dengan `PlayerStats.cs`. Skrip ini akan otomatis menampung kekuatan yang Saka dapatkan.

### C. Setup Pintu Lorong (Memilih Jalur)
Pintu ini yang akan menghadang Saka, dan memberi Boon setelah dilewati.
1. Di setiap pintu keluar ruangan/lorong, pastikan ada objek dengan `BoxCollider2D`.
2. Centang **Is Trigger** di BoxCollider-nya.
3. Tarik skrip `BoonDoor.cs` ke pintu tersebut.
4. Di Inspector pintu tersebut, atur **Door Reward Type** (Lontara/Batak/Kawi).
5. *(Opsional)* Jika pintu ini punya "saingan" di lorong sebelahnya, masukkan objek pintu sebelahnya ke kolom **Other Doors To Lock**. (Jadi jika pintu ini dilewati Saka, pintu saingannya akan hilang/tertutup permanen).

### D. Setup Ruangan (Room Manager)
Manajer ini yang bertugas mendeteksi apakah musuh sudah mati semua.
1. Buat Objek Kosong (*Create Empty*) di tengah ruangan tempur, beri nama `Room_Manager`.
2. Pasangkan skrip `RoomManager.cs`.
3. Masukkan semua objek musuh (misal: Jamur, Gana) yang ada di ruangan itu ke kolom **Enemies In Room**.
4. Masukkan objek Pintu (dari Langkah C) ke kolom **Exit Doors**. 
*(Pintu-pintu ini awalnya akan disembunyikan/dikunci oleh sistem, dan baru akan muncul/terbuka setelah seluruh musuh di dalam list mati).*

### E. Setup Arca/Benda Bersejarah (Jurnal Edukasi)
Sistem interaksi untuk menyimpan sejarah monster ke dalam buku.
1. Pasang skrip `LoreInteractable.cs` ke objek visual arca/patung di peta Anda.
2. Tambahkan `BoxCollider2D` dan centang **Is Trigger**.
3. Masukkan data mitologi (`LoreData`) yang sudah Anda buat di langkah A ke kolom **Lore Data To Unlock**.
4. Buat objek kosong (misalnya bernama `Global_Managers`), lalu pasangkan skrip `JournalManager.cs`. Ini akan bertugas men-*save* progres bacaan pemain secara permanen ke hardisk.

---
*Catatan: UI Canvas untuk Pemilihan 3 Boon dan UI Buku Jurnal sengaja belum dikaitkan secara penuh karena Anda belum memiliki Aset/Kerangka Canvas-nya di Unity. Sistem saat ini diatur untuk mengambil pilihan pertama secara otomatis (Auto-Pick) atau menampilkannya di Console untuk tujuan pengujian (Testing).*
