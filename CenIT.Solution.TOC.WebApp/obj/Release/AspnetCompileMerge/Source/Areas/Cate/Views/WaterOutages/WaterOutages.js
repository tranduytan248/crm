var _WaterOutagesActionURLs = {
    WaterOutages_GetData: "/Cate/WaterOutages/Get"
};
var _tableWaterOutages;
$(document).ready(function () {
    initTableWaterOutages();
});

function initTableWaterOutages() {
    _tableWaterOutages = $("#DSWaterOutages").DataTable({
        "Responsive": true,
        "WaterOutages": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _WaterOutagesActionURLs.WaterOutages_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TuKhoa": function () { return $("#SearchWaterOutages #TuKhoa").val(); },
                "Status": function () { return $("#SearchWaterOutages #Status").val(); },
                "TuNgay": function () { return $("#SearchWaterOutages #TuNgay").val(); },
                "DenNgay": function () { return $("#SearchWaterOutages #DenNgay").val(); },
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
                "data": "Area",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return` 
                     ${row["IsPushNotification"] == 1 ? ` <div class="text-left my-auto mb-1"><span class="m-1 badge bgc-white border-1 brc-green-m2 btn-text-green radius-2">
                    Đã gửi thông báo
                </span></div>` : ` <div class="text-left my-auto mb-1"><span class="m-1 badge bgc-white border-1 brc-red-m2 btn-text-red radius-2">
                    Chưa gửi thông báo
                </span></div>` }

                    <div class="d-flex">
                        <div class="text-left my-auto ">${data}</div>
                    </div>
                    
                    `;
                }
            },
            {
                "data": "StartTime",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("HH:mm DD/MM/YYYY");
                }
            },
            {
                "data": "EndTime",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("HH:mm DD/MM/YYYY");
                }
            },
            {
                "data": "IsActived",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    var classColor = data ? 'green' : 'red';
                    return `<span class="badge bgc-${classColor} brc-${classColor} text-white badge-lg arrowed arrowed-in-right mb-1">${row.StatusName}</span>`;
                }
            },
            {
                "data": "PublicDate",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("HH:mm DD/MM/YYYY");
                }
            },
            {
                "data": "OutageId",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn px-4 btn-lighter-primary mr-1 v-hover dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';
                        html += _renderButton(true,
                            "EditWaterOutages",
                            "btn btn-lighter-primary mr-1 dropdown-item",
                            "/Cate/WaterOutages/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i> Cập nhật',
                            "Cập nhật", 1024);

                        if (row["IsActived"] == 0) {
                            html += _renderButton(true,
                                "UpdateStatusWaterOutages",
                                "btn btn-lighter-info mr-1 dropdown-item",
                                "/Cate/WaterOutages/UpdateStatus/" + data,
                                '<i class="fas fa-eye text-info text-120"></i> Hiển thị thông báo',
                                "Hiển thị thông báo");
                        }
                        else {
                            html += _renderButton(true,
                                "UpdateStatusWaterOutages",
                                "btn btn-lighter-info mr-1 dropdown-item",
                                "/Cate/WaterOutages/UpdateStatus/" + data,
                                '<i class="far fa-eye-slash text-info text-120"></i> Ẩn thông báo',
                                "Ẩn tin tức");
                        }
                        if (row["IsPushNotification"] == 0)
                        html += _renderButton(true,
                            "UpdateStatusSendNotificationWaterOutages",
                            "btn btn-lighter-warning mr-1 dropdown-item",
                            "/Cate/WaterOutages/UpdateStatusSendNotification/" + data,
                            '<i class="fas fa-bell text-warning text-120"></i> Gửi thông báo',
                            "Gửi thông báo");
                        html += _renderButton(true,
                            "DeleteWaterOutages",
                            "btn btn-lighter-danger mr-1 dropdown-item",
                            "/Cate/WaterOutages/Delete/" + data,
                            '<i class="far fa-trash-alt text-danger text-120"></i> Xóa',
                            "Xoá");
                        
                    }
                    html += "</span>";

                    return html;
                }
            }
        ]
    });
}


function WaterOutages_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableWaterOutages.ajax.reload(null, false);
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
                        _tableWaterOutages.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
//Contructor CKEDITOR
function reloadCKEditor() {
    var editor = CKEDITOR.instances['Reason'];
    if (editor) { editor.destroy(true); }
    editor = CKEDITOR.replace('Reason', {
        filebrowserWindowWidth: '1000',
        filebrowserWindowHeight: '750',
    });
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

