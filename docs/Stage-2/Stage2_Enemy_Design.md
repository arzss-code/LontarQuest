# Desain Monster Stage 2: Dwarapala & Yaksa

Dokumen ini merincikan desain dan implementasi teknis untuk 3 tipe monster pada **Stage 2: Perpustakaan Melayang**, termasuk sprite setup, animasi, AI behavior, stat balancing, dan integrasi ke scene.

---

## Daftar Monster

| Tipe                 | Nama            | Peran                  | Serangan                             | Referensi Visual                              |
| -------------------- | --------------- | ---------------------- | ------------------------------------ | --------------------------------------------- |
| **Kroco**      | Dwarapala Kecil | Musuh reguler (Tanker) | Melee AoE — Palu Gada               | `Assets/Arts/Enemies/Stage2-Dwarapala/`     |
| **Mini Boss**  | Dwarapala Besar | Mini Boss per-ruangan  | Melee AoE — Palu Gada (lebih kuat)  | Sama, skala 1.5x + tint**ungu**         |
| **Boss Utama** | Yaksa           | Boss akhir Stage 2     | Ranged — Panah Energi + Laser Trail | `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/` |

---

## 1. Dwarapala Kroco (Musuh Reguler)

### 1.1 Deskripsi

Raksasa batu penjaga candi berwajah menyeramkan yang membawa Gada (palu besar). Bergerak lambat tapi sangat teritorial. Ketika pemain memasuki area deteksinya, Dwarapala akan mengejar dan melancarkan serangan AoE menghantam tanah yang berdampak di area sekitarnya.

### 1.2 Sprite & Animasi

**Sumber sprite:** `Assets/Arts/Enemies/Stage2-Dwarapala/Sprites/`

#### Idle (3 frame — `Idle.png`)

| Sprite     | Arah                       | Penggunaan                                                |
| ---------- | -------------------------- | --------------------------------------------------------- |
| `Idle_0` | Front (menghadap kamera)   | Idle saat tidak bergerak, arah bawah                      |
| `Idle_1` | Back (membelakangi kamera) | Idle arah atas                                            |
| `Idle_2` | Side Right                 | Idle arah kanan. Untuk arah kiri, gunakan`flipX = true` |

#### Walk (12 frame — `Walk.png`)

Spritesheet terbagi dalam 3 baris × 4 kolom, setiap baris merupakan 1 siklus walk loop:

| Baris            | Sprite                    | Arah                 | Frame                                                    |
| ---------------- | ------------------------- | -------------------- | -------------------------------------------------------- |
| Baris 1 (atas)   | `Walk_0` → `Walk_3`  | **Front Walk** | 4 frame jalan menghadap kamera                           |
| Baris 2 (tengah) | `Walk_4` → `Walk_7`  | **Back Walk**  | 4 frame jalan membelakangi kamera                        |
| Baris 3 (bawah)  | `Walk_8` → `Walk_11` | **Side Walk**  | 4 frame jalan ke samping.`flipX` untuk arah berlawanan |

#### Attack & Death (`AttackAndDeath.png`)

> **Penting:** Spritesheet ini berisi beberapa frame noise kecil (< 100 pixel). Hanya gunakan frame utama berikut:

**Attack (6 frame — 2 arah × 3 frame):**

| Arah                   | Frame                                       | Deskripsi Gerakan                                                          |
| ---------------------- | ------------------------------------------- | -------------------------------------------------------------------------- |
| **Front Attack** | `AttackAndDeath_9` → `_10` → `_11`  | Angkat gada → ayunkan ke samping → hantam tanah (AoE)                    |
| **Side Attack**  | `AttackAndDeath_12` → `_13` → `_17` | Angkat gada ke atas → ayunkan ke depan → hantam tanah dengan efek impact |

**Death (4 frame):**

| Frame                | Deskripsi                        |
| -------------------- | -------------------------------- |
| `AttackAndDeath_0` | Dwarapala terhuyung, mulai retak |
| `AttackAndDeath_1` | Jatuh berlutut, retakan membesar |
| `AttackAndDeath_2` | Ambruk ke tanah, batu pecah      |
| `AttackAndDeath_3` | Hancur total, tersisa puing batu |

#### Daftar Animation Clip (.anim)

Simpan di: `Assets/Arts/Enemies/Stage2-Dwarapala/Animations/`

| Nama Clip                 | Loop | FPS | Frame              | Keterangan                 |
| ------------------------- | ---- | --- | ------------------ | -------------------------- |
| `Dwarapala_IdleFront`   | ✅   | 1   | Idle_0             | menghadap depan            |
| `Dwarapala_IdleBack`    | ✅   | 1   | Idle_1             | menghadap belakang         |
| `Dwarapala_IdleLeft`    | ✅   | 1   | Idle_2             | side-view, flipX = ✅      |
| `Dwarapala_IdleRight`   | ✅   | 1   | Idle_2             | side-view, flipX = ❌      |
| `Dwarapala_WalkFront`   | ✅   | 8   | Walk_0 → Walk_3   | jalan depan                |
| `Dwarapala_WalkBack`    | ✅   | 8   | Walk_4 → Walk_7   | jalan belakang             |
| `Dwarapala_WalkLeft`    | ✅   | 8   | Walk_8 → Walk_11  | jalan samping, flipX = ✅  |
| `Dwarapala_WalkRight`   | ✅   | 8   | Walk_8 → Walk_11  | jalan samping, flipX = ❌  |
| `Dwarapala_AttackFront` | ❌   | 8   | AtkFront_0 → _2   | hantam depan               |
| `Dwarapala_AttackLeft`  | ❌   | 8   | AtkSide_0 → _2    | hantam samping, flipX = ✅ |
| `Dwarapala_AttackRight` | ❌   | 8   | AtkSide_0 → _2    | hantam samping, flipX = ❌ |
| `Dwarapala_Death`       | ❌   | 6   | Death_0 → Death_3 | hancur                     |

> FPS 8 memberikan nuansa berat dan lambat yang cocok untuk karakter tanker.

#### Animator Controller: `DwarapalaController.controller`

**Parameter Animator:**

| Parameter  | Tipe    | Keterangan                         |
| ---------- | ------- | ---------------------------------- |
| `MoveX`  | Float   | Arah pergerakan/hadapan sumbu X    |
| `MoveY`  | Float   | Arah pergerakan/hadapan sumbu Y    |
| `Speed`  | Float   | 0 = idle, > 0 = walking            |
| `Attack` | Trigger | Memicu state Attack                |
| `Die`    | Trigger | Memicu state Death (dari AnyState) |

**Arsitektur: Hibrida (Blend Tree 2D + State Biasa)**

> **PENTING**: Gerakan dasar (`Idle` dan `Walk`) menggunakan **2D Simple Directional Blend Tree** dengan parameter `MoveX` dan `MoveY` untuk perpindahan arah 360 derajat yang instan dan bebas glitch. Aksi tempur (`Attack`) menggunakan **State terpisah per arah** agar *Animation Events* terpanggil secara stabil.

**State Machine:**

```
Entry → Idle (Default, 2D Blend Tree)

Idle → Walk             [kondisi: Speed > 0.1, Has Exit Time: ❌, Duration: 0]
Walk → Idle             [kondisi: Speed < 0.1, Has Exit Time: ❌, Duration: 0]

AnyState → Attack_Front [kondisi: trigger "Attack" + MoveX antara -0.5 s/d 0.5, Can Transition To Self: ❌]
AnyState → Attack_Left  [kondisi: trigger "Attack" + MoveX < -0.5, Can Transition To Self: ❌]
AnyState → Attack_Right [kondisi: trigger "Attack" + MoveX > 0.5, Can Transition To Self: ❌]

Attack_Front → Idle     [Has Exit Time: ✅, Exit Time: 1, Duration: 0, Conditions: kosong]
Attack_Left → Idle      [Has Exit Time: ✅, Exit Time: 1, Duration: 0, Conditions: kosong]
Attack_Right → Idle     [Has Exit Time: ✅, Exit Time: 1, Duration: 0, Conditions: kosong]

AnyState → Death        [kondisi: trigger "Die", Can Transition To Self: ❌]
```

**Blend Tree — Idle (2D Simple Directional, parameter: MoveX, MoveY):**

| Position (X, Y) | Motion                       | Keterangan |
| --------------- | ---------------------------- | ---------- |
| `(0, -1)`     | `Dwarapala_IdleFront.anim` | Down       |
| `(0, 1)`      | `Dwarapala_IdleBack.anim`  | Up         |
| `(-1, 0)`     | `Dwarapala_IdleLeft.anim`  | Left       |
| `(1, 0)`      | `Dwarapala_IdleRight.anim` | Right      |

**Blend Tree — Walk (2D Simple Directional, parameter: MoveX, MoveY):**

| Position (X, Y) | Motion                       | Keterangan |
| --------------- | ---------------------------- | ---------- |
| `(0, -1)`     | `Dwarapala_WalkFront.anim` | Down       |
| `(0, 1)`      | `Dwarapala_WalkBack.anim`  | Up         |
| `(-1, 0)`     | `Dwarapala_WalkLeft.anim`  | Left       |
| `(1, 0)`      | `Dwarapala_WalkRight.anim` | Right      |

**State Attack (Non-Blend Tree, State Biasa):**

| State            | Motion                         | Keterangan                                                              |
| ---------------- | ------------------------------ | ----------------------------------------------------------------------- |
| `Attack_Front` | `Dwarapala_AttackFront.anim` | Dipicu jika MoveX berada di rentang [-0.5, 0.5] (arah vertikal dominan) |
| `Attack_Left`  | `Dwarapala_AttackLeft.anim`  | Dipicu jika MoveX < -0.5 (arah kiri dominan)                            |
| `Attack_Right` | `Dwarapala_AttackRight.anim` | Dipicu jika MoveX > 0.5 (arah kanan dominan)                            |

**Animation Events pada clip Attack:**

| Event           | Frame                                      | Method              |
| --------------- | ------------------------------------------ | ------------------- |
| `OnAttackHit` | Frame terakhir (saat gada menyentuh tanah) | Spawn AoE damage    |
| `OnAttackEnd` | Setelah frame terakhir                     | Reset state ke Idle |

### 1.3 AI Behavior

**Tipe:** Chase & Smash

```
State Machine:
  IDLE → (player terdeteksi) → CHASE
  CHASE → (dalam jangkauan serang) → CHARGING
  CHARGING → (charge selesai) → ATTACK
  ATTACK → (cooldown) → CHASE / IDLE
  * → (HP 0) → DEATH
  CHASE → (terlalu jauh dari spawn) → RETURN → IDLE
```

**Detail Behavior:**

| Parameter           | Nilai            | Keterangan                                                       |
| ------------------- | ---------------- | ---------------------------------------------------------------- |
| `detectionRadius` | 7f               | Jarak deteksi pemain                                             |
| `moveSpeed`       | 2f               | Lambat (karakter tanker)                                         |
| `stopDistance`    | X: 1.2f, Y: 1.8f | Jarak berhenti (diatur per sumbu untuk mengatasi tabrakan fisik) |
| `attackRange`     | X: 1.5f, Y: 2.0f | Jangkauan serangan melee (diatur per sumbu)                      |
| `attackCooldown`  | 2.5s             | Jeda antar serangan                                              |
| `leashRadius`     | 10f              | Jarak maks dari titik spawn sebelum kembali                      |
| `chargeTime`      | 0.4s             | Delay sebelum serangan (angkat gada)                             |

**Mekanisme Serangan (Palu Gada AoE):**

1. Dwarapala berhenti bergerak, mengunci arah menghadap pemain
2. Trigger animasi Attack (angkat gada)
3. Animation Event `OnAttackHit` → `Physics2D.OverlapCircleAll()`:
   - **Radius AoE:** 2f (lingkaran di depan Dwarapala)
   - **Layer:** Player
   - **Damage:** 25
   - **Efek:** Knockback ke arah berlawanan dari Dwarapala
4. Animation Event `OnAttackEnd` → kembali ke state Chase/Idle

### 1.4 Statistik

| Stat            | Nilai |
| --------------- | ----- |
| Max HP          | 80    |
| Damage          | 25    |
| AoE Radius      | 2f    |
| Move Speed      | 2f    |
| Attack Cooldown | 2.5s  |
| Knockback Force | 5f    |
| Stun Duration   | —    |

### 1.5 Prefab Setup

**Nama Prefab:** `Enemy_Dwarapala`
**Lokasi:** `Assets/Prefab/Enemy_Dwarapala.prefab`

```
Enemy_Dwarapala (GameObject)
│  Tag: "Enemy"
│  Layer: Enemy
│
├── Components:
│   ├── SpriteRenderer
│   │     Sprite: Idle_0
│   │     Sorting Layer: Default
│   │     Order in Layer: 5
│   │
│   ├── Animator
│   │     Controller: DwarapalaController
│   │
│   ├── Rigidbody2D
│   │     Body Type: Kinematic
│   │     Freeze Rotation: Z ✅
│   │
│   ├── BoxCollider2D
│   │     Is Trigger: false
│   │     Size: fit body Dwarapala
│   │
│   ├── CircleCollider2D
│   │     Is Trigger: true
│   │     Radius: 0.5
│   │     (untuk deteksi hit dari PlayerAttackHitbox)
│   │
│   ├── Stage2EnemyStats
│   │     maxHP: 80
│   │     isBoss: false
│   │
│   ├── Stage2EnemyMovement
│   │     mode: Chase
│   │     moveSpeed: 2
│   │     detectionRadius: 7
│   │     stopDistance: X: 1.2, Y: 1.8
│   │     leashRadius: 10
│   │
│   ├── Stage2EnemyAttack
│   │     attackType: MeleeAoE
│   │     damage: 25
│   │     attackRange: X: 1.5, Y: 2.0
│   │     attackCooldown: 2.5
│   │     meleeRadius: 2.0
│   │     meleeHitboxOffset: X: 0.8, Y: 0.5
│   │     bodyCenterOffset: X: 0, Y: 0.7
│   │
│   ├── Stage2EnemyAnimator
│   └── Stage2AnimationRelay
│
└── Children:
    └── AimPoint (Empty GameObject)
          Position: (0, 0.5, 0) — titik bidik ArrowProjectile pemain
```

---

## 2. Dwarapala Mini Boss

### 2.1 Deskripsi

Versi lebih besar dan lebih berbahaya dari Dwarapala reguler. Tubuhnya berwarna **ungu gelap** dengan aura mistis, menandakan kekuatan supernatural yang lebih besar. Menggunakan spritesheet yang sama dengan Dwarapala Kroco tetapi di-scale 1.5x dan diberi tint warna ungu. Serangannya lebih mematikan: radius AoE lebih lebar, damage lebih tinggi, dan menimbulkan **stun** sementara pada pemain serta **camera shake** saat gada menghantam tanah.

### 2.2 Perbedaan Visual

| Properti                 | Kroco              | Mini Boss                                    |
| ------------------------ | ------------------ | -------------------------------------------- |
| `Transform.localScale` | (1, 1, 1)          | **(1.5, 1.5, 1)**                      |
| `SpriteRenderer.color` | (1, 1, 1, 1) White | **(0.6, 0.3, 0.8, 1) Ungu**            |
| Efek Tambahan            | —                 | Particle System aura ungu (opsional)         |
| Health Bar               | Tidak ada          | **Ada** (UI Slider melayang di atas)   |
| Camera Shake saat Attack | Tidak              | **Ya** (durasi 0.2s, intensitas 0.15f) |

**Nilai warna ungu yang disarankan (RGBA):**

- Base tint: `(0.6, 0.3, 0.8, 1.0)` — Ungu medium
- Alternatif gelap: `(0.45, 0.2, 0.65, 1.0)` — Ungu gelap misterius
- Alternatif terang: `(0.7, 0.4, 0.9, 1.0)` — Ungu cerah mencolok

### 2.3 AI Behavior

Sama dengan Dwarapala Kroco, tetapi dengan penambahan:

- **Stun pada pemain:** Saat serangan AoE mengenai pemain, pemain tidak bisa bergerak selama 0.5 detik
- **Camera Shake:** Saat gada menghantam tanah, kamera bergetar (efek dramatis)
- **Lebih lambat tapi lebih tangguh:** Move speed sedikit lebih rendah, HP jauh lebih besar

### 2.4 Statistik

| Stat            | Kroco | Mini Boss      | Perubahan           |
| --------------- | ----- | -------------- | ------------------- |
| Max HP          | 80    | **200**  | +150%               |
| Damage          | 25    | **40**   | +60%                |
| AoE Radius      | 2f    | **3f**   | +50%                |
| Move Speed      | 2f    | **1.8f** | -10% (lebih lambat) |
| Attack Cooldown | 2.5s  | **3.0s** | +20% (lebih lama)   |
| Knockback Force | 5f    | **8f**   | +60%                |
| Stun Duration   | 0s    | **0.5s** | Baru                |
| Camera Shake    | Tidak | **Ya**   | Baru                |

### 2.5 Prefab Setup

**Nama Prefab:** `MiniBoss_Dwarapala`
**Lokasi:** `Assets/Prefab/MiniBoss_Dwarapala.prefab`

Duplikat dari `Enemy_Dwarapala` dengan modifikasi:

```
MiniBoss_Dwarapala (GameObject)
│  Tag: "Enemy"
│  Layer: Enemy
│  Transform.localScale: (1.5, 1.5, 1)    ← BEDA
│
├── Components:
│   ├── SpriteRenderer
│   │     Color: (0.6, 0.3, 0.8, 1.0)     ← BEDA (ungu)
│   │
│   ├── Stage2EnemyStats
│   │     maxHP: 200                        ← BEDA
│   │     isBoss: false
│   │
│   ├── Stage2EnemyMovement
│   │     moveSpeed: 1.8                    ← BEDA
│   │
│   ├── Stage2EnemyAttack
│   │     meleeDamage: 40                   ← BEDA
│   │     aoeDamageRadius: 3                ← BEDA
│   │     attackCooldown: 3.0               ← BEDA
│   │     knockbackForce: 8                 ← BEDA
│   │     stunDuration: 0.5                 ← BARU
│   │     cameraShakeOnHit: true            ← BARU
│   │
│   ├── Stage2EnemyHealthBar              ← BARU
│   │
│   └── (semua komponen lain sama)
│
└── Children:
    ├── AimPoint
    └── HealthBarCanvas (World Space Canvas)  ← BARU
          └── Slider (HP bar)
```

---

## 3. Yaksa — Boss Utama Stage 2

### 3.1 Deskripsi

Roh alam setengah dewa/iblis yang melayang di udara. Yaksa memiliki sayap besar dengan kristal energi biru bercahaya dan membawa busur mistis. Gaya bertarungnya adalah **jarak jauh** — ia menjaga jarak dari pemain dan menembakkan **Panah Energi** yang diikuti jejak laser biru. Jika pemain terlalu dekat, Yaksa akan mundur untuk mempertahankan jarak ideal.

### 3.2 Sprite & Animasi

**Sumber sprite:** `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Sprites/Sprites.png`

Spritesheet berisi 3 baris:

#### Baris 1 — Walk / Semua Arah (9 frame)

Seluruh baris atas adalah animasi **walk** untuk 3 arah. Idle menggunakan sprite walk yang sama dengan FPS lebih lambat.

| Sprite     | Arah                 | Penggunaan                                          |
| ---------- | -------------------- | --------------------------------------------------- |
| Frame 1–2 | **Walk Front** | Yaksa jalan/melayang menghadap kamera (2 frame)     |
| Frame 3–6 | **Walk Side**  | Yaksa jalan/melayang dilihat dari samping (4 frame) |
| Frame 7–9 | **Walk Back**  | Yaksa jalan/melayang membelakangi kamera (3 frame)  |

> Karena Yaksa **melayang**, perbedaan idle dan walk hanya pada kecepatan FPS animasi (idle = 4 fps, walk = 10 fps).

#### Baris 2 — Shoot / Attack (7 frame)

| Frame      | Deskripsi                                                  |
| ---------- | ---------------------------------------------------------- |
| Frame 1    | Posisi siap, busur terangkat, panah energi mulai terbentuk |
| Frame 2    | Tarik tali busur, energi mengumpul                         |
| Frame 3    | Busur tertarik penuh, energi bersinar terang               |
| Frame 4    | Panah dilepas, laser trail terlihat memanjang              |
| Frame 5    | Follow-through, busur kembali                              |
| Frame 6–7 | Recovery, kembali ke posisi idle                           |

#### Baris 3 — Death / Defeated (6 frame)

| Frame      | Deskripsi                                      |
| ---------- | ---------------------------------------------- |
| Frame 1–2 | Yaksa terhuyung, kristal energi meredup        |
| Frame 3    | Tubuh mulai hancur/pecah dari bawah            |
| Frame 4–5 | Ledakan partikel energi biru, tubuh menghilang |
| Frame 6    | Sisa energi menguap, hanya jejak cahaya biru   |

#### Daftar Animation Clip (.anim)

Simpan di: `Assets/Arts/Enemies/Stage2-MakaraOrYaksa/Animations/`

| Nama Clip           | Loop | FPS | Frame               | Catatan                                  |
| ------------------- | ---- | --- | ------------------- | ---------------------------------------- |
| `Yaksa_WalkFront` | ✅   | 10  | Baris 1, frame 1–2 | Walk depan (2 frame)                     |
| `Yaksa_WalkSide`  | ✅   | 10  | Baris 1, frame 3–6 | Walk samping (4 frame, flipX untuk kiri) |
| `Yaksa_WalkBack`  | ✅   | 10  | Baris 1, frame 7–9 | Walk belakang (3 frame)                  |
| `Yaksa_IdleFront` | ✅   | 4   | Baris 1, frame 1–2 | Reuse sprite WalkFront, FPS lambat       |
| `Yaksa_IdleSide`  | ✅   | 4   | Baris 1, frame 3–6 | Reuse sprite WalkSide, FPS lambat        |
| `Yaksa_IdleBack`  | ✅   | 4   | Baris 1, frame 7–9 | Reuse sprite WalkBack, FPS lambat        |
| `Yaksa_Shoot`     | ❌   | 10  | Baris 2, frame 1–7 | Serangan jarak jauh                      |
| `Yaksa_Death`     | ❌   | 8   | Baris 3, frame 1–6 | Animasi mati                             |

> FPS 10 pada Walk/Shoot memberikan feel cepat. FPS 4 pada Idle memberikan kesan "diam melayang".

#### Animator Controller: `YaksaController.controller`

**Parameter Animator:**

| Parameter | Tipe    | Keterangan                         |
| --------- | ------- | ---------------------------------- |
| `MoveX` | Float   | Arah pergerakan/hadapan sumbu X    |
| `MoveY` | Float   | Arah pergerakan/hadapan sumbu Y    |
| `Speed` | Float   | 0 = idle, > 0 = moving             |
| `Shoot` | Trigger | Memicu state Shoot                 |
| `Die`   | Trigger | Memicu state Death (dari AnyState) |

**Arsitektur: Hibrida (Blend Tree 2D + State Biasa)**

**State Machine (7 transition):**

```
Entry → Idle (Default, 2D Blend Tree)

Idle → Walk          [kondisi: Speed > 0.1, Has Exit Time: ❌, Duration: 0]
Walk → Idle          [kondisi: Speed < 0.1, Has Exit Time: ❌, Duration: 0]

AnyState → Shoot_Left  [kondisi: trigger "Shoot" + MoveX < 0, Can Transition To Self: ❌]
AnyState → Shoot_Right [kondisi: trigger "Shoot" + MoveX >= 0, Can Transition To Self: ❌]

Shoot_Left → Idle    [Has Exit Time: ✅, Exit Time: 1, Conditions: kosong]
Shoot_Right → Idle   [Has Exit Time: ✅, Exit Time: 1, Conditions: kosong]

AnyState → Death     [kondisi: trigger "Die", Can Transition To Self: ❌]
```

> Karena Shoot hanya ada 2 clip (Left/Right), tidak perlu Blend Tree — cukup 2 state biasa yang dipilih via parameter `MoveX`.

**Blend Tree — Idle (2D Simple Directional, parameter: MoveX, MoveY):**

| Position (X, Y) | Motion                   | Keterangan |
| --------------- | ------------------------ | ---------- |
| `(0, -1)`     | `Yaksa_IdleFront.anim` | Down       |
| `(0, 1)`      | `Yaksa_IdleBack.anim`  | Up         |
| `(-1, 0)`     | `Yaksa_IdleLeft.anim`  | Left       |
| `(1, 0)`      | `Yaksa_IdleRight.anim` | Right      |

**Blend Tree — Walk (2D Simple Directional, parameter: MoveX, MoveY):**

| Position (X, Y) | Motion                   | Keterangan |
| --------------- | ------------------------ | ---------- |
| `(0, -1)`     | `Yaksa_WalkFront.anim` | Down       |
| `(0, 1)`      | `Yaksa_WalkBack.anim`  | Up         |
| `(-1, 0)`     | `Yaksa_WalkLeft.anim`  | Left       |
| `(1, 0)`      | `Yaksa_WalkRight.anim` | Right      |

**State Shoot (non Blend Tree):**

| State           | Motion                    | Keterangan                                  |
| --------------- | ------------------------- | ------------------------------------------- |
| `Shoot_Left`  | `Yaksa_ShootLeft.anim`  | Dipicu jika MoveX < 0 (arah kiri dominan)   |
| `Shoot_Right` | `Yaksa_ShootRight.anim` | Dipicu jika MoveX >= 0 (arah kanan dominan) |

**Animation Events pada clip Shoot:**

| Event                 | Frame                        | Method                       |
| --------------------- | ---------------------------- | ---------------------------- |
| `OnShootProjectile` | Frame 4 (saat panah dilepas) | Spawn EnergyArrow projectile |
| `OnAttackEnd`       | Frame 7 (akhir recovery)     | Reset state ke Idle          |

### 3.3 AI Behavior

**Tipe:** Keep Distance & Snipe

```
State Machine:
  IDLE → (player terdeteksi) → REPOSITION
  REPOSITION → (jarak ideal tercapai) → AIM
  AIM → (lock target 0.3s) → SHOOT
  SHOOT → (cooldown) → REPOSITION
  REPOSITION → (player terlalu dekat) → RETREAT → REPOSITION
  * → (HP 0) → DEATH
```

**Detail Behavior:**

| Parameter             | Nilai | Keterangan                                  |
| --------------------- | ----- | ------------------------------------------- |
| `detectionRadius`   | 12f   | Deteksi jauh (ranged boss)                  |
| `moveSpeed`         | 2.5f  | Lebih cepat dari Dwarapala (perlu reposisi) |
| `preferredDistance` | 6f    | Jarak ideal untuk menembak                  |
| `retreatDistance`   | 3f    | Jika pemain lebih dekat dari ini → mundur  |
| `retreatSpeed`      | 3f    | Kecepatan mundur (lebih cepat dari maju)    |
| `attackRange`       | 8f    | Jangkauan tembak maksimal                   |
| `attackCooldown`    | 1.8s  | Jeda antar tembakan                         |
| `aimDuration`       | 0.3s  | Lock-on ke pemain sebelum menembak          |
| `leashRadius`       | 15f   | Radius maks dari titik spawn                |

**Pola Gerakan:**

1. **Reposisi:** Yaksa selalu berusaha menjaga jarak `preferredDistance` dari pemain
2. **Mundur:** Jika pemain berlari mendekati dan jarak < `retreatDistance`, Yaksa terbang mundur dengan kecepatan lebih tinggi
3. **Tembak:** Saat jarak ideal, Yaksa berhenti, mengunci arah ke pemain (`aimDuration`), lalu menembakkan Panah Energi
4. **Melayang:** Yaksa selalu sedikit bergoyang naik-turun secara sinusoidal (efek `Mathf.Sin(Time.time)` pada posisi Y) untuk kesan melayang

### 3.4 Senjata: Panah Energi + Laser Trail

Panah Energi Yaksa bukan proyektil biasa — ia berupa **panah bercahaya yang meninggalkan jejak laser panjang** saat terbang.

#### Desain Visual Panah Energi

```
  ──────════════►◆
  Trail Laser    Panah Energi
  (LineRenderer) (Sprite + Glow)
```

**Komponen Utama:**

| Komponen                  | Fungsi                                                                                 |
| ------------------------- | -------------------------------------------------------------------------------------- |
| **Sprite**          | Sprite panah energi biru dari spritesheet Yaksa                                        |
| **Trail Renderer**  | Jejak cahaya biru yang mengikuti panah, fade out sepanjang jalur                       |
| **Point Light 2D**  | Cahaya biru kecil yang mengikuti panah (opsional, untuk URP 2D)                        |
| **Particle System** | Partikel kecil di ujung panah (percikan energi)                                        |
| **Line Renderer**   | Garis laser tipis dari titik tembak ke posisi panah (opsional, untuk efek "terhubung") |

#### Detail Trail Renderer (Efek Laser)

```
Trail Renderer Settings:
  Time: 0.4s              (berapa lama trail bertahan)
  Width:
    Start: 0.15           (tebal di dekat panah)
    End: 0.02              (tipis menghilang)
  Color:
    Start: (0.3, 0.8, 1.0, 0.9)   — Biru cyan terang
    End:   (0.1, 0.3, 0.8, 0.0)    — Biru gelap transparan
  Material: Sprites-Default atau Custom Additive shader
  Min Vertex Distance: 0.1
```

#### Detail Particle System (Percikan Energi di Ujung Panah)

```
Particle System Settings:
  Duration: Lifetime panah
  Emission Rate: 30 particles/sec
  Start Lifetime: 0.3s
  Start Size: 0.05 – 0.1
  Start Color: Biru cyan (0.5, 0.9, 1.0)
  Shape: Cone (angle 15°, arah mundur/berlawanan gerak)
  Renderer: Billboard, Material Additive
  Size over Lifetime: Mengecil
  Color over Lifetime: Fade out
```

#### Script: `EnergyArrow.cs`

```
Properti:
  damage: 15
  speed: 8f
  lifetime: 4s
  homingStrength: 1.5f    (homing ringan ke arah pemain)
  maxHomingAngle: 30°     (batas belok, agar tidak terlalu OP)
  trailRenderer: ref      (Trail Renderer component)
  
Behavior:
  1. Spawn di FirePoint (posisi busur Yaksa)
  2. Arah awal: menuju posisi pemain saat ditembak
  3. Selama terbang: sedikit mengoreksi arah ke pemain (homing ringan)
     - Koreksi dibatasi maxHomingAngle per detik
     - Setelah 60% lifetime, homing dimatikan (terbang lurus)
  4. Trail Renderer otomatis membuat jejak laser
  5. OnTriggerEnter2D:
     - Jika hit "Player" → PlayerStats.TakeDamage(damage), destroy panah
     - Jika hit Wall/Obstacle → destroy panah
  6. Setelah lifetime habis → destroy panah
  
Detach Trail on Destroy:
  - Saat panah di-destroy, Trail Renderer di-detach ke empty GameObject
  - Trail fade out secara natural (Time sudah di-set)
  - Empty GameObject auto-destroy setelah trail selesai fade
```

**Prefab:** `Assets/Prefab/EnergyArrow.prefab`

```
EnergyArrow (GameObject)
│  Layer: Enemy
│
├── Components:
│   ├── SpriteRenderer
│   │     Sprite: Yaksa_Arrow (dari spritesheet)
│   │     Color: (0.5, 0.9, 1.0, 1.0) — tint biru cyan
│   │     Sorting Order: 10
│   │
│   ├── Rigidbody2D
│   │     Body Type: Kinematic
│   │
│   ├── CircleCollider2D
│   │     Is Trigger: true
│   │     Radius: 0.15
│   │
│   ├── EnergyArrow (script)
│   │     damage: 15
│   │     speed: 8
│   │     lifetime: 4
│   │     homingStrength: 1.5
│   │
│   ├── Trail Renderer
│   │     (settings seperti di atas)
│   │
│   └── Particle System (child atau component)
│         (settings percikan energi seperti di atas)
│
└── Children:
    └── GlowLight (Optional)
          Light 2D (Point, warna biru, intensity 0.5, radius 1)
```

### 3.5 Statistik

| Stat               | Nilai        |
| ------------------ | ------------ |
| Max HP             | 350          |
| Projectile Damage  | 15 per panah |
| Projectile Speed   | 8f           |
| Attack Cooldown    | 1.8s         |
| Move Speed         | 2.5f         |
| Retreat Speed      | 3f           |
| Preferred Distance | 6f           |
| Retreat Distance   | 3f           |
| Detection Radius   | 12f          |
| Homing Strength    | 1.5f         |

### 3.6 Prefab Setup

**Nama Prefab:** `Boss_Yaksa`
**Lokasi:** `Assets/Prefab/Boss_Yaksa.prefab`

```
Boss_Yaksa (GameObject)
│  Tag: "Enemy"
│  Layer: Enemy
│
├── Components:
│   ├── SpriteRenderer
│   │     Sprite: Yaksa_IdleFront_0
│   │     Order in Layer: 5
│   │
│   ├── Animator
│   │     Controller: YaksaController
│   │
│   ├── Rigidbody2D
│   │     Body Type: Kinematic
│   │     Freeze Rotation: Z ✅
│   │
│   ├── BoxCollider2D
│   │     Is Trigger: false
│   │     Size: fit body Yaksa
│   │
│   ├── CircleCollider2D
│   │     Is Trigger: true
│   │     Radius: 0.6
│   │
│   ├── Stage2EnemyStats
│   │     maxHP: 350
│   │     isBoss: true
│   │
│   ├── Stage2EnemyMovement
│   │     mode: KeepDistance
│   │     moveSpeed: 2.5
│   │     detectionRadius: 12
│   │     preferredDistance: 6
│   │     retreatDistance: 3
│   │     retreatSpeed: 3
│   │     leashRadius: 15
│   │
│   ├── Stage2EnemyAttack
│   │     attackType: RangedProjectile
│   │     rangedDamage: 15
│   │     rangedRange: 8
│   │     attackCooldown: 1.8
│   │     projectilePrefab: EnergyArrow
│   │     firePoint: ref → FirePoint child
│   │
│   ├── Stage2EnemyAnimator
│   └── Stage2AnimationRelay
│
└── Children:
    ├── AimPoint (Empty GameObject)
    │     Position: (0, 0.5, 0)
    │
    └── FirePoint (Empty GameObject)
          Position: (0.5, 0.3, 0) — posisi tangan/busur tempat panah muncul
```

---

## 4. Boss Arena: Stage2BossArena

### 4.1 Deskripsi

Ruangan khusus untuk pertarungan melawan Yaksa. Menggunakan pola yang sama dengan `BossArenaController` Stage 1 (Kepala Kala).

### 4.2 Alur

```
1. Pemain masuk ke trigger area → LockArena()
   - Aktifkan blockade pintu masuk
   - Aktifkan Yaksa (SetActive true + enable AI)
   - Tampilkan UI health bar boss
   - Set quest: "Kalahkan Yaksa"

2. Pertarungan berlangsung
   - Health bar boss berkurang seiring damage
   - Yaksa reposisi dan menembak

3. Yaksa kalah (HP 0) → UnlockArena()
   - Sembunyikan health bar
   - Jalankan animasi kematian Yaksa
   - Complete quest
   - Buka pintu/blockade menuju area selanjutnya
   - Spawn reward (LontarBossDrop atau portal ke Stage 3)
```

### 4.3 Setup di Scene

```
BossArena_Yaksa (Empty GameObject)
│  BoxCollider2D (Is Trigger: true, ukuran seluruh arena)
│
├── Stage2BossArena (script)
│     bossObject: ref → Boss_Yaksa
│     bossHealthUI: ref → BossHealthSlider
│     blockades: [EntryDoor, ExitBlockade]
│     questOnBoss: "Kalahkan Yaksa"
│     questOnClear: "Lanjutkan ke Ruang Inti"
│
├── Boss_Yaksa (prefab instance, disabled)
├── EntryDoor (blockade GameObject)
├── ExitBlockade (blockade GameObject)
└── BossHealthCanvas (Screen Space Canvas)
      └── BossHealthSlider (UI Slider)
```

---

## 5. Penempatan di Stage 2

### 5.1 Layout Ruangan yang Disarankan

```
[Entry dari Stage 1]
       │
       ▼
┌──────────────┐
│   Room 1     │  ← 3x Dwarapala Kroco (perkenalan musuh baru)
│  (Tutorial)  │
└──────┬───────┘
       │ Boon Door
       ▼
┌──────────────┐
│   Room 2     │  ← 4x Dwarapala Kroco (lebih banyak, formasi ketat)
│  (Escalate)  │
└──────┬───────┘
       │ Safe Room (Lore: Prasasti Dwarapala)
       ▼
┌──────────────┐
│   Room 3     │  ← 1x MiniBoss Dwarapala + 2x Kroco pendamping
│ (Mini Boss)  │
└──────┬───────┘
       │ Boon Door
       ▼
┌──────────────┐
│   Room 4     │  ← 5x Dwarapala Kroco (tantangan akhir sebelum boss)
│  (Gauntlet)  │
└──────┬───────┘
       │ Safe Room (Lore: Prasasti Yaksa)
       ▼
┌──────────────┐
│  Boss Arena  │  ← 1x Boss Yaksa
│   (Yaksa)    │
└──────┬───────┘
       │
       ▼
[Portal ke Stage 3]
```

### 5.2 Jumlah Enemy per Room

| Room            | Dwarapala Kroco | MiniBoss Dwarapala | Yaksa       |
| --------------- | --------------- | ------------------ | ----------- |
| Room 1          | 3               | —                 | —          |
| Room 2          | 4               | —                 | —          |
| Room 3          | 2               | 1                  | —          |
| Room 4          | 5               | —                 | —          |
| Boss Arena      | —              | —                 | 1           |
| **Total** | **14**    | **1**        | **1** |

---

## 6. Script C# yang Perlu Dibuat

Semua script baru disimpan di: `Assets/Scripts/Stage-2/`

| # | Nama File                   | Fungsi                                                                                                    |
| - | --------------------------- | --------------------------------------------------------------------------------------------------------- |
| 1 | `Stage2EnemyStats.cs`     | HP, damage flash, death, health bar — implement`IDamageable` + `TakeDamage(int)` via SendMessage     |
| 2 | `Stage2EnemyMovement.cs`  | AI movement: Chase (Dwarapala) dan KeepDistance (Yaksa), leash, direction update                          |
| 3 | `Stage2EnemyAttack.cs`    | Serangan: MeleeAoE (Dwarapala) dan RangedProjectile (Yaksa), cooldown, Animation Event handler            |
| 4 | `Stage2EnemyAnimator.cs`  | Bridge AI ↔ Animator: set Direction (int 0-3), Speed (float), flipX, trigger Attack/Shoot/Die            |
| 5 | `Stage2AnimationRelay.cs` | Animation Event relay: OnAttackHit, OnAttackEnd, OnShootProjectile → panggil method di Stage2EnemyAttack |
| 6 | `EnergyArrow.cs`          | Proyektil Yaksa: terbang + homing ringan + Trail Renderer laser + damage on hit + auto-destroy            |
| 7 | `Stage2BossArena.cs`      | Arena boss Yaksa: lock/unlock arena, health bar UI, quest integration                                     |
| 8 | `Stage2EnemyHealthBar.cs` | Health bar melayang (World Space Canvas): smooth HP decrease, follow enemy position                       |

---

## 7. Arsitektur Komponen

```
┌─────────────────────────────────────────────────────┐
│                   Enemy Prefab                       │
│                                                      │
│  ┌──────────────────┐   ┌─────────────────────┐     │
│  │ Stage2EnemyStats │◄──│ PlayerAttackHitbox   │     │
│  │ (HP, Death)      │   │ (SendMessageUpwards) │     │
│  └────────┬─────────┘   └─────────────────────┘     │
│           │                                          │
│  ┌────────▼─────────┐   ┌─────────────────────┐     │
│  │ Stage2EnemyMove  │──►│ Stage2EnemyAnimator  │     │
│  │ (AI Movement)    │   │ (Direction + FlipX)  │     │
│  └────────┬─────────┘   └──────────┬──────────┘     │
│           │                        │                 │
│  ┌────────▼─────────┐   ┌──────────▼──────────┐     │
│  │ Stage2EnemyAttack│◄──│ Stage2AnimRelay      │     │
│  │ (Melee/Ranged)   │   │ (Animation Events)   │     │
│  └────────┬─────────┘   └─────────────────────┘     │
│           │                                          │
│           ▼                                          │
│  ┌──────────────────┐                                │
│  │ PlayerStats      │ ← TakeDamage / Knockback       │
│  │ (Player HP)      │                                │
│  └──────────────────┘                                │
└─────────────────────────────────────────────────────┘
```

---

## 8. Kompatibilitas dengan Sistem yang Ada

| Sistem                                | Cara Integrasi                                                                  | Catatan                                  |
| ------------------------------------- | ------------------------------------------------------------------------------- | ---------------------------------------- |
| `PlayerAttackHitbox`                | Enemy punya method`TakeDamage(int)` + tag `"Enemy"`                         | Kompatibel via`SendMessageUpwards`     |
| `ArrowProjectile`                   | Enemy di layer`Enemy` + punya `AimPoint` child                              | Homing arrow pemain cari`AimPoint`     |
| `BurnEffect`                        | `TakeDamage(int)` dipanggil berulang oleh DoT                                 | Otomatis kompatibel                      |
| `DamagePopupManager`                | Panggil`DamagePopupManager.Create()` di `TakeDamage()`                      | Floating damage numbers                  |
| `RoomManager`                       | Enemy di-list pada`enemiesInRoom`, di-poll null check                         | Destroy on death → auto-detected        |
| `QuestManager`                      | `Stage2BossArena` panggil `SetObjective()` / `CompleteCurrentObjective()` | Quest text update                        |
| `JournalManager`                    | Lore ScriptableObject`Lore_Dwarapala` + `Lore_Yaksa` dipasang di Safe Room  | Terpisah dari enemy script               |
| `PlayerController.ApplyKnockback()` | Dipanggil saat serangan Dwarapala mengenai pemain                               | Knockback direction dari enemy ke player |

---

## 9. Urutan Pengerjaan

### Tahap 1 — Sprite & Animasi (di Unity Editor)

- [X] Re-slice `Idle.png`, `Walk.png`, `AttackAndDeath.png` (Dwarapala)
- [X] Re-slice `Sprites.png` (Yaksa)
- [X] Buat semua Animation Clips (9 clip Dwarapala + 7 clip Yaksa)
- [X] Buat `DwarapalaController.controller` dengan state machine
- [X] Buat `YaksaController.controller` dengan state machine
- [X] Tambahkan Animation Events pada clip Attack/Shoot/Death

### Tahap 2 — Coding Script (bisa paralel)

- [X] `Stage2EnemyStats.cs`
- [X] `Stage2EnemyAnimator.cs`
- [X] `Stage2AnimationRelay.cs`
- [X] `Stage2EnemyMovement.cs`
- [X] `Stage2EnemyAttack.cs`
- [X] `EnergyArrow.cs`
- [X] `Stage2BossArena.cs`
- [X] `Stage2EnemyHealthBar.cs`

## 5. Checklist Progress Tracking (Enemy Design & Setup)

### Tahap 3 — Prefab Assembly (di Unity Editor)
*(Panduan langkah lengkap: [Stage2_Tahap3_Assembly_Level.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Tahap3_Assembly_Level.md))*

- [X] Buat prefab `Enemy_Dwarapala` + assign semua komponen (termasuk Heavy Knockback)
- [X] Buat prefab `MiniBoss_Dwarapala` (duplikat + modifikasi)
- [X] Buat prefab `EnergyArrow` + Trail Renderer + Circle Collider Offset
- [X] Buat prefab `Boss_Yaksa` + assign semua komponen (dengan VisualParent melayang)

### Tahap 4 — Scene Integration
*(Panduan langkah lengkap: [Stage2_Tahap4_Scene_Integration.md](file:///d:/Unity/Unity%20Project/LontarQuest/docs/Stage-2/Stage2_Tahap4_Scene_Integration.md))*

- [ ] Buat `Lore_Dwarapala.asset` dan `Lore_Yaksa.asset` di `Assets/Data/Lores/`
- [ ] Setup `Stage2IntroStarter` + `IntroDialogue` (monolog intro Saka)
- [ ] Ganti `QuestManager` dengan `Stage2QuestManager` di Canvas
- [ ] Setup `Library_RoomManager` — 11 Enemy_Dwarapala (perpustakaan utama)
- [ ] Setup `MiniBoss_RoomManager` — 1 MiniBos + 3 Enemy (area mini boss)
- [ ] Setup `Coridor_RoomManager` — 3 Enemy_Dwarapala (lorong menuju boss)
- [ ] Setup `BossArena_Yaksa` dengan `Stage2BossArena` + `IntroDialogue` (monolog pasca-boss)
- [ ] Pasang Prasasti Lore (`Dwarapala` & `Yaksa`)
- [ ] Pasang `ExitWall_Boss` + `EndPortal` (target: `Stage3`)

### Tahap 5 — Polish & Testing

- [ ] Test semua animasi arah (4 arah + flipX)
- [ ] Test damage pemain → enemy dan enemy → pemain
- [ ] Test boss fight flow (lock arena → fight → unlock)
- [ ] Tuning stat balancing (HP, damage, cooldown)
- [ ] Tambahkan SFX (hit, death, shoot, impact)
- [ ] Tambahkan VFX (camera shake, AoE indicator, death particles)
- [ ] Tambahkan health bar Mini Boss dan Boss
