var _GroupCategoryActionURLs = {
    GroupCategory_GetData: "/Cate/GroupCategory/Get"
};
var _tableGroupCategory;
$(document).ready(function () {
    initTableGroupCategory();
});

function initTableGroupCategory() {
    _tableGroupCategory = $("#DSGroupCategory").DataTable({
        "Responsive": true,
        "GroupCategory": {
            "processing":
                "<div class='overlay'><i class='fas fa-cog fa-spin'></i></div>"
        },
        "lengthChange": true,
        "processing": true,
        "serverSide": true,
        "ajax":
        {
            "url": _GroupCategoryActionURLs.GroupCategory_GetData,
            "type": "POST",
            "dataType": "JSON",
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
                "data": "Group_Category_Code",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "Group_Category_Name",
                "defaultContent": "",
                "render": function (data, type, row, meta) {
                    return data;
                }
            },
            {
                "data": "Group_Category_ID",
                "style": "width:100px;",
                "orderable": false,
                "render": function (data, type, row, meta) {
                    var html = '<span class="">';

                    if (type === "display") {
                        html += _renderButton(true,
                            "ListCate",
                            "btn btn-lighter-green m-1",
                            "/Cate/Category/List?GroupId=" + data,
                            '<i class="fas fa-list-ol text-green text-120"></i>',
                            "Danh sách danh mục", 1024);
                            
                        html += _renderButton(true,
                            "EditGroupCategory",
                            "btn btn-lighter-primary m-1",
                            "/Cate/GroupCategory/Edit/" + data,
                            '<i class="far fa-edit text-primary text-120"></i>',
                            "Cập nhật");

                        html += _renderButton(true,
                            "DeleteGroupCategory",
                            "btn btn-lighter-danger m-1",
                            "/Cate/GroupCategory/Delete/" + data,
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


function GroupCategory_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                _tableGroupCategory.ajax.reload(null, false);
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
                        _tableGroupCategory.ajax.reload(null, false);
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}
