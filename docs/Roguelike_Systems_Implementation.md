# Panduan Implementasi Sistem Roguelike & Jurnal LontarQuest untuk Pemula

Dokumen ini berisi panduan teknis yang dijelaskan langkah demi langkah (*step-by-step*) khusus untuk pemula. Panduan ini akan memandu Anda merakit sistem *Boon* (Hadiah Kekuatan) dan *Jurnal Nusantara* (Koleksi Mitologi) di Unity Editor dengan sangat detail.

---

## Urutan Implementasi (Wajib Berurutan)
Lakukan urutan perakitan ini agar tidak terjadi *error* di Unity:
1. Buat Data Master Dulu
2. Pasang Skrip ke Karakter Saka
3. Bangun Ruangan (Room Manager)
4. Pasang Pintu Hadiah (Boon Door)
5. Pasang Arca Pemicu Cerita (Lore)
6. Pasang Manajer Global

---

## Detail Langkah Eksekusi di Unity

### A. Membuat Data Aksara & Mitologi (ScriptableObject)
Kita akan membuat "cetakan" data kekuatan dan data buku cerita. Anda **tidak perlu ngoding** sama sekali di bagian ini.

**1. Membuat Folder Data**
- Di bagian bawah layar Unity, cari tab **Project** (tempat semua aset berada).
- Buka folder `Assets`.
- Klik kanan di area kosong -> pilih **Create** -> **Folder**. Beri nama folder tersebut `Data`.

**2. Membuat Data Kekuatan (Boon)**
- Masuk ke folder `Data` yang baru dibuat.
- Klik kanan di area kosong -> pilih **Create** -> **LontarQuest** -> **Boon Data**.
- Beri nama file baru ini, misalnya `Boon_KecepatanLontara`.
- Klik file tersebut. Di sebelah kanan layar (tab **Inspector**), isilah datanya:
  - `Boon Name`: Tulis nama kekuatan (misal: Lari Kencang Lontara).
  - `Boon Icon`: (Opsional) Tarik gambar ikon dari tab Project ke kotak ini.
  - `Type`: Pilih `Lontara`, `Batak`, atau `Kawi`.
  - `Stat Modifiers`: Ubah angka `Size` menjadi 1, lalu tekan Enter. Akan muncul *Element 0*. Isi `Stat Type` dengan `MoveSpeed`, dan `Value` dengan angka bonus kecepatannya (misal: 2).

**3. Membuat Data Buku Mitologi (Lore)**
- Masih di folder `Data`, klik kanan -> **Create** -> **LontarQuest** -> **Lore Data**.
- Beri nama filenya, misal `Lore_KepalaKala`.
- Klik file tersebut, lihat ke **Inspector**:
  - `Lore ID`: Tulis ID unik tanpa spasi, misal `monster_kepalakala`.
  - `Title`: Nama monster/karakter (Kepala Kala).
  - `Description`: Tulis kisah/mitologi monster tersebut selengkap mungkin.
  - `Lore Image`: (Opsional) Tarik dan masukkan gambar monster.

---

### B. Memasang Sistem ke Player (Saka)
Saka perlu dipasang sistem agar bisa menerima hadiah kekuatan dari ruangan.

1. Di tab **Project**, cari *Prefab* Saka (biasanya di folder `Assets/Prefabs/` atau tempat Anda menyimpannya).
2. Klik ganda (*Double Click*) file Saka tersebut untuk masuk ke dalam layar biru (*Prefab Mode*).
3. Di tab **Hierarchy** (kiri atas), pastikan objek Saka terpilih.
4. Lihat ke tab **Inspector** (kanan). Tarik layar (*scroll*) ke paling bawah, lalu klik tombol **Add Component**.
5. Ketik `PlayerModifier`, lalu klik skrip tersebut untuk menambahkannya ke Saka.
6. Keluar dari *Prefab Mode* dengan mengklik tombol panah kiri `<` (Back) di pojok kiri atas jendela Hierarchy.

---

### C. Membuat Pintu Lorong (Memilih Jalur Hadiah)
Ini adalah pintu yang akan dilalui Saka setelah mengalahkan musuh di dalam sebuah ruangan tempur. Pintu ini yang akan menyuntikkan kekuatan ke Saka.

1. Di tab **Hierarchy**, klik kanan -> **2D Object** -> **Sprites** -> **Square** (atau jika Anda punya gambar pintu sendiri, tarik ke Scene). Beri nama objek ini `Pintu_Lontara`.
2. Di **Inspector**, klik **Add Component**, ketik dan tambahkan `BoxCollider2D`.
3. Di dalam BoxCollider2D, **WAJIB centang kotak `Is Trigger`**. (Ini agar Saka bisa menembus pintu, bukan menabraknya dan mentok seperti tembok).
4. Posisikan pintu ini di ujung lorong keluar ruangan Anda.
5. Klik **Add Component** lagi, ketik dan tambahkan skrip `BoonDoor`.
6. Di komponen `Boon Door (Script)`, ubah **Door Reward Type** menjadi `Lontara`, `Batak`, atau `Kawi` sesuai jenis pintunya.
7. *(Opsional: Lakukan Langkah 1-6 untuk membuat pintu sebelahnya (misal `Pintu_Batak`) jika ruangan ini punya jalan bercabang dua)*.

---

### D. Mengatur Ruangan & Musuh (Room Manager)
Sistem ini bertugas mengunci pintu di awal, mengawasi semua musuh, dan baru akan membuka pintu jika semua musuh mati.

1. Di **Hierarchy**, klik kanan area kosong -> **Create Empty**. Beri nama `Room_1_Manager`.
2. Posisikan objek kosong ini di tengah-tengah ruangan tempur tersebut.
3. Klik objek `Room_1_Manager`, lalu di **Inspector** klik **Add Component**, ketik dan tambahkan skrip `RoomManager`.
4. **Memasukkan Musuh:**
   - Masih di Inspector `Room_1_Manager`, cari bagian **Enemies In Room**.
   - Klik tanda panah kecil di sebelahnya untuk membuka daftar. Ubah angka `Size` sesuai jumlah musuh yang ada di dalam ruangan itu (misal: 3). Tekan Enter.
   - Tarik objek musuh satu per satu dari tab **Hierarchy** ke kotak *Element 0*, *Element 1*, dan *Element 2*.
5. **Memasukkan Pintu:**
   - Cari bagian **Exit Doors** di bawahnya.
   - Ubah `Size` sesuai jumlah pintu keluar ruangan tersebut (misal: 2, jika ada jalan cabang).
   - Tarik objek `Pintu_Lontara` dan `Pintu_Batak` (dari Langkah C) dari tab **Hierarchy** ke dalam kotak Element pintu tersebut.
6. Selesai! Sekarang saat game dimulai, pintu-pintu tersebut akan otomatis hilang, dan baru akan muncul/terbuka setelah 3 musuh tadi dikalahkan.

---

### E. Memasang Prasasti/Arca (Buku Jurnal Edukasi)
Ini adalah objek yang jika disentuh oleh Saka, akan membuka cerita mitologi baru di halaman Jurnal.

1. Masukkan gambar arca/buku/prasasti ke dalam Scene. Beri nama objeknya `Prasasti_KepalaKala`.
2. Klik `Prasasti_KepalaKala` di **Hierarchy**. Di **Inspector**, tambahkan komponen **BoxCollider2D** dan **centang `Is Trigger`**.
3. Di Scene view, klik tombol *Edit Collider* (ikon kotak dengan 4 titik di komponen BoxCollider2D) dan perbesar kotak hijaunya agar menutupi area di sekitar prasasti. (Saka harus menyentuh area hijau ini untuk berinteraksi).
4. Klik **Add Component**, tambahkan skrip `LoreInteractable`.
5. Di Inspector skrip tersebut, cari kolom **Lore Data To Unlock**.
6. Tarik file data `Lore_KepalaKala` (yang dibuat di Langkah A.3) dari folder `Data` ke dalam kolom tersebut.

---

### F. Menyambungkan Sistem Global (Game Manager)
Langkah terakhir yang sangat penting agar buku jurnal bisa dibaca dan progres bacaan pemain tersimpan permanen walau game ditutup.

1. Di **Hierarchy**, klik kanan area kosong -> **Create Empty**. Beri nama `Global_Managers`.
2. Klik objek `Global_Managers`, di **Inspector** klik **Add Component**, tambahkan skrip `JournalManager`.
3. Klik **Add Component** lagi, tambahkan skrip `BoonUIManager`.

---

🎉 **Selesai! Anda siap menguji coba (Playtest) sistemnya.**
- Coba mainkan (tekan tombol *Play*).
- Pukul/kalahkan semua musuh di dalam ruangan yang sudah disetel `RoomManager`-nya. Pintu akan otomatis terbuka.
- Jalan melewati pintu tersebut, Saka akan langsung menerima efek *Boon* (bonus status).
- Sentuh arca/prasasti di ujung jalan, dan Jurnal Kepala Kala akan terbuka!
*(Catatan: Sistem UI/Canvas visual di layar belum terhubung sepenuhnya, tapi sistem dasarnya sudah berjalan di balik layar dan bisa dicek lewat jendela Console Unity)*.
