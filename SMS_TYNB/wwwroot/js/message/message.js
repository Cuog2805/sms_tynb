$(document).ready(function () {
    loadData();

    $("#searchInput").on("input", function () {
        loadData();
    })

    $("#btnSendMessage").on("click", function () {
        sendMessage();
    })

    // Custom validator
    $.validator.addMethod("filesize", function (value, element, param) {
        if (element.files.length === 0) return true;
        for (let i = 0; i < element.files.length; i++) {
            if (element.files[i].size > param) {
                return false;
            }
        }
        return true;
    }, function (param, element) {
        return `Kích thước mỗi tệp không được vượt quá ${Math.round(param / 1024 / 1024)}MB`;
    });
});


let allItems = [];
let selectedItems = [];

function loadData() {
    let model = {
        searchInput: $('#searchInput').val(),
        TrangThai: 1
    };

    $.ajax({
        url: '/Group/LoadDataWpNhomCanbos',
        type: 'GET',
        data: $.param({
            'model.searchInput': model.searchInput,
            'model.TrangThai': model.TrangThai
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
                console.log("xhr", xhr);
                alertify.error("Lỗi khi gửi tin nhắn");
            }
        });
    }

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
        // Thêm thông tin số người chọn
        $("#messageAssignTotal").text(`${selectedItems.length}`);
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

    // Xử lý node nhóm
    if (node.id.startsWith("nhom_")) {
        // Mở cây
        treeRef.open_all();
        
        // Đợi cây mở xong rồi mới xử lý
        setTimeout(() => {
            processGroupChildren(node.id, isChecked, treeRef);
            syncSelectedItems();
        }, 300);
    } else {
        syncSelectedItems();
    }
}

function processGroupChildren(nodeId, isChecked, treeRef) {
    // Lấy tất cả node con
    const allChildren = treeRef.get_children_dom(nodeId, true);
    
    allChildren.each(function () {
        const childNode = treeRef.get_node(this);
        
        if (childNode.id.startsWith("canbo_")) {
            if (isChecked) {
                treeRef.check_node(childNode.id);
            } else {
                treeRef.uncheck_node(childNode.id);
            }
        }
    });
}

function syncSelectedItems() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    const checkedNodes = treeRef.get_checked(true);

    selectedItems = [];

    checkedNodes.forEach(node => {
        if (node.id.startsWith("canbo_") && node.data) {
            selectedItems.push(node.data);
        }
    });

    displaySelectedItems();
}

function removeSelectedItem(item) {
    selectedItems = selectedItems.filter(selected => 
        !(selected.IdCanbo === item.IdCanbo && selected.IdNhom === item.IdNhom)
    );

    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        const uniqueId = `canbo_${item.IdCanbo}_nhom_${item.IdNhom}`;
        treeRef.uncheck_node(uniqueId);
    }

    displaySelectedItems();
}

function unselectAll() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        treeRef.uncheck_all();
    }
    selectedItems = [];
    displaySelectedItems();
}

function selectAll() {
    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        // Mở toàn bộ cây trước
        treeRef.open_all();
        
        setTimeout(() => {
            const allNodes = treeRef.get_json('#', { flat: true });
            allNodes.forEach(node => {
                if (node.id.startsWith("canbo_")) {
                    treeRef.check_node(node.id);
                }
            });
            syncSelectedItems();
        }, 300);
    }
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
            'FileDinhKem': {
                extension: "doc|docx|pdf|png|jpg",
                filesize: 5 * 1024 * 1024
            }
        },
        messages: {
            'Data.Noidung': {
                required: "Vui lòng nhập nội dung tin nhắn"
            },
            'FileDinhKem': {
                extension: "Chỉ chấp nhận .doc, .docx, .pdf",
                filesize: "Kích thước mỗi tệp không được vượt quá 5MB"
            }
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}
