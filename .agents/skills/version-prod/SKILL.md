---
name: version-prod
description: Tự động đóng gói và tạo version phát hành Production theo mẫu "build <tên version>" (ví dụ: "build v1.0.0", "build 2026.09.08"). Tự động biên dịch Release, kết nối đến FTP Production thông qua các key FTP_SERVER_PROD, FTP_USERNAME_PROD, FTP_PASSWORD_PROD để so sánh diff: lấy ra các file chưa có trên FTP và các file có version cập nhật, rồi copy toàn bộ vào thư mục version/<tên version>/. TUÂN THỦ LOẠI TRỪ (IGNORE) HOÀN TOÀN CÁC THƯ MỤC BIN VÀ OBJ TRUNG GIAN. LUÔN kích hoạt skill này khi người dùng gõ lệnh theo mẫu "build tên version" (ví dụ: build v1.0, build release-1.0, build 2026.09.08).
---

# Version Prod Workflow (build <tên version>)

Khi người dùng nhập câu lệnh theo mẫu **`build <tên version>`** (ví dụ: `build v1.0.0`, `build 2026.09.08`, `build prod-patch-1`), agent PHẢI kích hoạt skill này và thực thi quy trình tuần tự dưới đây.

---

## 🚫 QUY TẮC QUAN TRỌNG: IGNORE THƯ MỤC BIN & OBJ
1. **TUYỆT ĐỐI KHÔNG** commit hay push bất kỳ thư mục `bin/` và `obj/` trung gian nào của các project con (`Modules.*`, `CenIT.*`, `Jobs.*`, `Core.*`) lên GitHub.
2. **CHỈ DUY NHẤT** các thư mục phát hành sau được phép giữ lại:
   - `publish_source/` (gói xuất bản demo/đầy đủ)
   - `version/` và `version/**` (các gói cập nhật version theo từng phiên bản)
   - `dlls/` (thư viện ngoài)
3. Tệp `.gitignore` ở thư mục gốc luôn phải duy trì các quy tắc sau:
   ```gitignore
   [Bb]in/
   [Oo]bj/
   .vs/
   *.user
   *.suo
   !publish_source/
   !publish_source/**
   !version/
   !version/**
   !dlls/
   !dlls/**
   ```

---

## 🔑 THÔNG TIN KẾT NỐI FTP PRODUCTION
Các key kết nối được cấu hình trên GitHub Repository Secrets và biến môi trường:
- `FTP_SERVER_PROD`: Địa chỉ IP máy chủ FTP Production (ví dụ `10.57.47.3`).
- `FTP_USERNAME_PROD`: Tài khoản FTP Production (ví dụ `crm`).
- `FTP_PASSWORD_PROD`: Mật khẩu FTP Production (ví dụ `Kh@2026`).

---

## QUY TRÌNH THỰC HIỆN CHI TIẾT

### Bước 1 — Bóc tách Tên Version từ Câu lệnh
- Từ câu lệnh của người dùng (ví dụ: `build v1.0.0`, `build 2026.09.08`), trích xuất chuỗi `<tên version>`:
  - Nếu user nhập `build v1.0.0` -> `<tên version>` = `v1.0.0`
  - Nếu user nhập `build 2026.09.08` -> `<tên version>` = `2026.09.08`
- Loại bỏ các ký tự đặc biệt nguy hiểm cho hệ điều hành, đảm bảo tên thư mục hợp lệ.

---

### Bước 2 — Kiểm tra Trạng thái Git
Kiểm tra nhanh nhánh hiện tại và đảm bảo không có file `bin`/`obj` trung gian bị theo dõi:
```powershell
git status
```

---

### Bước 3 — Thực thi Script Tạo Version Production
Chạy script PowerShell tự động hóa toàn bộ quy trình:
```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build_version_prod.ps1 -VersionName "<tên version>"
```

*Script này tự động thực hiện 5 tác vụ:*
1. **Biên dịch Release**: Sử dụng MSBuild để build `CenIT.Solution.TOC.WebApp.csproj` (target `Package`, `Configuration=Release`).
2. **Quét bản build**: Quét toàn bộ tệp và kích thước trong gói build (`PackageTmp`).
3. **Kết nối FTP Production**: 
   - Đăng nhập FTP bằng thông tin `FTP_SERVER_PROD`, `FTP_USERNAME_PROD`, `FTP_PASSWORD_PROD`.
   - Quét đệ quy toàn bộ danh sách tệp trên FTP Prod (lấy tên, kích thước).
4. **So sánh (Diff) logic chuẩn xác**:
   - **File NEW**: Tệp xuất hiện trong bản build nhưng **chưa từng có** trên FTP Prod.
   - **File MODIFIED**: Tệp **đã có** trên FTP Prod nhưng có kích thước khác biệt (hoặc assembly version thay đổi).
   - **File UNCHANGED**: Tệp giống hệt trên FTP Prod -> Bỏ qua, không đưa vào gói patch.
5. **Sao chép, Tạo Manifest và Ghi Chú Cập Nhật (UPDATE_NOTES.md)**:
   - Toàn bộ file NEW và MODIFIED được sao chép vào thư mục: `version/<tên version>/` (giữ nguyên cấu trúc thư mục tương đối như `bin/`, `Views/`, `Contents/`, `Configs/`,...).
   - Tạo tệp `manifest.json` và `manifest.txt` ghi lại chi tiết trạng thái từng tệp.
   - **TỰ ĐỘNG TẠO TỆP `UPDATE_NOTES.md`**: Ghi rõ nội dung cập nhật, lịch sử git commit gần nhất, bảng danh sách chi tiết các file MODIFIED (kèm lý do và kích thước) và danh sách các file NEW theo từng nhóm (Assemblies, Views, Configs, Contents) cùng hướng dẫn triển khai.

*(Lưu ý về môi trường mạng: Nếu máy trạm bị chặn firewall sang dải IP của FTP Prod `10.57.47.3`, script sẽ thông báo rõ ràng và hỗ trợ đóng gói đầy đủ hoặc chuyển qua chạy trên GitHub Actions workflow `.github/workflows/build-version-prod.yml`).*

---

### Bước 4 — Kiểm tra Thư mục `version/<tên version>`
Kiểm tra danh sách tệp và nội dung báo cáo:
```powershell
Get-ChildItem -Path "version\<tên version>"
Get-Content -Path "version\<tên version>\UPDATE_NOTES.md"
Get-Content -Path "version\<tên version>\manifest.txt"
```

---

### Bước 5 — Báo cáo Kết quả cho Người dùng
Xuất báo cáo tổng kết rõ ràng bao gồm:
1. **Tên version**: `<tên version>`
2. **Thư mục lưu trữ**: `version/<tên version>/`
3. **Số lượng file mới (NEW)** và **Số lượng file cập nhật (MODIFIED)**
4. **Trạng thái so sánh với FTP Prod**
5. **Tệp Ghi chú Cập nhật**: `version/<tên version>/UPDATE_NOTES.md` (chứa danh sách chính xác file nào cập nhật, nội dung cập nhật)
6. **Đường dẫn tệp Manifest**: `version/<tên version>/manifest.txt` và `manifest.json`
6. **Xác nhận**: Đã tuân thủ loại trừ hoàn toàn các thư mục `bin/` và `obj/` trung gian của mã nguồn.

---

## 🛠️ Triển khai qua GitHub Actions (Tùy chọn)
Nếu cần build và trích xuất version trực tiếp từ GitHub runner sử dụng GitHub Secrets:
1. Vào tab **Actions** trên GitHub repository `tranduytan248/crm`.
2. Chọn workflow **Build Version Production**.
3. Nhấn **Run workflow**, điền ô `version_name` (ví dụ `v1.0.0`) và bấm **Run**.
4. Sau khi hoàn thành, tải gói file zip của version trực tiếp tại mục **Artifacts**.
