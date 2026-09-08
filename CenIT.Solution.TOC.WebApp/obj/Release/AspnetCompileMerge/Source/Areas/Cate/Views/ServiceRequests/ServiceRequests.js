var _ServiceRequestsActionURLs = {
    ServiceRequests_GetData: "/Cate/ServiceRequests/Get",
    ServiceRequests_Export: "/Cate/ServiceRequests/Export",
};
var dataLocal;
var _tableServiceRequests;
$(document).ready(function () {
    initTableServiceRequests();
    HienThiSoLuong();

    _initDateRangePicker('#receivedOn-container', '#receivedOn-wrapper', '#DateFrom', '#DateTo');
    //_initDateRangePicker('#giveResultOn-container', '#giveResultOn-wrapper', '#GiveResultFromDate', '#GiveResultToDate');

    $('.selectpicker').on('loaded.bs.select', function (e, clickedIndex, isSelected, previousValue) {
        $(this).next('button').tooltip({ trigger: 'hover' });
    });

});
function toIso(dateStr) {
    if (!dateStr) return "";

    // format dd/MM/yyyy → yyyy-MM-dd
    const parts = dateStr.split('/');
    if (parts.length === 3)
        return `${parts[2]}-${parts[1]}-${parts[0]}`;

    return dateStr; // nếu đã là yyyy-MM-dd thì giữ nguyên
}

function ExportThongKeYeuCauDangKy() {
    var dateFrom = toIso($("#SearchServiceRequests #DateFrom").val());
    var dateTo = toIso($("#SearchServiceRequests #DateTo").val());

    var requestType = $('input[name="RequestType"]:checked').val();

    var statusArr = [];
    $('#SearchServiceRequests input[name="Status"]:checked').each(function () {
        statusArr.push($(this).val());
    });

    const params = new URLSearchParams({
        DateFrom: dateFrom || "",
        DateTo: dateTo || "",
        RequestType: requestType || "",
        Status: statusArr.join(',') || ""
    });

    const url = `${_ServiceRequestsActionURLs.ServiceRequests_Export}?${params.toString()}`;
    window.open(url, "_blank");
}



function initTableServiceRequests() {
    _tableServiceRequests = $("#DSServiceRequests").DataTable({
        "Responsive": true,
        "ServiceRequests": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "order": [[1, "desc"]],
        "ajax":
        {
            "url": _ServiceRequestsActionURLs.ServiceRequests_GetData,
            "type": "POST",
            "ServiceRequests": "JSON",
            "data": {
                "DateFrom": function () { return $("#SearchServiceRequests #DateFrom").val(); },
                "DateTo": function () { return $("#SearchServiceRequests #DateTo").val(); },
                "RequestType": function () { return $('input[name="RequestType"]:checked').val(); },
                "Status": function () {
                    var stt = [];
                    $('#SearchServiceRequests input[name="Status"]:checked').each(function () {
                        stt.push(this.value);
                    });
                    return stt.join();
                },
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
                "data": "RequestDate", "defaultContent": "",
                "className": "text-center",
                "render": function (data, type, row, meta) {
                    return moment(data).format("DD/MM/YYYY HH:mm:ss");
                }
            },
            {
                "data": "RequesterName",
                "defaultContent": "",
                "className": "w-50",
                "render": function (data, type, row, meta) {
                    var ttThayDoi = $('input[name="RequestType"]:checked').val() == 'thaydoithongtindadangky'
                        ? `<div class="change-info" data-id="${row.RequestId}"><i class="fas fa-lg fa-spinner icon text-primary-l1"></i> Đang lấy thông tin...</div>`
                        : '';
                    return `<div>
                    <div class="text-blue text-600">${data}</div>
                        <div class="text-90 text-secondary-d1"><i class="w-2 text-center text-green-d2 fas fa-mobile"></i> ${row.Phone ?? ''}</div>
                        <div class="text-90 text-secondary-d1 font-italic"><i class="w-2 text-center text-danger-d2 fas fa-map-marker-alt"></i> ${row.AddressCus ?? ''}</div>
                    </div>`  + ttThayDoi;
                }
            },
            {
                "data": "StatusProcessName",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return `<span class="{0}">
                                    <span class="px-2">{1}</span>
                                </span>`.format($('#sl_' + row.Status).parent().parent().attr('class'), data);
                }
            },
            {
                "data": "RequestId",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "UpdateStatusProcess",
                            "btn btn-lighter-primary mr-1",
                            "/Cate/ServiceRequests/UpdateStatusProcess/" + data,
                            '<i class="far fa-edit text-primary text-120"></i>',
                            "Thay đổi tiến trình xử lý", 'fullscreen');

                        html += _renderButton(true,
                            "PreviewForm",
                            "btn btn-lighter-warning mr-1",
                            "/Cate/ServiceRequests/renderHTMLForm/" + data,
                            '<i class="fa fa-eye text-warning text-120"></i>',
                            "Xem chi tiết yêu cầu");
                    }
                    html += "</span>";

                    return html;
                }
            }
        ],
        drawCallback: function (settings) {
            var api = this.api();
            $('.change-info').each(function () {
                var el = $(this);
                var id = el.data('id');
                if (el.data('loaded')) return;
                $.get(`/Cate/ServiceRequests/GetChangeRegistrationInfoByRequestId/${id}`, function (html) {
                    el.html(html);
                    el.data('loaded', true);
                });
            });
        }
    });
}


function ServiceRequests_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableServiceRequests.ajax.reload(null, false);
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
                        _tableServiceRequests.ajax.reload(null, false);
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

function _search() {
    _tableServiceRequests.ajax.reload(null, false);
    HienThiSoLuong();
}

function HienThiSoLuong() {
    for (var i = 0; i < $('input[name=RequestType]').length; i++) {
        var idSpan = $('input[name=RequestType]').eq(i).prop('value');
        $('#sl_' + idSpan).html(`<i class="fas fa-lg fa-spinner icon text-primary-l1"></i>`);
    }
    for (var i = 0; i < $('input[name=Status]').length; i++) {
        var idSpan = $('input[name=Status]').eq(i).prop('value');
        $('#sl_' + idSpan).html(`<i class="fas fa-lg fa-spinner icon text-primary-l1"></i>`);
    }

    $.ajax({
        "url": _ServiceRequestsActionURLs.ServiceRequests_GetData,
        "type": "POST",
        "dataType": "JSON",
        "data": {
            "DateFrom": function () { return $("#SearchServiceRequests #DateFrom").val(); },
            "DateTo": function () { return $("#SearchServiceRequests #DateTo").val(); },
            "RequestType": '-1',
            "Status": '-1'
        },
        success: function (response) {
            dataLocal = response.data;
            for (var i = 0; i < $('input[name=RequestType]').length; i++) {
                var idSpan = $('input[name=RequestType]').eq(i).prop('value');
                $('#sl_' + idSpan).text('(' + dataLocal.filter(x => x.RequestType == idSpan).length + ')');
            }
            _changeGroup($('input[name="RequestType"]:checked').val());
        },
        error: function (response) { /*alert('SOS')*/ }
    });
}

function _changeGroup(groupID) {
    //alert(groupID)
    for (var i = 0; i < $('input[name=Status]').length; i++) {
        var idSpan = $('input[name=Status]').eq(i).prop('value');
        $('#sl_' + idSpan).text('(' + dataLocal.filter(x => x.Status == idSpan & x.RequestType == groupID).length + ')');
    }
}
