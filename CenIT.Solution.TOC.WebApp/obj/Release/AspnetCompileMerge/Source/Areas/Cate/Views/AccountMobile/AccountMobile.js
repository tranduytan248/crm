var _AccountMobileActionURLs = {
    AccountMobile_GetData: "/Cate/AccountMobile/Get",
    AccountMobile_Export: "/Cate/AccountMobile/Export"
};
var _tableAccountMobile;
$(document).ready(function () {
    initTableAccountMobile();
});
function ExportDanhSachKhachHang() {
   
    const params = new URLSearchParams({
        TuKhoa: $("#SearchAccountMobile #TuKhoa").val(),
        Type: $("#SearchAccountMobile #Type").val()
    });

    const url = `${_AccountMobileActionURLs.AccountMobile_Export}?${params.toString()}`;
    window.open(url, "_blank");
}
function initTableAccountMobile() {
    _tableAccountMobile = $("#DSAccountMobile").DataTable({
        "Responsive": true,
        "AccountMobile": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _AccountMobileActionURLs.AccountMobile_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TuKhoa": function () { return $("#SearchAccountMobile #TuKhoa").val(); },
                "Type": function () { return $("#SearchAccountMobile #Type").val(); },
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
                "data": "CustomerName",
                "defaultContent": ""
            }, {
                "data": "AccountUser",
                "defaultContent": "",
                //"className":"text-left",
                //"render": function (data, type, row, meta) {
                //    var color = row.TypeAccount == 'phone' ? 'success' : 'warning';
                //    var loaiTK = row.TypeAccount == 'phone' ? 'Số ĐT' : 'Mã HĐ';
                //    return `<span class="badge badge-${color}">${loaiTK}</span><div>${data}</div>`;
                //}
            }, 
            {
                "data": "IsActive",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    var classColor = data ? 'green' : 'red';
                    return `<span class="badge bgc-${classColor} brc-${classColor} text-white badge-lg arrowed arrowed-in-right mb-1">${data ? "Đang hoạt động" : "Đã bị khóa"}</span>`;
                }
            },
            {
                "data": "isSubscribedToEmail",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    var classColor = data ? 'green' : 'red';
                    return `<span class="badge bgc-${classColor} brc-${classColor} text-white badge-lg arrowed arrowed-in-right mb-1">${data ? "Đã đăng ký gửi mail" : "Chưa đăng ký gửi mail"}</span>`;
                }
            },
            {
                "data": "Account_ID",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';
                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn btn-lighter-primary mr-1 dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';

                        if (row.IsActive) {
                            html += _renderButton(true,
                                "TieuThuDinhKy",
                                "btn btn-outline-info btn-a-outline-info mr-1 dropdown-item",
                                "/Cate/AccountMobile/TieuThuDinhKy/" + data,
                                '<i class="fas fa-tint text-120 mr-1"></i> Lượng nước tiêu thụ theo kỳ',
                                "Lượng nước tiêu thụ theo kỳ", "fullscreen");

                            html += _renderButton(true,
                                "ChangePassword",
                                "btn btn-outline-primary btn-a-outline-primary mr-1 dropdown-item",
                                "/Cate/AccountMobile/ChangePassword/" + data,
                                '<i class="fa fa-key text-120 mr-1"></i> Đổi mật khẩu',
                                "Đổi mật khẩu");

                            html += _renderButton(true,
                                "DeActiveAccount",
                                "btn btn-outline-danger btn-a-outline-danger mr-1 dropdown-item",
                                "/Cate/AccountMobile/DeActive/" + data,
                                '<i class="fas fa-user-lock text-120 mr-1"></i> Ngưng hoạt động',
                                "Ngưng hoạt động");
                        } else {
                            html += _renderButton(true,
                                "ActiveAccount",
                                "btn btn-outline-success btn-a-outline-success mr-1 dropdown-item",
                                "/Cate/AccountMobile/Active/" + data,
                                '<i class="fa fa-check-square text-120 mr-1"></i> Kích hoạt',
                                "Kích hoạt");
                        }
                        html += '</div></div>';
                    }
                    html += "</span>";
                    return html;
                }
            }
        ]
    });
}


function AccountMobile_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableAccountMobile.ajax.reload(null, false);
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
                        _tableAccountMobile.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}