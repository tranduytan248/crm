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
    initSearchDatepicker();
    initTableDigitalSales();
});

function initSearchDatepicker() {
    if ($.fn.datepicker) {
        $('#dpFromDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            language: 'vi'
        });
        $('#dpToDate').datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            todayHighlight: true,
            language: 'vi'
        });
    }
}

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
                d.Keyword = ($("#Keyword").val() || $("#SearchKeyword").val() || "").trim();
                d.BusinessType = $("#BusinessType").val() || $("#SearchBusinessType").val() || "";
                d.StatusID = $("#StatusID").val() || $("#SearchStatusID").val() || "";
                d.CustomerID = $("#CustomerID").val() || 0;
                d.ProductServiceID = 0;
                d.EmployeeID = $("#EmployeeID").val() || $("#SearchEmployeeID").val() || "";
                d.DepartmentID = $("#DepartmentID").val() || $("#SearchDepartmentID").val() || "";
                d.FromDate = $("#FromDate").val() || $("#SearchFromDate").val() || "";
                d.ToDate = $("#ToDate").val() || $("#SearchToDate").val() || "";
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

function executeResponseMessage(message, defaultText, isSuccess) {
    if (!message && defaultText) {
        message = defaultText;
    }
    if (message && typeof message === "string") {
        if (message.indexOf("$.aceToaster") !== -1 || message.indexOf("toastr") !== -1 || message.indexOf("eval") !== -1) {
            try {
                eval(message);
                return;
            } catch (e) {
                console.error("Execute message script error:", e);
            }
        }
    }
    if (typeof $.aceToaster !== "undefined") {
        $.aceToaster.add({
            placement: 'tr',
            body: "<div class='p-3'>" + (message || defaultText) + "</div>",
            width: '420px',
            delay: 4000,
            className: isSuccess ? 'bgc-success-d2 text-white' : 'bgc-danger-d2 text-white'
        });
    } else if (typeof toastr !== "undefined") {
        if (isSuccess) {
            toastr.success(message || defaultText);
        } else {
            toastr.error(message || defaultText);
        }
    } else {
        alert(message || defaultText);
    }
}

function reloadSalesTable() {
    if (_tableDigitalSales) {
        _tableDigitalSales.ajax.reload(null, false);
    }
}

function loadStatusesByBusinessType(businessType, selectedStatusId) {
    var $status = $('#StatusID, #SearchStatusID');
    var currentVal = selectedStatusId !== undefined ? selectedStatusId : $status.val();

    $status.empty().append('<option value="">-- Chọn trạng thái --</option>');

    $.get('/Cate/DigitalSales/GetStatusesByBusinessType', {
        businessType: businessType ? businessType : ''
    }, function (data) {
        if (data && data.length > 0) {
            var hasCurrentVal = false;
            $.each(data, function (i, item) {
                var isSelected = (currentVal && item.id == currentVal);
                if (isSelected) hasCurrentVal = true;
                $status.append(
                    $('<option>').val(item.id).text(item.name).prop('selected', isSelected)
                );
            });
            if (!hasCurrentVal) {
                $status.val('');
            }
        } else {
            $status.val('');
        }
        $status.trigger("chosen:updated");
        if ($.fn.select2) {
            $status.trigger("change.select2");
        }
        reloadSalesTable();
    });
}

function resetSalesSearch() {
    $("#Keyword, #SearchKeyword").val("");
    $("#BusinessType, #SearchBusinessType").val("");
    $("#BusinessType, #SearchBusinessType").trigger("chosen:updated");
    if ($.fn.select2) {
        $("#BusinessType, #SearchBusinessType").trigger("change.select2");
    }

    $("#DepartmentID, #SearchDepartmentID").val("");
    $("#DepartmentID, #SearchDepartmentID").trigger("chosen:updated");
    if ($.fn.select2) {
        $("#DepartmentID, #SearchDepartmentID").trigger("change.select2");
    }

    $("#FromDate, #SearchFromDate").val("");
    $("#ToDate, #SearchToDate").val("");
    if ($.fn.datepicker) {
        $('#dpFromDate').datepicker('update', '');
        $('#dpToDate').datepicker('update', '');
    }

    var $employee = $("#EmployeeID, #SearchEmployeeID");
    $employee.empty().append('<option value="">-- Chọn nhân viên --</option>');
    $.get('/Cate/DigitalSales/GetEmployeesByDepartment', { departmentId: 0 }, function (data) {
        if (data && data.length > 0) {
            $.each(data, function (i, item) {
                $employee.append($('<option>').val(item.Value).text(item.Text));
            });
        }
        $employee.trigger("chosen:updated");
        if ($.fn.select2) {
            $employee.trigger("change.select2");
        }
    });

    loadStatusesByBusinessType("");
}

function DigitalSales_OnProcessSuccess(response, formId) {
    var $modal = $("#modal_" + formId);
    if ($modal.length === 0) {
        $modal = $("#modalContainer .modal.show");
    }
    if ($modal.length === 0) {
        $modal = $(".modal.show");
    }

    // 1. Phục hồi trạng thái nút Lưu
    var $btnSave = $modal.find("#btnSave, .modal-footer #btnSave, button[type='submit']");
    $btnSave.prop("disabled", false).html('<i class="fa fa-save"></i> Lưu');

    if (response && response.status !== undefined) {
        // TRƯỜNG HỢP 1: JSON response
        if (response.status === true) {
            // Hiển thị Toastr thành công NGAY LẬP TỨC
            executeResponseMessage(response.message, "Thao tác thành công!", true);

            // Đóng modal và dọn dẹp backdrop
            $modal.modal("hide");
            $(".modal-backdrop").remove();
            $("body").removeClass("modal-open").css("padding-right", "");

            // Điều hướng sang trang chi tiết 360 độ hoặc tải lại bảng
            if (response.id) {
                setTimeout(function () {
                    window.location.href = _digitalSalesUrls.detail + "/" + response.id;
                }, 300);
            } else {
                reloadSalesTable();
            }
        } else {
            // Báo lỗi nghiệp vụ
            executeResponseMessage(response.message, "Thao tác thất bại!", false);
        }
    } else {
        // TRƯỜNG HỢP 2: HTML PartialView response do validation lỗi
        var $body = $modal.find("#bodyForm");
        if ($body.length === 0) {
            $body = $("#bodyForm");
        }
        $body.html(response);

        // Khởi tạo lại plugins (select2, datepicker, ckeditor...)
        if (typeof initDigitalSalesFormPlugins === "function") {
            initDigitalSalesFormPlugins();
        }

        // BẮT BUỘC BẬT TOASTR CẢNH BÁO CHO NGƯỜI DÙNG BIẾT
        var $firstError = $body.find(".text-danger:visible").first();
        var warnMsg = ($firstError.length && $firstError.text().trim())
            ? $firstError.text().trim()
            : "Vui lòng kiểm tra và nhập đầy đủ các trường bắt buộc (*)!";
        executeResponseMessage(warnMsg, warnMsg, false);

        // Cuộn hoặc focus vào ô lỗi đầu tiên
        if ($firstError.length > 0) {
            var $targetInput = $firstError.prev().find("input, select, textarea");
            if ($targetInput.length === 0) {
                $targetInput = $firstError.closest(".mb-3").find("input, select, textarea, button");
            }
            if ($targetInput.length > 0) {
                $targetInput.first().focus();
            }
        }

        // Re-bind lại sự kiện click cho nút Lưu
        $modal.find("#btnSave, .modal-footer #btnSave").off("click.digitalsales").on("click.digitalsales", function (e) {
            e.preventDefault();
            $("form#" + formId).submit();
        });
    }
}

function openAddSalesModal() {
    var idModal = "modal_AddDigitalSales";
    var $modal = $("#" + idModal);
    if ($modal.length === 0) {
        var htmlModal = '<div class="modal fade" id="' + idModal + '" data-backdrop="static" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-xl" style="max-width: 1024px;" role="document">' +
            '<div id="modal-content" class="modal-content border-0 shadow-lg radius-2 overflow-hidden"></div>' +
            '</div></div>';
        $("#modalContainer").html(htmlModal);
        $modal = $("#" + idModal);
    }
    if (typeof _onWaiting === "function") _onWaiting();
    $modal.find("#modal-content").load(_digitalSalesUrls.add, function () {
        if (typeof _endWaiting === "function") _endWaiting();
        $modal.modal("show");
    });
}

function openEditSalesModal(id) {
    var idModal = "modal_EditDigitalSales";
    var $modal = $("#" + idModal);
    if ($modal.length === 0) {
        var htmlModal = '<div class="modal fade" id="' + idModal + '" data-backdrop="static" tabindex="-1" role="dialog" aria-hidden="true">' +
            '<div class="modal-dialog modal-xl" style="max-width: 1024px;" role="document">' +
            '<div id="modal-content" class="modal-content border-0 shadow-lg radius-2 overflow-hidden"></div>' +
            '</div></div>';
        $("#modalContainer").html(htmlModal);
        $modal = $("#" + idModal);
    }
    if (typeof _onWaiting === "function") _onWaiting();
    $modal.find("#modal-content").load(_digitalSalesUrls.edit + "/" + id, function () {
        if (typeof _endWaiting === "function") _endWaiting();
        $modal.modal("show");
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
                        executeResponseMessage(res.message, "Chuyển trạng thái thành công!", true);
                        $modal.modal("hide");
                        $(".modal-backdrop").remove();
                        $("body").removeClass("modal-open").css("padding-right", "");
                        reloadSalesTable();
                    } else {
                        executeResponseMessage(res.message, "Không thể chuyển trạng thái!", false);
                    }
                },
                error: function () {
                    executeResponseMessage("Lỗi kết nối máy chủ!", "Lỗi kết nối máy chủ!", false);
                }
            });
        });
    });
}

function deleteSales(id) {
    if (!confirm("Bạn có chắc chắn muốn xóa hồ sơ kinh doanh này không?")) return;
    $.post(_digitalSalesUrls.delete, { id: id }, function (res) {
        if (res.status) {
            executeResponseMessage(res.message, "Xóa thành công!", true);
            reloadSalesTable();
        } else {
            executeResponseMessage(res.message, "Không thể xóa hồ sơ!", false);
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
