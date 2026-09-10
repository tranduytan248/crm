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
### Cập nhật chẩn đoán kết nối
- [x] Xác nhận workflow đã nhận được `FTP_SERVER_DEMO` và bắt đầu kết nối.
- [x] Xác định GitHub-hosted runner lỗi `Timeout (control socket)` khi mở kết nối FTP.
- [x] Kiểm tra máy nội bộ `10.57.33.71` kết nối thành công tới FTP `10.57.30.10:21`.
- [ ] Triển khai trực tiếp từ máy nội bộ hoặc cấu hình self-hosted runner trong mạng VNPT.

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

---

# 2026-09-09 Vấn đề: Chuyển deploy FTP Demo sang chạy cục bộ

## 1. Mô tả vấn đề
Sửa skill `deploy-ftp-demo` để tự động upload `publish_source/` trực tiếp từ máy trong mạng VNPT, không thông qua GitHub Actions.

## 2. Phân tích ban đầu
- Bối cảnh: GitHub-hosted runner không truy cập được FTP nội bộ `10.57.30.10:21`, trong khi máy làm việc kết nối được.
- Mục tiêu: Lệnh deploy chạy trực tiếp, ổn định và báo lỗi chính xác khi upload không hoàn tất.
- Phạm vi: Skill deploy, script PowerShell upload FTP và cách lưu credential cục bộ.
- Ràng buộc: Không commit mật khẩu FTP; chỉ báo thành công khi mọi file được upload.
- Quyết định: Dùng credential DPAPI cục bộ, ưu tiên biến môi trường nếu được cung cấp; bỏ hoàn toàn GitHub khỏi quy trình skill.

## 3. Câu hỏi làm rõ
1. Có triển khai trực tiếp từ máy trong mạng VNPT không? → Người dùng đã xác nhận.
2. Có bỏ GitHub Actions khỏi quy trình skill không? → Người dùng đã xác nhận.

## 4. Câu trả lời & Quyết định
1. Triển khai FTP cục bộ tới server nội bộ bằng script PowerShell.
2. Lưu credential trong `.secrets/ftp-demo.credential.xml` được mã hóa theo tài khoản Windows và bị Git ignore.
3. Không ghi mật khẩu trong skill, script hoặc log.

## 5. Checklist
### Chuẩn bị
- [x] Kiểm tra kết nối TCP tới FTP nội bộ.
- [x] Tạo credential DPAPI cục bộ và thêm `.secrets/` vào `.gitignore`.

### Thực hiện
- [x] Cập nhật skill để bỏ checkout, push và GitHub Actions.
- [x] Xóa mật khẩu hard-code khỏi script deploy.
- [x] Bổ sung kiểm tra DLL publish và thống kê file upload thất bại.

### Kiểm tra / Nghiệm thu
- [x] Kiểm tra cú pháp PowerShell hợp lệ.
- [x] Kiểm tra credential cục bộ đọc được bởi tài khoản Windows hiện tại.
- [x] Kiểm tra `10.57.30.10:21` đang kết nối được.
- [x] Chạy deploy thực tế: upload thành công 3.721/3.721 file lên FTP Demo `10.57.30.10:21`.

### Ghi chú
- Mật khẩu từng tồn tại trong lịch sử Git; cần đổi mật khẩu FTP sau khi hoàn tất chuyển đổi.

---

# 2026-09-09 Vấn đề: Website Demo lỗi ngay sau deploy FTP cục bộ

## 1. Mô tả vấn đề
Sau khi upload thành công 3.721 file từ `publish_source/` lên FTP Demo, website phát sinh lỗi ngay.

## 2. Phân tích ban đầu
- Bối cảnh: Bản ASP.NET MVC 5 được upload từng file trực tiếp vào website đang chạy.
- Mục tiêu: Khôi phục website Demo và xác định nguyên nhân trước khi deploy lại.
- Phạm vi: HTTP error, IIS/Application log, `Web.config`, cấu hình môi trường, DLL và tính nhất quán của bản upload.
- Ràng buộc: Chưa có nội dung lỗi/HTTP status và chưa xác định toàn site hay một chức năng bị ảnh hưởng.
- Rủi ro / Giả định: Upload thành công về vận chuyển không chứng minh ứng dụng khởi động thành công; upload tuần tự có thể làm IIS nạp một bộ file chưa đồng nhất trong quá trình triển khai.
- Phương án sơ bộ: (1) thu thập lỗi HTTP/IIS; (2) đối chiếu cấu hình Demo; (3) rollback bản ổn định nếu cần khôi phục khẩn cấp; (4) cải tiến deploy theo gói/staging để tránh trạng thái nửa chừng.

## 3. Câu hỏi làm rõ
1. Website hiển thị chính xác mã và nội dung lỗi gì? Cần ảnh đầy đủ hoặc text lỗi, gồm HTTP 500/500.19/502/404 nếu có.
2. Toàn bộ website lỗi hay chỉ chức năng vừa cập nhật? URL nào đang lỗi?
3. Website hoạt động bình thường ngay trước lần deploy này không?
4. Có quyền xem IIS Event Viewer/log ứng dụng hoặc quyền phục hồi bản Demo cũ không?

## 4. Câu trả lời & Quyết định
1. URL lỗi: `http://crm.cenit.vn/`.
2. Lỗi toàn ứng dụng: `System.ArgumentException: Format of the initialization string does not conform to specification starting at index 0` trong `SqlConnection` khi `Application_Start` tải cache.
3. Kiểm tra cục bộ xác nhận cả 4 connection string trong `publish_source/Web.config` đều là token MSDeploy dạng `$(ReplacableToken_...)`, không phải connection string SQL; các chuỗi trong WebApp nguồn và `Source_Prod` đều hợp lệ.
4. Nguyên nhân gốc: target `Package` của MSBuild tự động parameterize connection string, sau đó `build_publish.ps1` copy trực tiếp `PackageTmp` sang `publish_source` mà không chạy bước MSDeploy thay token.
5. Quyết định đề xuất: tắt `AutoParameterizationWebConfigConnectionStrings` khi tạo package, rebuild, kiểm tra mọi connection string bằng `SqlConnectionStringBuilder`, rồi mới deploy lại.

## 5. Checklist
### Chuẩn bị
- [x] Xác định lỗi HTTP và stack trace khởi động ứng dụng.
- [x] Xác nhận connection string trong `publish_source/Web.config` không hợp lệ.
- [x] Xác định token MSDeploy là nguyên nhân trực tiếp.

### Thực hiện
- [ ] Cập nhật `build_publish.ps1` để tắt auto-parameterization connection string.
- [ ] Build lại `publish_source` ở cấu hình Release.
- [ ] Chặn deploy nếu `Web.config` còn token `$(ReplacableToken_...)` hoặc connection string không parse được.
- [ ] Deploy lại bản đã kiểm tra lên FTP Demo.

### Kiểm tra / Nghiệm thu
- [ ] Xác nhận `http://crm.cenit.vn/` khởi động không còn lỗi connection string.
- [ ] Xác nhận đăng nhập và truy vấn database Demo hoạt động.
- [ ] Xác nhận 4 provider kết nối đều dùng cấu hình hợp lệ.

### Ghi chú
- Không dùng trực tiếp `PackageTmp` khi connection string còn được MSDeploy parameterize.

---

# 2026-09-09 Vấn đề: Giữ cấu hình riêng khi deploy FTP

## 1. Mô tả vấn đề
Khi deploy FTP Demo, không upload `Web.config` và `Configs/AppSettings.config` vì Demo và Production sử dụng cấu hình môi trường khác nhau.

## 2. Phân tích ban đầu
- Bối cảnh: Quy trình hiện tại upload toàn bộ `publish_source`, từng ghi đè cấu hình server và làm ứng dụng lỗi.
- Mục tiêu: Cập nhật code ứng dụng nhưng giữ nguyên cấu hình đang hoạt động trên server.
- Phạm vi: Chỉ loại trừ `Web.config` gốc và `Configs/AppSettings.config`; các Web.config con vẫn triển khai.
- Ràng buộc: So sánh đường dẫn không phân biệt hoa thường trên Windows/FTP.
- Rủi ro / Giả định: Hai file cấu hình đã tồn tại và hợp lệ trên server trước khi deploy.
- Phương án: Lọc hai đường dẫn trong script trước khi upload và báo cáo chúng dưới trạng thái `SKIP`.

## 3. Câu hỏi làm rõ
1. Có giữ nguyên cả hai file trên server ở mọi lần deploy không? → Có, theo yêu cầu người dùng.
2. Có tiếp tục upload các `Web.config` nằm trong thư mục con không? → Có, chỉ loại trừ đúng hai đường dẫn được nêu.

## 4. Câu trả lời & Quyết định
1. Xem hai file cấu hình môi trường là server-owned và không ghi đè qua FTP.
2. Báo rõ số file được bỏ qua để kết quả deploy có thể kiểm chứng.

## 5. Checklist
### Chuẩn bị
- [x] Xác định chính xác hai đường dẫn cần bảo vệ.

### Thực hiện
- [x] Cập nhật skill với quy tắc không ghi đè cấu hình môi trường.
- [x] Cập nhật script để lọc đường dẫn không phân biệt hoa thường.
- [x] Bổ sung báo cáo file `SKIP` và tổng số file bỏ qua.

### Kiểm tra / Nghiệm thu
- [x] Xác nhận bằng kiểm thử khô rằng đúng hai file bị loại trừ.
- [x] Xác nhận `Views/Web.config` vẫn nằm trong danh sách upload.
- [x] Chạy deploy thực tế ngày 2026-09-10: upload thành công 3.720/3.720 file, bỏ qua `Web.config` và `Configs/AppSettings.config`; URL Demo phản hồi HTTP 200 và chuyển tới trang đăng nhập SSO.

### Ghi chú
- Server mới hoặc thư mục đã bị xóa sạch phải được khôi phục hai file cấu hình hợp lệ trước khi chạy deploy.

### Cập nhật chẩn đoán quyền ghi JobLogs
- [x] Xác nhận ứng dụng đã qua bước đọc connection string và tới `Application_Start` đăng ký jobs.
- [x] Xác định tài khoản IIS Application Pool không có quyền ghi vào `Contents/JobLogs`.
- [x] Xác định `JobLogWriter.FlushLogToFile` gọi `File.AppendAllText` không có cơ chế fallback, khiến lỗi ghi log làm sập quá trình khởi động và che lỗi gốc của `Jobs.ClearData.dll`.
- [ ] Xác định tên Application Pool/identity đang chạy website Demo.
- [ ] Cấp quyền `Modify` có kế thừa cho identity đó trên `Contents/JobLogs`.
- [ ] Khởi động lại Application Pool và kiểm tra website.
- [ ] Gia cố `JobLogWriter` để lỗi ghi log không làm sập `Application_Start`.
- [x] Xác định chính xác lệnh yêu cầu quyền ghi: `File.AppendAllText` tại `TSFramework.Libs/Models/Log/JobLogWriter.cs:144`, đường dẫn cố định `Contents/JobLogs`.
- [ ] Tạm đặt `App_Register_Job=0` trên Demo để cô lập khối khởi tạo job, sau đó phục hồi về `1` khi ACL đã sửa.
