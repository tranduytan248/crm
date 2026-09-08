var _NotificationActionURLs = {
    Notification_GetData: "/Cate/NotificationByEmail/GetContractRegisterReceiveEmail",
};
var _tableNotification;
//$(document).ready(function () {
//    initTableNotification();
//});

function initTableNotification() {
    _tableNotification = $("#DSContractRegisterReceiveEmail").DataTable({
        "Responsive": true,
        "Notification": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _NotificationActionURLs.Notification_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "Period": function () { return $("#Searching #Period").val(); },
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
                "data": "Email",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "AccountUser",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "ContractCode",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            }        ,
            {
                "data": "CustomerName",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "Amount",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    if (!data) return "";
                    return data.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
                }
            },
            {
                "data": "ContractCode",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "SendNotificationEmailByContractCode",
                            "btn btn-lighter-warning mr-1 dropdown-item",
                            "/Cate/NotificationByEmail/SendNotificationEmailByContractCode/?accountId=" + row["Account_ID"] + "&contractCode=" + data + "&period=" + moment(row["Period"]).format("YYYY/MM/DD"),
                            '<i class="fas fa-bell text-warning text-120"></i> Gửi thông báo',
                            "Gửi thông báo");
                    }
                        html += "</span>";            
                    return html;
                }
            }
        ]
    });
}


function Notification_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableNotification.ajax.reload(null, false);
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
                        GetStatistic();
                        //_tableNotification.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
