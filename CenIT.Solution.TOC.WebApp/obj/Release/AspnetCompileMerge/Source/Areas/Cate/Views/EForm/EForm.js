Dropzone.autoDiscover = false;
var myDropzone = new Dropzone("#dz-area", {
    url: "#",// <--- thêm cái này
    previewTemplate: $("#preview-template").html(),
    autoProcessQueue: false,
    filesizeBase: 1000, // 1000 byte = 1KB (có thể sửa thành 1024)
    addRemoveLinks: true,

    maxFiles: EFORM_UPLOAD_MAXFILES,
    maxFilesize: EFORM_UPLOAD_MAXFILESIZE,
    acceptedFiles: EFORM_UPLOAD_ACCEPTEDFILES,

    // -- Các chuỗi tiếng Việt --
    dictDefaultMessage: "Kéo thả ảnh vào đây hoặc nhấn để chọn",
    dictFallbackMessage: "Trình duyệt của bạn không hỗ trợ kéo thả file.",
    dictFileTooBig: "Tệp quá lớn ({{filesize}}MiB). Kích thước tối đa: {{maxFilesize}}MiB.",
    dictInvalidFileType: "Loại tệp không hợp lệ. Vui lòng chọn ảnh.",
    dictResponseError: "Lỗi từ máy chủ: Mã {{statusCode}}.",
    dictCancelUpload: "Hủy",
    dictCancelUploadConfirmation: "Bạn có chắc muốn hủy upload?",
    dictRemoveFile: `<span class="w-100 text-danger text-90 mt-1 border-0 btn btn-lighter-danger btn-sm"><i class="fas fa-trash-alt px-1"></i> Xóa</span>`,
    dictMaxFilesExceeded: "Bạn không thể upload thêm tệp.",
    thumbnail: function (file, dataUrl) {
        if (file.previewElement) {
            $(file.previewElement).removeClass("dz-file-preview");
            $(file.previewElement).find("[data-dz-thumbnail]").each(function () {
                var thumbnailElement = this;
                thumbnailElement.alt = file.name;
                thumbnailElement.src = dataUrl;
            });

            setTimeout(function () { $(file.previewElement).addClass("dz-image-preview") }, 1);
        }
    }
}); // new Dropzone

// Khi file thêm vào → đưa sang input file
let errorFiles = [];

myDropzone.on("addedfile", syncFiles);

myDropzone.on("removedfile", function (file) {
    // xóa khỏi danh sách lỗi nếu có
    errorFiles = errorFiles.filter(x => x.file !== file);
    syncFiles();
});

myDropzone.on("error", function (file, message) {
    file.isRejected = true;
    errorFiles.push({ file, error: message });
    syncFiles(); // đồng bộ lại input
});

function syncFiles() {
    let dt = new DataTransfer();
    let input = document.getElementById(fileAttr);

    myDropzone.files.forEach(f => {
        if (f instanceof File && !f.isRejected) {
            dt.items.add(f);
        }
    });

    input.files = dt.files;
}

/** Sửa lỗi bàn phím che ô input trên điện thoại Thu Đạt*/
let focusedInput = null;

document.addEventListener('focusin', function (e) {
    if (e.target.matches('input, textarea, select')) {
        focusedInput = e.target;
        $('#keyboardSpacer').removeClass('d-none');
    }
});
document.addEventListener('focusout', function (e) {
    if (e.target.matches('input, textarea, select')) {
        focusedInput = null;
        $('#keyboardSpacer').addClass('d-none');
    }
});
window.addEventListener('scroll', function () {
    if (!focusedInput) return;

    const rect = focusedInput.getBoundingClientRect();
    const topSafe = 20; // khoảng cách an toàn với mép trên

    // Nếu input bị kéo vượt lên trên
    if (rect.top < topSafe) {
        window.scrollBy({
            top: rect.top - topSafe,
            behavior: 'instant' // không animate để tránh giật
        });
    }
}, { passive: true });
/** ------------------------------------------------*/

$("#btnSendRequest").click(function (e) {
    $('#dz-alert').html('');
    if (errorFiles.length > 0) {
        var alert = `<div role="alert" class="alert alert-warning bgc-warning-l4 brc-warning-m3 border-2 d-flex align-items-center">
                <i class="fas fa-exclamation-circle mr-3 fa-2x text-orange"></i>
                <div class="text-dark-tp2">Phát hiện ${errorFiles.length} không hợp lệ. Vui lòng kiểm tra lại!</div>
                <button type="button" class="close align-self-start ml-auto text-danger-d2 text-150" data-dismiss="alert" aria-label="Close">
                    <span aria-hidden="true">×</span>
                </button>
            </div>`;
        $('#dz-alert').html(alert);
        e.preventDefault();
        return;
    }
    $('form#SendRequest').submit();
});

$('form#SendRequest').ajaxForm({
    beforeSubmit: function (arr, $form, options) {
        $('form#SendRequest input, form#SendRequest textarea, form#SendRequest button').attr("disabled", true);
        return true;
    },
    success: function (response) {
        if (response.status != undefined) {
            if (response.returnUrl != undefined) {
                window.location = window.location.origin + response.returnUrl;
            }
        } else {
            $('form#SendRequest input, form#SendRequest textarea, form#SendRequest button').removeAttr("disabled");
            $("form#SendRequest #bodyForm").html(response);
        }
    }
});


function SetHeightTextarea(listIdTextArea) {
    for (var i = 0; i < listIdTextArea.length; i++) {
        document.querySelector('#' + listIdTextArea[i]).style.height = document.querySelector('#' + listIdTextArea[i]).scrollHeight + 'px';
        document.querySelector('#' + listIdTextArea[i]).addEventListener('input', function () {
            this.style.height = 'auto'; // Đặt chiều cao về 'auto' để tính toán chiều cao mới
            this.style.height = `${this.scrollHeight}px`; // Đặt chiều cao mới dựa trên scrollHeight
        });
    }
}