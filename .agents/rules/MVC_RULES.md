# 📘 BỘ QUY TẮC PHÁT TRIỂN ỨNG DỤNG MVC (CENIT TOC CRM)

> **Tài liệu tham chiếu chuẩn:** Phân hệ Cơ hội kinh doanh (`Cate/RM_BusinessOpportunity`).  
> **Áp dụng cho:** Toàn bộ lập trình viên và AI Agent khi xây dựng View, Controller, JavaScript, Data Model trong hệ thống.  
> **Nguyên tắc cốt lõi:** Tuân thủ kiến trúc phân tầng CenIT TOC, kế thừa Layout chuẩn, bắt buộc dùng `@Html.*` helper, không dùng thẻ HTML thuần cho Form Controls, quản lý vòng đời Modal & AJAX thông báo chặt chẽ, 100% UTF-8 with BOM.

---

## 1. QUY TẮC ENCODING & FONT TIẾNG VIỆT (STRICT ENCODING)

- **Định dạng file bắt buộc:** 100% file `.cshtml`, `.js`, `.cs`, `.sql`, `.xml`, `.md` **BẮT BUỘC** phải lưu dưới dạng **UTF-8 with BOM** (`0xEF, 0xBB, 0xBF`).
- **Nghiêm cấm tuyệt đối:**
  - Không để xuất hiện ký tự mojibake hoặc lỗi font tiếng Việt: ``, `?`, `Ã¡`, `Ã´`,...
  - Không lưu file dưới định dạng ANSI, UTF-16, hoặc UTF-8 No BOM (gây lỗi hiển thị tiếng Việt trên IIS/ASP.NET MVC Razor).
- **Cấu hình chuẩn trong `Web.config`:**
  ```xml
  <system.web>
    <globalization uiCulture="en" culture="en-GB" fileEncoding="utf-8" requestEncoding="utf-8" responseEncoding="utf-8" />
  </system.web>
  ```

---

## 2. QUY CHUẨN FORM CONTROLS: @Html.* HELPERS vs THẺ HTML THUẦN

### 2.1. Nguyên tắc vàng
> **TUYỆT ĐỐI HẠN CHẾ THẺ HTML THUẦN** cho các thành phần Form Controls (`<label>`, `<input>`, `<select>`, `<textarea>`).  
> Chỉ sử dụng thẻ HTML thuần khi cả source code dự án không có `@Html.*` helper tương ứng phù hợp.

### 2.2. Bảng đối chiếu quy chuẩn (DOs and DON'Ts)

| Thành phần giao diện | ❌ BỊ CẤM (Thẻ HTML thuần) | ✅ BẮT BUỘC DÙNG (`@Html.*` Helper) | Ghi chú & Cú pháp mẫu CenIT TOC |
| :--- | :--- | :--- | :--- |
| **Nhãn trường (Label)** | `<label>Tên cơ hội (*)</label>` | `@Html.TitleFor(m => m.OpportunityName, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })` | Tự động lấy tên hiển thị và dấu sao đỏ `(*)` bắt buộc từ thuộc tính `[Required]` trong Model |
| **Ô nhập văn bản** | `<input type="text" name="..." />` | `@Html.TextBoxFor(m => m.OpportunityName, new { @class = "form-control", placeholder = "..." })` | Tự động binding dữ liệu 2 chiều, giữ lại giá trị khi validation lỗi |
| **Ô ẩn (Hidden ID)** | `<input type="hidden" name="ID" />` | `@Html.HiddenFor(m => m.BusinessOpportunityID)` | Giữ ID khóa chính, ID khóa ngoại, token |
| **Danh mục chọn (Select)** | `<select name="..."><option>...</select>` | `@Html.DropDownListFor(m => m.ContactPerson_ID, new SelectList(Model.ListContactPerson, "Value", "Text"), "-- Chọn người liên hệ --", new { @class = "form-control" })` | Dropdown chọn 1 giá trị chuẩn CenIT TOC |
| **Văn bản nhiều dòng** | `<textarea name="...">...</textarea>` | `@Html.TextAreaFor(m => m.Description, new { @class = "form-control", rows = 3, placeholder = "..." })` | Nhập mô tả, ghi chú |
| **Đính kèm tệp tin** | `<input type="file" name="..." />` | `@Html.FileFor(m => m.DinhKemFile, ".*", true, new { @class = "form-control", placeholder = "..." })` | Tải file đính kèm, hỗ trợ cấu hình định dạng và multi-file |
| **Hộp kiểm (Checkbox)** | `<input type="checkbox" name="..." />` | `@Html.CheckBoxFor(m => m.IsActive, new { @class = "..." })` | Trạng thái kích hoạt, cờ boolean |
| **Thông báo lỗi xác thực** | `<span class="text-danger">Lỗi...</span>` | `@Html.ValidationMessageFor(m => m.OpportunityName, "", new { @class = "text-danger" })` | Bắt buộc đặt ngay dưới control nhập liệu |
| **Bảo mật Form** | `<input name="__RequestVerificationToken"...>` | `@Html.AntiForgeryToken()` | Bắt buộc đặt ở đầu mỗi Form |

---

## 3. QUY CHUẨN CẤU TRÚC VIEW VÀ KÍCH THƯỚC MODAL

### 3.1. Kích thước Modal (`data_width`)
Khi cấu hình nút mở modal tại View chính (`Index.cshtml`), thuộc tính `data_width` quyết định độ rộng chuẩn của Modal Bootstrap:

1. **`data_width = "1024px"` (Form lớn / Phức tạp)**:
   - Áp dụng cho: Màn hình Thêm mới/Sửa có nhiều thông tin, bố trí 2 cột, có đính kèm file, bảng con, hoặc nhiều tab (Ví dụ: `RM_BusinessOpportunity`, `Project`, `Contract`).
2. **`data_width = "800px"` (Form trung bình)**:
   - Áp dụng cho: Màn hình danh mục vừa phải, 1-2 cột, khoảng 6 - 10 trường nhập liệu.
3. **`data_width = "600px"` (Form nhỏ / Xác nhận)**:
   - Áp dụng cho: Modal xác nhận Xóa (`_Delete.cshtml`), đổi trạng thái nhanh, cập nhật nhanh 1-2 trường.

---

### 3.2. Cấu trúc màn hình chính (`Index.cshtml`)

```cshtml
@using TSFramework.Libs.Processors
@model Core.Cate.Models.RM_BusinessOpportunitySearchModel
@{
    ViewBag.Title = AppProcessor.Messagor.GetMessage("BusinessOpportunity_Title");
}

@section PageTitle {
    @ViewBag.Title
}

@section PageAction {
    @Html.Button(
        true,
        "AddBusinessOpportunity",
        Url.Action("Add", new { area = "Cate" }),
        "<i class='fa fa-plus'></i>",
        AppProcessor.Messagor.GetMessage("Button_Add"),
        new {
            @class = "btn btn-light-blue btn-h-blue btn-a-blue border-0 radius-3 py-2 text-600 text-90",
            data_width = "1024px" // Quy định kích thước modal chuẩn
        }
    )
}

<div class="mt-1">
    @Html.Partial("_Search")
</div>

<div class="mt-3">
    <div class="card dcard">
        <div class="col-sm-12 table-responsive-md">
            <table id="DSBusinessOpportunity" class="d-style w-100 table text-dark-m1 text-95 border-y-1 brc-black-tp11 collapsed dtr-table" width="100%">
                <thead class="sticky-nav text-secondary-m1 text-uppercase text-85 bgc-secondary-l1">
                    <tr>
                        <th>@AppProcessor.Messagor.GetMessage("Column_No")</th>
                        <th>Thông tin chính</th>
                        <th>Thành viên tham gia</th>
                        <th>@AppProcessor.Messagor.GetMessage("Column_Action")</th>
                    </tr>
                </thead>
            </table>
        </div>
    </div>
</div>

@section BottomScript {
    <script src="~/Areas/Cate/Views/RM_BusinessOpportunity/RM_BusinessOpportunity.js"></script>
}
```

---

### 3.3. Cấu trúc Modal Thêm mới / Sửa (`_Add.cshtml`, `_Edit.cshtml`)

```cshtml
@using TSFramework.Libs.Processors
@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Add"), "Cơ hội kinh doanh");
    Layout = "~/Views/Shared/_Form.cshtml"; // BẮT BUỘC kế thừa Layout _Form.cshtml
}

@section FormType {
    success  @* 'success' cho Thêm mới, 'primary' cho Chỉnh sửa *@
}

@section FormIcon {
    <i class="fas fa-plus"></i>
}

@section FormBody {
    @using (Html.BeginForm("Add", "RM_BusinessOpportunity", new { area = "Cate" }, FormMethod.Post, 
        new { encType = "multipart/form-data", id = "AddBusinessOpportunity", @class = "form-horizontal" }))
    {
        <div id="bodyForm">
            @* Bọc Partial View giao diện trong <div id="bodyForm"> *@
            @Html.Partial("_AddCHKD_View")
        </div>
    }

    <script type="text/javascript">
        $('form#AddBusinessOpportunity').ajaxForm({
            success: function (response) {
                BusinessOpportunity_OnProcessSuccess(response, "AddBusinessOpportunity");
            }
        });
    </script>
}
```

---

### 3.4. Cấu trúc Partial View ruột (`_AddCHKD_View.cshtml` / `_CHKD_View.cshtml`)

```cshtml
@using TSFramework.Libs.Processors
@model Core.Cate.Models.RM_BusinessOpportunityModel

@Html.AntiForgeryToken()
<div id="CHKD">
    @Html.HiddenFor(model => model.BusinessOpportunityID)
    @Html.HiddenFor(model => model.CustomerID)

    <div class="form-group row">
        @Html.TitleFor(model => model.OpportunityName, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })
        <div class="col-md-9">
            @Html.TextBoxFor(model => model.OpportunityName, new { @class = "form-control", placeholder = "Nhập tên cơ hội kinh doanh" })
            @Html.ValidationMessageFor(model => model.OpportunityName, "", new { @class = "text-danger" })
        </div>
    </div>

    <div class="form-group row">
        @Html.TitleFor(model => model.ContactPerson_ID, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })
        <div class="col-md-9">
            @Html.DropDownListFor(model => model.ContactPerson_ID, new SelectList(Model.ListContactPerson, "Value", "Text"), "-- Chọn người liên hệ --", new { @class = "form-control" })
            @Html.ValidationMessageFor(model => model.ContactPerson_ID, "", new { @class = "text-danger" })
        </div>
    </div>

    <div class="form-group row">
        @Html.TitleFor(model => model.Description, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })
        <div class="col-md-9">
            @Html.TextAreaFor(model => model.Description, new { @class = "form-control", rows = 3, placeholder = "Nhập nội dung mô tả" })
            @Html.ValidationMessageFor(model => model.Description, "", new { @class = "text-danger" })
        </div>
    </div>

    <div class="form-group row">
        @Html.TitleFor(model => model.FileAttach, new { @class = "col-sm-2 col-form-label text-sm-right pr-0 font-bold" })
        <div class="col-md-9">
            @Html.FileFor(model => model.DinhKemFile, ".*", true, new { @class = "form-control", placeholder = "Đính kèm tệp tin" })
        </div>
    </div>
</div>
```

---

### 3.5. Cấu trúc Modal Xác nhận Xóa (`_Delete.cshtml`)

```cshtml
@using TSFramework.Libs.Processors
@model Core.Cate.Models.RM_BusinessOpportunityModel

@{
    ViewBag.Title = string.Format(AppProcessor.Messagor.GetMessage("Modal_Title_Delete"), "Cơ hội kinh doanh");
    Layout = "~/Views/Shared/_FormConfirm.cshtml"; // BẮT BUỘC kế thừa Layout _FormConfirm.cshtml
}

@section FormType {
    danger
}

@section FormBody {
    @using (Ajax.BeginForm("Delete", "RM_BusinessOpportunity", new { area = "Cate" }, new AjaxOptions
    {
        HttpMethod = "POST",
        OnSuccess = "BusinessOpportunity_OnProcessSuccess(data, 'DeleteBusinessOpportunity')"
    }, new { @class = "form-horizontal", id = "DeleteBusinessOpportunity" }))
    {
        @Html.AntiForgeryToken()
        @Html.HiddenFor(model => model.BusinessOpportunityID)
        <h5>@Html.Raw(ViewBag.ConfirmMessage)</h5>
    }
}
```

---

## 4. QUY CHUẨN JAVASCRIPT: XỬ LÝ PHẢN HỒI THÊM / SỬA / XÓA (`_OnProcessSuccess`)

Hàm JavaScript callback khi hoàn tất xử lý Form là thành phần **tối quan trọng**, quyết định tính trơn tru và chính xác của giao diện:

```javascript
function BusinessOpportunity_OnProcessSuccess(response, formId) {
    // 1. Trường hợp Server trả về JSON (Có response.status): Nghiệp vụ thành công hoặc thất bại có thông báo
    if (response.status != undefined) {
        // Kịch bản A: Người dùng đánh dấu tích "Tiếp tục tạo mới / Không đóng modal" (#chkNotDismissModal)
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message); // Kích hoạt Toast/Alert thông báo từ Server
                if (typeof _tableBusinessOpportunity !== "undefined" && _tableBusinessOpportunity) {
                    _tableBusinessOpportunity.ajax.reload(null, false); // Reload DataTables không nhảy trang
                }
                response.status = undefined;
                // Tải lại nội dung form rỗng để người dùng tiếp tục nhập
                var urlAction = $("#ModalContent #modal_" + formId + " form").attr("action");
                $("#ModalContent #modal_" + formId + " #modal-content").load(urlAction, function () {
                    if (typeof _initElement === "function") _initElement();
                });
            }
        } 
        // Kịch bản B: Người dùng không tích "Tiếp tục tạo mới" -> Đóng modal chuẩn mực
        else {
            $("#ModalContent #modal_" + formId).modal("hide");
            // Đợi modal ẩn xong hoàn toàn mới kích hoạt thông báo và reload dữ liệu (tránh xung đột giao diện)
            $("#ModalContent #modal_" + formId).on("hidden.bs.modal", function () {
                if (response.status != undefined) {
                    eval(response.message); // Kích hoạt Toast thông báo thành công
                    if (typeof _tableBusinessOpportunity !== "undefined" && _tableBusinessOpportunity) {
                        _tableBusinessOpportunity.ajax.reload(null, false);
                    }
                    response.status = undefined;
                }
            });
        }
    } 
    // 2. Trường hợp Server trả về HTML (Validation thất bại do ModelState.IsValid == false):
    // Thay thế toàn bộ HTML ruột form vào thẻ #bodyForm để hiển thị ngay các lỗi màu đỏ (@Html.ValidationMessageFor)
    // mà KHÔNG làm mất dữ liệu người dùng đã nhập, KHÔNG đóng modal!
    else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
```

---

## 5. QUY CHUẨN CONTROLLER (APPCONTROLLER & ACTIONS)

### 5.1. Kế thừa & Attributes
- Controller **BẮT BUỘC** kế thừa từ `AppController` (`Modules.Cate` / `Core.Sys.BaseApp`).
- Các Action tương tác Ajax / Modal **BẮT BUỘC** trang trí các Attribute:
  - `[AjaxOnly]`: Chỉ cho phép gọi qua Ajax.
  - `[ActionType(Type = EnumActionType.Create / Edit / Delete / View)]`: Kiểm soát phân quyền chức năng.
  - `[HttpPost]` / `[HttpGet]`.
  - `[ValidateAntiForgeryToken]` hoặc `[ValidateInput(false)]` (khi có CKEditor/HTML input).

### 5.2. Xử lý Thêm mới (Add POST)
```csharp
[AjaxOnly]
[HttpPost]
[ActionType(Type = EnumActionType.Create)]
public ActionResult Add(RM_BusinessOpportunityModel model)
{
    // BƯỚC 1: Kiểm tra tính hợp lệ dữ liệu (Model Validation)
    if (!ModelState.IsValid)
    {
        // Tải lại các danh mục DropDownList / SelectList để nạp lại vào View
        model.ListContactPerson = GetContactPersons(model.CustomerID);
        // QUY TẮC BẮT BUỘC: Trả về PartialView ruột để Javascript thế vào #bodyForm
        return PartialView("_AddCHKD_View", model);
    }

    // BƯỚC 2: Thực thi lưu dữ liệu qua Biz / Cache
    var result = _businessOpportunityCache.Save(model, User.UserName);

    // BƯỚC 3: Tạo thông báo chuẩn qua CreateMessage
    string response;
    if (result == 0)
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.Add, EnumMsgIcon.Error);
    else if (result == -9)
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
    else
        response = CreateMessage($"Cơ hội kinh doanh [{model.OpportunityName}]", EnumProcessType.Add, EnumMsgIcon.Success);

    // QUY TẮC BẮT BUỘC: Trả về Json với status = true và message chứa đoạn script CreateMessage
    return Json(new { status = true, message = response }, JsonRequestBehavior.AllowGet);
}
```

### 5.3. Xử lý Xóa (Delete POST)
```csharp
[AjaxOnly]
[HttpPost]
[ActionType(Type = EnumActionType.Delete)]
public ActionResult Delete(RM_BusinessOpportunityModel model)
{
    var deleted = _businessOpportunityCache.Delete(model, User.UserName);
    var response = CreateMessage($"Cơ hội kinh doanh", EnumProcessType.Delete, deleted > 0 ? EnumMsgIcon.Success : EnumMsgIcon.Error);
    return Json(new { status = true, message = response });
}
```

---

## 6. BẢNG CHECKLIST KIỂM THỬ TRƯỚC KHI BÀN GIAO (QA CHECKLIST)

Mọi màn hình hoặc tính năng mới trước khi bàn giao phải vượt qua 100% các tiêu chí:

- [ ] **Encoding:** Toàn bộ file liên quan (`.cshtml`, `.js`, `.cs`, `.sql`) là **UTF-8 with BOM**.
- [ ] **Form Controls:** 100% sử dụng `@Html.TitleFor`, `@Html.TextBoxFor`, `@Html.DropDownListFor`, `@Html.TextAreaFor`, `@Html.FileFor`. Không còn thẻ HTML thuần cho controls.
- [ ] **Validation:** Đầy đủ `@Html.ValidationMessageFor` màu đỏ dưới các ô nhập liệu bắt buộc.
- [ ] **Modal Width:** Thuộc tính `data_width` đặt đúng chuẩn (`1024px`, `800px`, hoặc `600px`).
- [ ] **Form Layout:** Kế thừa đúng `Layout = "~/Views/Shared/_Form.cshtml"` hoặc `_FormConfirm.cshtml`.
- [ ] **Thẻ Body Form:** Form có chứa `<div id="bodyForm">` bọc PartialView ruột.
- [ ] **JS Callback:** Đã cài đặt đầy đủ hàm `[Entity]_OnProcessSuccess` xử lý cả 3 kịch bản (Thêm tiếp, Đóng modal, Trả về lỗi PartialView).
- [ ] **Controller Valid:** Trả về `PartialView("_[Entity]_View", model)` khi `!ModelState.IsValid`.
- [ ] **Controller Response:** Sử dụng hàm `CreateMessage(...)` với các `EnumProcessType` và `EnumMsgIcon` chuẩn.
- [ ] **Automated Tests:** Bộ unit test / regression test đạt 100% PASS (3 tầng: Happy Path, Edge Cases, Error Handling theo `TESTING.md`).
