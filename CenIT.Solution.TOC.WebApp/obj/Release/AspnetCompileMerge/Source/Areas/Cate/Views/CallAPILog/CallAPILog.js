var _CallAPILogActionURLs = {
    CallAPILog_GetData: "/Cate/CallAPILog/Get"
};
var _tableCallAPILog;

$(document).ready(function () {
    initTableCallAPILog();
});

function initTableCallAPILog() {
    _tableCallAPILog = $("#DSCallAPILog").DataTable({
        "Responsive": true,
        "CallAPILog": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _CallAPILogActionURLs.CallAPILog_GetData,
            "type": "POST",
            "dataType": "JSON"           
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
                "data": "APINameOrURL",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }

            },
            {
                "data": "Request",
                "defaultContent": ""

            },
            {
                "data": "Response",
                "defaultContent": ""
               

            }
            ,
            {
                "data": "CreatedDate",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("DD/MM/YYYY HH:MM:SS");
                }

            }
           
        ]
    });
}

