var _tableDigitalSales;
var _digitalSalesUrls = {
    get: "/Cate/DigitalSales/Get",
    add: "/Cate/DigitalSales/Add",
    edit: "/Cate/DigitalSales/Edit",
    delete: "/Cate/DigitalSales/Delete",
    detail: "/Cate/DigitalSales/Detail",
    changeStatusModal: "/Cate/DigitalSales/ChangeStatusModal",
    changeStatus: "/Cate/DigitalSales/ChangeStatus",
    getContactPersons: "/Cate/DigitalSales/GetContactPersons"
};

$(document).ready(function () {
    initTableDigitalSales();
});

function initTableDigitalSales() {
    _tableDigitalSales = $("#tblDigitalSales").DataTable({
        responsive: true,
        processing: true,
        serverSide: true,
        ordering: false,
        searching: false,
        pageLength: 20,
        dom: '<"dt-top d-flex justify-content-between align-items-center mb-2 px-3 pt-3"l>t<"dt-bottom d-flex justify-content-between align-items-center px-3 py-3"ip>',
        ajax: {
            url: _digitalSalesUrls.get,
            type: "POST",
            dataType: "JSON",
            data: function (d) {
                d.Keyword = $("#SearchKeyword").val();
                d.BusinessType = $("#SearchBusinessType").val();
                d.StatusID = $("#SearchStatusID").val();
                d.CustomerID = $("#SearchCustomerID").val();
                d.ProductServiceID = $("#SearchProductServiceID").val();
                d.EmployeeID = $("#SearchEmployeeID").val();
                d.DepartmentID = $("#SearchDepartmentID").val();
                d.FromDate = $("#SearchFromDate").val();
                d.ToDate = $("#SearchToDate").val();
            }
        },
        columns: [
            {
                data: null,
                className: "text-center",
                render: function (data, type, row, meta) {
                    return meta.row + meta.settings._iDisplayStart + 1;
                }
            },
            {
                data: "Code",
                render: function (data, type, row) {
                    return '<a href="' + _digitalSalesUrls.detail + '/' + row.DigitalSalesID + '" class="font-weight-bold text-primary">' + (data || '—') + '</a>';
                }
            },
            {
                data: "Title",
                render: function (data, type, row) {
                    var html = '<a href="' + _digitalSalesUrls.detail + '/' + row.DigitalSalesID + '" class="font-weight-bold text-dark text-95 d-block">' + data + '</a>';
                    if (row.ProductServiceNames) {
                        html += '<small class="text-secondary"><i class="fa fa-tag mr-1"></i>' + row.ProductServiceNames + '</small>';
                    }
                    return html;
                }
            },
            {
                data: "BusinessType",
                className: "text-center",
                render: function (data, type, row) {
                    if (data === 2) {
                        return '<span class="badge badge-success px-2 py-1">Dự án</span>';
                    }
                    return '<span class="badge badge-primary px-2 py-1">Cơ hội</span>';
                }
            },
            {
                data: "StatusName",
                className: "text-center",
                render: function (data, type, row) {
                    var badgeClass = "badge-secondary";
                    if (row.StatusID === 1) badgeClass = "badge-secondary";
                    else if (row.StatusID === 2) badgeClass = "badge-info";
                    else if (row.StatusID === 3 || row.StatusID === 6) badgeClass = "badge-danger";
                    else if (row.StatusID === 7) badgeClass = "badge-primary";
                    else if (row.StatusID === 8) badgeClass = "badge-success";
                    else badgeClass = "badge-warning text-dark";

                    return '<span class="badge ' + badgeClass + ' px-2 py-1">' + (data || '—') + '</span>';
                }
            },
            {
                data: "CustomerName",
                render: function (data, type, row) {
                    var html = '<span class="font-weight-bold">' + (data || '—') + '</span>';
                    if (row.ContactPersonName) {
                        html += '<br/><small class="text-secondary">' + row.ContactPersonName + '</small>';
                    }
                    return html;
                }
            },
            {
                data: "AssignedEmployeeName",
                render: function (data, type, row) {
                    return '<span class="text-dark">' + (data || '—') + '</span>';
                }
            },
            {
                data: "TotalExpectedRevenue",
                className: "text-right font-weight-bold text-primary",
                render: function (data) {
                    return data ? Number(data).toLocaleString('vi-VN') : '0';
                }
            },
            {
                data: "TotalActualRevenue",
                className: "text-right font-weight-bold text-success",
                render: function (data) {
                    return data ? Number(data).toLocaleString('vi-VN') : '0';
                }
            },
            {
                data: "ProgressPercentage",
                className: "text-center",
                render: function (data, type, row) {
                    var percent = data || 0;
                    var colorClass = percent >= 100 ? "bgc-success" : (percent >= 50 ? "bgc-primary" : "bgc-warning");
                    return '<div class="d-flex align-items-center justify-content-center">' +
                        '<div class="progress flex-grow-1 mr-2" style="height: 8px;">' +
                        '<div class="progress-bar ' + colorClass + '" style="width:' + percent + '%"></div>' +
                        '</div>' +
                        '<span class="text-80 font-weight-bold">' + percent + '%</span>' +
                        '</div>';
                }
            },
            {
                data: null,
                className: "text-center",
                render: function (data, type, row) {
                    return '<div class="action-buttons">' +
                        '<a href="' + _digitalSalesUrls.detail + '/' + row.DigitalSalesID + '" class="text-primary mr-2" title="Xem chi tiết 360 độ"><i class="fa fa-eye"></i></a>' +
                        '<a href="javascript:void(0);" onclick="openChangeStatusModal(' + row.DigitalSalesID + ');" class="text-warning-d2 mr-2" title="Chuyển trạng thái"><i class="fa fa-exchange-alt"></i></a>' +
                        '<a href="javascript:void(0);" onclick="openEditSalesModal(' + row.DigitalSalesID + ');" class="text-info mr-2" title="Chỉnh sửa"><i class="fa fa-edit"></i></a>' +
                        '<a href="javascript:void(0);" onclick="deleteSales(' + row.DigitalSalesID + ');" class="text-danger" title="Xóa"><i class="fa fa-trash-alt"></i></a>' +
                        '</div>';
                }
            }
        ],
        language: {
            processing: "Đang tải dữ liệu...",
            emptyTable: "Không có dữ liệu phù hợp",
            info: "Hiển thị _START_ đến _END_ trong tổng số _TOTAL_ bản ghi",
            infoEmpty: "Không có bản ghi nào",
            infoFiltered: "(lọc từ _MAX_ bản ghi)",
            lengthMenu: "Hiển thị _MENU_ bản ghi",
            paginate: {
                first: "Đầu",
                previous: "Trước",
                next: "Sau",
                last: "Cuối"
            }
        }
    });
}

function reloadSalesTable() {
    if (_tableDigitalSales) {
        _tableDigitalSales.ajax.reload();
    }
}

function resetSalesSearch() {
    $("#frmSearchDigitalSales")[0].reset();
    if ($.fn.select2) {
        $("#frmSearchDigitalSales select").val("").trigger("change");
    }
    reloadSalesTable();
}

function openAddSalesModal() {
    $.get(_digitalSalesUrls.add, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalAddSales");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmAddSales").on("submit", function (e) {
            e.preventDefault();
            var formData = new FormData(this);
            $.ajax({
                url: _digitalSalesUrls.add,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        alert(res.message || "Khởi tạo Cơ hội thành công!");
                        if (res.id) {
                            window.location.href = _digitalSalesUrls.detail + "/" + res.id;
                        } else {
                            reloadSalesTable();
                        }
                    } else {
                        alert(res.message || "Có lỗi xảy ra khi lưu cơ hội!");
                    }
                },
                error: function () {
                    alert("Lỗi kết nối máy chủ!");
                }
            });
        });
    });
}

function openEditSalesModal(id) {
    $.get(_digitalSalesUrls.edit + "/" + id, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalEditSales");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmEditSales").on("submit", function (e) {
            e.preventDefault();
            var formData = new FormData(this);
            $.ajax({
                url: _digitalSalesUrls.edit,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        alert(res.message || "Cập nhật thành công!");
                        reloadSalesTable();
                    } else {
                        alert(res.message || "Có lỗi xảy ra!");
                    }
                },
                error: function () {
                    alert("Lỗi kết nối máy chủ!");
                }
            });
        });
    });
}

function openChangeStatusModal(id) {
    $.get(_digitalSalesUrls.changeStatusModal + "/" + id, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalChangeStatus");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmChangeStatus").on("submit", function (e) {
            e.preventDefault();
            var formData = new FormData(this);
            $.ajax({
                url: _digitalSalesUrls.changeStatus,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        alert(res.message || "Chuyển trạng thái thành công!");
                        reloadSalesTable();
                    } else {
                        alert(res.message || "Không thể chuyển trạng thái!");
                    }
                },
                error: function () {
                    alert("Lỗi kết nối máy chủ!");
                }
            });
        });
    });
}

function deleteSales(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa hồ sơ kinh doanh này không?")) return;
    $.post(_digitalSalesUrls.delete, { id: id }, function (res) {
        if (res.status) {
            alert(res.message || "Xóa thành công!");
            reloadSalesTable();
        } else {
            alert(res.message || "Không thể xóa hồ sơ!");
        }
    });
}

function loadContactPersonsByCustomer(customerId, targetSelector) {
    if (!customerId) {
        $(targetSelector).empty().append('<option value="">-- Chọn người liên hệ --</option>');
        return;
    }
    $.get(_digitalSalesUrls.getContactPersons, { customerId: customerId }, function (items) {
        var $target = $(targetSelector);
        $target.empty().append('<option value="">-- Chọn người liên hệ --</option>');
        if (items && items.length > 0) {
            $.each(items, function (idx, item) {
                $target.append($('<option>', { value: item.id, text: item.name }));
            });
        }
        if ($.fn.select2) {
            $target.trigger("change");
        }
    });
}
