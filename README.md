# 🎮 2D Top-Down Shooter

A complete 2D top-down shooter game built in **Unity** as part of **CS250 at Southern New Hampshire University**. Developed collaboratively by a team of 6, covering gameplay mechanics, UI systems, audio, enemy AI, and art.

---

## 👥 Team

| Name | Graduation |
|---|---|
| Khoi Ho | CS 2026 |
| Graham Zambrowicz | CS 2026 |
| Colton Dover | CS 2026 |
| Mike Lanier | CS 2027 |
| Prasiddha Pokhrel | CS 2028 |
| Marcos Garcia | CS 2028 |

---

## 🕹️ Features

### 🎥 Camera Movement
- Smooth camera follow system (`CameraFollow.cs`, `PlayerCamera.cs`)
- Player-controlled zoom in/out

### ⚔️ Player Combat
- **Shooting** — bullet spawning and tracking (`Bullet.cs`, `Projectile.cs`, `ProjectileSpawner.cs`)
- **Melee Attack** — close-range combat
- **Grenade Throwing** — throwable explosives (`GrenadeBoxPickup.cs`)

### 🤖 Enemy AI
- Multiple enemy types: Melee, Pistol, Shotgun, SMG, Rifle
- Enemy combat and character logic (`EnemyCharacter.cs`, `EnemyCombat.cs`)
- Navigation and pathfinding system

### 🌍 Level Transition
- Multiple scenes per level (`Scene/`)
- Fade-to-black transition between scenes

### 🧰 Environment & Pickups
- **Medkits** — restore player health (`Health Pack/`)
- **Explosive Barrels** — destructible environment (`DestructibleBox.cs`)
- **Item pickups** (`PickUp.cs`, `PickupItem.cs`)

### 📊 Stat System
- Data-driven stat system for characters, weapons, and items (`StatSystem/`)
- No hard-coding — stats defined via ScriptableObjects
- Extensible for future modifiers and buffs

### 🖥️ UI
- Built with **Unity UI Toolkit** (`UI Toolkit/`, `GameUI.uxml`)
- HUD displays ammo count, grenade count, and equipped weapon
- Variables dynamically linked to `StatComponent`

### 💀 Death Screen
- Triggers when player health reaches zero (`DeathScreenManager.cs`, `DeathScreenUI.uxml`)
- **GAME OVER** screen with **Retry** and **Main Menu** options

### 🔫 Gun System
- Multiple weapon types (`NewPistol.cs`, `RIfle.cs`)
- Shooting, bullet tracking, and reload coroutines
- Mouse aim with `FollowMouse.cs`

### 🔊 Sound
- Centralized audio management (`MusicManager.cs`)
- Death, gunshot, reload, footsteps, and grenade SFX (`Sound/`, `Sound Effects/`)

### 🎨 Art & Sprites
- Player, enemy, and environment sprites (`Sprites/`, `S.Enemys/`)
- Tileset with floor, objects, and walls (`Tiles/`, `TilePalette/`)

---

## 🛠️ Tech Stack

![Unity](https://img.shields.io/badge/Unity-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)

- **Engine:** Unity 2022.3 LTS
- **Language:** C#
- **UI System:** Unity UI Toolkit (UXML)
- **Input:** Unity Input System (`InputSystem_Actions.cs`)
- **Assets:** ScriptableObjects for stat definitions

---

## 🚀 How to Run

### 1. Clone the repository
```bash
git clone https://github.com/marcosgale/2D-Top-Down-Shooter.git
cd 2D-Top-Down-Shooter
```

### 2. Open in Unity
- Open **Unity Hub**
- Click **Add project from disk** and select the cloned folder
- Open with **Unity 2022.3 LTS** or later

### 3. Play
- Open the main scene from `Scene/`
- Press **▶ Play** in the Unity Editor

---

## 📂 Repository Structure

```
2D-Top-Down-Shooter/
├── Enemy Scripts/          # Enemy AI and combat scripts
├── Environment/            # Environmental object scripts
├── Health Pack/            # Medkit pickup logic
├── Input/                  # Input system configuration
├── Inventory/              # Inventory system
├── Movement/               # Player and enemy movement
├── Navigation/             # Pathfinding and nav agents
├── Packages/               # Unity package dependencies
├── Player/                 # Player-specific scripts
├── ProjectSettings/        # Unity project configuration
├── S.Enemys/               # Enemy sprite assets
├── Scene/                  # Game scenes
├── Settings/               # Game settings assets
├── Sound/                  # Background music
├── Sound Effects/          # In-game SFX
├── Sprites/                # Sprite assets
├── StatSystem/             # ScriptableObject stat definitions
├── Tests/                  # Unit tests
├── TextMesh Pro/           # TMP assets
├── TilePalette/            # Tile palette assets
├── Tiles/                  # Tilemap tiles
├── Triggers/               # Scene and event triggers
├── UI/                     # UI scripts
├── UI Toolkit/             # UXML UI files
├── Bullet.cs               # Bullet behavior
├── CameraFollow.cs         # Camera follow logic
├── DeathScreenManager.cs   # Death screen controller
├── DeathScreenUI.uxml      # Death screen UI layout
├── DestructibleBox.cs      # Destructible objects
├── Door.cs                 # Door/trigger logic
├── EnemyCharacter.cs       # Enemy base character
├── EnemyCombat.cs          # Enemy combat system
├── FollowMouse.cs          # Mouse aim rotation
├── GameUI.uxml             # Main HUD layout
├── GrenadeBoxPickup.cs     # Grenade pickup
├── IDamageable.cs          # Damageable interface
├── InputSystem_Actions.cs  # Generated input actions
├── MusicManager.cs         # Audio manager
├── NewPistol.cs            # Pistol weapon logic
├── PickUp.cs               # Pickup base class
├── PickupItem.cs           # Pickup item logic
├── PlayerCamera.cs         # Camera zoom and follow
├── PlayerCharacter.cs      # Player stats and health
├── PlayerMovement.cs       # Player movement controller
├── Projectile.cs           # Projectile base class
├── ProjectileSpawner.cs    # Bullet spawner
├── RIfle.cs                # Rifle weapon logic
└── .gitignore
```

---

## 🧠 Key Concepts Demonstrated

- **ScriptableObject architecture** — decoupled stat system for weapons, characters, and items
- **Unity UI Toolkit** — UXML-based HUD with live stat binding
- **Scene management** — multi-scene levels with transition effects
- **Coroutine-based systems** — reload timing, death triggers, scene fading
- **Interface-driven design** — `IDamageable` for consistent damage handling
- **Unity Input System** — new Input System with generated action classes
- **Audio management** — centralized `MusicManager` for all in-game audio

---

## 🔮 Future Plans

- Stat modifier system (buffs, debuffs, item effects)
- Expanded enemy behaviors and AI patterns
- Additional weapon types and animations
- More levels and environmental variety

---

## 📝 Academic Context

**Course:** CS250 — Software Development Lifecycle  
**Institution:** Southern New Hampshire University (SNHU)  
**Engine:** Unity | **Language:** C#
