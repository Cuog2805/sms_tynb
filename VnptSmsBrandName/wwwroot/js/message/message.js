$(document).ready(function () {
    loadData();

    let debounceTimer;
    $("#searchInput").on("input", function () {
        clearTimeout(debounceTimer);

        debounceTimer = setTimeout(function () {
            loadData();
        }, 300);
    });

    $("#btnSendMessage").on("click", function () {
        sendMessage();
    })
});


let allItems = [];
let selectedItems = [];

function loadData() {
    let model = {
        searchInput: $('#searchInput').val(),
        IsDeleted: 0
    };

    $.ajax({
        url: '/Group/LoadDataGroupEmployee',
        type: 'GET',
        data: $.param({
            'model.searchInput': model.searchInput,
            'model.IsDeleted': model.IsDeleted
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
        const messageContent = $("#Content").val();

        formData.append('content', messageContent);

        formData.append('canbos', JSON.stringify(selectedItems));

        // File
        const fileInput = $("#FileDinhKem")[0];
        const files = fileInput.files;
        if (files && files.length > 0) {
            for (let i = 0; i < files.length; i++) {
                formData.append("fileDinhKem", files[i]);
            }
        }

        // selectedFiles từ modal
        if (selectedFiles && selectedFiles.length > 0) {
            selectedFiles.forEach((file, index) => {
                formData.append(`selectedFileIds[${index}]`, file.FileId);
            });
        }

        $.ajax({
            url: '/Message/SendMessage',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.state === 'success') {
                    // Hiển thị modal thông báo thành công
                    showSuccessModal(res.data);

                    $("#messageForm")[0].reset();
                    $("#messageAssignList").empty();
                    $("#selectedFilesList").empty();
                    $("#messageAssignTotal").text('0');
                    const treeRef = $.jstree.reference('#messageCheckBoxTree');
                    if (treeRef) {
                        treeRef.uncheck_all();    
                        treeRef.close_all();       
                    }

                    selectedItems = [];

                    //reset lại input file
                    if (typeof clearSelectedFiles === 'function') {
                        clearSelectedFiles();
                    } else {
                        $('#selectedFilesList').empty();
                        $("#FileDinhKem").val('');
                        if (typeof selectedFiles !== 'undefined') {
                            selectedFiles = [];
                        }
                    }

                    //cập nhật lại hiển thị danh sách
                    displaySelectedItems();
                } else {
                    alertify.error(res.msg);
                }
            },
            error: function (xhr, error) {
                console.log("xhr", xhr);
                console.log("error", error);
                alertify.error("Lỗi khi gửi tin nhắn");
            }
        });
    }

}

function showSuccessModal(data) {
    // nội dung tin nhắn
    $('#smsContent').text(data.ContentSend);
    $('#smsInfo').text(data.CreatedBy + " - " + formatDateTime(new Date(data.CreatedAt)));

    // số lượng người nhận
    $('#smsSucceedNumber').html(data.NumberMessages);
    $('#smsErrorNumber').html(data.ErrorMessages);

    $('#successMessageModal').modal('show');
}

//tree action
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
                        .attr("for", `selected-${item.EmployeeId}`)
                        .html(`${item.Name} ${item.PhoneNumber ? '- ' + item.PhoneNumber : ''} ${item.Description ? '- ' + item.Description : ''} - ${item.GroupName}`)
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
        idToNodeMap[item.GroupId] = {
            id: "nhom_" + item.GroupId,
            text: item.Name,
            children: [],
        };
    });

    // Gắn vào cha hoặc root
    items.forEach(item => {
        const node = idToNodeMap[item.GroupId];
        if (item.GroupParentId && idToNodeMap[item.GroupParentId]) {
            idToNodeMap[item.GroupParentId].children.push(node);
        } else {
            roots.push(node);
        }

        // Thêm danh sách cán bộ
        if (Array.isArray(item.Employees) && item.Employees.length > 0) {
            item.Employees.forEach(cb => {
                // Kiểm tra cán bộ này từ nhóm này đã được chọn chưa
                const isSelected = selectedItems.some(selected =>
                    selected.EmployeeId === cb.EmployeeId && selected.GroupId === item.GroupId
                );

                const uniqueId = `canbo_${cb.EmployeeId}_nhom_${item.GroupId}`;

                // Thêm GroupId vào data của cán bộ
                const canboWithGroup = {
                    ...cb,
                    GroupId: item.GroupId
                };

                node.children.push({
                    id: uniqueId,
                    text: `${cb.Name} - ${cb.PhoneNumber || ''}. Mô tả: ${cb.Description || '___'}`,
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
        openChildrenNode(treeRef, node.id);
        
        // Đợi cây mở xong rồi mới xử lý
        setTimeout(() => {
            processGroupChildren(node.id, isChecked, treeRef);
            syncSelectedItems();
        }, 300);
    } else {
        syncSelectedItems();
    }
}

function openChildrenNode(treeRef, nodeId) {
    treeRef.open_node(nodeId);

    // lấy node con 
    const children = treeRef.get_children_dom(nodeId);

    // mở node con 
    children.each(function () {
        const childId = $(this).attr('id');
        if (childId) {
            treeRef.open_node(childId);
        }
    });
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
    if (!treeRef) return;

    const checkedNodes = treeRef.get_checked(true);

    selectedItems = [];

    for (const node of checkedNodes) {
        if (node.id.startsWith("canbo_") && node.data) {
            selectedItems.push(node.data);
        }
    }

    displaySelectedItems();
}

function removeSelectedItem(item) {
    selectedItems = selectedItems.filter(selected => 
        !(selected.EmployeeId === item.EmployeeId && selected.GroupId === item.GroupId)
    );

    const treeRef = $.jstree.reference('#messageCheckBoxTree');
    if (treeRef) {
        const uniqueId = `canbo_${item.EmployeeId}_nhom_${item.GroupId}`;
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
        }, 0);
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
            'Data.Content': {
                required: true
            },
            'FileDinhKem': {
                extension: "doc|docx|pdf|png|jpg",
                filesize: 5 * 1024 * 1024
            }
        },
        messages: {
            'Data.Content': {
                required: "Vui lòng nhập nội dung tin nhắn"
            },
            'FileDinhKem': {
                extension: "Chỉ chấp nhận .doc, .docx, .pdf, .png, .jpg",
                filesize: "Kích thước mỗi tệp không được vượt quá 5MB"
            }
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}
