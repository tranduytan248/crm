var _tableDigitalSalesProcess;
$(document).ready(function () {
    _tableDigitalSalesProcess = $("#DSDigitalSalesProcess").DataTable({
        responsive: true, lengthChange: true, processing: true, serverSide: true, order: [[1, "asc"], [6, "asc"]], language: { emptyTable: "Chưa có quy trình nào. Hãy chọn Thêm để tạo quy trình đầu tiên.", processing: "Đang tải dữ liệu..." },
        ajax: { url: "/Cate/DigitalSalesProcess/Get", type: "POST", dataType: "JSON", data: function (d) { d.businessType = $("#digitalProcessBusinessType").val(); d.statusId = $("#digitalProcessStatus").val(); } },
        columns: [
            { data: null, orderable: false, render: function (d,t,r,m) { return m.settings._iDisplayStart + m.row + 1; } },
            { data: "BusinessTypeName" }, { data: "StatusName" }, { data: "ProcessCode" }, { data: "ProcessName" },
            { data: "Description", defaultContent: "", orderable: false, render: digitalText }, { data: "SortOrder" },
            { data: "IsActive", orderable: false, render: function (v) { return v ? '<span class="badge badge-success"><i class="fa fa-check mr-1"></i>Có</span>' : '<span class="text-secondary">Không</span>'; } },
            { data: "ProcessID", orderable: false, render: function (id, type, row) {
                return _renderButton(true, "DigitalSalesProgress", "btn btn-lighter-primary mr-2 digital-sales-action", "/Cate/DigitalSalesProcess/Progress/" + id, '<i class="fa fa-list-ol text-primary text-120"></i>', "Quản lý " + row.ProgressCount + " tiến trình", 1100) +
                    _renderButton(true, "EditDigitalSalesProcess", "btn btn-lighter-warning mr-2 digital-sales-action", "/Cate/DigitalSalesProcess/Edit/" + id, '<i class="far fa-edit text-warning text-120"></i>', "Cập nhật", 760) +
                    _renderButton(true, "DeleteDigitalSalesProcess", "btn btn-lighter-danger digital-sales-action", "/Cate/DigitalSalesProcess/Delete/" + id, '<i class="far fa-trash-alt text-danger text-120"></i>', "Xóa", 600);
            }}
        ]
    });
    $("#digitalProcessBusinessType").on("change", function () {
        var type = $(this).val(); var $status = $("#digitalProcessStatus"); $status.val("");
        $status.find("option[data-business-type]").each(function () { $(this).toggle(!type || $(this).data("business-type").toString() === type); });
    });
    $("#btnFilterDigitalProcess").on("click", function () { _tableDigitalSalesProcess.ajax.reload(); });
    $("#digitalProcessLoadError button").on("click", function () { _tableDigitalSalesProcess.ajax.reload(); });
    $("#DSDigitalSalesProcess").on("xhr.dt", function () { $("#digitalProcessLoadError").addClass("d-none"); }).on("error.dt", function () { var offline=!navigator.onLine; $("#digitalProcessLoadError span").text(offline ? "Đang mất kết nối mạng. Kiểm tra kết nối rồi thử lại." : "Không thể tải danh sách quy trình. Vui lòng thử lại."); $("#digitalProcessLoadError").removeClass("d-none"); });
});
function digitalText(value, type) { if (type !== "display") return value || ""; return '<div class="digital-sales-description">' + $("<div/>").text(value || "—").html() + '</div>'; }
function DigitalSalesProcess_OnProcessSuccess(response, formId) {
    if (response.status !== undefined) { $("#ModalContent #modal_" + formId).modal("hide").on("hidden.bs.modal", function () { eval(response.message); _tableDigitalSalesProcess.ajax.reload(null, false); }); }
    else { $("#ModalContent #modal_" + formId + " #bodyForm").html(response); }
}
function DigitalSalesProgress_Init() {
    var $root = $("#digitalProgressManager"); if (!$root.length) return;
    var processId = $root.data("process-id");
    function resetForm() { var form = $("#digitalProgressForm")[0]; form.reset(); $("#ProgressID").val(0); $("#ProgressSortOrder").val(1); $("#ProgressIsActive").prop("checked", true); $("#digitalProgressErrors").empty(); $("#btnSaveDigitalProgress").html('<i class="fa fa-save mr-1"></i>Lưu'); }
    function loadItems() {
        $.getJSON("/Cate/DigitalSalesProcess/ProgressList/" + processId, function (response) {
            var items = response.data || [], $body = $("#digitalProgressTable tbody").empty();
            $("#digitalProgressEmpty").toggleClass("d-none", items.length > 0); $("#digitalProgressTable").toggleClass("d-none", items.length === 0);
            $.each(items, function (index, item) {
                var $row = $("<tr/>"); $("<td/>").text(index + 1).appendTo($row); $("<td/>").text(item.ProgressCode).appendTo($row); $("<td/>").text(item.ProgressName).appendTo($row); $("<td/>").text(item.Description || "—").appendTo($row); $("<td/>").text(item.SortOrder).appendTo($row);
                $("<td/>").append(item.IsActive ? $('<span class="badge badge-success"><i class="fa fa-check mr-1"></i>Có</span>') : $('<span class="text-secondary">Không</span>')).appendTo($row);
                var $actions = $("<td/>"); $('<button type="button" class="btn btn-lighter-warning mr-2 digital-sales-action" title="Sửa"><i class="far fa-edit text-warning"></i></button>').on("click", function () { editItem(item.ProgressID); }).appendTo($actions);
                $('<button type="button" class="btn btn-lighter-danger digital-sales-action" title="Xóa"><i class="far fa-trash-alt text-danger"></i></button>').on("click", function () { deleteItem(item.ProgressID, item.ProgressName); }).appendTo($actions); $actions.appendTo($row); $body.append($row);
            });
        });
    }
    function editItem(id) { $.getJSON("/Cate/DigitalSalesProcess/ProgressItem/" + id, function (r) { if (!r.status) return; var x=r.data; $("#ProgressID").val(x.ProgressID); $("#ProgressCode").val(x.ProgressCode); $("#ProgressName").val(x.ProgressName); $("#ProgressDescription").val(x.Description); $("#ProgressSortOrder").val(x.SortOrder); $("#ProgressIsActive").prop("checked", x.IsActive); $("#btnSaveDigitalProgress").html('<i class="fa fa-save mr-1"></i>Cập nhật'); $("#ProgressCode").focus(); }); }
    function deleteItem(id, name) { if (!window.confirm('Xóa tiến trình mẫu "' + name + '"?')) return; var token=$("#digitalProgressForm input[name='__RequestVerificationToken']").val(); $.post("/Cate/DigitalSalesProcess/DeleteProgress", { id:id, __RequestVerificationToken:token }, function(r) { if(r.message) eval(r.message); if(r.status){ resetForm(); loadItems(); if(_tableDigitalSalesProcess) _tableDigitalSalesProcess.ajax.reload(null,false); } }); }
    $("#digitalProgressForm").off("submit.digitalProgress").on("submit.digitalProgress", function(e) { e.preventDefault(); var $button=$("#btnSaveDigitalProgress").prop("disabled",true).html('<i class="fa fa-spinner fa-spin mr-1"></i>Đang lưu'); $("#digitalProgressErrors").empty(); $.post("/Cate/DigitalSalesProcess/SaveProgress", $(this).serialize(), function(r) { if(r.message) eval(r.message); if(r.status){ resetForm(); loadItems(); if(_tableDigitalSalesProcess) _tableDigitalSalesProcess.ajax.reload(null,false); } else { $("#digitalProgressErrors").text((r.errors||["Không thể lưu tiến trình."]).join(" ")); } }).always(function(){ $button.prop("disabled",false); if($("#ProgressID").val()==="0") $button.html('<i class="fa fa-save mr-1"></i>Lưu'); }); });
    loadItems();
}
