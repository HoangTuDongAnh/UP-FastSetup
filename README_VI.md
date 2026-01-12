<div align="center">

# [🇬🇧 English](README.md) | 🇻🇳 Tiếng Việt

</div>

---

# UP-FastSetup 🚀

[![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**UP-FastSetup** là công cụ khởi tạo dự án (Project Bootstrapper) mạnh mẽ dành cho Unity. Nó giúp tự động hóa các quy trình thiết lập thủ công nhàm chán bằng cách tạo cấu trúc thư mục chuẩn, cài đặt Git Submodules và khởi tạo Scene cơ sở (Bootstrap scene) kèm các Prefab hệ thống.

Công cụ được thiết kế tối ưu cho quy trình làm việc theo **Kiến trúc Module** (sử dụng Git Submodules + Fork).

---

## ✨ Tính năng nổi bật

* **📂 Tự động hóa cấu trúc thư mục**: Tạo cây thư mục chuẩn, gọn gàng (Art, Code, Design...) dựa trên file cấu hình JSON.
* **🔗 Tích hợp Git Submodule**: Tự động thêm các module dùng chung (như Core, Audio, UI) thông qua lệnh `git submodule add`, giúp code sạch và dễ tái sử dụng.
* **🎬 Trình soạn thảo Scene (Scene Composer)**: Tự động tạo `Bootstrap.unity`, cấu hình Build Settings (đưa lên đầu danh sách), và sinh ra các Prefab hệ thống (ví dụ: AudioManager, UIRoot).
* **⚙️ Cấu hình bằng Code**: Mọi thao tác được điều khiển bởi một file `setup-config.json` duy nhất.
* **📝 Đặt tên động (Dynamic Naming)**: Hỗ trợ từ khóa `{ProjectName}` để tự động thay thế tên thư mục theo tên dự án thực tế.
* **✅ Chuẩn Git**: Tự động tạo file `.gitkeep` cho các thư mục rỗng để đảm bảo Git theo dõi được cấu trúc dự án.

---

## 📂 Cấu trúc thư mục chi tiết

Công cụ tạo ra cấu trúc thư mục phân chia theo **Vai trò (Role-based)** và **Module**, đảm bảo khả năng mở rộng cho dự án lớn.

### 1. Chiến lược thư mục gốc
* **`__MyGame/`**: (Hai dấu gạch dưới) Thư mục chứa tài nguyên riêng biệt của dự án hiện tại. Được ưu tiên hiển thị trên cùng.
* **`_MyCore/`**: (Một dấu gạch dưới) Nơi chứa các Module dùng chung (Git Submodules) được đồng bộ giữa các dự án.
* **`3rdParty/`**: Thư mục chứa các tài sản tải từ Asset Store (tách biệt để không lẫn lộn code).

### 2. Sơ đồ cây thư mục

```text
Assets/
├── __MyGame/                 <-- 🏠 DỰ ÁN CỤ THỂ (Ưu tiên cao)
│   ├── Art/                  # [Khu vực Artist] - Asset thô & hiển thị
│   │   ├── Animation/
│   │   ├── Material/
│   │   ├── Model/            # FBX, Meshes
│   │   ├── Sprite/           # UI Images, 2D Sprites
│   │   ├── Texture/
│   │   └── Shader/
│   │
│   ├── Audio/                # [Khu vực Audio]
│   │   ├── Music/
│   │   ├── Sound/            # SFX (Hiệu ứng âm thanh)
│   │   └── Mixer/
│   │
│   ├── Code/                 # [Khu vực Programmer]
│   │   └── Script/
│   │       ├── _Common/      # Tiện ích cục bộ, Hằng số (Const)
│   │       ├── Gameplay/     # Logic game chính (Player, Enemy...)
│   │       └── UI/           # Logic UI (Home, HUD, Popup...)
│   │
│   └── Design/               # [Khu vực Designer] - Asset đã cấu hình
│       ├── Config/           # ScriptableObjects (Dữ liệu game)
│       ├── Prefab/           # Các GameObject hoàn chỉnh
│       └── Scene/            # Các màn chơi & Bootstrap
│
├── _MyCore/                  <-- 🔗 MODULE DÙNG CHUNG (Git Submodules)
│   ├── Audio/                # ví dụ: UP-Audio-Manager
│   ├── UI/                   # ví dụ: UP-UGUI-Implement
│   ├── Data/
│   └── Utility/
│
├── 3rdParty/                 <-- Tài sản bên thứ 3 (Không sửa code tại đây)
├── Editor/                   <-- Công cụ dự án (Vị trí của UP-FastSetup)
├── Plugins/                  <-- Thư viện Native (.dll, .so, .jar)
├── Resources/                <-- (Hạn chế sử dụng)
└── Settings/                 <-- Cấu hình Unity (URP, Input System)
```

---

## 🛠️ Yêu cầu tiên quyết

Trước khi sử dụng, hãy đảm bảo bạn đã:

1. Cài đặt **Git** trên máy tính.
2. Khởi tạo Dự án Unity là một Git Repository (`git init`).

---

## 📦 Cài đặt

### Qua Unity Package Manager (UPM)

1. Mở Unity -> Window -> **Package Manager**.
2. Nhấn dấu `+` ở góc trái -> Chọn **Add package from git URL...**
3. Dán đường dẫn repository này vào:
```
[https://github.com/YourUsername/UP-FastSetup.git](https://github.com/YourUsername/UP-FastSetup.git)
```

---

## 🚀 Hướng dẫn sử dụng nhanh

### Bước 1: Tạo file cấu hình

1. Trên thanh menu Unity, chọn: `FastSetup` -> `0. Create Config File`.
2. Một file mới sẽ xuất hiện tại `Assets/Editor/FastSetup/setup-config.json`.
3. (Tùy chọn) Mở file này để chỉnh sửa tên dự án hoặc thêm bớt các module.

### Bước 2: Khởi chạy Setup

1. Chọn menu: `FastSetup` -> `1. Initialize Structure (Execute)`.
2. Công cụ sẽ tự động:
* Tạo toàn bộ thư mục.
* Kéo các Git Submodules về (mất vài phút tùy tốc độ mạng).
* Tạo Bootstrap Scene và spawn các Prefab cần thiết.
* Thêm Bootstrap Scene vào Build Settings.



---

## ⚙️ Cấu hình (`setup-config.json`)

Bạn có toàn quyền kiểm soát quy trình setup thông qua file JSON:

```json
{
  "projectName": "TenDuAnCuaBan",
  "rootNamespace": "TenStudio",
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