<div align="center">

# 🇬🇧 English | [🇻🇳 Tiếng Việt](README_VI.md)

</div>

---

# UP-FastSetup 🚀

[![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**UP-FastSetup** is a powerful Project Bootstrapper for Unity. It automates the tedious process of setting up a new project by generating folder structures, installing Git Submodules, and composing the initial Bootstrap scene with necessary system prefabs.

Designed for a **Modular Architecture** workflow (using Git Submodules + Fork).

---

## ✨ Key Features

* **📂 Automated Folder Structure**: Generates a standardized, clean folder hierarchy (Art, Code, Design...) based on your configuration.
* **🔗 Git Submodule Integration**: Automatically adds shared modules (Core, Audio, UI) via `git submodule add`, keeping your project modular.
* **🎬 Scene Composer**: Auto-creates a `Bootstrap` scene, configures Build Settings, and spawns essential System Prefabs (e.g., AudioManager, UIRoot).
* **⚙️ Configuration as Code**: Everything is driven by a simple JSON file.
* **📝 Dynamic Naming**: Supports placeholders like `{ProjectName}` to adapt to any project identity.
* **✅ Git Ready**: Automatically handles `.gitkeep` for empty folders to ensure they are tracked by Git.

---

## 📂 Detailed Folder Structure

The tool generates a **Role-based** and **Modular** folder structure designed for scalability.

### 1. Root Level Strategy
* **`__MyGame/`**: (Double Underscore) Dedicated folder for the current project's assets. Sorted to the top for easy access.
* **`_MyCore/`**: (Single Underscore) container for Shared Modules (Git Submodules) reused across projects.
* **`3rdParty/`**: Isolated folder for Asset Store packages (to keep `Plugins` clean).

### 2. Hierarchy Tree

```text
Assets/
├── __MyGame/                 <-- 🏠 PROJECT SPECIFIC (High Priority)
│   ├── Art/                  # [Artist Area] - Raw assets & visuals
│   │   ├── Animation/
│   │   ├── Material/
│   │   ├── Model/            # FBX, Meshes
│   │   ├── Sprite/           # UI Images, 2D Sprites
│   │   ├── Texture/
│   │   └── Shader/
│   │
│   ├── Audio/                # [Audio Area]
│   │   ├── Music/
│   │   ├── Sound/            # SFX
│   │   └── Mixer/
│   │
│   ├── Code/                 # [Programmer Area]
│   │   └── Script/
│   │       ├── _Common/      # Local utilities, Consts, Helpers
│   │       ├── Gameplay/     # Core Game Logic (Player, Enemy...)
│   │       └── UI/           # UI Logic (Home, HUD, Popup...)
│   │
│   └── Design/               # [Designer Area] - Configured Assets
│       ├── Config/           # ScriptableObjects (Game Data)
│       ├── Prefab/           # Ready-to-use GameObjects
│       └── Scene/            # Game Levels & Bootstrap
│
├── _MyCore/                  <-- 🔗 SHARED MODULES (Git Submodules)
│   ├── Audio/                # e.g., UP-Audio-Manager
│   ├── UI/                   # e.g., UP-UGUI-Implement
│   ├── Data/
│   └── Utility/
│
├── 3rdParty/                 <-- Imported Assets (Do not modify code here)
├── Editor/                   <-- Project Tools (UP-FastSetup location)
├── Plugins/                  <-- Native Libraries (.dll, .so, .jar)
├── Resources/                <-- (Avoid using if possible)
└── Settings/                 <-- Project Configs (URP, Input System)
```

---
## 🛠️ Prerequisites

Before using this tool, ensure you have:
1.  **Git** installed on your machine.
2.  Initialized your Unity Project as a Git repository (`git init`).

---

## 📦 Installation

### Via Unity Package Manager (UPM)
1.  Open Unity -> Window -> **Package Manager**.
2.  Click the `+` icon -> **Add package from git URL...**
3.  Paste the repository URL:
    ```
    [https://github.com/YourUsername/UP-FastSetup.git](https://github.com/YourUsername/UP-FastSetup.git)
    ```

---

## 🚀 Quick Start

### Step 1: Create Configuration File
1.  In Unity, go to the menu: `FastSetup` -> `0. Create Config File`.
2.  A new file will be created at `Assets/Editor/FastSetup/setup-config.json`.
3.  (Optional) Edit this JSON file to customize your folder structure or add more modules.

### Step 2: Initialize Project
1.  Go to the menu: `FastSetup` -> `1. Initialize Structure (Execute)`.
2.  The tool will:
    * Create directories.
    * Pull Git Submodules (this might take a minute).
    * Create the Bootstrap scene and spawn prefabs.
    * Add the Bootstrap scene to Build Settings.

---

## ⚙️ Configuration (`setup-config.json`)

You can fully customize the setup process by editing the JSON file.

```json
{
  "projectName": "MyAwesomeGame",
  "rootNamespace": "MyStudio",
  "folders": [
    "Assets/{ProjectName}/Art/Models",
    "Assets/{ProjectName}/Code/Scripts",
    "Assets/_MyCore"
  ],
  "modules": [
    {
      "name": "Audio Module",
      "url": "[https://github.com/YourUsername/UP-Audio-Manager.git](https://github.com/YourUsername/UP-Audio-Manager.git)",
      "path": "Assets/_MyCore/Audio"
    }
  ],
  "bootstrap": {
    "sceneName": "Bootstrap",
    "savePath": "Assets/{ProjectName}/Design/Scene",
    "systemPrefabs": [
      "Assets/_MyCore/Audio/Prefabs/AudioManager.prefab"
    ]
  }
}
```