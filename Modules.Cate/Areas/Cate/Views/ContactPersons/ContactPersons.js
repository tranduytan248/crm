var _tableContactPersons;

function htmlEncode(str) {
    if (str === null || str === undefined) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#39;');
}

$(document).ready(function () {
    initTableContactPersons();
    initExportContactPersons();
});

function getIntOrNull(val) {
    if (val === undefined || val === null || val === "") return null;
    var n = parseInt(val);
    return isNaN(n) ? null : n;
}

function SearchContactPersons() {
    _tableContactPersons.ajax.reload();
}

function initTableContactPersons() {
    _tableContactPersons = $("#DSContactPersons").DataTable({
        responsive: true,
        lengthChange: true,
        processing: true,
        serverSide: true,
        ordering: true,
        order: [[1, "asc"]],
        ajax: {
            url: "/Cate/ContactPersons/Get",
            type: "POST",
            dataType: "JSON",
            data: function (d) {
                d.Keyword = $("#cpKeyword").val();
                d.Gender = getIntOrNull($("#cpGender").val());
                d.Status = getIntOrNull($("#cpStatus").val());
            }
        },
        columns: [
            {
                // Cột STT — tự tính theo vị trí dòng
                data: null,
                orderable: false,
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1;
                }
            },
            {
                data: "CodePerson",
                defaultContent: ""
            },
            {
                data: "FullName",
                render: function (data, type, row) {
                    var html = '<strong class="text-dark">' + htmlEncode(data) + '</strong>';
                    var customerName = row.CustomerName;
                    if (customerName && typeof customerName === 'string' && customerName.trim().length > 0) {
                        var entries = customerName.trim().split(' | ');
                        var innerHtml = '';
                        var maxShow = 3;
                        entries.forEach(function (item, index) {
                            item = item.trim();
                            if (!item) return;
                            var parts = item.split('::');
                            var company = htmlEncode((parts[0] || '').trim());
                            var position = htmlEncode((parts[1] || '').trim());
                            var email = htmlEncode((parts[2] || '').trim());
                            if (!company) return;
                            var isHidden = index >= maxShow ? ' style="display:none;" class="extra-item"' : '';
                            innerHtml += '<div' + isHidden + '>';
                            innerHtml += '<small>';
                            innerHtml += '<span class="text-primary font-weight-bold">' + company + '</span>';
                            if (position) {
                                innerHtml += ' <span class="text-muted">(' + position + ')</span>';
                            }
                            innerHtml += '</small>';
                            if (email) {
                                innerHtml += '<br><small class="text-muted"><i class="fa fa-envelope mr-1"></i>' + email + '</small>';
                            }
                            innerHtml += '</div>';
                        });
                        if (innerHtml) {
                            html += '<div class="mt-1">' + innerHtml;
                            if (entries.length > maxShow) {
                                html += '<div>';
                                html += '<a href="javascript:void(0);" class="text-primary btn-show-more" onclick="';
                                html += 'var p=this.parentNode.parentNode;';
                                html += 'var items=p.getElementsByClassName(\'extra-item\');';
                                html += 'for(var i=0;i<items.length;i++){items[i].style.display=\'block\';}';
                                html += 'this.style.display=\'none\';';
                                html += '">';
                                html += '+ Xem thêm (' + (entries.length - maxShow) + ')';
                                html += '</a>';
                                html += '</div>';
                            }
                            html += '</div>';
                        }
                    }
                    return html;
                }
            },
            { data: "Email", defaultContent: "" },
            { data: "Address", defaultContent: "" },
            {
                data: null,
                render: function (data, type, row) {
                    var html = '';
                    if (row.Phone) html += '<div><i class="fa fa-phone text-success mr-1"></i>' + htmlEncode(row.Phone) + '</div>';
                    if (row.Mobile) html += '<div><i class="fa fa-mobile text-info mr-1"></i>' + htmlEncode(row.Mobile) + '</div>';
                    if (row.Zalo) html += '<div><i class="fa fa-comment text-primary mr-1"></i>' + htmlEncode(row.Zalo) + '</div>';
                    return html || '<span class="text-muted">—</span>';
                }
            },
            {
                data: "Status",
                render: function (data) {
                    return data == 1
                        ? '<span class="badge badge-success">Đang hoạt động</span>'
                        : '<span class="badge badge-secondary">Ngừng hoạt động</span>';
                }
            },
            {
                data: "ContactPerson_ID",
                orderable: false,
                render: function (data, type, row, meta) {
                    var html = '<span class="">';
                    if (type === "display") {
                        html += '<div class="dropdown d-inline-block"><button class="btn btn-lighter-primary mr-1 dropdown-toggle" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><i class="fa fa-ellipsis-h text-120"></i></button><div class="dropdown-menu dropdown-menu-right">';
                        html += _renderButton(true,
                            "EditContactPersons",
                            "btn btn-lighter-primary mr-1 btn-a-outline-primary dropdown-item",
                            "/Cate/ContactPersons/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120 mr-1"></i> Cập nhật',
                            "Cập nhật");
                        html += _renderButton(true,
                            "DeleteContactPersons",
                            "btn btn-lighter-danger mr-1 btn-a-outline-danger dropdown-item",
                            "/Cate/ContactPersons/Delete/" + data,
                            '<i class="far fa-trash-alt text-danger text-120 mr-1"></i> Xoá',
                            "Xoá");
                        html += '</div></div>';
                    }
                    html += "</span>";
                    return html;
                }
            }
        ]
    });
}

function initExportContactPersons() {
    $(document).on("click", "#btnExportContactPersons", function () {
        var url = "/Cate/ContactPersons/Export";
        var params = {
            keyword: $("#cpKeyword").val(),
            gender: getIntOrNull($("#cpGender").val()),
            status: getIntOrNull($("#cpStatus").val())
        };
        window.location = url + "?" + $.param(params);
    });
}

function ContactPersons_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                _tableContactPersons.ajax.reload(null, false);
                eval(response.message);
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
                        _tableContactPersons.ajax.reload(null, false);
                        eval(response.message);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function initContactPersonsImport() {
    var msg = window._cpImportMsg || {};

    // Chọn file
    $(document).on("change", "#cpImportFileInput", function () {
        var label = this.files.length > 0 ? this.files[0].name : "Chọn file...";
        $(this).next(".custom-file-label").text(label);
    });

    // Đọc & kiểm tra file
    $(document).on("click", "#cpBtnReadFile", function () {
        var inputEl = document.getElementById("cpImportFileInput");
        if (!inputEl || !inputEl.files || inputEl.files.length === 0) {
            toastr.warning("Vui lòng chọn file trước khi tiếp tục.");
            return;
        }
        var fd = new FormData();
        fd.append("importFile", inputEl.files[0]);
        var $btn = $(this);
        $("#cpUploadProgress").removeClass("d-none");
        $btn.prop("disabled", true).html('<i class="fa fa-spinner fa-spin"></i> Đang xử lý...');
        $.ajax({
            url: msg.previewUrl || "/Cate/ContactPersons/ImportPreview",
            type: "POST",
            data: fd,
            processData: false,
            contentType: false,
            success: function (res) {
                $("#cpUploadProgress").addClass("d-none");
                $btn.prop("disabled", false).html('<i class="fa fa-search"></i> Đọc & Kiểm tra file');
                if (!res.status) { toastr.error(res.message); return; }
                cpRenderImportPreview(res, msg);
            },
            error: function () {
                $("#cpUploadProgress").addClass("d-none");
                $btn.prop("disabled", false).html('<i class="fa fa-search"></i> Đọc & Kiểm tra file');
                toastr.error("Lỗi đọc file. Vui lòng thử lại.");
            }
        });
    });

    // Quay lại bước upload
    $(document).on("click", "#cpBtnBack", function () {
        $("#cpStepPreview").addClass("d-none");
        $("#cpStepUpload").removeClass("d-none");
        $("#cpFooterPreview").addClass("d-none");
        $("#cpFooterUpload").removeClass("d-none");
        $("#cpImportFileInput").val("").next(".custom-file-label").text("Chọn file...");
        $("#cpBtnConfirm").prop("disabled", false).html('<i class="fa fa-check"></i> Xác nhận nhập');
        $("#cpBtnExportErrorRows").addClass("d-none");
    });

    // Xác nhận nhập
    $(document).on("click", "#cpBtnConfirm", function () {
        var $btn = $(this);
        $btn.prop("disabled", true).html('<i class="fa fa-spinner fa-spin"></i> Đang nhập dữ liệu...');
        $.ajax({
            url: msg.confirmUrl || "/Cate/ContactPersons/ImportConfirm",
            type: "POST",
            dataType: "json",
            dataFilter: function (data) {
                var res = JSON.parse(data);
                delete res.status;
                return JSON.stringify(res);
            },
            success: function (res) {
                $btn.prop("disabled", false).html('<i class="fa fa-check"></i> Xác nhận nhập');

                if (res.successCount > 0) {
                    toastr.success("Nhập thành công " + res.successCount + " người liên hệ!");
                    _tableContactPersons.ajax.reload(null, false);
                }

                if (res.failCount > 0 && res.duplicateRows && res.duplicateRows.length > 0) {
                    var html =
                        '<div class="alert alert-warning py-2 mb-2">' +
                        '<i class="fa fa-exclamation-triangle"></i> ' +
                        '<strong>' + res.failCount + '</strong> dòng bị trùng hoặc thất bại:' +
                        '</div>' +
                        '<table class="table table-sm table-bordered table-hover small">' +
                        '<thead style="background:#fff3cd;">' +
                        '<tr><th>#</th><th>Họ và tên</th><th>Điện thoại</th><th>Lý do</th></tr>' +
                        '</thead><tbody>';

                    $.each(res.duplicateRows, function (i, r) {
                        html += '<tr>' +
                            '<td class="text-center">' + (i + 1) + '</td>' +
                            '<td>' + htmlEncode(r.FullName || r.fullName) + '</td>' +
                            '<td>' + htmlEncode(r.Phone || r.phone) + '</td>' +
                            '<td class="text-danger">' + htmlEncode(r.Reason || r.reason) + '</td>' +
                            '</tr>';
                    });

                    html += '</tbody></table>';
                    $("#cpImportResult").removeClass("d-none").html(html);
                    $("#cpFooterPreview").addClass("d-none");
                    $("#cpFooterDone").removeClass("d-none");

                } else if (res.successCount > 0) {
                    setTimeout(function () {
                        $("#ModalContent .modal:visible").modal("hide");
                    }, 1500);
                }
            },
            error: function () {
                toastr.error("Có lỗi xảy ra trong quá trình nhập dữ liệu. Vui lòng thử lại.");
                $btn.prop("disabled", false).html('<i class="fa fa-check"></i> Xác nhận nhập');
            }
        });
    });

    // Xuất dòng lỗi validation
    $(document).on("click", "#cpBtnExportErrorRows", function () {
        var $btn = $(this);
        var cookieName = "cpExportErrorDone_" + Date.now();
        $btn.prop("disabled", true).html('<i class="fa fa-spinner fa-spin"></i> Đang xuất...');
        var $iframe = $("<iframe>").hide().appendTo("body");
        var exportUrl = (msg.exportErrorUrl || "/Cate/ContactPersons/ExportErrorRows")
            + "?cookieName=" + encodeURIComponent(cookieName);
        $iframe.attr("src", exportUrl);
        var checkTimer = setInterval(function () {
            if (document.cookie.indexOf(cookieName + "=done") !== -1) {
                clearInterval(checkTimer);
                document.cookie = cookieName + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                $iframe.remove();
                $btn.prop("disabled", false).html('<i class="fa fa-download"></i> Xuất dòng lỗi');
            }
        }, 500);
        setTimeout(function () {
            clearInterval(checkTimer);
            $iframe.remove();
            $btn.prop("disabled", false).html('<i class="fa fa-download"></i> Xuất dòng lỗi');
        }, 180000);
    });

    // Xuất dòng trùng
    $(document).on("click", "#cpBtnExportDuplicateRowsDone", function () {
        var $btn = $(this);
        var cookieName = "cpExportDupDone_" + Date.now();
        $btn.prop("disabled", true).html('<i class="fa fa-spinner fa-spin"></i> Đang xuất...');
        var $iframe = $("<iframe>").hide().appendTo("body");
        $iframe.attr("src", "/Cate/ContactPersons/ExportDuplicateRows?cookieName=" + encodeURIComponent(cookieName));
        var checkTimer = setInterval(function () {
            if (document.cookie.indexOf(cookieName + "=done") !== -1) {
                clearInterval(checkTimer);
                document.cookie = cookieName + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                $iframe.remove();
                $btn.prop("disabled", false).html('<i class="fa fa-download"></i> Xuất dòng trùng');
            }
        }, 500);
        setTimeout(function () {
            clearInterval(checkTimer);
            $iframe.remove();
            $btn.prop("disabled", false).html('<i class="fa fa-download"></i> Xuất dòng trùng');
        }, 180000);
    });
}

function cpRenderImportPreview(res, msg) {
    msg = msg || {};
    var s = '<div class="col-md-6"><div class="alert alert-success mb-0 py-2"><i class="fa fa-check-circle"></i> <strong>' + res.totalValid + '</strong> dòng hợp lệ, sẵn sàng nhập</div></div>' +
        '<div class="col-md-6">' + (res.totalError > 0
            ? '<div class="alert alert-danger mb-0 py-2"><i class="fa fa-exclamation-circle"></i> <strong>' + res.totalError + '</strong> dòng có lỗi (sẽ bỏ qua)</div>'
            : '<div class="alert alert-success mb-0 py-2"><i class="fa fa-check"></i> Không có dòng lỗi</div>') + '</div>';
    $("#cpImportSummary").html(s);
    $("#cpCntValid").text(res.totalValid);
    $("#cpCntError").text(res.totalError);

    var vh = "";
    if (res.validRows && res.validRows.length > 0) {
        $.each(res.validRows, function (i, r) {
            vh += "<tr>"
                + "<td class='text-center'>" + parseInt(r.RowNumber || 0) + "</td>"
                + "<td>" + htmlEncode(r.FullName) + "</td>"
                + "<td>" + htmlEncode(r.GenderName) + "</td>"
                + "<td>" + htmlEncode(r.Position) + "</td>"
                + "<td>" + htmlEncode(r.Phone) + "</td>"
                + "<td>" + htmlEncode(r.Email) + "</td>"
                + "<td>" + htmlEncode(r.CustomerShortName) + "</td>"
                + "<td>" + htmlEncode(r.CustomerName) + "</td>"
                + "</tr>";
        });
    } else {
        vh = '<tr><td colspan="8" class="text-center text-muted py-3">Không có dữ liệu hợp lệ</td></tr>';
    }
    $("#cpTbodyValid").html(vh);

    var eh = "";
    if (res.errorRows && res.errorRows.length > 0) {
        $.each(res.errorRows, function (i, r) {
            var errList = r.Errors || r.errors || [];
            var errHtml = "";
            if (Array.isArray(errList) && errList.length > 0) {
                errHtml = '<ul class="mb-0 pl-3">';
                $.each(errList, function (j, e) { errHtml += "<li>" + htmlEncode(e) + "</li>"; });
                errHtml += "</ul>";
            }
            eh += '<tr class="table-danger">'
                + '<td class="text-center">' + parseInt(r.RowNumber || 0) + '</td>'
                + '<td>' + htmlEncode(r.FullName) + '</td>'
                + '<td class="text-danger small">' + errHtml + '</td>'
                + '</tr>';
        });
    } else {
        eh = '<tr><td colspan="3" class="text-center text-muted py-3">Không có dòng lỗi</td></tr>';
    }
    $("#cpTbodyError").html(eh);

    $("#cpStepUpload").addClass("d-none");
    $("#cpStepPreview").removeClass("d-none");
    $("#cpFooterUpload").addClass("d-none");
    $("#cpFooterPreview").removeClass("d-none");

    if (res.totalValid === 0) {
        $("#cpBtnConfirm").prop("disabled", true);
    }
    if (res.totalError > 0) {
        $("#cpBtnExportErrorRows").removeClass("d-none");
        if (res.totalValid === 0) {
            $("#cpImportTabs a[href='#cpTabError']").tab("show");
        }
    } else {
        $("#cpBtnExportErrorRows").addClass("d-none");
    }
}