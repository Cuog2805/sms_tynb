$(document).ready(function () {
    // Khởi tạo Select2 cho dropdown IdNhom
    $('#IdNhom').select2({
        theme: 'bootstrap-5',
        placeholder: '--Chọn nhóm--',
        allowClear: true,
        width: '100%',
        dropdownParent: $('#IdNhom').parent()
    });

    //Load dữ liệu
    loadData();
    let debounceTimer;
    $("#searchInput").on("input", function () {
        clearTimeout(debounceTimer);

        debounceTimer = setTimeout(function () {
            currentPagination.pageNumber = 1;
            loadData();
        }, 300);
    });

    //phân trang bảng dữ liệu
    const groupCheckBoxTablePageSizeId = $("#groupCheckBoxTablePagination").attr("id");
    $("#pageSize-" + groupCheckBoxTablePageSizeId).on("change", function () {
        currentPagination.pageNumber = 1;
        currentPagination.pageSize = parseInt($(this).val());
        loadData();
    })

    //phân trang bảng selected
    const groupAssignTablePaginationId = $("#groupAssignTablePagination").attr("id");
    $("#pageSize-" + groupAssignTablePaginationId).on("change", function () {
        selectedItemsPagination.pageNumber = 1;
        selectedItemsPagination.pageSize = parseInt($(this).val());
        displaySelectedItems();
    })

    $("#GroupId").on("change", function () {
        var selectedValue = $(this).val();

        if (!selectedValue || selectedValue === "" || selectedValue === null) {
            return;
        }

        loadDetail(selectedValue);
    })
});

let currentPagination = {
    pageNumber: 1,
    pageSize: 10
};

// pagination cho selectedItems
let selectedItemsPagination = {
    pageNumber: 1,
    pageSize: 10
};

let paginationData = {
    total: 0,
    data: []
};

let allItems = [];
let selectedItems = [];

function loadData() {
    let model = {
        searchInput: $('#searchInput').val(),
        IsDeleted: 0
    };
    let pageable = {
        pageNumber: currentPagination.pageNumber,
        pageSize: currentPagination.pageSize,
        sort: 'Name'
    };

    $.ajax({
        url: '/Contact/LoadData',
        type: 'GET',
        data: $.param({
            'model.searchInput': model.searchInput,
            'model.IsDeleted': model.IsDeleted,
            'pageable.PageNumber': pageable.pageNumber,
            'pageable.PageSize': pageable.pageSize,
            'pageable.Sort': pageable.sort
        }),
        success: function (response) {
            if (response.state === "success") {
                paginationData.total = response.content.Total;
                paginationData.data = response.content.Data;

                currentPagination.pageNumber = pageable.pageNumber;
                currentPagination.pageSize = pageable.pageSize;

                displayItems(paginationData.data, currentPagination.pageNumber, currentPagination.pageSize);
                CreatePaginationMinimal(paginationData.total, currentPagination.pageNumber, currentPagination.pageSize, $("#groupCheckBoxTablePagination"), loadPage);
            }
        },
        error: function (xhr) {
            alertify.error('Đã có lỗi xảy ra');
            console.log("XHR:", xhr);
        }
    });
}

function loadPage(pageNumber) {
    currentPagination.pageNumber = pageNumber;
    loadData();
}

function loadSelectedItemsPage(pageNumber) {
    selectedItemsPagination.pageNumber = pageNumber;
    displaySelectedItems();
}

function loadDetail(id) {
    $.ajax({
        url: '/Group/LoadDetailGroupEmployee',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            selectedItems = response.data.Employees;

            // Reset về trang đầu khi load detail mới
            selectedItemsPagination.pageNumber = 1;

            loadData()
            displaySelectedItems();
        },
        error: function (xhr, status, error) {
            alertify.error('Đã có lỗi xảy ra');
            console.log("XHR:", xhr);
        }
    });
}

function submitGroupAssign() {
    const selectedGroupId = $("#GroupId").val();

    const canbos = selectedItems.map(item => ({
        EmployeeId: item.EmployeeId,
        Name: item.Name,
        PhoneNumber: item.PhoneNumber,
        Description: item.Description,
    }));

    const groupViewModel = {
        GroupId: parseInt(selectedGroupId) || 0,
        Employees: canbos
    };

    $.ajax({
        url: '/Group/Assign',
        type: 'POST',
        data: { model: groupViewModel },
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.success(response.msg || 'Phân nhóm thành công');
            } else {
                alertify.error(response.msg || 'Đã có lỗi xảy ra');
            }
        },
        error: function (xhr, status, error) {
            alertify.error('Đã có lỗi xảy ra');
            console.log("XHR:", xhr);
        },
    });
}

function displayItems(items) {
    $("#groupCheckBoxTableBody").empty();
    if (items && items.length > 0) {
        items.forEach((item, index) => {
            const isSelected = selectedItems.some(selected => selected.EmployeeId === item.EmployeeId);
            const row = $("<tr>").css("cursor", "pointer")
                .append(
                    $("<td>").append(
                        $("<input>")
                            .css("cursor", "pointer")
                            .addClass("form-check-input")
                            .attr("type", "checkbox")
                            .attr("id", `check-${item.EmployeeId}`)
                            .prop("checked", isSelected)
                            .data("item", item)
                            .on("change", function (e) {
                                e.stopPropagation();
                                handleItemSelect($(this));
                            })
                            .on("click", function (e) {
                                e.stopPropagation();
                            })
                    ),
                    $("<td>").text(item.Name),
                    $("<td>").text(item.PhoneNumber),
                    $("<td>").text(item.Description)
                ).on("click", function () {
                    const checkbox = $(this).find(".form-check-input");
                    const isChecked = checkbox.prop("checked");
                    checkbox.prop("checked", !isChecked).trigger("change");
                });

            $("#groupCheckBoxTableBody").append(row);
        });
    } else {
        $("#groupCheckBoxTableBody").append($("<div>").append("Không có dữ liệu"));
    }
}

function displaySelectedItems() {
    $("#groupAssignTableBody").empty();

    if (selectedItems && selectedItems.length > 0) {
        const startIndex = (selectedItemsPagination.pageNumber - 1) * selectedItemsPagination.pageSize;
        const endIndex = startIndex + selectedItemsPagination.pageSize;
        const currentPageItems = selectedItems.slice(startIndex, endIndex);

        currentPageItems.forEach((item, index) => {
            const row = $("<tr>").css("cursor", "pointer")
                .append(
                    $("<td>").append(
                        $("<button>")
                            .addClass("btn btn-sm btn-outline-danger me-2")
                            .html("×")
                            .attr("title", "Bỏ chọn")
                            .on("click", function () {
                                removeSelectedItem(item);
                            })
                    ),
                    $("<td>").text(item.Name),
                    $("<td>").text(item.PhoneNumber),
                    $("<td>").text(item.Description)
                );
            $("#groupAssignTableBody").append(row);
        });


        // Thêm thông tin số người chọn
        $("#groupAssignTotal").text(`${selectedItems.length}`);
    } else {
        $("#groupAssignTableBody").append($("<tr>").append($("<td>").attr("colspan", "100%").text("Chưa có ai được chọn")));

        $("#groupAssignTablePagination").empty();
        // Thêm thông tin số người chọn
        $("#groupAssignTotal").text(`${selectedItems.length}`);
    }

    // tạo phân trang cho selected 
    CreatePaginationMinimal(
        selectedItems.length,
        selectedItemsPagination.pageNumber,
        selectedItemsPagination.pageSize,
        $("#groupAssignTablePagination"),
        loadSelectedItemsPage
    );
}

function handleItemSelect(checkbox) {
    const item = checkbox.data("item");
    const isChecked = checkbox.prop("checked");

    if (isChecked) {
        // Thêm vào danh sách đã chọn nếu chưa có
        if (!selectedItems.some(selected => selected.EmployeeId === item.EmployeeId)) {
            selectedItems.push(item);
        }
    } else {
        // Bỏ khỏi danh sách đã chọn
        selectedItems = selectedItems.filter(selected => selected.EmployeeId !== item.EmployeeId);
    }

    // Kiểm tra và điều chỉnh trang hiện tại nếu cần
    const maxPage = Math.ceil(selectedItems.length / selectedItemsPagination.pageSize);
    if (selectedItemsPagination.pageNumber > maxPage && maxPage > 0) {
        selectedItemsPagination.pageNumber = maxPage;
    }

    displaySelectedItems();
}

function handleSelectedItemRemove(checkbox) {
    const item = checkbox.data("item");
    const isChecked = checkbox.prop("checked");

    if (!isChecked) {
        removeSelectedItem(item);
    }
}

function removeSelectedItem(item) {
    // Bỏ khỏi danh sách đã chọn
    selectedItems = selectedItems.filter(selected => selected.IdCanbo !== item.IdCanbo);

    // Cập nhật checkbox bên trái
    $(`#check-${item.IdCanbo}-${item.MaCanbo}`).prop("checked", false);

    const maxPage = Math.ceil(selectedItems.length / selectedItemsPagination.pageSize);
    if (selectedItemsPagination.pageNumber > maxPage && maxPage > 0) {
        selectedItemsPagination.pageNumber = maxPage;
    } else if (selectedItems.length === 0) {
        selectedItemsPagination.pageNumber = 1;
    }

    displaySelectedItems();
}

function selectAll() {
    const visibleItems = [];
    $("#groupCheckBoxTableBody .form-check-input").each(function () {
        const item = $(this).data("item");
        if (item) {
            visibleItems.push(item);
            $(this).prop("checked", true);
        }
    });

    // Thêm vào danh sách đã chọn - tránh trùng
    visibleItems.forEach(item => {
        if (!selectedItems.some(selected => selected.EmployeeId === item.EmployeeId)) {
            selectedItems.push(item);
        }
    });

    displaySelectedItems();
}

function unselectAllSelected() {
    selectedItems = [];
    selectedItemsPagination.pageNumber = 1; // Reset về trang đầu
    $("#groupCheckBoxTableBody .form-check-input").prop("checked", false);
    displaySelectedItems();
}