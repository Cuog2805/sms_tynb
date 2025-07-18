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
});

let currentPagination = {
    pageNumber: 1,
    pageSize: 10
};

let paginationData = {
    total: 0,
    data: []
};

// trạng thái form
let formState = {
    isEditing: false,
    selectedRowId: null,
    originalData: null
};

function loadData() {
    let searchInput = $('#searchInput').val();
    let pageable = {
        pageNumber: currentPagination.pageNumber,
        pageSize: currentPagination.pageSize,
        sort: ''
    };

    $.ajax({
        url: '/Group/LoadData',
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
                CreatePagination(paginationData.total, currentPagination.pageNumber, currentPagination.pageSize, $("#pagination"));
            }
        },
        error: function (xhr, status, error) {
            alert("Lỗi khi load dữ liệu");
            console.log("XHR:", xhr);
        }
    });
}

function displayItems(items, pageNumber, pageSize) {
    let tableHtml = '';
    const startIndex = (pageNumber - 1) * pageSize;

    if (items && items.length > 0) {
        items.forEach((item, index) => {
            tableHtml += `
                <tr id="row-${item.IdNhom}" onclick="selectRow(${item.IdNhom})" style="cursor: pointer;">
                    <td class="text-center">${startIndex + index + 1}</td>
                    <td>${item.TenNhom}</td>
                    <td>${item.TenNhomCha || ''}</td>
                    <td>${item.TrangThai}</td>
                </tr>
            `;
        });
    } else {
        tableHtml = `
            <tr>
                <td colspan="4" class="text-center text-muted">
                    <i class="fas fa-info-circle"></i>
                    Không có dữ liệu
                </td>
            </tr>
        `;
    }
    $('#groupTableBody').html(tableHtml);
}

function selectRow(rowId) {
    if (formState.isEditing && formState.selectedRowId !== rowId) {
        // Reset form state
        formState.isEditing = false;
        formState.originalData = null;
        disableFormInputs();
    }

    let rows = document.querySelectorAll("#groupTableBody tr");
    rows.forEach(r => r.classList.remove("table-primary"));

    $(`#row-${rowId}`).addClass('table-primary');

    formState.selectedRowId = rowId;

    loadDetail(rowId);
    updateButtonStates();
}

function loadPage(pageNumber) {
    currentPagination.pageNumber = pageNumber;
    loadData();
}

function loadDetail(id) {
    if (id) {
        $.ajax({
            url: '/Group/LoadDetail',
            type: 'GET',
            data: { id: id },
            success: function (response) {
                $('#data-form').html(response);

                disableFormInputs();
                updateButtonStates();
            },
            error: function (xhr) {
                console.log("XHR:", xhr);
            }
        });
    }
}

function addWpNhom(formData) {
    $.ajax({
        url: '/Group/Create',
        type: 'POST',
        data: formData,
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.set('notifier', 'position', 'top-right');
                alertify.success(response.msg || 'Thêm thành công');

                // Reset form state
                formState.isEditing = false;
                formState.selectedRowId = null;
                formState.originalData = null;

                loadData();
                clearForm();
                disableFormInputs();
                updateButtonStates();
            } else {
                alertify.error(response.msg || 'Đã có lỗi xảy ra');
            }
        },
        error: function (xhr, status, error) {
            console.log("XHR:", xhr);
        },
    });
}

function editWpNhom(formData) {
    $.ajax({
        url: '/Group/Update',
        type: 'POST',
        data: formData,
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.set('notifier', 'position', 'top-right');
                alertify.success(response.msg || 'Cập nhật thành công');

                // Reset form state
                formState.isEditing = false;
                formState.originalData = null;

                loadData();
                disableFormInputs();
                updateButtonStates();
            } else {
                alertify.error(response.msg || 'Đã có lỗi xảy ra');
            }
        },
        error: function (xhr, status, error) {
            console.log("XHR:", xhr);
        },
    });
}

// form action
function initFormValidate() {
    $('#groupForm').validate({
        rules: {
            'Data.TenNhom': {
                required: true
            },
            'Data.TrangThai': {
                required: true
            }
        },
        messages: {
            'Data.TenNhom': {
                required: "Vui lòng nhập tên nhóm"
            },
            'Data.TrangThai': {
                required: "Vui lòng chọn trạng thái"
            }
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}

function disableFormInputs() {
    $('#groupForm input, #groupForm select').prop('disabled', true);
}

function enableFormInputs() {
    $('#groupForm input, #groupForm select').prop('disabled', false);
}

function updateButtonStates() {
    const hasSelectedRow = formState.selectedRowId !== null;
    const isEditing = formState.isEditing;

    $('#btnAdd').prop('disabled', false);

    $('#btnEdit').prop('disabled', !hasSelectedRow || isEditing);

    $('#btnAddGroup, #btnAssignGroup').prop('disabled', isEditing);

    if (isEditing) {
        $('#btnSave, #btnCancel').show();
    } else {
        $('#btnSave, #btnCancel').hide();
    }
}

function startEdit() {
    if (!formState.selectedRowId) {
        return;
    }

    formState.originalData = {
        IdNhom: $('#IdNhom').val(),
        TenNhom: $('#TenNhom').val(),
        IdNhomCha: $('#IdNhomCha').val(),
        TrangThai: $('#TrangThai').val()
    };

    formState.isEditing = true;
    enableFormInputs();
    updateButtonStates();

    $('#TenNhom').focus();
}

function cancelEdit() {
    if (formState.originalData) {
        $('#IdNhom').val(formState.originalData.IdNhom);
        $('#TenNhom').val(formState.originalData.TenNhom);
        $('#IdNhomCha').val(formState.originalData.IdNhomCha);
        $('#TrangThai').val(formState.originalData.TrangThai);
    }

    formState.isEditing = false;
    formState.originalData = null;
    disableFormInputs();
    updateButtonStates();

    if ($('#groupForm').data('validator')) {
        $('#groupForm').data('validator').resetForm();
    }
    $('#groupForm').find('.is-invalid').removeClass('is-invalid');
}

function beforeAdd() {
    let rows = document.querySelectorAll("#groupTableBody tr");
    rows.forEach(r => r.classList.remove("table-primary"));

    formState.selectedRowId = null;
    formState.isEditing = true;
    formState.originalData = null;

    clearForm();
    $('#IdNhom').val('');
    enableFormInputs();
    updateButtonStates();

    $('#TenNhom').focus();
}

function clearForm() {
    $('#IdNhom').val('');
    $('#TenNhom').val('');
    $('#IdNhomCha').val('');
    //$('#TrangThai').val('');
}

function submitForm() {
    if (!$('#groupForm').data('validator')) {
        initFormValidate();
    }


    if ($('#groupForm').valid()) {
        const formData = {
            IdNhom: $('#IdNhom').val() || '',
            IdNhomCha: $('#IdNhomCha').val() || null,
            TenNhom: $('#TenNhom').val() || '',
            TrangThai: $('#TrangThai').val() || 1,
        };

        if (formData.IdNhom) {
            editWpNhom(formData);
        } else {
            addWpNhom(formData);
        }
    }
}