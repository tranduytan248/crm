var _detailUrls = {
    editSales: "/Cate/DigitalSales/Edit",
    changeStatusModal: "/Cate/DigitalSales/ChangeStatusModal",
    changeStatus: "/Cate/DigitalSales/ChangeStatus",
    getContactPersons: "/Cate/DigitalSales/GetContactPersons",

    addProductModal: "/Cate/DigitalSales/AddProductModal",
    editProductModal: "/Cate/DigitalSales/EditProductModal",
    saveProduct: "/Cate/DigitalSales/SaveProduct",
    deleteProduct: "/Cate/DigitalSales/DeleteProduct",

    addMemberModal: "/Cate/DigitalSales/AddMemberModal",
    editMemberModal: "/Cate/DigitalSales/EditMemberModal",
    saveMember: "/Cate/DigitalSales/SaveMember",
    deleteMember: "/Cate/DigitalSales/DeleteMember",

    addTrackingModal: "/Cate/DigitalSales/AddTrackingModal",
    editTrackingModal: "/Cate/DigitalSales/EditTrackingModal",
    saveTracking: "/Cate/DigitalSales/SaveTracking",
    updateTrackingStatus: "/Cate/DigitalSales/UpdateTrackingStatus",
    deleteTracking: "/Cate/DigitalSales/DeleteTracking",

    uploadAttachment: "/Cate/DigitalSales/UploadAttachment",
    deleteAttachment: "/Cate/DigitalSales/DeleteAttachment",
    toggleKeyProject: "/Cate/DigitalSales/ToggleKeyProject",
    toggleFollow: "/Cate/DigitalSales/ToggleFollow"
};

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

$(document).ready(function () {
    // Keep active tab on reload if anchor hash exists
    var hash = window.location.hash;
    if (hash) {
        $('.nav-tabs a[href="' + hash + '"]').tab('show');
    }
    $('.nav-tabs a').on('shown.bs.tab', function (e) {
        window.location.hash = e.target.hash;
    });
});

/* ================= 1. Chỉnh sửa thông tin chung ================= */
function openEditSalesModal(id) {
    window.CKEDITOR_BASEPATH = "/Contents/Modules/Major/ckeditor4/";
    var idModal = "modal_EditDigitalSales";
    var htmlModal = '<div class="modal fade" id="' + idModal + '" data-backdrop="static" tabindex="-1" role="dialog" aria-hidden="true">' +
        '<div class="modal-dialog modal-xl" style="max-width: 1024px;" role="document">' +
        '<div id="modal-content" class="modal-content border-0 shadow-lg radius-2 overflow-hidden"></div>' +
        '</div></div>';
    $("#modalContainer").html(htmlModal);
    var $modal = $("#" + idModal);

    function doLoadEdit() {
        if (typeof _onWaiting === "function") _onWaiting();
        $modal.find("#modal-content").load(_detailUrls.editSales + "/" + id, function () {
            if (typeof _endWaiting === "function") _endWaiting();
            $modal.modal("show");
            $modal.off("shown.bs.modal.plugins").on("shown.bs.modal.plugins", function () {
                if (typeof initDigitalSalesFormPlugins === "function") {
                    initDigitalSalesFormPlugins();
                }
            });
        });
    }

    if (typeof CKEDITOR === "undefined") {
        if (typeof _onWaiting === "function") _onWaiting();
        $.getScript("/Contents/Modules/Major/ckeditor4/ckeditor.js", function () {
            if (typeof _endWaiting === "function") _endWaiting();
            doLoadEdit();
        });
    } else {
        doLoadEdit();
    }
}

function DigitalSales_OnProcessSuccess(response, formId) {
    var $modal = $("#modal_" + formId);
    if ($modal.length === 0) {
        $modal = $("#modalContainer .modal.show");
    }
    if ($modal.length === 0) {
        $modal = $(".modal.show");
    }

    var $btnSave = $modal.find("#btnSave, .modal-footer #btnSave, button[type='submit']");
    $btnSave.prop("disabled", false).html('<i class="fa fa-save"></i> Lưu');

    if (response && response.status !== undefined) {
        if (response.status === true) {
            executeResponseMessage(response.message, "Cập nhật thành công!", true);
            $modal.modal("hide");
            $(".modal-backdrop").remove();
            $("body").removeClass("modal-open").css("padding-right", "");
            setTimeout(function () {
                location.reload();
            }, 600);
        } else {
            executeResponseMessage(response.message, "Thao tác thất bại!", false);
        }
    } else {
        var $body = $modal.find("#bodyForm");
        if ($body.length === 0) {
            $body = $("#bodyForm");
        }
        $body.html(response);
        if (typeof initDigitalSalesFormPlugins === "function") {
            initDigitalSalesFormPlugins();
        }
        var $firstError = $body.find(".text-danger:visible").first();
        var warnMsg = ($firstError.length && $firstError.text().trim())
            ? $firstError.text().trim()
            : "Vui lòng kiểm tra và nhập đầy đủ các trường bắt buộc (*)!";
        executeResponseMessage(warnMsg, warnMsg, false);
    }
}

/* ================= 2. Chuyển đổi trạng thái (Gatekeeper) ================= */
function openChangeStatusModal(id) {
    $.get(_detailUrls.changeStatusModal + "/" + id, function (html) {
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
                url: _detailUrls.changeStatus,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        $modal.on("hidden.bs.modal", function () {
                            executeResponseMessage(res.message, "Chuyển trạng thái thành công!", true);
                            setTimeout(function () { location.reload(); }, 600);
                        });
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

/* ================= 3. Sản phẩm / Dịch vụ số (Tab 2) ================= */
function openAddProductModal(salesId) {
    if (typeof _onWaiting === "function") _onWaiting();
    $.get(_detailUrls.addProductModal, { digitalSalesId: salesId })
        .done(function (html) {
            if (typeof _endWaiting === "function") _endWaiting();
            $("#modalContainer").html(html);
            var $modal = $("#modalProduct");
            if ($.fn.select2) {
                $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
            }
            $modal.modal("show");
        })
        .fail(function () {
            if (typeof _endWaiting === "function") _endWaiting();
            executeResponseMessage("Không thể tải form thêm sản phẩm!", "Lỗi tải dữ liệu!", false);
        });
}

function openEditProductModal(id, salesId) {
    if (typeof _onWaiting === "function") _onWaiting();
    $.get(_detailUrls.editProductModal, { id: id, digitalSalesId: salesId })
        .done(function (html) {
            if (typeof _endWaiting === "function") _endWaiting();
            $("#modalContainer").html(html);
            var $modal = $("#modalProduct");
            if ($.fn.select2) {
                $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
            }
            $modal.modal("show");
        })
        .fail(function () {
            if (typeof _endWaiting === "function") _endWaiting();
            executeResponseMessage("Không thể tải form sửa sản phẩm!", "Lỗi tải dữ liệu!", false);
        });
}

function deleteProductItem(id, salesId) {
    var $modal = $('#modalConfirmDeleteProduct');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalConfirmDeleteProduct" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1065;">' +
            '<div class="modal-dialog modal-dialog-centered" style="max-width: 450px;" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-danger text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0"><i class="fa fa-exclamation-triangle mr-1"></i> Xác nhận xóa sản phẩm</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<div class="modal-body p-3 text-center">' +
            '<i class="fa fa-trash-alt fa-3x text-danger mb-3 d-block"></i>' +
            '<p class="text-dark mb-1 font-weight-bold text-105">Bạn có chắc chắn muốn xóa sản phẩm / dịch vụ này không?</p>' +
            '<small class="text-muted text-85">Thao tác này sẽ xóa sản phẩm khỏi hồ sơ và không thể hoàn tác.</small>' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l5 d-flex justify-content-center">' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3" data-dismiss="modal"><i class="fa fa-times mr-1"></i> Hủy bỏ</button>' +
            '<button type="button" id="btnConfirmDeleteProductSubmit" class="btn btn-sm btn-danger radius-1 px-4 font-bold"><i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa</button>' +
            '</div>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalConfirmDeleteProduct');
    }

    $modal.find('#btnConfirmDeleteProductSubmit').off('click').on('click', function () {
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang xóa...');
        $.post(_detailUrls.deleteProduct, { id: id, salesId: salesId }, function (res) {
            $btn.prop('disabled', false).html('<i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa');
            $modal.modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');
            if (res.status) {
                executeResponseMessage(res.message, "Xóa sản phẩm thành công!", true);
                setTimeout(function () { location.reload(); }, 600);
            } else {
                executeResponseMessage(res.message, "Không thể xóa sản phẩm!", false);
            }
        });
    });

    $modal.modal('show');
}

/* ================= 4. Thành viên tham gia (Tab 3) ================= */
function openAddMemberModal(salesId) {
    if (typeof _onWaiting === "function") _onWaiting();
    $.get(_detailUrls.addMemberModal, { digitalSalesId: salesId })
        .done(function (html) {
            if (typeof _endWaiting === "function") _endWaiting();
            $("#modalContainer").html(html);
            var $modal = $("#modalMember");
            if ($.fn.select2) {
                $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
            }
            $modal.modal("show");
        })
        .fail(function (xhr) {
            if (typeof _endWaiting === "function") _endWaiting();
            executeResponseMessage("Không thể tải danh sách nhân sự tham gia. Vui lòng thử lại sau!", "Lỗi tải dữ liệu!", false);
        });
}

function openEditMemberModal(id, salesId) {
    if (typeof _onWaiting === "function") _onWaiting();
    $.get(_detailUrls.editMemberModal, { id: id, digitalSalesId: salesId })
        .done(function (html) {
            if (typeof _endWaiting === "function") _endWaiting();
            $("#modalContainer").html(html);
            var $modal = $("#modalEditMember");
            if ($.fn.select2) {
                $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
            }
            $modal.modal("show");
        })
        .fail(function (xhr) {
            if (typeof _endWaiting === "function") _endWaiting();
            executeResponseMessage("Không thể tải thông tin vai trò thành viên. Vui lòng thử lại sau!", "Lỗi tải dữ liệu!", false);
        });
}

function deleteMemberItem(id, salesId) {
    var $modal = $('#modalConfirmDeleteMember');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalConfirmDeleteMember" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1065;">' +
            '<div class="modal-dialog modal-dialog-centered" style="max-width: 450px;" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-danger text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0"><i class="fa fa-exclamation-triangle mr-1"></i> Xác nhận xóa thành viên</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<div class="modal-body p-3 text-center">' +
            '<i class="fa fa-user-times fa-3x text-danger mb-3 d-block"></i>' +
            '<p class="text-dark mb-1 font-weight-bold text-105">Bạn có chắc chắn muốn xóa thành viên này khỏi hồ sơ không?</p>' +
            '<small class="text-muted text-85">Nhân sự sẽ không còn quyền truy cập hồ sơ này nữa.</small>' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l5 d-flex justify-content-center">' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3" data-dismiss="modal"><i class="fa fa-times mr-1"></i> Hủy bỏ</button>' +
            '<button type="button" id="btnConfirmDeleteMemberSubmit" class="btn btn-sm btn-danger radius-1 px-4 font-bold"><i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa</button>' +
            '</div>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalConfirmDeleteMember');
    }

    $modal.find('#btnConfirmDeleteMemberSubmit').off('click').on('click', function () {
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang xóa...');
        $.post(_detailUrls.deleteMember, { id: id, salesId: salesId }, function (res) {
            $btn.prop('disabled', false).html('<i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa');
            $modal.modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');
            if (res.status) {
                executeResponseMessage(res.message, "Xóa thành viên thành công!", true);
                setTimeout(function () { location.reload(); }, 600);
            } else {
                executeResponseMessage(res.message, "Không thể xóa thành viên!", false);
            }
        });
    });

    $modal.modal('show');
}

/* ================= 5. Tiến trình & Checklist (Tab 4) ================= */
function openAddTrackingModal(salesId) {
    $.get(_detailUrls.addTrackingModal, { digitalSalesId: salesId }, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalTracking");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmTrackingModal").on("submit", function (e) {
            e.preventDefault();
            var formData = new FormData(this);
            $.ajax({
                url: _detailUrls.saveTracking,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        $modal.on("hidden.bs.modal", function () {
                            executeResponseMessage(res.message, "Lưu tiến trình thành công!", true);
                            setTimeout(function () { location.reload(); }, 600);
                        });
                    } else {
                        executeResponseMessage(res.message, "Không thể lưu tiến trình!", false);
                    }
                },
                error: function () {
                    executeResponseMessage("Lỗi kết nối máy chủ!", "Lỗi kết nối máy chủ!", false);
                }
            });
        });
    });
}

function openEditTrackingModal(id, salesId) {
    $.get(_detailUrls.editTrackingModal, { id: id, digitalSalesId: salesId }, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalTracking");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmTrackingModal").on("submit", function (e) {
            e.preventDefault();
            var formData = new FormData(this);
            $.ajax({
                url: _detailUrls.saveTracking,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        $modal.on("hidden.bs.modal", function () {
                            executeResponseMessage(res.message, "Cập nhật tiến trình thành công!", true);
                            setTimeout(function () { location.reload(); }, 600);
                        });
                    } else {
                        executeResponseMessage(res.message, "Không thể cập nhật tiến trình!", false);
                    }
                },
                error: function () {
                    executeResponseMessage("Lỗi kết nối máy chủ!", "Lỗi kết nối máy chủ!", false);
                }
            });
        });
    });
}

function deleteTrackingItem(id, salesId) {
    var $modal = $('#modalConfirmDeleteTracking');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalConfirmDeleteTracking" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1065;">' +
            '<div class="modal-dialog modal-dialog-centered" style="max-width: 450px;" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-danger text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0"><i class="fa fa-exclamation-triangle mr-1"></i> Xác nhận xóa công việc</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<div class="modal-body p-3 text-center">' +
            '<i class="fa fa-tasks fa-3x text-danger mb-3 d-block"></i>' +
            '<p class="text-dark mb-1 font-weight-bold text-105">Bạn có chắc chắn muốn xóa tiến trình / checklist này không?</p>' +
            '<small class="text-muted text-85">Thao tác này không thể hoàn tác.</small>' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l5 d-flex justify-content-center">' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3" data-dismiss="modal"><i class="fa fa-times mr-1"></i> Hủy bỏ</button>' +
            '<button type="button" id="btnConfirmDeleteTrackingSubmit" class="btn btn-sm btn-danger radius-1 px-4 font-bold"><i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa</button>' +
            '</div>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalConfirmDeleteTracking');
    }

    $modal.find('#btnConfirmDeleteTrackingSubmit').off('click').on('click', function () {
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang xóa...');
        $.post(_detailUrls.deleteTracking, { id: id, salesId: salesId }, function (res) {
            $btn.prop('disabled', false).html('<i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa');
            $modal.modal('hide');
            $('.modal-backdrop').remove();
            $('body').removeClass('modal-open').css('padding-right', '');
            if (res.status) {
                executeResponseMessage(res.message, "Xóa tiến trình thành công!", true);
                setTimeout(function () { location.reload(); }, 600);
            } else {
                executeResponseMessage(res.message, "Không thể xóa tiến trình!", false);
            }
        });
    });

    $modal.modal('show');
}

function loadContactPersonsByCustomer(customerId, targetSelector) {
    if (!customerId) {
        $(targetSelector).empty().append('<option value="">-- Chọn người liên hệ --</option>');
        return;
    }
    $.get(_detailUrls.getContactPersons, { customerId: customerId }, function (items) {
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

/* ================= 6. Quản lý tệp đính kèm (Attachments) ================= */
function previewImageDirect(src, title) {
    var $modal = $('#modalImagePreview');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalImagePreview" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1070;">' +
            '<div class="modal-dialog modal-lg modal-dialog-centered" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-dark text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0 text-truncate" id="imgPreviewTitle"><i class="fa fa-image mr-1"></i> Xem ảnh</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<div class="modal-body p-2 text-center bgc-black-tp9 d-flex align-items-center justify-content-center" style="min-height: 300px; max-height: 80vh; overflow: auto;">' +
            '<img id="imgPreviewSource" src="" class="img-fluid radius-1 shadow" style="max-height: 75vh; object-fit: contain;" alt="Xem ảnh" />' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l4 d-flex justify-content-between">' +
            '<span class="text-secondary text-85 font-italic" id="imgPreviewFileName"></span>' +
            '<div>' +
            '<a id="btnDownloadPreviewImage" href="#" class="btn btn-sm btn-primary radius-1 px-3"><i class="fa fa-download mr-1"></i> Tải về</a>' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3 ml-2" data-dismiss="modal">Đóng</button>' +
            '</div>' +
            '</div>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalImagePreview');
    }

    $modal.find('#imgPreviewTitle').html('<i class="fa fa-image mr-1"></i> ' + (title || 'Xem ảnh'));
    $modal.find('#imgPreviewFileName').text(title || '');
    $modal.find('#imgPreviewSource').attr('src', src);
    $modal.find('#btnDownloadPreviewImage').attr('href', '/Cate/DigitalSales/DownloadAttachment?filePath=' + encodeURIComponent(src));
    $modal.modal('show');
}

function openUploadAttachmentModal(salesId) {
    var $modal = $('#modalUploadAttachment');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalUploadAttachment" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1060;">' +
            '<div class="modal-dialog modal-md modal-dialog-centered" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-primary text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0"><i class="fa fa-cloud-upload-alt mr-1"></i> Tải lên tệp đính kèm mới</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<form id="frmUploadAttachment" enctype="multipart/form-data">' +
            '<div class="modal-body p-3">' +
            '<input type="hidden" name="id" id="uploadSalesId" value="" />' +
            '<div class="form-group mb-3">' +
            '<label class="font-bold text-secondary-d2 mb-1 d-block text-90">Chọn tệp tài liệu / hình ảnh:</label>' +
            '<input type="file" name="fileUpload" id="fileUploadInput" multiple="multiple" class="form-control-file border p-2 rounded bgc-grey-l5" required />' +
            '<small class="text-muted text-85 mt-1 d-block"><i class="fa fa-info-circle text-info mr-1"></i>Có thể chọn cùng lúc nhiều tệp (hình ảnh, Word, Excel, PDF, Zip...).</small>' +
            '</div>' +
            '<div id="selectedFilesList" class="p-2 bgc-grey-l4 radius-1 text-85 text-secondary" style="display: none; max-height: 120px; overflow-y: auto;"></div>' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l4">' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3" data-dismiss="modal"><i class="fa fa-times mr-1"></i> Đóng</button>' +
            '<button type="submit" id="btnSubmitUpload" class="btn btn-sm btn-primary radius-1 px-4 font-bold"><i class="fa fa-cloud-upload-alt mr-1"></i> Tải lên ngay</button>' +
            '</div>' +
            '</form>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalUploadAttachment');

        $modal.find('#fileUploadInput').on('change', function () {
            var files = this.files;
            var $list = $modal.find('#selectedFilesList');
            if (files && files.length > 0) {
                var html = '<strong>' + files.length + ' tệp đã chọn:</strong><ul class="mb-0 pl-3 mt-1">';
                for (var i = 0; i < files.length; i++) {
                    html += '<li>' + files[i].name + ' (' + (files[i].size / 1024).toFixed(1) + ' KB)</li>';
                }
                html += '</ul>';
                $list.html(html).show();
            } else {
                $list.empty().hide();
            }
        });

        $modal.find('#frmUploadAttachment').on('submit', function (e) {
            e.preventDefault();
            var $btn = $modal.find('#btnSubmitUpload');
            var formData = new FormData(this);
            $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang tải lên...');

            $.ajax({
                url: _detailUrls.uploadAttachment,
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    $btn.prop('disabled', false).html('<i class="fa fa-cloud-upload-alt mr-1"></i> Tải lên ngay');
                    if (res.status) {
                        $modal.modal('hide');
                        executeResponseMessage(res.message, "Tải lên tệp thành công!", true);
                        setTimeout(function () { location.reload(); }, 600);
                    } else {
                        executeResponseMessage(res.message, "Tải lên tệp thất bại!", false);
                    }
                },
                error: function () {
                    $btn.prop('disabled', false).html('<i class="fa fa-cloud-upload-alt mr-1"></i> Tải lên ngay');
                    executeResponseMessage("Lỗi kết nối máy chủ!", "Lỗi kết nối máy chủ!", false);
                }
            });
        });
    }

    $modal.find('#uploadSalesId').val(salesId);
    $modal.find('#fileUploadInput').val('');
    $modal.find('#selectedFilesList').empty().hide();
    $modal.modal('show');
}

function confirmDeleteAttachment(salesId, filePath, fileName) {
    var $modal = $('#modalConfirmDeleteAttachment');
    if ($modal.length === 0) {
        var modalHtml = '<div class="modal fade" id="modalConfirmDeleteAttachment" tabindex="-1" role="dialog" aria-hidden="true" style="z-index: 1065;">' +
            '<div class="modal-dialog modal-dialog-centered" style="max-width: 450px;" role="document">' +
            '<div class="modal-content border-0 shadow-lg radius-2 overflow-hidden">' +
            '<div class="modal-header bgc-danger text-white py-2 px-3">' +
            '<h6 class="modal-title font-bold text-white mb-0"><i class="fa fa-exclamation-triangle mr-1"></i> Xác nhận xóa tệp</h6>' +
            '<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
            '</div>' +
            '<div class="modal-body p-3 text-center">' +
            '<i class="fa fa-trash-alt fa-3x text-danger mb-3 d-block"></i>' +
            '<p class="text-dark mb-1 font-weight-bold">Bạn có chắc chắn muốn xóa tệp này khỏi hồ sơ không?</p>' +
            '<p class="text-secondary-d2 font-mono text-90 px-2 py-1 bgc-grey-l4 radius-1 text-truncate" id="delAttachmentFileName"></p>' +
            '<small class="text-muted text-85">Thao tác này sẽ gỡ tệp khỏi hồ sơ và không thể hoàn tác.</small>' +
            '</div>' +
            '<div class="modal-footer py-2 bgc-grey-l4 d-flex justify-content-center">' +
            '<button type="button" class="btn btn-sm btn-outline-secondary radius-1 px-3" data-dismiss="modal"><i class="fa fa-times mr-1"></i> Hủy bỏ</button>' +
            '<button type="button" id="btnConfirmDeleteAttachmentSubmit" class="btn btn-sm btn-danger radius-1 px-4 font-bold"><i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa</button>' +
            '</div>' +
            '</div></div></div>';
        $('body').append(modalHtml);
        $modal = $('#modalConfirmDeleteAttachment');
    }

    $modal.find('#delAttachmentFileName').text(fileName || filePath).attr('title', fileName || filePath);
    $modal.find('#btnConfirmDeleteAttachmentSubmit').off('click').on('click', function () {
        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin mr-1"></i> Đang xóa...');
        $.ajax({
            url: _detailUrls.deleteAttachment,
            type: 'POST',
            data: { id: salesId, filePath: filePath },
            success: function (res) {
                $btn.prop('disabled', false).html('<i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa');
                if (res.status) {
                    $modal.modal('hide');
                    executeResponseMessage(res.message, "Xóa tệp đính kèm thành công!", true);
                    setTimeout(function () { location.reload(); }, 600);
                } else {
                    executeResponseMessage(res.message, "Xóa tệp thất bại!", false);
                }
            },
            error: function () {
                $btn.prop('disabled', false).html('<i class="fa fa-trash-alt mr-1"></i> Đồng ý xóa');
                executeResponseMessage("Lỗi kết nối máy chủ!", "Lỗi kết nối máy chủ!", false);
            }
        });
    });

    $modal.modal('show');
}

function toggleKeyProject(salesId, isChecked) {
    var $chk = $('#chkIsKeyProject');
    var $lbl = $('#lblKeyProject');
    $chk.prop('disabled', true);

    $.ajax({
        url: _detailUrls.toggleKeyProject,
        type: "POST",
        data: { id: salesId, isKeyProject: isChecked },
        dataType: "JSON",
        success: function (res) {
            $chk.prop('disabled', false);
            if (res && res.status) {
                if (isChecked) {
                    $lbl.removeClass('text-secondary-d1').addClass('text-orange-d2');
                    $('#badgeKeyProject').removeClass('d-none');
                } else {
                    $lbl.removeClass('text-orange-d2').addClass('text-secondary-d1');
                    $('#badgeKeyProject').addClass('d-none');
                }
                executeResponseMessage(res.message, isChecked ? "Đã đánh dấu là Dự án trọng điểm!" : "Đã bỏ đánh dấu Dự án trọng điểm.", true);
            } else {
                $chk.prop('checked', !isChecked);
                executeResponseMessage(res ? res.message : "Thao tác không thành công!", null, false);
            }
        },
        error: function () {
            $chk.prop('disabled', false);
            $chk.prop('checked', !isChecked);
            executeResponseMessage("Lỗi kết nối máy chủ, vui lòng thử lại!", null, false);
        }
    });
}

function toggleFollowSales(salesId, isChecked) {
    var $chk = $('#chkIsFollowed');
    var $lbl = $('#lblFollowSales');
    $chk.prop('disabled', true);

    $.ajax({
        url: _detailUrls.toggleFollow,
        type: "POST",
        data: { id: salesId, isFollowed: isChecked },
        dataType: "JSON",
        success: function (res) {
            $chk.prop('disabled', false);
            if (res && res.status) {
                if (isChecked) {
                    $lbl.removeClass('text-secondary-d1').addClass('text-danger-d1');
                    $('#badgeFollowed').removeClass('d-none');
                } else {
                    $lbl.removeClass('text-danger-d1').addClass('text-secondary-d1');
                    $('#badgeFollowed').addClass('d-none');
                }
                executeResponseMessage(res.message, isChecked ? "Đã lưu vào danh sách quan tâm!" : "Đã bỏ quan tâm dự án.", true);
            } else {
                $chk.prop('checked', !isChecked);
                executeResponseMessage(res ? res.message : "Thao tác không thành công!", null, false);
            }
        },
        error: function () {
            $chk.prop('disabled', false);
            $chk.prop('checked', !isChecked);
            executeResponseMessage("Lỗi kết nối máy chủ, vui lòng thử lại!", null, false);
        }
    });
}
