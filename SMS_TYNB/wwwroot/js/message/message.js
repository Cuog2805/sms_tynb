$(document).ready(function () {
    loadData();

    $("#searchInput").on("input", function () {
        loadData();
    })

    $("#btnSendMessage").on("click", function () {
        sendMessage();
    })
});


let allItems = [];
let selectedItems = [];

function loadData() {
    let searchInput = $('#searchInput').val();
    let pageable = {
        pageNumber: 1,
        pageSize: 999999,
        sort: ''
    };

    $.ajax({
        url: '/Group/LoadDataWpNhomCanbos',
        type: 'GET',
        data: $.param({
            searchInput: searchInput,
            'pageable.PageNumber': pageable.pageNumber,
            'pageable.PageSize': pageable.pageSize,
            'pageable.Sort': pageable.sort
        }),
        success: function (response) {
            if (response.state === "success") {
                allItems = response.data || [];

                const treeData = buildTreeDataFromGroups(allItems);
                initJsTree(treeData);
                displaySelectedItems();
            }
        },
        error: function (xhr, status, error) {
            console.log("XHR:", xhr);
        }
    });
}

function sendMessage() {
    if (!$('#messageForm').data('validator')) {
        initFormValidate();
    }

    if ($('#messageForm').valid()) {
        const formData = new FormData();

        formData.append('Noidung', $("#Noidung").val());
        
        if (selectedItems && selectedItems.length > 0) {
            selectedItems.forEach((item, index) => {
                for (let key in item) {
                    formData.append(`WpCanbos[${index}].${key}`, item[key]);
                }
            });
        }

        // File
        const fileInput = $("#FileDinhKem")[0];
        const files = fileInput.files;
        if (files && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                formData.append("fileDinhKem", files[i]);
            }
        }

        $.ajax({
            url: '/Message/SendMessage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.state === 'success') {
                    alertify.success(res.msg);
                    $("#messageForm")[0].reset();
                    selectedItems = [];
                } else {
                    alertify.error(res.msg);
                }
            },
            error: function (xhr) {
                alertify.error(xhr.responseJSON?.msg || "Lỗi không xác định");
            }
        });
    }

}

function buildTreeDataFromGroups(items) {
    const idToNodeMap = {};
    const roots = [];

    // Tạo node nhóm
    items.forEach(item => {
        idToNodeMap[item.IdNhom] = {
            id: "nhom_" + item.IdNhom,
            text: item.TenNhom,
            children: [],
        };
    });

    // Gắn vào cha hoặc root
    items.forEach(item => {
        const node = idToNodeMap[item.IdNhom];
        if (item.IdNhomCha && idToNodeMap[item.IdNhomCha]) {
            idToNodeMap[item.IdNhomCha].children.push(node);
        } else {
            roots.push(node);
        }

        // Thêm danh sách cán bộ
        if (Array.isArray(item.WpCanbos) && item.WpCanbos.length > 0) {
            item.WpCanbos.forEach(cb => {
                // Kiểm tra cán bộ này từ nhóm này đã được chọn chưa
                const isSelected = selectedItems.some(selected =>
                    selected.IdCanbo === cb.IdCanbo && selected.IdNhom === item.IdNhom
                );

                const uniqueId = `canbo_${cb.IdCanbo}_nhom_${item.IdNhom}`;

                // Thêm IdNhom vào data của cán bộ
                const canboWithGroup = {
                    ...cb,
                    IdNhom: item.IdNhom,
                    TenNhom: item.TenNhom
                };

                node.children.push({
                    id: uniqueId,
                    text: `${cb.TenCanbo} - ${cb.SoDt || ''}`,
                    data: canboWithGroup,
                    state: {
                        selected: isSelected
                    }
                });
            });
        }
    });

    return roots;
}

function handleTreeNodeChange(node, isChecked) {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');

    if (node.id.startsWith("nhom_")) {
        openAllChildren(node.id, treeRef, () => {
            processGroupChildren(node.id, isChecked, treeRef);
            syncSelectedItems();
        });
    } else {
        syncSelectedItems();
    }
}


function processGroupChildren(nodeId, isChecked, treeRef) {
    // Lấy tất cả node con trực tiếp
    const directChildren = treeRef.get_children_dom(nodeId);

    directChildren.each(function () {
        const childNode = treeRef.get_node(this);

        if (childNode.id.startsWith("canbo_")) {
            // Xử lý node cán bộ
            if (isChecked) {
                treeRef.check_node(childNode.id);
            } else {
                treeRef.uncheck_node(childNode.id);
            }
        } else if (childNode.id.startsWith("nhom_")) {
            // Xử lý đệ quy node nhóm con
            processGroupChildren(childNode.id, isChecked, treeRef);
        }
    });
}

function openAllChildren(nodeId, treeRef, callback) {
    treeRef.open_node(nodeId, function () {
        const children = treeRef.get_node(nodeId).children || [];
        let remaining = children.length;

        if (remaining === 0) {
            callback(); // Không có children => gọi luôn
            return;
        }

        // Đệ quy mở từng child và chỉ gọi callback khi tất cả đã mở
        children.forEach(childId => {
            const childNode = treeRef.get_node(childId);
            if (childNode.id.startsWith("nhom_")) {
                openAllChildren(childId, treeRef, () => {
                    remaining--;
                    if (remaining === 0) callback();
                });
            } else {
                remaining--;
                if (remaining === 0) callback();
            }
        });
    });
}


function syncSelectedItems() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    const checkedNodes = treeRef.get_checked(true); // true = trả về node objects

    selectedItems = [];

    checkedNodes.forEach(node => {
        if (node.id.startsWith("canbo_") && node.data) {
            selectedItems.push(node.data);
        }
    });

    displaySelectedItems();
}

// Cải thiện hàm removeSelectedItem để sync đúng với ID unique
function removeSelectedItem(item) {
    // Bỏ khỏi danh sách đã chọn
    selectedItems = selectedItems.filter(selected =>
        !(selected.IdCanbo === item.IdCanbo && selected.IdNhom === item.IdNhom)
    );

    // Cập nhật tree bên trái với ID unique
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        const uniqueId = `canbo_${item.IdCanbo}_nhom_${item.IdNhom}`;
        treeRef.uncheck_node(uniqueId);
    }

    displaySelectedItems();
}

function initJsTree(treeData) {
    if ($.jstree.reference('#messageCheckBoxTree')) {
        $('#messageCheckBoxTree').jstree("destroy");
    }

    $('#messageCheckBoxTree').jstree({
        core: {
            data: treeData,
            themes: {
                name: 'default',
                dots: true,
                icons: false
            }
        },
        checkbox: {
            three_state: false,
            cascade: 'up+down',
            tie_selection: false
        },
        plugins: ['checkbox', 'wholerow'],
    }).on('check_node.jstree uncheck_node.jstree', function (e, data) {
        handleTreeNodeChange(data.node, e.type === 'check_node');
    }).on('ready.jstree', function () {
        const searchInput = $('#searchInput').val();
        // Mở cây
        //if () {

        //}
        if (searchInput && searchInput.trim() !== '') {
            $('#messageCheckBoxTree').jstree('open_all');
        }
    });;
}

function displaySelectedItems() {
    $("#messageAssignList").empty();
    if (selectedItems && selectedItems.length > 0) {
        selectedItems.forEach((item, index) => {
            const row = $("<div>").addClass("checkbox-item selected-item").append(
                $("<div>").addClass("form-check").append(
                    $("<button>")
                        .addClass("btn btn-sm btn-outline-danger me-2")
                        .html("×")
                        .attr("title", "Bỏ chọn")
                        .on("click", function () {
                            removeSelectedItem(item);
                        }),
                    $("<label>")
                        .addClass("form-check-label")
                        .attr("for", `selected-${item.IdCanbo}`)
                        .html(`${item.TenCanbo} ${item.SoDt ? '- ' + item.SoDt : ''} ${item.Mota ? '- ' + item.Mota : ''} - ${item.TenNhom}`)
                )
            );
            $("#messageAssignList").append(row);
        });

        // Thêm thông tin số người chọn
        $("#messageAssignTotal").text(`${selectedItems.length}`);
    } else {
        $("#messageAssignList").append($("<div>").addClass("text-muted").text("Chưa có ai được chọn"));
    }
}

function handleSelectedItemRemove(checkbox) {
    const item = checkbox.data("item");
    const isChecked = checkbox.prop("checked");

    if (!isChecked) {
        removeSelectedItem(item);
    }
}

//function removeSelectedItem(item) {
//    // Bỏ khỏi danh sách đã chọn
//    selectedItems = selectedItems.filter(selected => selected.IdCanbo !== item.IdCanbo);

//    // Cập nhật tree bên trái
//    const treeRef = $.jstree.reference('#messageCheckBoxTree');
//    if (treeRef) {
//        treeRef.uncheck_node(`canbo_${item.IdCanbo}`);
//    }

//    displaySelectedItems();
//}

function selectAll() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        const allNodes = treeRef.get_json('#', { flat: true });
        allNodes.forEach(node => {
            if (node.id.startsWith("canbo_")) {
                treeRef.check_node(node.id);
            }
        });
    }
}

function unselectAll() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        treeRef.uncheck_all();
    }
    displaySelectedItems();
}

function unselectAllSelected() {
    selectedItems = [];
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        treeRef.uncheck_all();
    }
    displaySelectedItems();
}

function searchInTree(searchText) {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        treeRef.search(searchText);
    }
}

//form action
function initFormValidate() {
    $('#messageForm').validate({
        rules: {
            'Data.Noidung': {
                required: true
            },
        },
        messages: {
            'Data.Noidung': {
                required: "Vui lòng nhập nội dung tin nhắn"
            },
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}