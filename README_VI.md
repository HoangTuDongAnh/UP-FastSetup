<div align="center">

# [🇬🇧 English](README.md) | 🇻🇳 Tiếng Việt

</div>

---

# UP-FastSetup 🚀

[![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**UP-FastSetup** là một công cụ khởi tạo dự án (Project Bootstrapper) mạnh mẽ dành cho Unity. Nó tự động hóa các quy trình nhàm chán khi thiết lập dự án mới bằng cách tạo cấu trúc thư mục, cài đặt Git Submodules, và soạn thảo Scene khởi đầu (Bootstrap scene) với các prefab hệ thống cần thiết.

Được thiết kế cho quy trình làm việc theo **Kiến trúc Module** (sử dụng Git Submodules + Fork).

---

## ✨ Tính năng chính

* **📂 Tự động hóa cấu trúc thư mục**: Tạo hệ thống phân cấp thư mục chuẩn, gọn gàng (Art, Code, Design...) dựa trên file cấu hình của bạn.
* **🔗 Tích hợp Git Submodule**: Tự động thêm các module dùng chung (Core, Audio, UI) thông qua lệnh `git submodule add`, giúp dự án giữ được tính module hóa.
* **🎬 Trình soạn thảo Scene (Scene Composer)**: Tự động tạo `Bootstrap` scene, cấu hình Build Settings, và sinh ra các System Prefabs thiết yếu (ví dụ: AudioManager, UIRoot).
* **⚙️ Cấu hình bằng Code**: Mọi thứ được vận hành chỉ bởi một file JSON đơn giản.
* **📝 Đặt tên động**: Hỗ trợ các từ khóa thay thế như `{ProjectName}` để thích ứng với tên của từng dự án cụ thể.
* **✅ Sẵn sàng cho Git**: Tự động xử lý file `.gitkeep` cho các thư mục rỗng để đảm bảo Git theo dõi được chúng.

---

## 📂 Cấu trúc thư mục chi tiết

Công cụ tạo ra một cấu trúc thư mục **Dựa trên vai trò (Role-based)** và **Theo Module**, được thiết kế để dễ dàng mở rộng.

### 1. Chiến lược thư mục gốc
* **`__MyGame/`**: (Hai dấu gạch dưới) Thư mục dành riêng cho các asset của dự án hiện tại. Được xếp lên đầu để dễ truy cập.
* **`_MyCore/`**: (Một dấu gạch dưới) Chứa các Module dùng chung (Git Submodules) được tái sử dụng qua nhiều dự án.
* **`3rdParty/`**: Thư mục cách ly dành cho các package tải từ Asset Store (để giữ cho thư mục `Plugins` sạch sẽ).

### 2. Cây thư mục

```text
Assets/
├── __MyGame/                 <-- 🏠 PROJECT SPECIFIC (Ưu tiên cao)
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
│   │   ├── Sound/            # SFX
│   │   └── Mixer/
│   │
│   ├── Code/                 # [Khu vực Programmer]
│   │   └── Script/
│   │       ├── _Common/      # Tiện ích cục bộ, Consts, Helpers
│   │       ├── Gameplay/     # Logic game cốt lõi (Player, Enemy...)
│   │       └── UI/           # Logic UI (Home, HUD, Popup...)
│   │
│   └── Design/               # [Khu vực Designer] - Asset đã cấu hình
│       ├── Config/           # ScriptableObjects (Dữ liệu game)
│       ├── Prefab/           # Các GameObject đã lắp ráp hoàn chỉnh
│       └── Scene/            # Các màn chơi & Bootstrap
│
├── _MyCore/                  <-- 🔗 SHARED MODULES (Git Submodules)
│   ├── Audio/                # ví dụ: UP-Audio-Manager
│   ├── UI/                   # ví dụ: UP-UGUI-Implement
│   ├── Data/
│   └── Utility/
│
├── 3rdParty/                 <-- Tài sản bên thứ 3 (Không sửa code ở đây)
├── Editor/                   <-- Công cụ dự án (Vị trí của UP-FastSetup)
├── Plugins/                  <-- Thư viện Native (.dll, .so, .jar)
├── Resources/                <-- (Hạn chế sử dụng nếu có thể)
└── Settings/                 <-- Cấu hình dự án (URP, Input System)