$(document).ready(function () {
    loadData();

    $("#searchInput").on("input", function () {
        currentPagination.pageNumber = 1;
        loadData();
    })
    $("#pageSize").on("change", function () {
        currentPagination.pageNumber = 1;
        currentPagination.pageSize = parseInt($(this).val());
        loadData();
    })

    $("#IdNhom").on("change", function () {
        loadDetail($(this).val());
    })
});

let currentPagination = {
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
    let searchInput = $('#searchInput').val();
    let pageable = {
        pageNumber: currentPagination.pageNumber,
        pageSize: currentPagination.pageSize,
        sort: 'TenCanbo'
    };

    $.ajax({
        url: '/Contact/LoadData',
        type: 'GET',
        data: $.param({
            searchInput: searchInput,
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
                CreatePaginationMinimal(paginationData.total, currentPagination.pageNumber, currentPagination.pageSize, $("#pagination"));
            }
        },
        error: function () {
            alert("Lỗi khi load dữ liệu");
            console.log("XHR:", xhr);
        }
    });
}

function loadPage(pageNumber) {
    currentPagination.pageNumber = pageNumber;
    loadData();
}

function loadDetail(id) {
    $.ajax({
        url: '/Group/LoadDetailWpNhomCanbos',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            selectedItems = response.data.WpCanbos;

            loadData()
            displaySelectedItems();
        },
        error: function (xhr, status, error) {
            console.log("XHR:", xhr);
        }
    });
}

function submitGroupAssign() {
    const selectedGroupId = $("#IdNhom").val();

    const wpCanbos = selectedItems.map(item => ({
        IdCanbo: item.IdCanbo,
        TenCanbo: item.TenCanbo,
        SoDt: item.SoDt,
        Mota: item.Mota,
    }));

    // Tạo WpNhomViewModel
    const wpNhomViewModel = {
        IdNhom: parseInt(selectedGroupId) || 0,
        WpCanbos: wpCanbos
    };

    $.ajax({
        url: '/Group/Assign',
        type: 'POST',
        data: { model: wpNhomViewModel },
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.set('notifier', 'position', 'top-right');
                alertify.success(response.msg || 'Phân nhóm thành công');
            } else {
                alertify.error(response.msg || 'Đã có lỗi xảy ra');
            }
        },
        error: function (xhr, status, error) {
            console.log("XHR:", xhr);
        },
    });
}

function displayItems(items) {
    $("#groupCheckBoxList").empty();
    if (items && items.length > 0) {
        items.forEach((item, index) => {
            const isSelected = selectedItems.some(selected => selected.IdCanbo === item.IdCanbo);
            const row = $("<div>").addClass("checkbox-item").append(
                $("<div>").addClass("form-check").append(
                    $("<input>")
                        .addClass("form-check-input")
                        .attr("type", "checkbox")
                        .attr("id", `check-${item.IdCanbo}-${item.MaCanbo}`)
                        .prop("checked", isSelected)
                        .data("item", item)
                        .on("change", function () {
                            handleItemSelect($(this));
                        }),
                    $("<label>")
                        .addClass("form-check-label")
                        .attr("for", `check-${item.IdCanbo}-${item.MaCanbo}`)
                        .html(`${item.TenCanbo} - ${item.SoDt} - ${item.Mota}`)
                )
            );
            $("#groupCheckBoxList").append(row);
        });
    } else {
        $("#groupCheckBoxList").append($("<div>").append("Không có dữ liệu"));
    }
}

function displaySelectedItems() {
    $("#groupAssignList").empty();
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
                        .attr("for", `selected-${item.IdCanbo}-${item.MaCanbo}`)
                        .html(`${item.TenCanbo} - ${item.SoDt} - ${item.Mota}`),
                )
            );
            $("#groupAssignList").append(row);
        });

        // Thêm thông tin số người chọn
        $("#groupAssignTotal").text(`${selectedItems.length}`);
    } else {
        $("#groupAssignList").append($("<div>").addClass("text-muted").text("Chưa có ai được chọn"));

        // Thêm thông tin số người chọn
        $("#groupAssignTotal").text(`${selectedItems.length}`);
    }
}

function handleItemSelect(checkbox) {
    const item = checkbox.data("item");
    const isChecked = checkbox.prop("checked");

    if (isChecked) {
        // Thêm vào danh sách đã chọn nếu chưa có
        if (!selectedItems.some(selected => selected.IdCanbo === item.IdCanbo)) {
            selectedItems.push(item);
        }
    } else {
        // Bỏ khỏi danh sách đã chọn
        selectedItems = selectedItems.filter(selected => selected.IdCanbo !== item.IdCanbo);
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

    displaySelectedItems();
}

function selectAll() {
    const visibleItems = [];
    $("#groupCheckBoxList .form-check-input").each(function () {
        const item = $(this).data("item");
        if (item) {
            visibleItems.push(item);
            $(this).prop("checked", true);
        }
    });

    // Thêm vào danh sách đã chọn - tránh trùng
    visibleItems.forEach(item => {
        if (!selectedItems.some(selected => selected.IdCanbo === item.IdCanbo)) {
            selectedItems.push(item);
        }
    });

    displaySelectedItems();
}

function unselectAllSelected() {
    selectedItems = [];
    $("#groupCheckBoxList .form-check-input").prop("checked", false);
    displaySelectedItems();
}