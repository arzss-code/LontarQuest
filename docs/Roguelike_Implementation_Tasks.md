# Roguelike & Journal System Implementation Tasks
*(Based on `Roguelike_Systems_Implementation.md`)*

- [ ] **A. Data Setup (ScriptableObjects)**
  - [ ] Create `Data` folder in `Assets/`.
  - [ ] Create Boon Data (e.g., `Boon_KecepatanLontara`).
  - [ ] Create Lore Data (e.g., `Lore_KepalaKala`).
- [ ] **B. Player Setup**
  - [ ] Add `PlayerModifier` script to the `Saka` prefab.
- [ ] **C. Boon Doors (Reward Paths)**
  - [ ] Create Door objects (e.g., `Pintu_Lontara`) with `BoxCollider2D` (Checked `Is Trigger`).
  - [ ] Add `BoonDoor` script and configure the reward type (`Lontara`, `Batak`, or `Kawi`).
- [ ] **D. Room & Enemy Management**
  - [ ] Create `RoomManager` object (e.g., `Room_1_Manager`) in combat rooms.
  - [ ] Assign enemies to the room manager.
  - [ ] Assign exit doors (Boon Doors) to the room manager.
- [ ] **E. Lore & Journal Objects**
  - [ ] Create Lore objects (e.g., `Prasasti_KepalaKala`) with `BoxCollider2D` (Checked `Is Trigger`).
  - [ ] Add `LoreInteractable` script and assign the corresponding Lore Data.
- [ ] **F. Global Systems**
  - [ ] Create a `Global_Managers` object.
  - [ ] Add `JournalManager` script.
  - [ ] Add `BoonUIManager` script.
