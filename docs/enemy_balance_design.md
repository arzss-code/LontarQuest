# Desain Keseimbangan Parameter Musuh — LontarQuest

Dokumen ini mendefinisikan seluruh parameter numerik untuk **Player**, **Monster**, **Boss**, **Trap**, **Projectile**, dan **Item** di setiap Stage. Desain disusun agar ada **kurva kesulitan yang proporsional dan terasa adil** dari Stage 1 → Stage 2 → Stage 3, dengan memperhitungkan faktor **kepadatan (density) musuh** di Stage 3, **progresi kurva bos yang mulus (smooth curve)**, serta **skala kekuatan Player vs Monster per Stage**.

---

## Filosofi Balancing

### Prinsip Utama

| Prinsip | Penjelasan |
| :--- | :--- |
| **TTK (Time To Kill) Musuh** | Saka harus bisa membunuh kroco dalam **3–5 hit melee** di setiap stage. Jika HP musuh naik, Boon yang sudah dikumpulkan mengkompensasi. |
| **TTD (Time To Die) Saka** | Saka harus bisa bertahan **4–6 hit** dari kroco tanpa Boon. Boss harus membunuh Saka dalam **5–8 hit** tanpa Boon. |
| **Scaling Antar Stage** | HP musuh naik **~2.5x per stage**, damage naik **~1.5x per stage**. Speed & aggression naik secara gradual. |
| **Boon Kompensasi** | Di akhir setiap stage, Saka diperkirakan memiliki **1–2 Boon aktif** yang mengimbangi kenaikan kesulitan stage berikutnya. |
| **Density-Aware (Stage 3)** | Di Stage 3, karena musuh spawn berkelompok (simultan), parameter individu diturunkan (~15%) untuk menjaga agar total ancaman kolektif tetap adil. |
| **Smooth Boss Curve** | Kurva HP dan Damage Bos disesuaikan agar kenaikannya proporsional (tidak melonjak secara ekstrem atau turun di tengah jalan). |
| **Boss = Uji Kemampuan** | Boss menguji seluruh skill yang dipelajari pemain di stage tersebut. HP boss = **4–5x HP kroco** stage itu. |

### Baseline Perhitungan (Saka tanpa Boon)

| Stat Saka | Nilai |
| :--- | :--- |
| Max HP | **200** |
| Melee Damage | **35** |
| Ranged Damage | **15** |
| Mana Cost (Panah) | **25** |
| Stamina Cost (Dodge) | **20** |
| Invincibility Frame | **0.6 detik** |

---

## Matriks Interaksi Player vs Monster Per Stage

Bagian ini mendefinisikan interaksi timbal-balik antara parameter Saka (termasuk proyeksi Boon) dengan monster di setiap stage.

### 🟢 STAGE 1: Reruntuhan Candi
* **Status Saka:** HP: **200** | Melee Damage: **35** | Ranged Damage: **15** | Asumsi Boon: **0 Boon**

| Entitas | HP | Damage | Hits to Kill Player (TTD Saka) | Hits to Kill Monster (TTK Musuh) | Analisis Keseimbangan |
| :--- | :---: | :---: | :---: | :---: | :--- |
| **Arca Gana** (Kroco) | 30 | 15 | **14 hit** | Melee: **1 hit**<br>Ranged: **2 hit** | Sangat ramah pemula. Musuh mati sekali tebas, memberi rasa kepuasan awal. |
| **Kepala Kala** (Boss) | 150 | 20 | **10 hit** | Melee: **5 hit**<br>Ranged: **10 hit** | Mengajarkan ritme dasar hit-and-run dan dodge telegrafik (kedip merah). |
| **Spike Trap** (Jebakan) | — | 10 | **20 hit** | — | Sekadar rintangan lingkungan penunjuk arah jalan. |

---

### 🟡 STAGE 2: Perpustakaan Melayang
* **Status Saka:** HP Efektif: **~250** (Asumsi Boon DR ~ 20%) | Melee DPS: **~42** (Asumsi Boon Atk Spd) | Ranged Damage: **15** | Asumsi Boon: **1–2 Boon**

| Entitas | HP | Damage | Hits to Kill Player (TTD Saka) | Hits to Kill Monster (TTK Musuh) | Analisis Keseimbangan |
| :--- | :---: | :---: | :---: | :---: | :--- |
| **Dwarapala Kroco** | 80 | 25 | **10 hit** (Base: 8 hit) | Melee: **2 hit**<br>Ranged: **6 hit** | Mulai membutuhkan 2 tebasan. Pemain dipaksa memperhatikan timing serang. |
| **Dwarapala Raksasa** (Mini Boss)| 180 | 35 | **7 hit** (Base: 6 hit) | Melee: **5 hit**<br>Ranged: **12 hit** | Serangan lambat tapi berat. Terdapat ancaman stun & knockback ke jurang. |
| **Yaksa** (Boss Utama) | 450 | 26 | **9 hit** (Base: 8 hit) | Melee: **11 hit**<br>Ranged: **30 hit** | Yaksa menjaga jarak. Pemain harus mengejar dengan dash dan memukul cepat. |

---

### 🔴 STAGE 3: Ruang Inti Dimensi (density-Aware)
* **Status Saka:** HP Efektif: **~300** (Boon DR + Regen) | Melee DPS: **~50** (Boon Level 2 / Stacking) | Ranged Damage: **15** | Asumsi Boon: **3–4 Boon**

| Entitas | HP | Damage | Hits to Kill Player (TTD Saka) | Hits to Kill Monster (TTK Musuh) | Analisis Keseimbangan |
| :--- | :---: | :---: | :---: | :---: | :--- |
| **Nisakala** (Kroco Simultan) | 85 | 24 | **12 hit** (Kolektif 3: **~4 hit**) | Melee: **2 hit**<br>Ranged: **6 hit** | Mati cepat secara individu, tapi tekanan keroyokan memaksa dash berkelanjutan. |
| **Bhatara Kala** (Boss Utama) | 850 (600+250) | 38 (Slam) | **8 hit** (Base: 5 hit) | Melee: **17 hit**<br>Ranged: **57 hit** | Klimaks mekanik. Shield membagi fase serang. Gravity pull memaksa timing dodge ketat. |
| **Cosmic Trap** (Jebakan) | — | 20 | **15 hit** (Base: 10 hit) | — | Ancaman serius saat pemain panik menghindari Void Shot Nisakala. |

---

## Rincian Parameter Monster Per Stage

### Stage 1: Reruntuhan Candi

#### Monster: Arca Gana (Melee Chaser)
| Parameter | Nilai | Justifikasi |
| :--- | :---: | :--- |
| **Max HP** | **30** | Mati dalam 1 hit melee (35 dmg). Pemain merasa kuat di awal. |
| **Attack Damage** | **15** | 200 ÷ 15 = ~13 hit untuk membunuh Saka. Sangat forgiving. |
| **Move Speed** | **3.0** | Cukup cepat untuk mengejar, tapi Saka bisa lari mundur. |
| **Detection Radius** | **6.0** | Deteksi menengah — memberi waktu pemain bereaksi. |
| **Stop Distance** | **1.2** | Berhenti dekat sebelum serang. |
| **Attack Distance** | **1.5** | Jangkauan serang pendek — mudah dihindari. |
| **Attack Radius (Hitbox)** | **1.5** | Area serang kecil dan fokus. |
| **Attack Cooldown** | **1.5 detik** | Jeda cukup lama — jendela besar untuk menyerang balik. |
| **Attack Hit Delay** | **0.3 detik** | Delay animasi sebelum damage — pemain bisa dodge. |
| **Leash Radius** | **8.0** | Kembali ke posisi jika terlalu jauh dari spawn. |
| **Knockback** | **— (tidak ada)** | Belum ada efek knockback di Stage 1. |

> **Sumber:** [GanaAI.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage-1/GanaAI.cs)

#### Boss: Kepala Kala (AoE Chaser)
| Parameter | Nilai | Justifikasi |
| :--- | :---: | :--- |
| **Max HP** | **150** | 5x HP Arca Gana. Pertarungan singkat tapi intens. |
| **Attack Damage** | **20** | 200 ÷ 20 = 10 hit. Forgiving tapi terasa berbahaya. |
| **Move Speed** | **3.0** | Sama dengan Gana — konsisten dengan ekspektasi Stage 1. |
| **Chase Distance** | **15.0** | Deteksi sangat lebar — boss selalu mengejar. |
| **Attack Distance** | **1.5** | Harus dekat sebelum meledak. |
| **Attack Radius (AoE)** | **2.0** | Area ledakan cukup untuk mengenai pemain yang tidak dodge. |
| **Charge Time** | **1.0 detik** | Telegraphing yang jelas — sprite berkedip merah memberi sinyal. |
| **Attack Cooldown** | **2.0 detik** | Jeda panjang di mana pemain bisa menyerang. |
| **Wander Speed** | **1.5** | Saat patroli, bergerak santai. |
| **Leash Radius** | **12.0** | Area leash lebar agar tidak mudah di-exploit. |

---

### Stage 2: Perpustakaan Melayang

#### Monster: Dwarapala Kroco (Melee AoE Tanker)
| Parameter | Nilai Saat Ini | **Nilai Baru** | Justifikasi Perubahan |
| :--- | :---: | :---: | :--- |
| **Max HP** | 80 | **80** ✅ | ~2–3 hit melee (35×3=105). Tanker tapi tidak spons. Seimbang. |
| **Attack Damage** | 25 | **25** ✅ | Terasa sakit tapi fair. Dengan DR 20%: ~10 hit. |
| **Move Speed** | 2.0 | **2.0** ✅ | Lambat = tanker. Saka bisa kiting. |
| **Detection Radius** | 7.0 | **7.0** ✅ | Menengah — tidak terlalu agresif. |
| **Stop Distance** | X:1.2, Y:1.8 | **X:1.2, Y:1.8** ✅ | Elips sesuai perspektif 2.5D. |
| **Attack Range** | X:1.5, Y:2.0 | **X:1.5, Y:2.0** ✅ | Jangkauan pendek. |
| **Melee AoE Radius** | 2.0 | **2.0** ✅ | Hantaman tanah — area yang bisa dihindari dengan dodge. |
| **Attack Cooldown** | 2.5 | **2.5 detik** ✅ | Jeda panjang — jendela besar untuk counterattack. |
| **Charge Time** | 0.4 | **0.4 detik** ✅ | Delay singkat sebelum smash. |
| **Knockback Force** | 5.0 | **5.0** ✅ | Dorongan ringan. Saka tetap bisa reposisi. |

#### Mini-Boss: Dwarapala Raksasa (Heavy Melee AoE)
| Parameter | Nilai Saat Ini | **Nilai Baru** | Justifikasi Perubahan |
| :--- | :---: | :---: | :--- |
| **Max HP** | 200 | **180** ⬇️ | 200 terlalu spons. 180 = ~5–6 hit melee. Terasa tangguh tanpa membosankan. |
| **Attack Damage** | 40 | **35** ⬇️ | 35 = 200÷35 ≈ 6 hit. Dengan Boon DR 20%: ~7 hit. Proporsional. |
| **Move Speed** | 1.8 | **1.8** ✅ | Sangat lambat — pemain bisa kiting. Tradeoff untuk damage tinggi. |
| **Melee AoE Radius** | 3.0 | **2.8** ⬇️ | 3.0 terlalu lebar, menyulitkan dodge di platform sempit. 2.8 tetap menakutkan. |
| **Knockback Force** | 8.0 | **7.0** ⬇️ | 8.0 di platform melayang = insta-death ke jurang. 7.0 masih berbahaya tapi survivable. |
| **Stun Duration** | 0.5 | **0.4 detik** ⬇️ | 0.4 detik lebih fair untuk menjaga kontrol permainan. |
| **Scale** | 1.5x | **1.5x** ✅ | Visual jelas berbeda. |

#### Boss: Yaksa (Ranged Sniper)
| Parameter | Nilai Saat Ini | **Nilai Baru** | Justifikasi Perubahan |
| :--- | :---: | :---: | :--- |
| **Max HP** | 350 | **450** ⬆️ | HP dinaikkan agar terasa kokoh dibandingkan Dwarapala Raksasa (180 HP) dan tidak mati terlalu cepat (Saka butuh ~13 hit melee murni). |
| **Projectile Damage** | 15 | **26** ⬆️ | Dinaikkan agar lebih mematikan dari Kroco Dwarapala (25). Saka (200 HP) mati dalam 8 hit tembakan Yaksa. |
| **Move Speed** | 2.5 | **2.5** ✅ | Kecepatan lari ideal. |
| **Retreat Speed** | 3.0 | **3.5** ⬆️ | Lebih cepat saat mundur — memaksa Saka menggunakan dash. |
| **Preferred Distance** | 6.0 | **6.0** ✅ | Jarak ideal tembak. |
| **Attack Cooldown** | 1.8 | **2.0 detik** ⬆️ | 2.0 memberi jeda reaksi yang cukup di platform sempit. |
| **Aim Duration** | 0.3 | **0.5 detik** ⬆️ | Telegraphing tembakan diperjelas (0.35s tracking + 0.15s locked). |

---

### Stage 3: Ruang Inti Dimensi

#### Monster: Nisakala (Ranged Assassin)
| Parameter | Nilai Saat Ini | **Nilai Baru (Density-Aware)** | Justifikasi Perubahan |
| :--- | :---: | :---: | :--- |
| **Max HP** | 100 | **85** ⬇️ | Total HP kelompok (3 musuh = 255 HP) masih dalam batas wajar disapu cepat dengan DPS ~50. |
| **Attack Damage** | 20 | **24** ⬆️ | Naik sedikit dari default, namun diturunkan dari rencana solo (30) agar kombinasi hit simultan tidak instakill. |
| **Retreat Speed** | — | **3.8** | Kecepatan mundur dikurangi agar Saka dapat mengejar mereka tanpa kehabisan stamina. |
| **Attack Range** | 8.0 | **7.0** ⬇️ | Jangkauan tembak dipersingkat agar mereka harus mendekat sedikit. |
| **Attack Cooldown** | 2.0 | **3.0 detik (Random)** ⬆ | Menggunakan desinkronisasi (`3.0s + Random.Range(-0.75f, 1.25f)`) agar tembakan musuh terpecah. |
| **Retreat Distance** | 3.0 | **2.5** ⬇ | Jarak memicu lari diperpendek agar pemain lebih mudah memukul melee. |
| **Safe Distance** | 6.0 | **5.0** ⬇ | Jarak berhenti mundur diperpendek agar musuh tetap dalam jangkauan visual. |

#### Final Boss: Bhatara Kala (Cosmic Gravity Boss)
| Parameter | Nilai Saat Ini | **Nilai Baru** | Justifikasi Perubahan |
| :--- | :---: | :---: | :--- |
| **Max HP** | 1000 | **600** ⬇️ | Diturunkan agar EHP total bos berada di angka 850 (600 HP + 250 Shield). Ini adalah peningkatan kesulitan yang logis (1.9x EHP Boss Stage 2). |
| **Max Shield** | 500 | **250** ⬇️ | Disesuaikan dengan batas EHP baru. Shield pecah setelah ~8 hit melee (stagger reward lebih mudah dijangkau). |
| **Shield Absorption** | 70% | **70%** ✅ | Rasio absorpsi ideal. 30% damage tetap melukai HP. |
| **Slam Damage** | 25 | **38** ⬆️ | Disesuaikan agar kurva damage boss mulus (S1: 20 -> S2: 26 -> S3: 38). Mengancam namun tetap adil. |
| **Shield Regen Delay** | 2.0 | **3.0 detik** ⬆ | Memberi jeda DPS nyata saat shield pecah sebelum mulai mengisi kembali. |
| **Shield Regen Duration** | 5.0 | **6.0 detik** ⬆ | Durasi pengisian shield dibuat sedikit lebih lama. |
| **Stagger Duration** | 3.0 | **3.5 detik** ⬆ | Memberi waktu memukul stagger yang lebih leluasa. |

---

## Ringkasan Perubahan Kode C#

### Perubahan Script / Inspector

| File / Target | Parameter | Lama | Baru |
| :--- | :--- | :---: | :---: |
| `Stage2_Enemy_Design.md` (Mini Boss) | `maxHP` | 200 | **180** |
| `Stage2_Enemy_Design.md` (Mini Boss) | `damage` | 40 | **35** |
| `Stage2_Enemy_Design.md` (Mini Boss) | `aoeDamageRadius` | 3.0 | **2.8** |
| `Stage2_Enemy_Design.md` (Mini Boss) | `knockbackForce` | 8.0 | **7.0** |
| `Stage2_Enemy_Design.md` (Mini Boss) | `stunDuration` | 0.5 | **0.4** |
| `Stage2_Enemy_Design.md` (Yaksa) | `maxHP` | 350 | **450** |
| `Stage2_Enemy_Design.md` (Yaksa) | `rangedDamage` | 15 | **26** |
| `Stage2_Enemy_Design.md` (Yaksa) | `retreatSpeed` | 3.0 | **3.5** |
| `Stage2_Enemy_Design.md` (Yaksa) | `attackCooldown` | 1.8 | **2.0** |
| `Stage2_Enemy_Design.md` (Yaksa) | `aimDuration` | 0.3 | **0.5** |
| [EnergyArrow.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage-2/EnergyArrow.cs) | `speed` | 8.0 | **7.0** |
| [EnergyArrow.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage-2/EnergyArrow.cs) | `homingStrength` | 1.5 | **1.2** |
| [EnergyArrow.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage-2/EnergyArrow.cs) | `maxHomingAngle` | 30 | **25** |
| [EnergyArrow.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage-2/EnergyArrow.cs) | `lifetime` | 4.0 | **3.5** |
| [EnemyStats.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyStats.cs) | `maxHP` | 100 | **85** |
| [EnemyMovement.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyMovement.cs) | `moveSpeed` | 4.0 | **4.0** |
| [EnemyMovement.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyMovement.cs) | `retreatDistance` | 3.0 | **2.5** |
| [EnemyMovement.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyMovement.cs) | `safeDistance` | 6.0 | **5.0** |
| [EnemyAttack.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyAttack.cs) | `attackRange` | 8.0 | **7.0** |
| [EnemyAttack.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/EnemyAttack.cs) | `attackCooldown` | 2.0 | **3.0 (Randomized)** |
| [VoidShot.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/VoidShot.cs) | `damage` | 20 | **24** |
| [VoidShot.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/VoidShot.cs) | `moveSpeed` | 10.0 | **7.5** |
| [VoidShot.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Stage/Enemies/VoidShot.cs) | `maxLifeTime` | 5.0 | **3.5** |
| [BossStats.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossStats.cs) | `maxHP` | 1000 | **600** |
| [BossStats.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossStats.cs) | `maxShield` | 500 | **250** |
| [BossStats.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossStats.cs) | `shieldRegenDelay` | 2.0 | **3.0** |
| [BossStats.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossStats.cs) | `shieldRegenDuration` | 5.0 | **6.0** |
| [BossController.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossController.cs) | `slamDamage` | 25 | **38** |
| [BossMovement.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossMovement.cs) | `moveSpeed` | 2.0 | **2.2** |
| [BossAttack.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossAttack.cs) | `attackRange` | 2.0 | **2.5** |
| [BossAttack.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/BossAttack.cs) | `longRangeDelay` | 3.0 | **3.5** |
| [GravityPullController.cs](file:///d:/Unity/Unity%20Project/LontarQuest/Assets/Scripts/Stage%203/Boss/GravityPullController.cs) | `pullForce` | 50.0 | **45.0** |

> [!NOTE]
> Beberapa parameter di atas (khususnya Stage 2) **mungkin sudah di-override di Inspector prefab Unity** dan bukan hanya di default value script. Pastikan untuk mengubah nilai di **prefab** (`Enemy_Dwarapala.prefab`, `MiniBoss_Dwarapala.prefab`, `Boss_Yaksa.prefab`, `Niskala_01.prefab`, `Elite_Niskala_01.prefab`) selain di script.
