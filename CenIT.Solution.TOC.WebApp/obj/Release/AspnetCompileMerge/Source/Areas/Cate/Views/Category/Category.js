var _CategoryActionURLs = {
    Category_GetData: "/Cate/Category/Get"
};
var _tableCategory;
$(document).ready(function () {
    initTableCategory();
});

function initTableCategory() {
    _tableCategory = $("#DSCategory").DataTable({
        "Responsive": true,
        "Category": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _CategoryActionURLs.Category_GetData,
            "type": "POST",
            "dataType": "JSON",
            "data": {
                "Group_Category_ID": function () { return $("#modal_ListCate #Group_Category_ID").val(); },
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
                "data": "CodeCategory",
                "defaultContent": "",
            },
            {
                "data": "NameCategory",
                "defaultContent": "",
            },
            {
                "data": "OrderID",
                "defaultContent": "",
            },
            {
                "data": "Cate_CategoryID",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "EditCategory",
                            "btn btn-lighter-primary m-1",
                            "/Cate/Category/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i>',
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteCategory",
                            "btn btn-lighter-danger m-1",
                            "/Cate/Category/Delete/" + data,
                            '<i class="far fa-trash-alt text-danger text-120"></i>',
                            "Xoá");
                    }
                    html += "</span>";

                    return html;
                }
            }
        ]
    });
}


function Category_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableCategory.ajax.reload(null, false);
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
                        eval(response.message);
                        _tableCategory.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
