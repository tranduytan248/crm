var _PaidInvoiceActionURLs = {
    PaidInvoice_GetData: "/Cate/PaidInvoice/Get",
    PaidInvoice_Export: "/Cate/PaidInvoice/Export"
};
var _tablePaidInvoice;
//$(document).ready(function () {
//    initTablePaidInvoice();
//});
function toTransactionDate(value) {
    // value: "yyyyMMddHHmmss"
    const year = parseInt(value.substring(0, 4));
    const month = parseInt(value.substring(4, 6)) - 1; // JS month 0-11
    const day = parseInt(value.substring(6, 8));
    const hour = parseInt(value.substring(8, 10));
    const minute = parseInt(value.substring(10, 12));
    const second = parseInt(value.substring(12, 14));

    return new Date(year, month, day, hour, minute, second);
}

function toTransactionDateString(value) {
    const d = toTransactionDate(value);

    const pad = n => n.toString().padStart(2, "0");

    return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} `
        + `${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
}

function toIso(dateStr) {
    if (!dateStr) return "";

    // format dd/MM/yyyy → yyyy-MM-dd
    const parts = dateStr.split('/');
    if (parts.length === 3)
        return `${parts[2]}-${parts[1]}-${parts[0]}`;

    return dateStr; // nếu đã là yyyy-MM-dd thì giữ nguyên
}

function ExportLichSuThanhToan() {
    var period = toIso($("#Searching #Period").val());


    const params = new URLSearchParams({
        Period: period || "",
    });

    const url = `${_PaidInvoiceActionURLs.PaidInvoice_Export}?${params.toString()}`;
    window.open(url, "_blank");
}

function initTablePaidInvoice() {
    _tablePaidInvoice = $("#DSPaidInvoice").DataTable({
        "Responsive": true,
        "PaidInvoice": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _PaidInvoiceActionURLs.PaidInvoice_GetData,
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
                "data": "ContractCode",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }

            },
            {
                "data": "Period",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return moment(data).format("MM/YYYY");
                }

            },
            {
                "data": "Amount",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    if (!data) return "";
                    return data.toLocaleString('it-IT', { style: 'currency', currency: 'VND' });
                }

            }
            ,
            {
                "data": "BillNumber",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }

            }
            ,
            {
                "data": "CreatedBy",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }

            }
            ,
            {
                "data": "PayDate",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return toTransactionDateString(data);
                }

            }
        ]
    });
}

