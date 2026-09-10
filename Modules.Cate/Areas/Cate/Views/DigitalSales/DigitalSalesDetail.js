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
    saveMember: "/Cate/DigitalSales/SaveMember",
    deleteMember: "/Cate/DigitalSales/DeleteMember",

    addTrackingModal: "/Cate/DigitalSales/AddTrackingModal",
    editTrackingModal: "/Cate/DigitalSales/EditTrackingModal",
    saveTracking: "/Cate/DigitalSales/SaveTracking",
    updateTrackingStatus: "/Cate/DigitalSales/UpdateTrackingStatus",
    deleteTracking: "/Cate/DigitalSales/DeleteTracking"
};

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
    $.get(_detailUrls.editSales + "/" + id, function (html) {
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
                url: _detailUrls.editSales,
                type: "POST",
                data: formData,
                contentType: false,
                processData: false,
                success: function (res) {
                    if (res.status) {
                        $modal.modal("hide");
                        alert(res.message || "Cập nhật thành công!");
                        location.reload();
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
                        alert(res.message || "Chuyển trạng thái thành công!");
                        location.reload();
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

/* ================= 3. Sản phẩm / Dịch vụ số (Tab 2) ================= */
function openAddProductModal(salesId) {
    $.get(_detailUrls.addProductModal, { digitalSalesId: salesId }, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalProduct");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmProductModal").on("submit", function (e) {
            e.preventDefault();
            $.post(_detailUrls.saveProduct, $(this).serialize(), function (res) {
                if (res.status) {
                    $modal.modal("hide");
                    alert(res.message || "Lưu sản phẩm thành công!");
                    location.reload();
                } else {
                    alert(res.message || "Không thể lưu sản phẩm!");
                }
            });
        });
    });
}

function openEditProductModal(id, salesId) {
    $.get(_detailUrls.editProductModal, { id: id, digitalSalesId: salesId }, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalProduct");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmProductModal").on("submit", function (e) {
            e.preventDefault();
            $.post(_detailUrls.saveProduct, $(this).serialize(), function (res) {
                if (res.status) {
                    $modal.modal("hide");
                    alert(res.message || "Cập nhật sản phẩm thành công!");
                    location.reload();
                } else {
                    alert(res.message || "Không thể lưu sản phẩm!");
                }
            });
        });
    });
}

function deleteProductItem(id, salesId) {
    if (!confirm("Bạn có chắc chắn muốn xóa sản phẩm / dịch vụ này không?")) return;
    $.post(_detailUrls.deleteProduct, { id: id }, function (res) {
        if (res.status) {
            alert(res.message || "Xóa sản phẩm thành công!");
            location.reload();
        } else {
            alert(res.message || "Không thể xóa sản phẩm!");
        }
    });
}

/* ================= 4. Thành viên tham gia (Tab 3) ================= */
function openAddMemberModal(salesId) {
    $.get(_detailUrls.addMemberModal, { digitalSalesId: salesId }, function (html) {
        $("#modalContainer").html(html);
        var $modal = $("#modalMember");
        if ($.fn.select2) {
            $modal.find(".select2").select2({ width: "100%", dropdownParent: $modal });
        }
        $modal.modal("show");

        $("#frmMemberModal").on("submit", function (e) {
            e.preventDefault();
            $.post(_detailUrls.saveMember, $(this).serialize(), function (res) {
                if (res.status) {
                    $modal.modal("hide");
                    alert(res.message || "Lưu thành viên thành công!");
                    location.reload();
                } else {
                    alert(res.message || "Không thể lưu thành viên!");
                }
            });
        });
    });
}

function deleteMemberItem(id, salesId) {
    if (!confirm("Bạn có chắc chắn muốn xóa thành viên này khỏi dự án?")) return;
    $.post(_detailUrls.deleteMember, { id: id }, function (res) {
        if (res.status) {
            alert(res.message || "Xóa thành viên thành công!");
            location.reload();
        } else {
            alert(res.message || "Không thể xóa thành viên!");
        }
    });
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
                        alert(res.message || "Lưu tiến trình thành công!");
                        location.reload();
                    } else {
                        alert(res.message || "Không thể lưu tiến trình!");
                    }
                },
                error: function () {
                    alert("Lỗi kết nối máy chủ!");
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
                        alert(res.message || "Cập nhật tiến trình thành công!");
                        location.reload();
                    } else {
                        alert(res.message || "Không thể cập nhật tiến trình!");
                    }
                },
                error: function () {
                    alert("Lỗi kết nối máy chủ!");
                }
            });
        });
    });
}

function deleteTrackingItem(id, salesId) {
    if (!confirm("Bạn có chắc chắn muốn xóa tiến trình / checklist này không?")) return;
    $.post(_detailUrls.deleteTracking, { id: id }, function (res) {
        if (res.status) {
            alert(res.message || "Xóa tiến trình thành công!");
            location.reload();
        } else {
            alert(res.message || "Không thể xóa tiến trình!");
        }
    });
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
