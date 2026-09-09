---
name: deploy-ftp-demo
description: Tự động hóa việc triển khai toàn bộ mã nguồn trong thư mục publish_source/ lên máy chủ FTP Demo thông qua GitHub Actions Job (hoặc script fallback cục bộ). Sử dụng 3 key bí mật cấu hình trên GitHub Secrets: FTP_SERVER_DEMO, FTP_USERNAME_DEMO, FTP_PASSWORD_DEMO. LUÔN kích hoạt skill này khi người dùng gõ hoặc yêu cầu "deploy ftp demo", "deloy ftp demo", "deploy demo", "đẩy code lên ftp demo", "upload publish_source lên ftp demo".
---

# Deploy FTP Demo Workflow

Khi người dùng yêu cầu **`deploy ftp demo`** (hoặc **`deloy ftp demo`**, **`deploy demo`**), agent PHẢI kích hoạt skill này và thực thi quy trình tuần tự dưới đây.

---

## 🔑 THÔNG TIN KEY TRÊN GITHUB SECRETS
Workflow trên GitHub Actions sử dụng 3 Secret được lưu trữ tại GitHub Repository (`Settings -> Secrets and variables -> Actions`):
- `FTP_SERVER_DEMO`: Địa chỉ IP máy chủ FTP Demo (ví dụ: `10.57.30.10`).
- `FTP_USERNAME_DEMO`: Tên tài khoản FTP Demo (ví dụ: `quanlydoanhthucenit`).
- `FTP_PASSWORD_DEMO`: Mật khẩu tài khoản FTP Demo.

---

## 🚫 QUY TẮC QUAN TRỌNG: IGNORE THƯ MỤC BIN & OBJ
- **TUYỆT ĐỐI KHÔNG** đẩy các thư mục `bin/` và `obj/` trung gian của các project mã nguồn lên GitHub.
- **CHỈ DUY NHẤT** thư mục gói phát hành `publish_source/` (và `version/`, `dlls/`) được phép đưa lên nhánh deploy.

---

## QUY TRÌNH THỰC HIỆN CHI TIẾT

### Bước 1 — Kiểm Tra Thư Mục `publish_source/`
Trước khi triển khai, kiểm tra xem thư mục `publish_source/` đã có đầy đủ bản build chưa:
```powershell
Test-Path "publish_source\bin\CenIT.Solution.TOC.WebApp.dll"
```
- Nếu thư mục `publish_source/` chưa có hoặc trống: Hãy chạy lệnh biên dịch trước:
  ```powershell
  powershell -ExecutionPolicy Bypass -File .\scripts\build_publish.ps1
  ```

---

### Bước 2 — Đồng Bộ `publish_source/` Sang Nhánh `upcode-demo` Và Push Lên GitHub
Lưu lại tên nhánh hiện tại và chuyển sang nhánh deploy `upcode-demo`:
```powershell
$currentBranch = git branch --show-current
git checkout upcode-demo
git pull origin upcode-demo
git merge $currentBranch -m "chore: deploy publish_source to ftp demo"
git push origin upcode-demo
git checkout $currentBranch
```
*(Nếu nhánh `upcode-demo` chưa có trên remote: dùng `git push -u origin upcode-demo`).*

---

### Bước 3 — Kích Hoạt Job GitHub Actions Tải Lên FTP Demo
Khi nhánh `upcode-demo` nhận commit mới, GitHub Action Workflow **`.github/workflows/deploy-demo.yml`** sẽ tự động kích hoạt:

```yaml
name: Deploy Demo via FTP
on:
  push:
    branches:
      - upcode-demo
  workflow_dispatch:

jobs:
  deploy:
    name: Upload Code to FTP Demo Server
    runs-on: ubuntu-latest
    steps:
      - name: 📥 Checkout repository
        uses: actions/checkout@v4

      - name: 🚀 Upload publish_source to FTP Demo
        uses: SamKirkland/FTP-Deploy-Action@v4.3.5
        with:
          server: ${{ vars.FTP_SERVER_DEMO }}
          username: ${{ vars.FTP_USERNAME_DEMO }}
          password: ${{ secrets.FTP_PASSWORD_DEMO }}
          local-dir: ./publish_source/
          server-dir: /
          dangerous-clean-slate: false
```

- **Tùy chọn kích hoạt thủ công qua GitHub CLI (nếu máy có cài `gh`)**:
  ```powershell
  gh workflow run deploy-demo.yml --ref upcode-demo
  ```

---

### Bước 4 — Phương Án Dự Phòng Trực Tiếp (Local Fallback)
> [!NOTE]
> Nếu máy chủ FTP Demo là IP mạng nội bộ (`10.57.30.10`) không mở cổng NAT ra ngoài Internet cho GitHub runner công cộng, có thể chạy trực tiếp script đẩy code từ máy trạm:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\deploy_ftp_demo.ps1
```
*Script này sẽ kết nối trực tiếp đến FTP Demo trong mạng LAN và tải toàn bộ các tệp trong `publish_source/` lên.*

---

### Bước 5 — Báo Cáo Kết Quả Cho Người Dùng
Xuất báo cáo trạng thái ngắn gọn:
1. **Nguồn triển khai**: Thư mục `publish_source/`
2. **Nhánh Git**: Đã đồng bộ và push lên nhánh `upcode-demo` trên GitHub.
3. **GitHub Action Job**: Workflow **Deploy Demo via FTP** đã được kích hoạt.
4. **Link theo dõi tiến độ Action**: `https://github.com/tranduytan248/crm/actions/workflows/deploy-demo.yml`
5. **Key sử dụng**: `FTP_SERVER_DEMO`, `FTP_USERNAME_DEMO`, `FTP_PASSWORD_DEMO` từ GitHub Secrets.
