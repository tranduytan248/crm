var _ReportActionURLs = {
    Report_GetData: "/Cate/Report/TongHopNhienLieu"
};
var _tableReport;
var fromDate;
var toDate;

$(document).ready(function () {
    df_datepicker('#SearchReport #FromDate');
    df_datepicker('#SearchReport #ToDate');
    initTableReport();
});

function SearchReport() {
    _tableReport.ajax.reload(null, false);
    fromDate = $("#SearchReport #FromDate").val();
    toDate = $("#SearchReport #ToDate").val();
}

function initTableReport() {
    _tableReport = $("#RPTongHopNhienLieu").DataTable({
        "Responsive": true,
        "language": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "dom": 'rt',
        "ajax":
        {
            "url": "/Cate/Report/TongHopNhienLieu",
            "type": "POST",
            "Report": "JSON",
            "data": {
                "FromDate": function () { return $("#SearchReport #FromDate").val(); },
                "ToDate": function () { return $("#SearchReport #ToDate").val(); },
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
                "data": "MaNhienLieu",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "TenNhienLieu",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "NhienLieuCap",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            }
        ]
    });
}
function ExportToExcel() {
    var tuNgay = moment(fromDate, "DD/MM/YYYY").format("MM/DD/YYYY");
    var denNgay = moment(toDate, "DD/MM/YYYY").format("MM/DD/YYYY");
    var url = "Cate/Report/ExportExcel";
    url += '?FromDate=' + tuNgay + '&ToDate=' + denNgay;
    window.location.href = url;
}
