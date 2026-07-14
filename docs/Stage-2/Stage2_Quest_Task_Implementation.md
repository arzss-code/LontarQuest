# Dokumen Implementasi: Sistem Quest & Task Stage 2

Dokumen ini merupakan kelanjutan dari `docs/Stage2_Enemy_Design.md` yang merinci analisis sistem quest dan objektif misi dari Stage 1 dan Stage 3, serta cetak biru (*blueprint*) implementasinya di **Stage 2** menggunakan pendekatan hibrida.

---

## 1. Analisis Sistem Misi Eksisting

### 1.1 Stage 1: Room-Trigger System (Linear & Spasial)
* **Pemicu**: Player menginjak trigger fisik ruangan (`RoomManager`).
* **Alur**: Kunci Pintu → Aktifkan Musuh → Ubah Teks Quest (string statis) → Semua Musuh Mati (Null Check di List) → Buka Pintu → Teks Quest Selesai.
* **Kelebihan**: Sangat dinamis, bekerja berdasarkan posisi spasial player. Sangat cocok untuk struktur roguelite bercabang.
* **Kekurangan**: Tidak ada umpan balik jumlah musuh (misal: hanya bertuliskan "Kalahkan semua musuh", tanpa info sisa musuh).

### 1.2 Stage 3: Queue Objective System (Structured & Data-Driven)
* **Pemicu**: Progress numerik global (`Stage3QuestManager` dengan daftar `ObjectiveData[]`).
* **Alur**: Quest sekuensial terstruktur (Misi 1 → Misi 2 → Misi 3). Setiap musuh mati akan memanggil `RegisterEnemyKill()` untuk menambahkan progress angka.
* **Kelebihan**: Tampilan informatif berupa counter kemajuan, misalnya: *"Kalahkan musuh (0/5)"*.
* **Kekurangan**: Sangat linear dan kaku. Jika diterapkan pada dungeon Stage 2 yang memiliki banyak ruangan dengan urutan pembersihan bebas, sistem antrean linear ini akan rusak.

---

## 2. Desain Solusi Hibrida untuk Stage 2

Untuk menggabungkan kelebihan dari kedua sistem di atas, Stage 2 menggunakan **Sistem Hibrida**:
* **Struktur Spasial**: Tetap menggunakan pemicu ruangan (Room-Trigger) agar urutan pembersihan ruangan bebas dipilih oleh player.
* **Umpan Balik Numerik**: Menambahkan fitur hitungan sisa musuh (*numerical counter progress*) pada HUD seperti di Stage 3 setiap kali player memasuki ruangan bertarung.

---

## 3. Cetak Biru Kode C# (Stage 2)

Agar tidak memodifikasi script global (`QuestManager.cs` dan `RoomManager.cs`), kita akan membuat script baru khusus untuk Stage 2 dengan menerapkan pewarisan polimorfis (OOP Polymorphism).

### 3.1 Script: `Stage2QuestManager.cs` (Turunan `QuestManager`)
Script ini dipasang pada Canvas di Stage 2 untuk menggantikan QuestManager biasa. Karena mewarisi `QuestManager`, script ini langsung dikenali oleh sistem global lain saat memanggil `QuestManager.Instance`.

```csharp
using UnityEngine;

public class Stage2QuestManager : QuestManager
{
    private string activeBaseText = "";
    private int currentTargetCount = 0;
    private int currentProgressCount = 0;

    /// <summary>
    /// Memulai objektif misi dengan perhitungan angka progress
    /// </summary>
    public void StartProgressObjective(string description, int targetCount)
    {
        activeBaseText = description;
        currentTargetCount = targetCount;
        currentProgressCount = 0;
        UpdateProgressUI();
    }

    /// <summary>
    /// Memperbarui jumlah kemajuan musuh mati
    /// </summary>
    public void SetProgress(int currentProgress)
    {
        if (currentTargetCount <= 0) return;
        currentProgressCount = Mathf.Clamp(currentProgress, 0, currentTargetCount);
        UpdateProgressUI();
    }

    private void UpdateProgressUI()
    {
        // Contoh: "Kalahkan Dwarapala Penjaga (2/5)"
        string progressText = $"{activeBaseText} ({currentProgressCount}/{currentTargetCount})";
        SetObjective(progressText);
    }
}
```

### 3.2 Script: `Stage2RoomManager.cs` (Dedicated Room Manager)
Menggantikan `RoomManager` global di Stage 2 agar kita bisa menambahkan perhitungan musuh secara aman.

```csharp
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class Stage2RoomManager : MonoBehaviour
{
    [Header("Door Configuration")]
    [SerializeField] private GameObject entryDoor;
    [SerializeField] private GameObject[] exitDoors;
    [SerializeField] private GameObject[] nextRoomBlockades;

    [Header("Visual Direction Guide")]
    [SerializeField] private GameObject guideParticlePrefab;

    [Header("Enemies in Room")]
    [SerializeField] private List<GameObject> enemiesInRoom = new List<GameObject>();

    [Header("Quest Setup")]
    [SerializeField] private string questOnEnter = "Bersihkan Ruangan";
    [SerializeField] private string questOnClear = "Temukan Jalan Keluar";

    private bool isRoomActive = false;
    private bool isRoomCleared = false;
    private int initialEnemyCount = 0;
    private int lastDeadCount = -1;

    private void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(false);
        }

        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(true);
            }
        }

        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null) enemy.SetActive(false);
        }

        initialEnemyCount = enemiesInRoom.Count;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isRoomActive && !isRoomCleared)
        {
            ActivateRoom();
        }
    }

    private void ActivateRoom()
    {
        isRoomActive = true;
        if (entryDoor != null) entryDoor.SetActive(true);

        foreach (GameObject enemy in enemiesInRoom)
        {
            if (enemy != null) enemy.SetActive(true);
        }

        // Jalankan UI Counter Misi jika QuestManager adalah tipe Stage2
        if (QuestManager.Instance is Stage2QuestManager s2Quest && initialEnemyCount > 0)
        {
            s2Quest.StartProgressObjective(questOnEnter, initialEnemyCount);
        }
        else if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective(questOnEnter);
        }
    }

    private void Update()
    {
        if (isRoomActive && !isRoomCleared)
        {
            // Hitung jumlah musuh yang masih hidup
            int currentAlive = 0;
            for (int i = enemiesInRoom.Count - 1; i >= 0; i--)
            {
                if (enemiesInRoom[i] != null) currentAlive++;
            }

            int deadCount = initialEnemyCount - currentAlive;

            // Update UI progress hanya saat ada perubahan angka
            if (deadCount != lastDeadCount)
            {
                lastDeadCount = deadCount;
                if (QuestManager.Instance is Stage2QuestManager s2Quest)
                {
                    s2Quest.SetProgress(deadCount);
                }
            }

            if (currentAlive == 0)
            {
                RoomCleared();
            }
        }
    }

    private void RoomCleared()
    {
        isRoomCleared = true;
        isRoomActive = false;

        if (entryDoor != null) entryDoor.SetActive(false);

        foreach (GameObject door in exitDoors)
        {
            if (door != null) door.SetActive(true);
        }

        if (nextRoomBlockades != null)
        {
            foreach (GameObject blockade in nextRoomBlockades)
            {
                if (blockade != null) blockade.SetActive(false);
            }
        }

        if (guideParticlePrefab != null)
        {
            SpawnGuideParticle();
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.SetObjective(questOnClear);
        }
    }

    private void SpawnGuideParticle()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Transform targetDoor = null;
        if (exitDoors != null && exitDoors.Length > 0 && exitDoors[0] != null)
        {
            targetDoor = exitDoors[0].transform;
        }
        else if (nextRoomBlockades != null && nextRoomBlockades.Length > 0 && nextRoomBlockades[0] != null)
        {
            targetDoor = nextRoomBlockades[0].transform;
        }

        if (targetDoor != null)
        {
            GameObject guide = Instantiate(guideParticlePrefab, player.transform.position, Quaternion.identity);
            GuideParticle script = guide.GetComponent<GuideParticle>();
            if (script != null)
            {
                script.Init(targetDoor);
            }
        }
    }
}
```

---

## 4. Alur Integrasi di Unity Editor (Stage 2)

Untuk merakit sistem misi hibrida ini di scene `Stage2.unity`:

1. **Ganti QuestManager**:
   * Pada objek **`QuestManager`** di scene (yang Anda buat pada panduan UI sebelumnya), **Remove Component** `QuestManager` bawaan.
   * Tambahkan komponen baru **`Stage2QuestManager`** ke objek tersebut.
   * Hubungkan slot referensi `Quest Panel` dan `Objective Text` dari Canvas ke script baru tersebut.
2. **Penyusunan Ruangan Dungeon**:
   * Di scene Stage 2, setiap ruangan (Room) dipasangi collider trigger dengan komponen **`Stage2RoomManager`** (bukan `RoomManager` biasa).
   * Drag pintu masuk, pintu hadiah, dan blokade jalan keluar ke slot masing-masing.
   * Daftarkan seluruh musuh di ruangan tersebut ke dalam list **`Enemies in Room`**.
   * Isi kolom teks **`Quest On Enter`** (misal: *"Kalahkan Pengawal Candi"*) dan **`Quest On Clear`** (misal: *"Lanjutkan Penyelidikan"*).

---

## 5. Checklist Progress Tracking (Quest & Room Implementation)

Gunakan checklist ini untuk melacak pembuatan kode dan setup di Editor:

- [x] Berkas kode `Stage2QuestManager.cs` berhasil dibuat.
- [x] Berkas kode `Stage2RoomManager.cs` berhasil dibuat.
- [ ] Ganti `QuestManager` bawaan di Canvas dengan `Stage2QuestManager` di scene.
- [ ] Hubungkan referensi TMP dan panel Canvas ke `Stage2QuestManager`.
- [ ] Pasang `Stage2RoomManager` di setiap Room trigger collider di scene.
- [ ] Daftarkan list GameObject musuh ke masing-masing `Stage2RoomManager`.
- [ ] Set string `Quest On Enter` dan `Quest On Clear` di setiap Room Manager.
- [ ] Lakukan verifikasi runtime: pastikan hitungan musuh (0/X) di HUD berjalan naik saat musuh terbunuh.
