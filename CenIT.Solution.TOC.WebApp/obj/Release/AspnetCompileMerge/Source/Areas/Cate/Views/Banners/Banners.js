var _BannersActionURLs = {
    Banners_GetData: "/Cate/Banners/Get"
};
var _tableBanners;
$(document).ready(function () {
    initTableBanners();
});

function initTableBanners() {
    _tableBanners = $("#DSBanners").DataTable({
        "Responsive": true,
        "Banners": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _BannersActionURLs.Banners_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "TuKhoa": function () { return $("#SearchBanners #TuKhoa").val(); },
                "Status": function () { return $("#SearchBanners #Status").val(); },
                "TuNgay": function () { return $("#SearchBanners #TuNgay").val(); },
                "DenNgay": function () { return $("#SearchBanners #DenNgay").val(); },
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
                "data": "ImageUrlPath",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return `
                     ${row["IsPushNotification"] == 1 ? ` <div class="text-center my-auto mb-1"><span class="m-1 badge bgc-white border-1 brc-green-m2 btn-text-green radius-2">
                    Đã gửi thông báo
                </span></div>` : ` <div class="text-center my-auto mb-1"><span class="m-1 badge bgc-white border-1 brc-red-m2 btn-text-red radius-2">
                    Chưa gửi thông báo
                </span></div>` }
                        <img src="${row.ImageUrlPath}" class="new_img  mr-2 border-2 brc-primary-l1 p-0 radius-1"/>
                    `;
                }
            },          
            {
                "data": "IsActived",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    var classColor = data == 1 ? 'green' : 'red';
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
                "data": "BannerId",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn px-4 btn-lighter-primary mr-1 v-hover dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';
                        html += _renderButton(true,
                            "EditBanners",
                            "btn btn-lighter-primary mr-1 dropdown-item",
                            "/Cate/Banners/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i> Cập nhật',
                            "Cập nhật", 1024);

                        if (row["IsActived"] == 0) {
                            html += _renderButton(true,
                                "UpdateStatusBanners",
                                "btn btn-lighter-info mr-1 dropdown-item",
                                "/Cate/Banners/UpdateStatus/" + data,
                                '<i class="fas fa-eye text-info text-120"></i> Hiển thị banner',
                                "Hiển thị banner");
                        }
                        else {
                            html += _renderButton(true,
                                "UpdateStatusBanners",
                                "btn btn-lighter-info mr-1 dropdown-item",
                                "/Cate/Banners/UpdateStatus/" + data,
                                '<i class="far fa-eye-slash text-info text-120"></i> Ẩn banner',
                                "Ẩn banner");
                        }
                        if (row["IsPushNotification"] == 0)
                            html += _renderButton(true,
                                "UpdateStatusSendNotificationBanners",
                                "btn btn-lighter-warning mr-1 dropdown-item",
                                "/Cate/Banners/UpdateStatusSendNotification/" + data,
                                '<i class="fas fa-bell text-warning text-120"></i> Gửi thông báo',
                                "Gửi thông báo");
                        html += _renderButton(true,
                            "DeleteBanners",
                            "btn btn-lighter-danger mr-1 dropdown-item",
                            "/Cate/Banners/Delete/" + data,
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


function Banners_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableBanners.ajax.reload(null, false);
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
                        _tableBanners.ajax.reload(null, false);
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

