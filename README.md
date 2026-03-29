# 👹 Dwarf vs Giant - 3D Asymmetric Party Game

**Dwarf vs Giant** is a 4v1 Asymmetric Multiplayer Party Game where four agile Dwarves must complete tasks and escape a localized area before being caught by the slow but powerful Giant. Built with Unity URP.

## 🌟 Core Gameplay Loop
- **Dwarves (Survivors):** Must utilize their small size to hide under tables, sneak through narrow corridors, and complete escape objectives.
- **The Giant (Hunter):** A towering figure that must track down the Dwarves using hearing, sight, and brute force to smash through specific obstacles.
- **The Catch:** The scale difference (1m vs 3m) allows Dwarves to crouch into spaces the Giant physically cannot enter, creating intense hide-and-seek mechanics.

---

## 🚀 Current Development Stage: Phase 1 (Single Player Prototype)
This repository currently contains the **Core Single Player Prototype**. 

### Highlighted Features in this version:
- **Dwarf Controller:** Walk, sprint, jump, and a dynamic crouch system that reduces the collider height by half.
- **Giant AI:** A NavMesh-based State Machine (Patrol, Chase, Attack) equipped with a Field of View (FoV) and Raycast line-of-sight detection.
- **Automated Prototype Scene:** Includes an Editor Tool to instantly build a test scene.

### How to Test the Prototype
1. Open the project in Unity (Tested on `6000.3.11f1`).
2. Go to the top menu bar and click: `Dwarf VS Giant -> Build Prototype Scene`.
3. Press **▶️ Play**.
   - **Controls:** `WASD` to Move, `Mouse` to Look, `Shift` to Sprint, `Ctrl/C` to Crouch.

---

## 🗺️ Roadmap
- [ ] Implement Task and Escape logic.
- [ ] Add Stealth abilities (Lockers, Traps, Distractions).
- [ ] Tension Polish (Heartbeat audio, screen shake on Giant approach).
- [ ] **Phase 3:** Multiplayer Integration using Netcode for GameObjects (NGO).

## 📦 Versioning Strategy
This project follows [Semantic Versioning 2.0.0](https://semver.org/).
Major versions will reflect the project phases (e.g., v1.0.0 for the Multiplayer release). Git tags will be used to mark stable builds.

- **Current Version:** `v0.1.0-alpha` (Phase 1 Prototype)
