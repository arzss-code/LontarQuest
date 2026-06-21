# Roguelike & Journal System Implementation Tasks
*(Based on `Roguelike_Systems_Implementation.md`)*

- [x] **A. Data Setup (ScriptableObjects)**
  - [x] Create `Data` folder in `Assets/`.
  - [x] Create Boon Data (e.g., `Boon_KecepatanLontara`).
  - [ ] Create Lore Data (e.g., `Lore_KepalaKala`).
- [x] **B. Player Setup**
  - [x] Add `PlayerModifier` script to the `Saka` prefab.
- [x] **C. Lontar Pickups (Reward Paths - Modified from Doors)**
  - [x] Create Lontar objects (e.g., `Lontar_Batak`) with `PolygonCollider2D` (Checked `Is Trigger`).
  - [x] Add `LontarPickup` script.
- [ ] **D. Room & Enemy Management**
  - [x] Create `RoomManager` object (e.g., `Room_1_Manager`) in combat rooms.
  - [ ] Assign enemies to the room manager.
  - [x] Assign exit mechanism / rewards to the room manager.
- [ ] **E. Lore & Journal Objects**
  - [ ] Create Lore objects (e.g., `Prasasti_KepalaKala`) with `BoxCollider2D` (Checked `Is Trigger`).
  - [ ] Add `LoreInteractable` script and assign the corresponding Lore Data.
- [ ] **F. Global Systems**
  - [x] Create a `Global_Managers` object.
  - [ ] Add `JournalManager` script.
  - [x] Add `BoonUIManager` script.
