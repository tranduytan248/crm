# 🪟 QUY CHUẨN MODAL, AJAX & THÔNG BÁO (MVC MODAL & AJAX RULES)

> **Tài liệu tham chiếu chuẩn:** Framework `_Form.cshtml`, `Customer.js`, `RM_BusinessOpportunity.js`.  
> **Áp dụng cho:** Toàn bộ popup modal, request AJAX và hệ thống thông báo Toastr trong CenIT TOC CRM.

---

## 1. QUY CHUẨN KÍCH THƯỚC MODAL (DATA_WIDTH STANDARDS)

### 1.1. Ba kích thước chuẩn tuyệt đối
Toàn bộ Modal form trong hệ thống CenIT TOC CRM phải sử dụng 1 trong 3 kích thước chuẩn:
- **`data_width = "1024px"` (Modal Lớn - Chuẩn mặc định cho Form nghiệp vụ):**
  - Áp dụng cho: Form Khách hàng, Cơ hội kinh doanh, Hợp đồng, Dự án, Chi tiết 360 độ.
  - Bố cục: Thường chia 2 cột (`col-md-6`), chứa nhiều trường thông tin, file đính kèm, bảng con và CKEditor.
- **`data_width = "800px"` (Modal Trung bình):**
  - Áp dụng cho: Form cấu hình, chuyển trạng thái, phân công nhân sự, giao việc nhanh.
- **`data_width = "600px"` (Modal Nhỏ):**
  - Áp dụng cho: Xác nhận xóa, kích hoạt/hủy kích hoạt, đổi mật khẩu, thao tác đơn giản (1 - 3 trường).

### 1.2. Khai báo nút mở Modal bằng `@Html.Button` Helper
```razor
@Html.Button(true, "AddCustomer", Url.Action("Add", new { area = "Cate" }),
             "<i class='fa fa-plus'></i>",
             AppProcessor.Messagor.GetMessage("Button_Add"),
             new { @class = "btn btn-light-blue border-0 radius-3 py-2 mr-2 text-600 text-90 pull-right", data_width = "1024px" })
```

---

## 2. QUY CHUẨN LAYOUT `_Form.cshtml` & `ajaxForm`

### 2.1. Cấu trúc Layout `_Form.cshtml`
Mọi Partial View Modal form (`_Add.cshtml`, `_Edit.cshtml`) đều phải kế thừa layout `_Form.cshtml`:
```razor
@{
    ViewBag.Title = "...";
    Layout = "~/Views/Shared/_Form.cshtml";
}
@section FormType { primary }      <!-- Màu sắc header: primary, info, success, warning, danger -->
@section FormIcon { <i class="..."></i> }
@section FormBody {
    <!-- Form HTML và nhúng Content PartialView -->
}
```

### 2.2. Xử lý Submit Form bằng `ajaxForm`
Đối với các form có upload file hoặc nội dung CKEditor, bắt buộc dùng `ajaxForm` (jquery.form plugin tích hợp sẵn trong Ace Admin):
```razor
@using (Html.BeginForm("Add", "Customer", new { area = "Cate" }, FormMethod.Post, new { encType = "multipart/form-data", id = "AddCustomer", @class = "form-horizontal" }))
{
    <div id="bodyForm">
        @Html.Partial("_Customer", Model)
    </div>
}

<script type="text/javascript">
    $('form#AddCustomer').ajaxForm({
        beforeSubmit: function() {
            // Đồng bộ CKEditor nếu có
            if (typeof CKEDITOR !== 'undefined') {
                for (var inst in CKEDITOR.instances) {
                    try { CKEDITOR.instances[inst].updateElement(); } catch(e) {}
                }
            }
            return true;
        },
        success: function (response) {
            Customer_OnProcessSuccess(response, "AddCustomer");
        }
    });
</script>
```

---

## 3. QUY CHUẨN CALLBACK `_OnProcessSuccess(response, formId)`

Mỗi module JavaScript bắt buộc phải có hàm callback nhận kết quả submit từ `ajaxForm`:

```javascript
function ModuleName_OnProcessSuccess(response, formId) {
    var $modal = $("#ModalContent #modal_" + formId);
    if ($modal.length === 0) {
        $modal = $("#modal_" + formId);
    }

    // TRƯỜNG HỢP 1: Response là JSON (Thành công hoặc Lỗi hệ thống)
    if (response.status !== undefined) {
        // 1. Thực thi script Toastr từ server trả về (hoặc hàm notification)
        if (typeof response.message === "string") {
            eval(response.message);
        }

        // 2. Nếu thành công: đóng modal và reload dữ liệu
        if (response.status === true) {
            var isKeepOpen = $modal.find("#chkNotDismissModal").is(":checked");
            if (isKeepOpen) {
                // Nếu người dùng chọn "Tiếp tục thêm mới"
                var urlAction = $modal.find("form").attr("action");
                $modal.find("#bodyForm").load(urlAction);
            } else {
                $modal.modal("hide");
            }
            // Reload lại bảng DataTable
            if (typeof refreshDataTable === "function") {
                refreshDataTable();
            }
        }
    } 
    // TRƯỜNG HỢP 2: Response là HTML PartialView (Validation thất bại)
    else {
        // Nạp lại HTML vào #bodyForm -> Các thẻ @Html.ValidationMessageFor sẽ tự động hiện chữ đỏ
        $modal.find("#bodyForm").html(response);
        
        // Tái khởi tạo các plugin giao diện sau khi HTML bị thay thế
        _initModalPlugins($modal);
    }
}
```

---

## 4. QUY CHUẨN THÔNG BÁO TOASTR & CHỐNG KẸT MODAL BACKDROP

### 4.1. Thông báo Toastr khép kín qua sự kiện `hidden.bs.modal`
Khi submit form thành công và cần đóng modal để hiển thị Toastr:
- **CẤM:** Bắn Toastr ngay khi modal chưa kịp đóng (gây che khuất hoặc mất hiệu ứng backdrop mượt mà).
- **CHUẨN:** Lắng nghe sự kiện `hidden.bs.modal` để hiển thị Toastr sau khi modal đã đóng hoàn toàn:
  ```javascript
  $modal.one("hidden.bs.modal", function () {
      executeResponseMessage(res.message, "Cập nhật thành công!", true);
      refreshDataTable();
  }).modal("hide");
  ```

### 4.2. Chống kẹt Backdrop khi mở Modal lồng nhau (Nested Modals)
Khi mở modal con (như Modal Tra cứu Khách hàng) từ trong modal cha (Thêm mới Cơ hội):
```javascript
$('#modalChildLookup').on('hidden.bs.modal', function () {
    if ($('#modalParent.show').length > 0) {
        $('body').addClass('modal-open');
    }
});
```
