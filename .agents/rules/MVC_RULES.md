# 📘 BỘ QUY TẮC PHÁT TRIỂN ỨNG DỤNG MVC (CENIT TOC CRM - MASTER INDEX)

> **Tài liệu tham chiếu chuẩn:** Module Khách hàng (`Cate/Customer`) & Phân hệ Cơ hội kinh doanh (`Cate/RM_BusinessOpportunity`).  
> **Áp dụng cho:** Toàn bộ lập trình viên và AI Agent khi xây dựng View, Controller, JavaScript, Data Model trong hệ thống CenIT TOC CRM.  
> **Nguyên tắc cốt lõi:** Kế thừa chuẩn `Cate/Customer` & `Cate/RM_BusinessOpportunity`, mô hình Form dùng chung (`_Entity.cshtml` + `_Form.cshtml`), xác thực kép (`@Html.ValidationMessageFor` inline cho lỗi trường + Toastr cho kết quả hệ thống), 100% `@Html.*` helper, tuân thủ Design Tokens Ace Admin v4, chuẩn chiều cao 32px cho Search Card, 100% UTF-8 with BOM và đồng bộ 3 nơi (Triple Mirroring).

---

## 📑 DANH MỤC CÁC QUY CHUẨN CHUYÊN ĐỀ (SUB-RULES)

Để việc tra cứu và tuân thủ đạt hiệu quả cao nhất, bộ quy tắc MVC được tổ chức thành các chuyên đề chuyên sâu:

1. 📋 [**MVC_FORM_RULES.md**](file:///d:/SVN/crm/.agents/rules/MVC_FORM_RULES.md):
   - **Mô hình Form dùng chung (Shared Form Pattern):** Gom 100% markup vào `_Entity.cshtml`, tối giản `_Add.cshtml` và `_Edit.cshtml` (~25 dòng) kế thừa `_Form.cshtml`.
   - **Quy chuẩn Xác thực dữ liệu kép (Dual-Layer Validation):** Bắt buộc dùng `@Html.ValidationMessageFor` inline hiển thị chữ đỏ dưới ô nhập liệu khi `!ModelState.IsValid`. Cấm dùng Toastr báo lỗi thay thế cho lỗi trường nhập liệu.
   - **Quy chuẩn Form Controls:** Bắt buộc 100% `@Html.*` helper, nghiêm cấm thẻ HTML thuần (`<label>`, `<input>`, `<select>`).
   - **Chống xung đột DOM ID:** Context suffixing, scoped selectors.
   - **Quy chuẩn CKEditor:** `[AllowHtml]`, `requestValidationMode="2.0"`, đồng bộ `updateElement()`.

2. 🪟 [**MVC_MODAL_AJAX_RULES.md**](file:///d:/SVN/crm/.agents/rules/MVC_MODAL_AJAX_RULES.md):
   - **Quy chuẩn kích thước Modal:** Chuẩn hóa 3 kích thước: `1024px` (Form nghiệp vụ lớn), `800px` (Trung bình), `600px` (Nhỏ).
   - **Layout `_Form.cshtml` & `ajaxForm`:** Kế thừa header/body/footer và submit form multipart có file đính kèm.
   - **Framework Callback `_OnProcessSuccess`:** Cơ chế nhận JSON (Toastr + đóng modal) hoặc HTML (render lại form kèm lỗi validation inline).
   - **Vòng đời Toastr & Chống kẹt Backdrop:** Kích hoạt qua `hidden.bs.modal`, xử lý modal lồng nhau.

3. 🧪 [**TESTING.md**](file:///d:/SVN/crm/.agents/rules/TESTING.md):
   - Kiểm thử Form Submit: xác minh trả về `PartialView` khi thiếu dữ liệu và `{ status: true }` khi thành công.
   - Kiểm thử Model Binding DisplayName an toàn (Anti-Null DisplayName).
   - Kiểm thử không có lỗi Console, mã phản hồi 200 OK.

---

## 🏛️ TỔNG HỢP NGUYÊN TẮC CỐT LÕI BẮT BUỘC (CORE PILLARS)

### 1. Định dạng File: UTF-8 with BOM & Triple Mirroring
- **100% file `.cshtml`, `.js`, `.cs`, `.sql`, `.xml`, `.md`, `.config`** BẮT BUỘC lưu dưới dạng **UTF-8 with BOM** (`0xEF, 0xBB, 0xBF`). Cấm tuyệt đối ANSI hoặc UTF-8 No BOM gây lỗi hiển thị tiếng Việt.
- **Triple Mirroring:** Mọi chỉnh sửa ở tầng View/JS phân hệ `Modules.Cate` phải được đồng bộ đồng thời sang 3 thư mục:
  1. `Modules.Cate/Areas/Cate/...`
  2. `publish_source/Areas/Cate/...`
  3. `CenIT.Solution.TOC.WebApp/Areas/Cate/...`

### 2. Mô hình Form dùng chung & Tái sử dụng giao diện
- **TUYỆT ĐỐI KHÔNG** tách rời nội dung form Thêm mới và Chỉnh sửa thành 2 file riêng biệt gây trùng lặp và phân mảnh mã nguồn.
- Tạo `_[Entity].cshtml` làm nội dung dùng chung duy nhất (Single Source of Truth).
- `_Add.cshtml` và `_Edit.cshtml` chỉ là các vỏ bọc form kết nối tới Layout `~/Views/Shared/_Form.cshtml`.

### 3. Quy chuẩn Xác thực kép (Dual-Layer Validation)
- Lỗi trường nhập liệu: Bắt buộc render inline qua `@Html.ValidationMessageFor` màu đỏ dưới chân ô nhập liệu.
- Controller: Nếu `!ModelState.IsValid`, trả về `PartialView("_[Entity]", model)` để giao diện tự hiển thị lỗi trực quan.
- Toastr Popup: Chỉ dùng cho thông báo kết quả chung sau khi thực thi nghiệp vụ xong.

### 4. Quy chuẩn Giao diện Danh sách & DataTable Ace Admin v4
- Chiều cao các control tìm kiếm (`_Search.cshtml`) chuẩn hóa đúng **32px** (`form-control-sm` hoặc class tương đương).
- **CẤM** nhúng Select2 vào thanh tìm kiếm ở trang danh sách nếu làm vỡ chiều cao 32px; ưu tiên dropdown chuẩn CenIT TOC.
- Cột thao tác trong DataTable phải sử dụng các icon và button helper chuẩn (`_renderButton`, `data_width = "1024px"`).

### 5. Quy chuẩn Cache-Busting cho Scripts
- Mọi script tag nhúng file `.js` tự viết phải có cache-busting timestamp:
  ```razor
  @section BottomScript {
      <script src="~/Areas/Cate/Views/DigitalSales/DigitalSales.js?v=@DateTime.Now.Ticks"></script>
  }
  ```
