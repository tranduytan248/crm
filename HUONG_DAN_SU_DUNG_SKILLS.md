# 📖 SỔ TAY HƯỚNG DẪN SỬ DỤNG HỆ THỐNG SKILLS

> Tài liệu tổng hợp toàn bộ các **Skills tự động hóa** được tích hợp trong dự án **CRM / BrewTask**. Mỗi skill đóng vai trò như một chuyên gia chuyên trách một mảng nghiệp vụ, sẵn sàng kích hoạt ngay khi bạn gõ câu lệnh hoặc yêu cầu tương ứng.

---

## ⚡ BẢNG TRA CỨU NHANH (QUICK CHEAT SHEET)

| STT | Tên Skill | Câu lệnh / Mẫu kích hoạt | Mục đích chính |
| :---: | :--- | :--- | :--- |
| 1 | **`version-prod`** | `build <tên version>`<br>*(vd: `build v1.0.0`, `build 2026.09.08`)* | Tự động biên dịch Release, so sánh diff với FTP Prod, lọc file mới và file cập nhật rồi copy vào `version/<tên version>/`. |
| 2 | **`upcode-demo`** | `upcode demo`<br>`đẩy code demo`, `deploy demo` | Biên dịch sang `publish_source/`, merge sang nhánh `upcode-demo` và đẩy lên GitHub để tự deploy lên FTP Demo. |
| 3 | **`phan-tich-van-de`** | `phân tích vấn đề`, `lên checklist`<br>*(hoặc khi đưa ra bài toán mới)* | 4 bước: Phân tích sâu -> Đặt câu hỏi làm rõ -> Xây dựng checklist hành động -> Ghi lại vào `Memory.md`. |
| 4 | **`unit-testing-test-generate`** | `tạo test`, `sinh unit test`<br>`viết test case` | Tự động phân tích code và sinh bộ Unit Test 3 tầng (Happy, Edge, Error) chuẩn AAA cho Dart và C#. |
| 5 | **`test-automator`** | `chạy test tự động`<br>`kiểm thử hồi quy` | Tự động hóa kiểm thử QA, chu trình TDD (Red-Green-Refactor), kiểm thử tích hợp (Integration Test). |
| 6 | **`ui-ux-designer`** | `thiết kế giao diện`<br>`design UI`, `tạo màn hình mới` | Thiết kế giao diện hiện đại, Design Tokens chuẩn VS Code Dark Theme, khoảng cách bội số 4. |
| 7 | **`chuyen-gia-nghiem-thu-design`** | `nghiệm thu giao diện`<br>`soi thiết kế`, `review UI`, `đẹp chưa` | Nhập vai chuyên gia UI khó tính chấm 7 hạng mục (Màu sắc, Typography, Layout...), kết luận ĐẠT / KHÔNG ĐẠT. |
| 8 | **`ui-visual-validator`** | `kiểm tra visual`<br>`bắt lỗi layout`, `check overflow` | Thẩm định trực quan, bắt lỗi tràn viền (overflow), touch target $\ge 48\text{dp}$, đủ 5 trạng thái UI. |
| 9 | **`wcag-audit-patterns`** | `kiểm toán accessibility`<br>`check wcag`, `kiểm tra trợ năng` | Kiểm toán tiêu chuẩn tiếp cận WCAG 2.2 AA (Tương phản $\ge 4.5:1$, Screen Reader, Keyboard Focus). |
| 10 | **`flutter-expert`** | `tối ưu flutter`, `sửa lỗi widget`<br>*(hoặc khi code app BrewTask)* | Clean Architecture, tối ưu render 60/120fps, chống jank frame, tuân thủ 100% dùng bộ widget `App*`. |

---

## 🚫 QUY TẮC QUAN TRỌNG NHẤT: IGNORE BIN & OBJ
- **TUYỆT ĐỐI KHÔNG ĐƯỢC ĐƯA THƯ MỤC `bin/` VÀ `obj/` CỦA CÁC PROJECT TRUNG GIAN LÊN GITHUB.**
- Tệp `.gitignore` luôn bảo vệ hệ thống với các quy tắc:
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
- **Chỉ có 3 thư mục phát hành sau được phép lưu trữ trên Git:**
  1. `publish_source/`: Chứa bản build đầy đủ để triển khai FTP Demo.
  2. `version/`: Chứa các gói patch/update production được đóng gói theo từng phiên bản.
  3. `dlls/`: Chứa các thư viện dll phụ thuộc từ bên ngoài.

---

## 📦 CHI TIẾT CÁC SKILL TRIỂN KHAI & ĐÓNG GÓI

### 1. Skill `version-prod` (Đóng gói Production theo phiên bản)
- **Mẫu lệnh**: `build <tên version>` (Ví dụ: `build v1.0.0`, `build 2026.09.08`, `build prod-patch-1`)
- **Key cấu hình trên GitHub**: `FTP_SERVER_PROD`, `FTP_USERNAME_PROD`, `FTP_PASSWORD_PROD`.
- **Cách thức hoạt động**:
  1. Tự động biên dịch WebApp ở chế độ `Release` (target `Package`).
  2. Kết nối tới máy chủ FTP Production để quét toàn bộ file hiện có.
  3. Thực hiện so sánh (Diff) logic:
     - **File NEW**: Tệp xuất hiện trong bản build mới mà trên FTP chưa có.
     - **File MODIFIED**: Tệp đã có trên FTP nhưng có sự thay đổi về kích thước hoặc nội dung.
     - **File UNCHANGED**: Tệp giống hệt trên FTP -> Bỏ qua.
  4. Tự động sao chép các file NEW và MODIFIED vào thư mục `version/<tên version>/` (giữ nguyên cấu trúc thư mục phân cấp web).
  5. Xuất báo cáo tóm tắt tại: `version/<tên version>/manifest.txt` và `manifest.json`.
- **Chạy trực tiếp từ PowerShell**:
  ```powershell
  powershell -ExecutionPolicy Bypass -File .\scripts\build_version_prod.ps1 -VersionName "v1.0.0"
  ```
- **Chạy trên GitHub Actions**: Vào tab **Actions** -> Chọn workflow **Build Version Production** -> Nhập `version_name` -> Bấm **Run workflow**.

---

### 2. Skill `upcode-demo` (Triển khai tự động lên môi trường Demo)
- **Mẫu lệnh**: `upcode demo`, `upload code demo`, `đẩy code demo`, `deploy demo`
- **Key cấu hình trên GitHub**: `FTP_SERVER_DEMO`, `FTP_USERNAME_DEMO`, `FTP_PASSWORD_DEMO`.
- **Cách thức hoạt động**:
  1. Biên dịch toàn bộ Solution ở chế độ `Release`.
  2. Đóng gói đầy đủ WebApp vào thư mục `publish_source/`.
  3. Tự động chuyển và merge code sang nhánh `upcode-demo`.
  4. Đẩy (push) nhánh `upcode-demo` lên GitHub.
  5. Workflow GitHub Action `.github/workflows/deploy-demo.yml` kích hoạt và đồng bộ toàn bộ thư mục `publish_source/` lên FTP Demo.
  6. Tự động chuyển về lại nhánh làm việc ban đầu.

---

## 🧠 CHI TIẾT CÁC SKILL QUẢN LÝ VÀ PHÂN TÍCH

### 3. Skill `phan-tich-van-de`
- **Mẫu lệnh**: `phân tích vấn đề`, `lên checklist`, hoặc khi bạn nêu ra một tính năng/lỗi mới.
- **Quy trình 4 giai đoạn**:
  1. **Phân tích bối cảnh**: Mục tiêu, phạm vi, ràng buộc kỹ thuật, rủi ro.
  2. **Đặt câu hỏi làm rõ**: Đặt tối đa 3–5 câu hỏi quan trọng để chốt yêu cầu với bạn.
  3. **Xây dựng checklist**: Chia nhỏ thành từng đầu việc cụ thể sau khi nhận câu trả lời.
  4. **Ghi vào Memory**: Lưu lại toàn bộ bối cảnh và tiến độ vào tệp `Memory.md` để không bị quên ngữ cảnh.

---

## 🧪 CHI TIẾT CÁC SKILL KIỂM THỬ & CHẤT LƯỢNG MÃ NGUỒN

### 4. Skill `unit-testing-test-generate`
- **Mẫu lệnh**: `tạo test cho hàm X`, `sinh unit test`, `viết test case`
- **Cách thức hoạt động**:
  - Sinh test theo cấu trúc **Arrange - Act - Assert (AAA)**.
  - Bao phủ đủ **3 tầng kịch bản**:
    1. *Happy Path*: Trường hợp chuẩn, hoạt động đúng logic.
    2. *Edge Cases*: Ranh giới, rỗng (`null`, `empty`, `0`, âm, chuỗi dài).
    3. *Error Handling*: Bắt đúng ngoại lệ (Exception) và mã lỗi.
  - Áp dụng cho cả **C# (.NET MVC 5)** và **Dart (Flutter)**.

### 5. Skill `test-automator`
- **Mẫu lệnh**: `chạy test tự động`, `kiểm thử hồi quy`
- **Cách thức hoạt động**: Thực thi chu trình TDD (Red -> Green -> Refactor), tự động chạy test suite khi sửa code, phát hiện hồi quy và đảm bảo tỷ lệ pass 100%.

---

## 🎨 CHI TIẾT CÁC SKILL THIẾT KẾ GIAO DIỆN & TIẾP CẬN

### 6. Skill `ui-ux-designer`
- **Mẫu lệnh**: `thiết kế màn hình X`, `design UI`, `tạo giao diện`
- **Quy chuẩn thiết kế**:
  - Hệ màu VS Code Dark Theme: Nền `#1E1E1E`, Sidebar `#252526`, Card `#2D2D2D`, Điểm nhấn `#0E639C` / `#007ACC`.
  - Khoảng cách chia hết cho 4dp (4, 8, 12, 16, 24, 32).
  - Phân cấp thị giác rõ ràng qua cỡ chữ và trọng số font.

### 7. Skill `chuyen-gia-nghiem-thu-design`
- **Mẫu lệnh**: `nghiệm thu giao diện`, `soi thiết kế`, `đẹp chưa`, `góp ý giao diện`
- **Đặc trưng**: Đóng vai chuyên gia **khó tính**, không khen xã giao, chấm điểm khắt khe qua 7 tiêu chí: Màu sắc, Typography, Layout, Trạng thái (Empty/Loading/Error), Touch Target, Phản hồi thao tác, Tính nhất quán. Kết luận dứt khoát: **ĐẠT** hoặc **KHÔNG ĐẠT**.

### 8. Skill `ui-visual-validator`
- **Mẫu lệnh**: `kiểm tra visual`, `bắt lỗi layout`, `check overflow`
- **Đặc trưng**: Soi từng pixel, kiểm tra kích thước vùng bấm tối thiểu $\ge 48\text{dp}$, bắt lỗi tràn màn hình (RenderFlex overflowed), đảm bảo hiển thị đúng trên mọi kích thước màn hình.

### 9. Skill `wcag-audit-patterns`
- **Mẫu lệnh**: `kiểm toán wcag`, `kiểm tra trợ năng`, `check tương phản`
- **Đặc trưng**: Kiểm toán khả năng tiếp cận WCAG 2.2 cấp độ AA. Kiểm tra tỷ lệ tương phản chữ/nền tối thiểu 4.5:1, hỗ trợ công cụ đọc màn hình và điều hướng bàn phím.

### 10. Skill `flutter-expert`
- **Mẫu lệnh**: Khi xử lý ứng dụng di động BrewTask.
- **Đặc trưng**: Quản lý State tối ưu, đảm bảo tốc độ khung hình 60/120fps không giật lag, **bắt buộc 100% sử dụng bộ Custom Widgets `App*`** có sẵn trong thư mục `lib/core/widgets/` thay vì dùng widget thô.

---

## 💡 MẸO SỬ DỤNG HÀNG NGÀY
1. **Khi muốn đưa bản build mới lên môi trường Demo**: Chỉ cần gõ `upcode demo`.
2. **Khi muốn tạo bản cập nhật cho Production**: Chỉ cần gõ `build <tên version>` (ví dụ: `build v1.0.1`).
3. **Khi chuẩn bị làm một tính năng mới hoặc bài toán phức tạp**: Chỉ cần mô tả yêu cầu, Skill `phan-tich-van-de` sẽ tự động phân tích và lập checklist giúp bạn.
4. **Khi vừa hoàn thành một giao diện**: Gõ `nghiệm thu giao diện` để chuyên gia soi lỗi trước khi bàn giao.
