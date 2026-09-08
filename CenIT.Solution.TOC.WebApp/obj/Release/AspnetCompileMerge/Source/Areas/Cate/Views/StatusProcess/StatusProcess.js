var _StatusProcessActionURLs = {
    StatusProcess_GetData: "/Cate/StatusProcess/Get"
};
var _tableStatusProcess;
$(document).ready(function () {
    initTableStatusProcess();
});

function initTableStatusProcess() {
    _tableStatusProcess = $("#DSStatusProcess").DataTable({
        "Responsive": true,
        "StatusProcess": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _StatusProcessActionURLs.StatusProcess_GetData,
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
                "data": "StatusProcessName",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "StatusProcessId",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "EditStatusProcess",
                            "btn btn-lighter-primary mr-1",
                            "/Cate/StatusProcess/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i>',
                            "Cập nhật");

                        //html += _renderButton(true,
                        //    "DeleteStatusProcess",
                        //    "btn btn-lighter-danger mr-1",
                        //    "/Cate/StatusProcess/Delete/" + data,
                        //    '<i class="far fa-trash-alt text-danger text-120"></i>',
                        //    "Xoá");
                    }
                    html += "</span>";

                    return html;
                }
            }
        ]
    });
}


function StatusProcess_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableStatusProcess.ajax.reload(null, false);
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
                        _tableStatusProcess.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
