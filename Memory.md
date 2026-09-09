---

# 2026-09-09 Vấn đề: GitHub Actions deploy FTP Demo thất bại

## 1. Mô tả vấn đề
Hai lần chạy workflow `Deploy Demo via FTP` trên nhánh `upcode-demo` đều báo lỗi sau khoảng 3–4 giây.

## 2. Phân tích ban đầu
- Bối cảnh: Workflow `.github/workflows/deploy-demo.yml` dùng `SamKirkland/FTP-Deploy-Action@v4.3.5` để tải `publish_source/` lên FTP Demo.
- Mục tiêu: Xác định nguyên nhân job lỗi và triển khai thành công bản demo.
- Phạm vi: Cấu hình GitHub Actions, ba GitHub Secrets FTP, khả năng kết nối FTP từ runner và phương án upload cục bộ.
- Ràng buộc: Repository riêng tư nên không thể đọc log Actions khi chưa xác thực; FTP có khả năng là địa chỉ mạng nội bộ.
- Rủi ro / Giả định: Job thất bại rất sớm nên có thể secret bị thiếu/rỗng; nếu FTP dùng IP `10.x` thì public GitHub runner có thể không có tuyến mạng. Script fallback hiện có thông tin đăng nhập mặc định trong mã nguồn, tạo rủi ro lộ bí mật và cần được khắc phục.
- Phương án sơ bộ: (1) đọc dòng lỗi chi tiết trong run; (2) bổ sung/sửa Secrets nếu thiếu; (3) nếu runner không vào được mạng nội bộ thì dùng máy nội bộ hoặc self-hosted runner; (4) xoay vòng thông tin FTP và loại bỏ bí mật khỏi source.

## 3. Câu hỏi làm rõ
1. Trong run lỗi, bước nào có dấu X đỏ và dòng lỗi cuối cùng ghi chính xác nội dung gì?
2. Trong `Settings > Secrets and variables > Actions`, cả ba secret `FTP_SERVER_DEMO`, `FTP_USERNAME_DEMO`, `FTP_PASSWORD_DEMO` đã tồn tại chưa?
3. `FTP_SERVER_DEMO` có phải là IP nội bộ dạng `10.x.x.x` và máy hiện tại có đang kết nối mạng/VPN VNPT không?

## 4. Câu trả lời & Quyết định
1. Log GitHub Actions báo: job không được khởi chạy do thanh toán tài khoản gần đây thất bại hoặc spending limit cần được tăng.
2. Quyết định: Không sửa workflow hoặc cấu hình FTP vì lỗi xảy ra trước khi runner bắt đầu chạy.
3. Hướng xử lý: Khắc phục Billing & plans rồi chạy lại workflow; nếu cần triển khai ngay thì dùng script fallback từ máy trong mạng/VPN VNPT.

## 5. Checklist
### Chuẩn bị
- [x] Xác nhận thông báo lỗi chính xác từ GitHub Actions.
- [ ] Kiểm tra phương thức thanh toán trong GitHub `Settings > Billing & plans`.
- [ ] Kiểm tra và tăng Actions spending limit nếu giới hạn đang bằng 0 hoặc đã dùng hết.

### Thực hiện
- [ ] Cập nhật phương thức thanh toán hoặc spending limit của tài khoản/tổ chức sở hữu repository.
- [ ] Chạy lại workflow `Deploy Demo via FTP` sau khi Billing hoạt động.
- [ ] Chạy `scripts/deploy_ftp_demo.ps1` từ máy trong mạng/VPN VNPT nếu cần deploy ngay mà không chờ GitHub Actions.

### Kiểm tra / Nghiệm thu
- [ ] Xác nhận GitHub runner bắt đầu job thay vì dừng ở bước khởi tạo.
- [ ] Xác nhận bước upload FTP hoàn tất thành công.
- [ ] Kiểm tra website Demo và chức năng vừa cập nhật.

### Ghi chú
- Workflow và source build hiện không phải hiện lỗi trong lần chạy này vì runner chưa được khởi tạo.
- Cần loại bỏ credential FTP mặc định khỏi source và đổi mật khẩu FTP đã lộ trong lịch sử repository.
