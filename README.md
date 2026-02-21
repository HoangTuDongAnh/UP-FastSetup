# HTDA Framework – FastSetup

FastSetup is an **Editor-only** module of the HTDA Framework.

It helps bootstrap a Unity project by:

- Creating a standardized folder structure
- Installing framework modules via Git submodules
- Generating a Bootstrap scene
- Supporting multiple project templates (General, Puzzle, Hypercasual, etc.)
- Allowing export of custom folder templates with metadata

---

## 📦 Package Info

- Package ID: `com.htda.framework.fastsetup`
- Assembly: `HTDA.Framework.FastSetup.Editor`
- Unity: `2022.3+`
- Editor-only (no runtime code)

---

## 🚀 Features

### 1️⃣ Create Project Config
Creates a config file at:
```
ProjectSettings/HTDA/FastSetup/setup-config.json
```

This file defines:
- Folder structure
- Framework modules to install
- Bootstrap scene settings

---

### 2️⃣ Multiple Folder Templates

Built-in templates:
- General
- Puzzle
- (Optional) Hypercasual

You can:
- Create config from template
- Export your own template with metadata
- Store project-level templates

Templates support metadata:
```json
{
  "meta": {
    "id": "puzzle",
    "name": "Puzzle",
    "description": "Puzzle game layout",
    "tags": ["puzzle", "mobile"],
    "version": "1.0.0"
  },
  "config": { ... }
}
```
---

### 3️⃣ Folder Generation

Supports:
- Flat folder list (folders)
- Group-based structure (folderGroups)
- Token replacement ({ProjectName})

Example:
```json
{
  "folderGroups": [
    {
      "root": "Assets/__{ProjectName}",
      "paths": [
        "Art",
        "Audio",
        "Code/Scripts",
        "Scenes"
      ]
    }
  ]
}
```

---

### 4️⃣ Install Framework Modules

Uses:
```
git submodule add <url> <path>
```

Recommended path:

```
Packages/com.htda.framework.core
```

Requirements:
- Git installed
- Project initialized as git repository

--- 

### 5️⃣ Bootstrap Scene

Automatically:
- Creates a new scene
- Spawns system prefabs
- Saves scene
- Adds it to Build Settings (index 0)

---

### 📂 Menu
```
HTDA/FastSetup/
    0 - Create Config (Default)
    0.1 - Create Config From Template...
    0.2 - Export Current Config As Template...
    1 - Run Setup
    Open Config Folder
    Open Templates Folder
```

---

### 🛠 Typical Workflow

1. Install FastSetup package.

2. Initialize project with git init.

3. Create config from template.

4. Adjust config if needed.

5. Run Setup.

6. Start building your game.

---

### 🧩 Extending Templates

You can export your current config as a template:
```
HTDA → FastSetup → Export Current Config As Template...
```
Templates are stored in:
```
ProjectSettings/HTDA/FastSetup/Templates
```

---

### 📌 Notes

- FastSetup does not modify runtime code.

- It does not depend on HTDA.Framework.Core.

- Safe to remove after project initialization (if desired).

© HTDA Framework