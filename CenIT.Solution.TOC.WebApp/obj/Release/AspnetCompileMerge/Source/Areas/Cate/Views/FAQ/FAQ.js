var _FAQActionURLs = {
    FAQ_GetData: "/Cate/FAQ/Get"
};
var _tableFAQ;
$(document).ready(function () {
    initTableFAQ();
});

function initTableFAQ() {
    _tableFAQ = $("#DSFAQ").DataTable({
        "Responsive": true,
        "FAQ": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _FAQActionURLs.FAQ_GetData,
            "type": "POST",
            "dataType": "JSON",
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
                "data": "Question",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return `<div class="text-left">${data.replaceAll('\r\n', '</br>')}</div>`;
                }
            },
            {
                "data": "Answer",
                "defaultContent": "", "visible": false,
                "render": function (data, type, row, meta) {
                    return `<div class="text-left">${data.replaceAll('\r\n', '</br>')}</div>`;
                }
            },
            {
                "data": "IsActive",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    if (type == "display") {
                        return data ? `<span class="badge badge-primary">Hiển thị</span>` : `<span class="badge badge-secondary">Ẩn</span>`
                    }
                    return data;
                }
            },
            {
                "data": "SortOrder",
                "defaultContent": "",
            },
            {
                "data": "Id",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';
                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn btn-lighter-primary mr-1 dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';
                        if (row.IsActive) {
                            html += _renderButton(true,
                                "UpdateStatus",
                                "btn btn-outline-warning btn-a-outline-warning mr-1 dropdown-item",
                                "/Cate/FAQ/HideQuestion/" + data,
                                '<i class="fas fa-eye-slash text-120 mr-1"></i> Ẩn câu hỏi',
                                "Ẩn câu hỏi");
                        } else {
                            html += _renderButton(true,
                                "UpdateStatus",
                                "btn btn-outline-success btn-a-outline-success mr-1 dropdown-item",
                                "/Cate/FAQ/ShowQuestion/" + data,
                                '<i class="fas fa-eye text-120 mr-1"></i> Hiển thị câu hỏi',
                                "Hiển thị câu hỏi");
                        }
                        html += _renderButton(true,
                            "EditFAQ",
                            "btn btn-outline-primary btn-a-outline-primary mr-1 dropdown-item",
                            "/Cate/FAQ/Edit/" + data,
                            '<i class="far fa-edit text-120 mr-1"></i> Cập nhật',
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeleteFAQ",
                            "btn btn-outline-danger btn-a-outline-danger mr-1 dropdown-item",
                            "/Cate/FAQ/Delete/" + data,
                            '<i class="far fa-trash-alt text-120 mr-1"></i> Xoá',
                            "Xoá");
                        html += '</div></div>';
                    }
                    html += "</span>";
                    return html;
                }
            }
        ]
    });
}


function FAQ_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableFAQ.ajax.reload(null, false);
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
                        _tableFAQ.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
