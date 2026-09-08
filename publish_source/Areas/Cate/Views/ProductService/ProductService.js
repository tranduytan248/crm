var _ProductServiceActionURLs = {
    ProductService_GetData: "/Cate/ProductService/Get"
};
var _productServiceTreeData = [];
var _productServiceCollapseState = {};

$(document).ready(function () {
    ProductService_LoadData();
});

function Search() {
    ProductService_LoadData();
}

function ProductService_NormalizeTreeData(data) {
    var rows = Array.isArray(data) ? data.slice() : [];

    rows.sort(function (left, right) {
        var leftSortPath = left.SortPath || "";
        var rightSortPath = right.SortPath || "";

        if (leftSortPath < rightSortPath) {
            return -1;
        }

        if (leftSortPath > rightSortPath) {
            return 1;
        }

        return 0;
    });

    return $.map(rows, function (item) {
        var isGroup = item.NodeType === "G";
        var nodeId = isGroup
            ? "G_" + (item.GroupServiceID || item.gID || 0)
            : "P_" + (item.ProductServiceID || item.pID || 0);
        var parentNodeId = null;

        if (isGroup) {
            if (parseInt(item.ParentGroupServiceID || 0, 10) > 0) {
                parentNodeId = "G_" + item.ParentGroupServiceID;
            }
        } else if (parseInt(item.ParentProductID || 0, 10) > 0) {
            parentNodeId = "P_" + item.ParentProductID;
        } else if (parseInt(item.GroupServiceID || 0, 10) > 0) {
            parentNodeId = "G_" + item.GroupServiceID;
        }

        item.NodeID = nodeId;
        item.ParentNodeID = parentNodeId;
        item.DisplayTreeName = item.NameProduct || item.DisplayName || "";

        return item;
    });
}

function ProductService_RenderActions(item) {
    var html = "<span>";

    if (!item || item.NodeType !== "P" || !item.ProductServiceID) {
        html += "</span>";
        return html;
    }

    html += '<div class="dropdown d-inline-block">';
    html += '<button class="btn btn-lighter-primary mr-1 dropdown-toggle" type="button"'
        + ' data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'
        + '<i class="fa fa-ellipsis-h text-120"></i></button>';
    html += '<div class="dropdown-menu dropdown-menu-right">';

    html += _renderButton(true,
        "ViewProductService",
        "btn btn-lighter-info mr-1 btn-a-outline-info dropdown-item",
        "/Cate/ProductService/View/" + item.ProductServiceID,
        '<i class="fas fa-eye text-info text-120 mr-1"></i> Xem chi tiết',
        "Xem chi tiết", 1024);

    html += _renderButton(true,
        "List",
        "btn btn-lighter-green mr-1 btn-a-outline-green dropdown-item",
        "/Cate/ProductServiceGroupFilePath/List?ProductServiceID=" + item.ProductServiceID,
        '<i class="fas fa-paperclip text-green text-120 mr-1"></i> Tài liệu đính kèm',
        "Tài liệu đính kèm", 1024);

    html += _renderButton(true,
        "EditProductService",
        "btn btn-lighter-primary mr-1 btn-a-outline-primary dropdown-item",
        "/Cate/ProductService/Edit/" + item.ProductServiceID,
        '<i class="far fa-edit text-primary text-120 mr-1"></i> Cập nhật',
        "Cập nhật", 1024);

    html += _renderButton(true,
        "DeleteProductService",
        "btn btn-lighter-danger mr-1 btn-a-outline-danger dropdown-item",
        "/Cate/ProductService/Delete/" + item.ProductServiceID,
        '<i class="far fa-trash-alt text-danger text-120 mr-1"></i> Xoá',
        "Xoá");

    html += '</div></div>';
    html += "</span>";
    return html;
}

function ProductService_HasChildren(nodeId) {
    for (var index = 0; index < _productServiceTreeData.length; index++) {
        if ((_productServiceTreeData[index].ParentNodeID || "") === nodeId) {
            return true;
        }
    }

    return false;
}

function ProductService_IsHidden(item) {
    var parentNodeId = item.ParentNodeID;

    while (parentNodeId) {
        if (_productServiceCollapseState[parentNodeId] === true) {
            return true;
        }

        var parentItem = null;

        for (var index = 0; index < _productServiceTreeData.length; index++) {
            if (_productServiceTreeData[index].NodeID === parentNodeId) {
                parentItem = _productServiceTreeData[index];
                break;
            }
        }

        parentNodeId = parentItem ? parentItem.ParentNodeID : null;
    }

    return false;
}

function ProductService_RenderTree() {
    var html = "";

    for (var index = 0; index < _productServiceTreeData.length; index++) {
        var item = _productServiceTreeData[index];
        var level = parseInt(item.Level || 0, 10);
        var hasChildren = ProductService_HasChildren(item.NodeID);
        var isCollapsed = _productServiceCollapseState[item.NodeID] === true;
        var isHidden = ProductService_IsHidden(item);
        var indentHtml = "<span class='d-inline-block' style='width:" + (level * 10) + "px;'></span>";
        var toggleHtml = "<span class='d-inline-block mr-1' style='width:8px;'></span>";
        var nameHtml = $("<div/>").text(item.DisplayTreeName || "").html();
        var rowClass = item.NodeType === "G" ? "product-service-group-row" : "product-service-item-row";

        if (hasChildren) {
            toggleHtml =
                "<a href='javascript:void(0)' class='product-service-toggle mr-1' data-node-id='" + item.NodeID + "'>"
                + "<span class='product-service-toggle-icon " + (isCollapsed ? "collapsed" : "expanded") + "'></span>"
                + "</a>";
        }

        html += "<tr class='" + rowClass + "'" + (isHidden ? " style='display:none;'" : "") + ">";
        html += "<td class='align-middle product-service-name-cell'>";
        html += indentHtml + toggleHtml;

        if (item.NodeType === "G") {
            html += "<strong>" + nameHtml + "</strong>";
        } else {
            html += nameHtml;
        }

        html += "</td>";
        html += "<td class='align-middle'>" + (item.NodeType === "P" ? (item.CodeProduct || "") : "") + "</td>";
        html += "<td class='align-middle'>" + (item.NodeType === "P" ? (item.ShortNameProduct || "") : "") + "</td>";
        html += "<td class='align-middle text-center'>";

        if (item.NodeType === "P" && item.IsActived) {
            html += '<i class="fa fa-check text-green fa-2x"></i>';
        }

        html += "</td>";
        html += "<td class='align-middle'>" + ProductService_RenderActions(item) + "</td>";
        html += "</tr>";
    }

    $("#DSProductService tbody").html(html);
}

function ProductService_LoadData() {
    $.ajax({
        url: _ProductServiceActionURLs.ProductService_GetData,
        type: "POST",
        dataType: "JSON",
        data: {
            Search: function () { return $('#Keyword').val(); },
            SearchFile: function () { return $('#SearchFile').val(); }
        },
        success: function (response) {
            _productServiceTreeData = ProductService_NormalizeTreeData(response && response.data ? response.data : []);
            ProductService_RenderTree();
        }
    });
}

$(document).off("click", ".product-service-toggle").on("click", ".product-service-toggle", function (e) {
    e.preventDefault();

    var nodeId = $(this).data("node-id");
    _productServiceCollapseState[nodeId] = _productServiceCollapseState[nodeId] !== true;
    ProductService_RenderTree();
});

function ProductService_OnProcessSuccess(response, formId) {
    if (response.status != undefined) {
        if ($("#ModalContent #modal_" + formId + " #chkNotDismissModal").is(":checked")) {
            if (response.status != undefined) {
                eval(response.message);
                ProductService_LoadData();
                response.status = undefined;
                var urlAction = $("#ModalContent #modal_" + formId + " form").attr("action");
                $("#ModalContent #modal_" + formId + " #modal-content").load(urlAction, function () {
                    _initElement();
                });
            }
        } else {
            $("#ModalContent #modal_" + formId).modal("hide");
            $("#ModalContent #modal_" + formId).on("hidden.bs.modal",
                function () {
                    if (response.status != undefined) {
                        eval(response.message);
                        ProductService_LoadData();
                        response.status = undefined;
                    }
                });
        }
    } else {
        $("#ModalContent #modal_" + formId + " #bodyForm").html(response);
    }
}

function ProductService_InitParentSelector() {
    var $selector = $("#ProductService_HierarchySelection");

    if ($selector.length === 0) {
        return;
    }

    var $groupInput = $("#ProductService_GroupServiceID");
    var $parentInput = $("#ProductService_ParentProductID");

    function syncHiddenFields() {
        var selectedValue = $selector.val();
        var $selectedOption = $selector.find("option:selected");

        if (!selectedValue) {
            $groupInput.val("");
            $parentInput.val("0");
            return;
        }

        var nodeType = $selectedOption.data("node-type");
        var groupId = $selectedOption.data("group-id");
        var parentProductId = $selectedOption.data("parent-product-id");

        $groupInput.val(groupId || "");

        if (nodeType === "P") {
            $parentInput.val(parentProductId || "0");
        } else {
            $parentInput.val("0");
        }
    }

    var selectedValue = "";

    if (parseInt($parentInput.val() || "0", 10) > 0) {
        selectedValue = "P_" + $parentInput.val();
    } else if (parseInt($groupInput.val() || "0", 10) > 0) {
        selectedValue = "G_" + $groupInput.val();
    }

    if (selectedValue) {
        $selector.val(selectedValue);
    }

    syncHiddenFields();

    $selector.off("change").on("change", function () {
        syncHiddenFields();
    });
}
