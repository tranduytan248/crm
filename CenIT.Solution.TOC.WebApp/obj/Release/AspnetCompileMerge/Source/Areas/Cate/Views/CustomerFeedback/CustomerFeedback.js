var _CustomerFeedbackActionURLs = {
    CustomerFeedback_GetData: "/Cate/CustomerFeedback/Get",
    CustomerFeedback_Export: "/Cate/CustomerFeedback/Export"
};
var _tableCustomerFeedback;
$(document).ready(function () {
    $('.t_tip').tooltip();
    initTableCustomerFeedback();
});
function toIso(dateStr) {
    if (!dateStr) return "";

    // format dd/MM/yyyy → yyyy-MM-dd
    const parts = dateStr.split('/');
    if (parts.length === 3)
        return `${parts[2]}-${parts[1]}-${parts[0]}`;

    return dateStr; // nếu đã là yyyy-MM-dd thì giữ nguyên
}

function ExportDanhSachKhachHang() {
    var dateFrom = toIso($("#SearchCustomerFeedback #TuNgay").val());
    var dateTo = toIso($("#SearchCustomerFeedback #DenNgay").val());


    const params = new URLSearchParams({
        TuNgay: dateFrom || "",
        DenNgay: dateTo || "",
        TuKhoa: $("#SearchCustomerFeedback #TuKhoa").val(),
        Status: $("#SearchCustomerFeedback #Status").val(),
        CategoryFeedbackID: $("#SearchCustomerFeedback #CategoryFeedbackID")
    });

    const url = `${_CustomerFeedbackActionURLs.CustomerFeedback_Export}?${params.toString()}`;
    window.open(url, "_blank");
}

function initTableCustomerFeedback() {
    _tableCustomerFeedback = $("#DSCustomerFeedback").DataTable({
        "Responsive": true,
        "CustomerFeedback": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "order": [[1, "desc"]],
        "ajax":
        {
            "url": _CustomerFeedbackActionURLs.CustomerFeedback_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TuKhoa": function () { return $("#SearchCustomerFeedback #TuKhoa").val(); },
                "TuNgay": function () { return $("#SearchCustomerFeedback #TuNgay").val(); },
                "DenNgay": function () { return $("#SearchCustomerFeedback #DenNgay").val(); },
                "Status": function () { return $("#SearchCustomerFeedback #Status").val(); },
                "CategoryFeedbackID": function () { return $("#SearchCustomerFeedback #CategoryFeedbackID").val(); },
            }
        },
        "columns": [
            {
                "data": "",
                "defaultContent": "1",
                "render": function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1;
                }
            },
            {
                "data": "SubmitDate",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("DD/MM/YYYY HH:mm:ss");
                }
            },
            {
                "data": "FullName",
                "defaultContent": "",
            },
            {
                "data": "Title",
                "defaultContent": "",
                "className": "text-left",
                "render": function (data, type, row, meta) {
                    var codeFb = row.CodeFeedback ? `<span class="mb-1 badge badge-lg bgc-yellow-l1 border-1 brc-black-tp9 text-brown-d3 px-2 t_tip" title data-original-title="Mã góp ý" data-rel="tooltip">${row.CodeFeedback}</span>` : ``;
                    return `<div>${codeFb}
                        <span class="badge bgc-blue-l3 brc-blue-l3 text-blue-d3 badge-lg arrowed arrowed-in-right mb-1 t_tip" title data-original-title="Chủ đề góp ý" data-rel="tooltip">${row.NameCategory}</span>
                    </div>
                    <div>${data}</div>`;
                }
            },
            {
                "data": "StatusFeedback",
                "defaultContent": "",
                "className": "text-right",
                "render": function (data, type, row, meta) {
                    return data == 1 ? `<span class="m-1 badge bgc-success-l5 border-1 border-r-4 radius-0 brc-success btn-text-success px-25 text-90">Đã phản hồi</span>`
                        : `<span class="m-1 badge bgc-danger-l5 border-1 border-r-3 radius-0 brc-danger btn-text-danger px-25 text-90">Mới</span>`;
                }
            },
            {
                "data": "FeedbackId",
                "style": "width:100px;",
                "className": "text-left",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';
                    if (type === "display") {

                        html += _renderButton(true,
                            "Detail",
                            "btn btn-lighter-warning mr-1",
                            "/Cate/CustomerFeedback/Detail/" + data,
                            '<i class="fas fa-eye text-warning text-120"></i>',
                            "Xem");

                        if (row.StatusFeedback == 0) {
                            html += _renderButton(true,
                                "Process",
                                "btn btn-lighter-primary mr-1",
                                "/Cate/CustomerFeedback/Process/" + data,
                                '<i class="far fa-paper-plane text-primary text-120"></i>',
                                "Phản hồi ý kiến");
                        }
                    }
                    html += "</span>";
                    return html;
                }
            }
        ]
    });
}


function CustomerFeedback_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableCustomerFeedback.ajax.reload(null, false);
                response.status = undefined;
                //$("#ModalContent #modal_" + formId + " form")[0].reset();
                var urlAction = $("#ModalContent #modal_" + formId + " form").attr("action");
                $("#ModalContent #modal_" + formId + " #modal-content").load(urlAction, function (data, textStatus, xhr) {
                    _initElement();
                });
            }
        } else {
            $("#ModalContent #modal_" + formId).modal("hide");
            $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
                function () {
                    if (response.status != undefined) {
                        eval(response.message);
                        _tableCustomerFeedback.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function _search() {
    _tableCustomerFeedback.ajax.reload(null, false);
}

