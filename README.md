# Magnetize Marble - Physics Puzzle Game

A digital physics-puzzle game for Android tablets designed as an interactive element for educational escape rooms. Players guide a magnetic marble through four faces of a rotating cube by strategically placing and tuning positive/negative magnets.

**Developed by:** Stefan Vacik & Christian Redl  
**Duration:** 5–15 minutes per playthrough  
**Platform:** Android (Pixel Tablet) / Windows  
**Engine:** Unity (2022 LTS+)

---

## Quick Start

### Requirements
- **Development:** Unity 2022.3.x LTS or later
- **Android Build:** Android SDK level 30+, Gradle 7.4+
- **Target Device:** Pixel Tablet (or equivalent Android tablet)

### Setup & Build

#### Clone & Open in Unity
```bash
git clone <repository-url>
cd Magnetize_Marble
# Open with Unity Hub or double-click Magnetize_Marble.sln
```

#### Build for Android (APK)
1. Go to **File → Build Settings**
2. Select **Android** platform
3. Configure Player Settings:
   - **Package Name:** `com.technikumwien.magnetizemarble`
   - **Target API Level:** 34+
   - **Minimum API Level:** 30
4. Click **Build AND Run** (or **Build** to generate APK file)
5. APK will be saved to `Builds/Android/MagnetizeMarble.apk`

#### Build for Windows
1. Go to **File → Build Settings**
2. Select **PC, Mac & Linux Standalone** with **Windows** target
3. Click **Build** → select output folder
4. Executable will be in `Builds/Windows/`

---

## Project Structure

```
Magnetize_Marble/
├── Assets/
│   ├── Scripts/
│   │   ├── Events/              # ScriptableObject event channels
│   │   ├── Managers/            # GameManager, LevelManager, MagnetsManager
│   │   ├── Magnet/              # MagnetController, MagnetModelController
│   │   ├── Marble/              # MarbleController physics
│   │   ├── Interactables/       # Drag/drop and button handling
│   │   ├── UI/                  # Menu controllers, level UI
│   │   ├── Helpers/             # Start point & drag zone markers
│   │   └── Setup/               # Initialization utilities
│   ├── Scenes/
│   │   ├── MainMenu.unity       # Main menu screen
│   │   ├── MarbleTestScene.unity # Active gameplay scene
│   │   ├── SettingsMenu.unity   # Settings/controls
│   │   └── CreditsMenu.unity    # Credits screen
│   ├── Prefabs/                 # Reusable game prefabs
│   ├── ScriptableObject/        # Event channels, configs, data
│   ├── Art/                     # Models, textures, sprites
│   └── Settings/                # Audio, graphics, input configs
├── Documentation/
│   ├── EscapeRoom_MagnetismCube_GDD.md  # Game Design Document
│   ├── TECHNICAL.md             # Architecture & tech details
│   ├── BUILD_GUIDE.md           # Expanded build instructions
│   ├── DELIVERY_CHECKLIST.md    # Final deliverables checklist
│   └── Images/                  # GDD reference images
├── Packages/                    # Unity dependencies
├── ProjectSettings/             # Unity configuration
├── Magnetize_Marble.sln         # Solution file
└── README.md                    # This file
```

---

## Architecture Overview

### Event-Driven System
The game uses **ScriptableObject event channels** for loose coupling between systems:

- **Void Event Channels:** Commands (Enable/Disable dragging, goal reached, rotation start/end)
- **Bool Event Channels:** State broadcasting (dragging enabled/disabled state)
- **GameObject Event Channels:** Object lifecycle (magnet created/removed)
- **Vector3 Event Channels:** Position data (optional streaming)

See `Assets/Scripts/Events/` for implementations.

### Core Systems

#### LevelManager
- Manages cube face progression (4 faces total)
- Auto-discovers `StartPoint` and `DragZone` scene markers
- Handles cube rotation animations
- Subscribes to `goalReachedEvent` to advance levels

#### MagnetsManager
- Tracks active magnets in the scene
- Handles magnet placement validation & clamping
- Subscribes to magnet creation/removal events
- Provides world position calculation from screen coordinates

#### MarbleController
- Physics-based marble simulation
- Detects goal zones and reset zones
- Raises `goalReachedEvent` when marble reaches exit
- Automatically resets for next attempt

#### MagnetController
- Applies magnetic force to marble via physics
- Adjustable strength levels (tap to cycle)
- Visual feedback (scale indicates strength)
- Polarity support (push/pull)

---

## Gameplay

### Objective
Route the marble through all four cube faces to a goal zone on each face. Completing the final face unlocks the physical escape room exit.

### Controls (Android Tablet)
- **Drag magnet from UI:** Select + drag a magnet onto the cube face
- **Tap placed magnet:** Cycle magnet strength (visual scale change)
- **Drag to trash zone:** Remove unwanted magnets
- **Play button:** Launch marble simulation

---

## Delivery Checklist (summary)
See `Documentation/DELIVERY_CHECKLIST.md` for the complete QA checklist and required deliverables.

---

## Contact
- **Christian Redl**
- **Stefan Vacik**


---

*Generated May 2026*
