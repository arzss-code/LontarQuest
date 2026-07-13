# Panduan Dokumentasi Stage 2: Musuh, UI, & Misi

Folder ini berisi seluruh dokumen rancangan dan panduan langkah kerja untuk pengembangan **Stage 2** di *LontarQuest*. Dokumen-dokumen ini diorganisasikan secara berurutan agar mempermudah pemahaman alur kerja dari tahap awal hingga pengujian akhir.

---

## 🗺️ Indeks Dokumen Pengerjaan

Silakan baca dan ikuti dokumen-dokumen berikut sesuai urutan tahap kerja Anda:

### 1. 📐 Rancangan Sistem & Desain
* **[Stage2_Enemy_Design.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Enemy_Design.md)**
  * **Deskripsi**: Dokumen spesifikasi teknis musuh Dwarapala, Mini Boss, dan Bos Yaksa. Berisi detail stats, perilaku AI, serta diagram arsitektur Animator Controller Hibrida (2D Blend Tree).
* **[Stage2_Quest_Task_Implementation.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Quest_Task_Implementation.md)**
  * **Deskripsi**: Cetak biru sistem misi hibrida yang menggabungkan trigger ruangan spasial dengan info jumlah musuh numerik di HUD (misal: "Kalahkan musuh 0/X").

---

### 2. 🎬 Langkah Kerja Editor: Sprite & Animasi (Tahap 1)
* **[Stage2_Tahap1_Sprite_Animasi.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Tahap1_Sprite_Animasi.md)**
  * **Deskripsi**: Panduan detail memotong sprite sheet di Sprite Editor (konfigurasi PPI 250), merekam klip animasi arah, menyusun parameter `MoveX`/`MoveY` pada Blend Tree 2D, serta menghubungkan Animation Events.

---

### 3. 📦 Langkah Kerja Editor: Perakitan Prefab & Level (Tahap 3)
* **[Stage2_Tahap3_Assembly_Level.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Tahap3_Assembly_Level.md)**
  * **Deskripsi**: Langkah merakit GameObject musuh di scene menjadi prefab. Panduan memasang rigidbody, collider trigger, offset hitbox, serta script logika stats dan AI movement (termasuk setting *Heavy Knockback*).

---

### 4. 🔗 Langkah Kerja Editor: Integrasi Misi & UI
* **[Stage2_Unity_Integration_Steps.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Unity_Integration_Steps.md)**
  * **Deskripsi**: Langkah akhir untuk menyusun alur kamar dungeon menggunakan `Stage2RoomManager` baru, menghubungkan `Canvas.prefab` ke `Stage2QuestManager`, merakit arena bos Yaksa, meletakkan peti harta, prasasti lore, dan portal keluar.

---

## 📈 Melacak Kemajuan (Progress Tracking)
Setiap dokumen di atas dilengkapi dengan bagian **Checklist Progress Tracking** di bagian akhir file. Tandai checklist tersebut dengan `[x]` setelah Anda menyelesaikan konfigurasi terkait di Unity Editor untuk memantau kemajuan Anda.
