var _BoPhanActionURLs = {
    BoPhan_GetData: "/Cate/BoPhan/Get"
};
var _tableBoPhan;
$(document).ready(function () {
    initTableBoPhan();
});

function initTableBoPhan() {
    _tableBoPhan = $("#DSBoPhan").DataTable({
        "Responsive": true,
        "BoPhan": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "pageLength": 100,
        "ajax":
        {
            "url": _BoPhanActionURLs.BoPhan_GetData,
            "type": "POST",
            "BoPhan": "JSON",
            "data": {
                "BoPhanChaIDs": function () {
                    return $("#SearchBoPhan select#ListBoPhanChaIDs").val() != null &&
                        $("#SearchBoPhan select#ListBoPhanChaIDs").val().length > 0
                        ? $("#SearchBoPhan select#ListBoPhanChaIDs").val()
                        : "";
                }
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
                "data": "MaBoPhan",
                "defaultContent": ""
            },
            {
                "data": "TitleView",
                "defaultContent": "",
                "className": "text-left",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "TenLinhVuc",
                "defaultContent": "",
            },
            {
                "data": "BoPhan_ID",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "EditBoPhan",
                            "btn btn-lighter-primary mr-1",
                            "/Cate/BoPhan/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i>',
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteBoPhan",
                            "btn btn-lighter-danger mr-1",
                            "/Cate/BoPhan/Delete/" + data,
                            '<i class="far fa-trash-alt text-danger text-120"></i>',
                            "Xoá");
                    }
                    html += "</span>";

                    return html;
                }
            }
        ]
    });
}


function BoPhan_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableBoPhan.ajax.reload(null, false);
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
                        _tableBoPhan.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
