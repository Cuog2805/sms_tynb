$(document).ready(function () {
    loadData();

    let debounceTimer;
    $("#searchInput").on("input", function () {
        clearTimeout(debounceTimer);

        debounceTimer = setTimeout(function () {
            currentPagination.pageNumber = 1;
            loadData();
        }, 300);
    });
    $("#pageSize").on("change", function () {
        currentPagination.pageNumber = 1;
        currentPagination.pageSize = parseInt($(this).val());
        loadData();
    })

    // Modal
	$('#data-form').on('show.bs.modal', function (e) {
		initFormValidate();
	});

	$('#data-form').on('hidden.bs.modal', function (e) {
		// Reset form khi modal đóng
		clearForm();
		formState.isEditing = false;
		formState.currentEditId = null;
	});

	// Form submit
	$(document).on("click", "#btnSave", function () {
		submitForm();
	});
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
    currentEditId: null
};

function loadData() {
    let model = {
        searchInput: $('#searchInput').val(),
    };
    let pageable = {
        pageNumber: currentPagination.pageNumber,
        pageSize: currentPagination.pageSize,
        sort: ''
    };

    $.ajax({
        url: '/Group/LoadData',
        type: 'GET',
        data: $.param({
            'model.searchInput': model.searchInput,
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
            alertify.error('Đã có lỗi xảy ra');
            console.log("XHR:", xhr);
        }
    });
}

function loadPage(pageNumber) {
    currentPagination.pageNumber = pageNumber;
    loadData();
}

function displayItems(items, pageNumber, pageSize) {
    let tableHtml = '';
    const startIndex = (pageNumber - 1) * pageSize;

    if (items && items.length > 0) {
        items.forEach((item, index) => {
            tableHtml += `
                <tr>
                    <td class="text-center">${startIndex + index + 1}</td>
                    <td>${item.Name}</td>
                    <td>${item.ParentName || ''}</td>
                    <td>${item.IsDeleted}</td>
                    <td class="text-center">
                        <button
                            class="btn btn-sm btn-primary"
                            type="button"
                            data-bs-toggle="modal"
                            data-bs-target="#data-form"
                            onclick="startEdit(${item.IdGroup})"
                            title="Sửa">
                            <i class="bi bi-pencil-square"></i>
                        </button>
                    </td>
                </tr>
            `;
        });
    } else {
        tableHtml = `
            <tr>
                <td colspan="100%" class="text-center text-muted">
                    <i class="fas fa-info-circle"></i>
                    Không có dữ liệu
                </td>
            </tr>
        `;
    }
    $('#groupTableBody').html(tableHtml);
}

function loadDetail(id) {
    if (id) {
        $.ajax({
            url: '/Group/LoadDetail',
            type: 'GET',
            data: { id: id },
            success: function (response) {
                $('#data-form').html(response);
                formState.isEditing = true;
                formState.currentEditId = id;
                $("#data-form-header").html("Cập nhật nhóm");
            },
            error: function (xhr) {
                console.log("XHR:", xhr);
            }
        });
    }
}

function addGroup(formData) {
    $.ajax({
        url: '/Group/Create',
        type: 'POST',
        data: formData,
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.success(response.msg || 'Thêm thành công');

                // Reset form state
                formState.isEditing = false;
                formState.currentEditId = null;

                loadData();
                $('#data-form').modal('hide');
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

function editGroup(formData) {
    $.ajax({
        url: '/Group/Update',
        type: 'POST',
        data: formData,
        dataType: "json",
        success: function (response) {
            if (response.state === 'success') {
                alertify.success(response.msg || 'Cập nhật thành công');

                // Reset form state
                formState.isEditing = false;
                formState.currentEditId = null;

                loadData();
                $('#data-form').modal('hide');
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

// form action
function initFormValidate() {
    $('#groupForm').validate({
        rules: {
            'Data.Name': {
                required: true
            },
            'Data.IsDeleted': {
                required: true
            }
        },
        messages: {
            'Data.Name': {
                required: "Vui lòng nhập tên nhóm"
            },
            'Data.IsDeleted': {
                required: "Vui lòng chọn trạng thái"
            }
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}

function startEdit(id) {
    formState.isEditing = true;
    formState.currentEditId = id;
    loadDetail(id);
}

function beforeAdd() {
    formState.isEditing = false;
    formState.currentEditId = null;
    clearForm();
    $("#data-form-header").html("Thêm nhóm");

    $('#TenNhom').focus();
}

function clearForm() {
    $('#IdGroup').val('');
    $('#IdOrganization').val('');
    $('#CreateBy').val('');
    $('#CreateAt').val('');
    $('#Name').val('');
    $('#IdGroupParent').val('');

    // Reset validation
    if ($('#groupForm').data('validator')) {
        $('#groupForm').data('validator').resetForm();
    }
    $('#groupForm').find('.text-danger').remove();
    $('#groupForm').find('.is-invalid').removeClass('is-invalid');
}

function submitForm() {
    if (!$('#groupForm').data('validator')) {
        initFormValidate();
    }

    if ($('#groupForm').valid()) {
        const formData = {
            IdGroup: $('#IdGroup').val(),
            IdOrganization: $('#IdOrganization').val(),
            CreateBy: $('#CreateBy').val(),
            CreateAt: $('#CreateAt').val(),
            Name: $('#Name').val(),
            IdGroupParent: $('#IdGroupParent').val(),
            IsDeleted: $('#IsDeleted').val(),
        };

        if (formState.isEditing && formState.currentEditId) {
            editGroup(formData);
        } else {
            addGroup(formData);
        }
    }
}