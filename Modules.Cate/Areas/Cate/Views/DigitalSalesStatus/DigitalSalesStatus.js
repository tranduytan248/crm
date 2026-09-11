var _tableDigitalSalesStatus;
$(document).ready(function () {
    _tableDigitalSalesStatus = $("#DSDigitalSalesStatus").DataTable({
        responsive: true, lengthChange: true, processing: true, serverSide: true, language: { emptyTable: "Chưa có trạng thái nào. Hãy chọn Thêm để tạo trạng thái đầu tiên.", processing: "Đang tải dữ liệu..." },
        order: [[1, "asc"], [5, "asc"]],
        ajax: { url: "/Cate/DigitalSalesStatus/Get", type: "POST", dataType: "JSON", data: function (d) { d.businessType = $("#digitalStatusBusinessType").val(); } },
        columns: [
            { data: null, orderable: false, render: function (d, t, r, m) { return m.settings._iDisplayStart + m.row + 1; } },
            { data: "BusinessTypeName", defaultContent: "" }, { data: "StatusCode", defaultContent: "" },
            { data: "StatusName", defaultContent: "" }, { data: "Description", defaultContent: "", orderable: false, render: renderDescription },
            { data: "SortOrder", defaultContent: 0 }, { data: "IsDefault", orderable: false, render: renderCheck },
            { data: "IsActive", orderable: false, render: renderCheck },
            { data: "StatusID", orderable: false, render: function (id) {
                return _renderButton(true, "EditDigitalSalesStatus", "btn btn-lighter-warning mr-2 digital-sales-action", "/Cate/DigitalSalesStatus/Edit/" + id, '<i class="far fa-edit text-warning text-120"></i>', "Cập nhật", 760) +
                    _renderButton(true, "DeleteDigitalSalesStatus", "btn btn-lighter-danger digital-sales-action", "/Cate/DigitalSalesStatus/Delete/" + id, '<i class="far fa-trash-alt text-danger text-120"></i>', "Xóa", 600);
            }}
        ]
    });
    $("#btnFilterDigitalStatus").on("click", function () { _tableDigitalSalesStatus.ajax.reload(); });
    $("#digitalStatusLoadError button").on("click", function () { _tableDigitalSalesStatus.ajax.reload(); });
    $("#DSDigitalSalesStatus").on("xhr.dt", function () { $("#digitalStatusLoadError").addClass("d-none"); }).on("error.dt", function () { var offline=!navigator.onLine; $("#digitalStatusLoadError span").text(offline ? "Đang mất kết nối mạng. Kiểm tra kết nối rồi thử lại." : "Không thể tải danh sách trạng thái. Vui lòng thử lại."); $("#digitalStatusLoadError").removeClass("d-none"); });
});
function renderCheck(value) { return value ? '<span class="badge badge-success"><i class="fa fa-check mr-1"></i>Có</span>' : '<span class="text-secondary">Không</span>'; }
function renderDescription(value, type) { if (type !== "display") return value || ""; return '<div class="digital-sales-description">' + $("<div/>").text(value || "—").html() + '</div>'; }
function DigitalSalesStatus_OnProcessSuccess(response, formId) {
    if (response.status !== undefined) {
        $("#ModalContent #modal_" + formId).modal("hide").on("hidden.bs.modal", function () { eval(response.message); _tableDigitalSalesStatus.ajax.reload(null, false); });
    } else { $("#ModalContent #modal_" + formId + " #bodyForm").html(response); }
}
