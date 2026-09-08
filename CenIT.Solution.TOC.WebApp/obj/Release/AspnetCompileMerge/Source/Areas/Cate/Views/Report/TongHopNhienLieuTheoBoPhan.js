var _ReportActionURLs = {
    Report_GetData: "/Cate/Report/TongHopNhienLieuTheoBoPhan"
};
var _tableReport;
var fromDate;
var toDate;
var loaiNhienLieu;

$(document).ready(function () {
    df_datepicker('#SearchReport #FromDate');
    df_datepicker('#SearchReport #ToDate');
    initTableReport();
});

function SearchReport() {
    _tableReport.ajax.reload(null, false);
    fromDate = $("#SearchReport #FromDate").val();
    toDate = $("#SearchReport #ToDate").val();
    loaiNhienLieu = $("#SearchReport #LoaiNhienLieu").val();
}

function initTableReport() {
    _tableReport = $("#RPTongHopNhienLieuTheoBoPhan").DataTable({
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
            "url": "/Cate/Report/TongHopNhienLieuTheoBoPhan",
            "type": "POST",
            "Report": "JSON",
            "data": {
                "FromDate": function () { return $("#SearchReport #FromDate").val(); },
                "ToDate": function () { return $("#SearchReport #ToDate").val(); },
                "LoaiNhienLieu": function () { return $("#SearchReport #LoaiNhienLieu").val(); }
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
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "TenBoPhan",
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
//function ExportToExcel() {
//    var tuNgay = moment(fromDate, "DD/MM/YYYY").format("MM/DD/YYYY");
//    var denNgay = moment(toDate, "DD/MM/YYYY").format("MM/DD/YYYY");
//    var url = "Cate/Report/ExportExcel";
//    url += '?FromDate=' + tuNgay + '&ToDate=' + denNgay + '&type=TheoBoPhan';
//    window.location.href = url;
//}
function ExportToExcel() {
    var tuNgay = moment(fromDate, "DD/MM/YYYY").format("MM/DD/YYYY");
    var denNgay = moment(toDate, "DD/MM/YYYY").format("MM/DD/YYYY");
    var url = "Cate/Report/ExportExcel1";
    url += '?FromDate=' + tuNgay + '&ToDate=' + denNgay + '&LoaiNhienLieu=' + loaiNhienLieu;
    window.location.href = url;
}