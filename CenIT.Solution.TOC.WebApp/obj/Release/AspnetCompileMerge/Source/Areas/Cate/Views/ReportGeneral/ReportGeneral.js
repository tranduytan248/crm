var _ServiceRequestsActionURLs = {
    ServiceRequests_GetData: "/Cate/ReportGeneral/Get",
    ServiceRequests_Export: "/Cate/ReportGeneral/Export"
};
var dataLocal;
var _tableServiceRequests;
$(document).ready(function () {
    initTableServiceRequests();

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
    var dateFrom = toIso($("#SearchReport #DateFrom").val());
    var dateTo = toIso($("#SearchReport #DateTo").val());
 

    const params = new URLSearchParams({
        DateFrom: dateFrom || "",
        DateTo: dateTo || ""
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
        "paging": false,
        "info": false,
        "searching": false,
        "ajax":
        {
            "url": _ServiceRequestsActionURLs.ServiceRequests_GetData,
            "type": "POST",
            "ServiceRequests": "JSON",
            "data": {
                "DateFrom": function () { return $("#SearchReport #DateFrom").val(); },
                "DateTo": function () { return $("#SearchReport #DateTo").val(); },
               
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
                "data": "TypeServiceName",
                "defaultContent": "",    
            },
            {
                "data": "New",
                "defaultContent": "",
                "className": "text-center",
               
            },         
            {
                "data": "Apply",
                "defaultContent": "",
                "className": "text-center",

            },
            {
                "data": "Processing",
                "defaultContent": "",
                "className": "text-center",

            }
            ,
            {
                "data": "Done",
                "defaultContent": "",
                "className": "text-center",

            }
            ,
            {
                "data": "Cancel",
                "defaultContent": "",
                "className": "text-center",

            }
            ,
            {
                "data": "Total",
                "defaultContent": "",
                "className": "text-center",

            }
        ]
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

function _search() {
    _tableServiceRequests.ajax.reload(null, false);
}

