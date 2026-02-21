# HTDA Framework – FastSetup

## Overview

FastSetup is an Editor-only utility module that helps initialize Unity projects following HTDA architectural standards.

It provides:

- Standardized folder structure generation
- Framework module installation via Git submodules
- Bootstrap scene creation
- Multiple template support with metadata

---

## Architecture Position

FastSetup belongs to:

Tier 3 – Project Bootstrap

It does NOT:
- Contain runtime logic
- Depend on Core
- Affect builds

---

## Config Location


ProjectSettings/HTDA/FastSetup/setup-config.json


This keeps project initialization logic separate from runtime Assets.

---

## Template System

FastSetup supports two template formats:

### 1️⃣ Legacy (SetupConfig only)

```json
{
  "projectName": "MyGame",
  "folderGroups": [...]
}
```

### 2️⃣ Metadata Container (recommended)

```json
{
  "meta": {
    "id": "general",
    "name": "General",
    "description": "General layout",
    "tags": ["default"],
    "version": "1.0.0"
  },
  "config": { ... }
}
```

---

## Folder Generation System

Supports:

- folders (flat)

- folderGroups (recommended)

folderGroups allows root + nested paths for scalable project layouts.

---

## Module Installation

Uses Git submodules to keep framework modules version-controlled and independent.

Example:
```
Packages/com.htda.framework.core
```

---

## Bootstrap Scene

Optional bootstrap configuration:

- Scene name

- Save location

- System prefabs

- Add to Build Settings

---

## Extending

You can:

- Add new template JSON files inside the package

- Export custom templates per project

- Customize module installation paths

- Disable bootstrap if not needed

---

## Best Practice

Use FastSetup only during project initialization phase.

Once project structure is stable, the tool can remain installed or be removed safely.

---

End of FastSetup documentation.# HTDA Framework – FastSetup
