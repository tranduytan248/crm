var _CustomerActionURLs = {
    Customer_GetData: "/Cate/Customer/Get"
};
var _tableCustomer;
$(document).ready(function () {
    initTableCustomer();
});

function initTableCustomer() {
    _tableCustomer = $("#DSCustomer").DataTable({
        "Responsive": true,
        "Customer": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _CustomerActionURLs.Customer_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TuKhoa": function () { return $("#SearchCustomer #TuKhoa").val(); },
                "TuNgay": function () { return $("#SearchCustomer #TuNgay").val(); },
                "DenNgay": function () { return $("#SearchCustomer #DenNgay").val(); },
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
                "data": "FullName",
                "defaultContent": ""
            }, {
                "data": "Username",
                "defaultContent": ""
            }, {
                "data": "Address",
                "defaultContent": ""
            },
            {
                "data": "CustomerId",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';
                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn btn-lighter-primary mr-1 dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';

                        if (row.IsActive) {
                            // html += _renderButton(true,
                            //     "EditCustomer",
                            //     "btn btn-outline-success btn-a-outline-success mr-1 dropdown-item",
                            //     "/Cate/Customer/Edit/" + data,
                            //     '<i class="fa fa-edit text-120 mr-1"></i> Cập nhật',
                            //     "Cập nhật", 1024);

                            //html += _renderButton(true,
                            //    "DeActiveUser",
                            //    "btn btn-outline-danger btn-a-outline-danger mr-1 dropdown-item",
                            //    "/Cate/Customer/DeActive/" + data,
                            //    '<i class="fas fa-user-lock text-120 mr-1"></i> Ngưng hoạt động',
                            //    "Ngưng hoạt động");

                            html += _renderButton(true,
                                "ResetPassword",
                                "btn btn-outline-purple btn-a-outline-purple mr-1 dropdown-item",
                                "/Cate/Customer/ResetPassword/" + data,
                                '<i class="fa fa-paper-plane text-120 mr-1"></i> Reset mật khẩu',
                                "Reset mật khẩu");

                            //html += _renderButton(true,
                            //    "DeleteCustomer",
                            //    "btn btn-outline-danger btn-a-outline-danger mr-1 dropdown-item",
                            //    "/Cate/Customer/Delete/" + data,
                            //    '<i class="fas fa-trash-alt text-120 mr-1"></i> Xóa tài khoản',
                            //    "Xóa tài khoản");
                        } else {
                            //html += _renderButton(true,
                            //    "ActiveUser",
                            //    "btn btn-outline-primary btn-a-outline-primary mr-1 dropdown-item",
                            //    "/Sys/User/Active/" + data,
                            //    '<i class="fa fa-check-square text-120 mr-1"></i> Kích hoạt',
                            //    "Kích hoạt");
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


function Customer_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableCustomer.ajax.reload(null, false);
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
                        _tableCustomer.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
function _initDateRangePicker(eleContainer, eleWrapper, eleFromDate, eleToDate) {
    // Give Result Contract Date
    var daterange_container = document.querySelector(eleContainer)
    // Inject DateRangePicker into our container
    var daterangePicker = DateRangePicker.DateRangePicker(daterange_container, {
        mode: 'dp-permanent',
        startOpts: {
            lang: {
                days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                months: [
                    'Tháng 1',
                    'Tháng 2',
                    'Tháng 3',
                    'Tháng 4',
                    'Tháng 5',
                    'Tháng 6',
                    'Tháng 7',
                    'Tháng 8',
                    'Tháng 9',
                    'Tháng 10',
                    'Tháng 11',
                    'Tháng 12',
                ],
                today: 'Hôm nay',
                clear: 'Xoá',
                close: 'Đóng',
            }
        },
        endOpts: {
            lang: {
                days: ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'],
                months: [
                    'Tháng 1',
                    'Tháng 2',
                    'Tháng 3',
                    'Tháng 4',
                    'Tháng 5',
                    'Tháng 6',
                    'Tháng 7',
                    'Tháng 8',
                    'Tháng 9',
                    'Tháng 10',
                    'Tháng 11',
                    'Tháng 12',
                ],
                today: 'Hôm nay',
                clear: 'Xoá',
                close: 'Đóng',
            },
        }
    }).on('statechange', function (_, rp) {
        // Update the inputs when the state changes
        var range = rp.state
        $(eleFromDate).val(range.start ? moment(range.start).format("DD/MM/YYYY") : '')
        $(eleToDate).val(range.end ? moment(range.end).format("DD/MM/YYYY") : '')
    })

    $(eleFromDate).on('focus', function () {
        daterange_container.classList.add('visible')
    });

    $(eleToDate).on('focus', function () {
        daterange_container.classList.add('visible')
    })

    var daterange_wrapper = document.querySelector(eleWrapper)
    var previousTimeout = null;
    $(daterange_wrapper).on('focusout', function () {
        if (previousTimeout) clearTimeout(previousTimeout)
        previousTimeout = setTimeout(function () {
            if (!daterange_wrapper.contains(document.activeElement)) {
                daterange_container.classList.remove('visible')
            }
        }, 10)
    })
}

