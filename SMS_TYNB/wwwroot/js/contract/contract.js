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

				displayItemsdisplayItems(paginationData.data, currentPagination.pageNumber, currentPagination.pageSize);
				CreatePagination(paginationData.total, currentPagination.pageNumber, currentPagination.pageSize, $("#pagination"));
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

function displayItems(items, pageNumber, pageSize) {
	let tableHtml = '';
	const startIndex = (pageNumber - 1) * pageSize;

	if (items && items.length > 0) {
		items.forEach((item, index) => {
			tableHtml += `
                <tr id="row-${item.IdCanbo}" onclick="selectRow(${item.IdCanbo})" style="cursor: pointer;">
                    <td class="text-center">${startIndex + index + 1}</td>
                    <td>${item.TenCanbo}</td>
                    <td>${item.SoDt}</td>
                    <td>${item.Gioitinh}</td>
                    <td>${item.Mota}</td>
                </tr>
            `;
		});
	} else {
		tableHtml = `
            <tr>
                <td colspan="5" class="text-center text-muted">
                    <i class="fas fa-info-circle"></i>
                    Không có dữ liệu
                </td>
            </tr>
        `;
	}
	$('#contactTableBody').html(tableHtml);
}

function selectRow(rowId) {
	if (formState.isEditing && formState.selectedRowId !== rowId) {
		// Reset form state
		formState.isEditing = false;
		formState.originalData = null;
		disableFormInputs();
	}

	let rows = document.querySelectorAll("#contactTableBody tr");
	rows.forEach(r => r.classList.remove("table-primary"));

	$(`#row-${rowId}`).addClass('table-primary');

	formState.selectedRowId = rowId;

	loadDetail(rowId);
	updateButtonStates();
}

function loadDetail(id) {
	if (id) {
		$.ajax({
			url: '/Contact/LoadDetail',
			type: 'GET',
			data: { id: id },
			success: function (response) {
				$('#data-form').html(response);

				disableFormInputs();
				updateButtonStates();
			},
			error: function () {
				alert("Lỗi khi load form");
			}
		});
	}
}

function addWpCanbo(formData) {
	$.ajax({
		url: '/Contact/Create',
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
		error: function () {

		}
	});
}
function editWpCanbo(formData) {
	$.ajax({
		url: '/Contact/Update',
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
		error: function () {

		}
	});
}


// form action
function initFormValidate() {
	$('#contactForm').validate({
		rules: {
			'Data.TenCanbo': {
				required: true
			},
			'Data.Gioitinh': {
				required: true
			},
			'Data.SoDt': {
				required: true
			},
			'Data.Trangthai': {
				required: true
			},
		},
		messages: {
			'Data.TenCanbo': {
				required: "Vui lòng nhập tên cán bộ"
			},
			'Data.Gioitinh': {
				required: "Vui lòng chọn giới tính"
			},
			'Data.SoDt': {
				required: "Vui lòng nhập số điện thoại"
			},
			'Data.Trangthai': {
				required: "Vui lòng chọn trạng thái"
			}
		},
		errorClass: "text-danger",
		errorElement: "div",
	});
}

function disableFormInputs() {
	$('#contactForm input, #contactForm select').prop('disabled', true);
}

function enableFormInputs() {
	$('#contactForm input, #contactForm select').prop('disabled', false);
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
		IdCanbo: $('#IdCanbo').val(),
		IdCanbo: $('#MaCanbo').val(),
		TenCanbo: $('#TenCanbo').val(),
		Gioitinh: $('#Gioitinh').val(),
		SoDt: $('#SoDt').val(),
		Mota: $('#Mota').val(),
		Trangthai: $('#Trangthai').val(),
	};

	formState.isEditing = true;
	enableFormInputs();
	updateButtonStates();

	$('#TenCanbo').focus();
}

function cancelEdit() {
	if (formState.originalData) {
		$('#IdCanbo').val(formState.originalData.IdCanbo);
		$('#MaCanbo').val(formState.originalData.MaCanbo);
		$('#TenCanbo').val(formState.originalData.TenCanbo);
		$('#Gioitinh').val(formState.originalData.Gioitinh);
		$('#SoDt').val(formState.originalData.SoDt);
		$('#Mota').val(formState.originalData.Mota);
		$('#Trangthai').val(formState.originalData.Trangthai);
	}

	formState.isEditing = false;
	formState.originalData = null;
	disableFormInputs();
	updateButtonStates();

	if ($('#contactForm').data('validator')) {
		$('#contactForm').data('validator').resetForm();
	}
	$('#contactForm').find('.is-invalid').removeClass('is-invalid');
}

function beforeAdd() {
	let rows = document.querySelectorAll("#contactTableBody tr");
	rows.forEach(r => r.classList.remove("table-primary"));

	formState.selectedRowId = null;
	formState.isEditing = true;
	formState.originalData = null;

	clearForm();
	$('#IdCanbo').val('');
	$('#MaCanbo>').val('');
	enableFormInputs();
	updateButtonStates();

	$('#TenCanbo').focus();
}

function clearForm() {
	$('#IdCanbo').val('');
	$('#MaCanbo').val('');
	$('#TenCanbo').val('');
	$('#SoDt').val('');
	$('#Mota').val('');
}

function submitForm() {
	if (!$('#contactForm').data('validator')) {
		initFormValidate();
	}

	var a = $('#IdCanbo').val();
	var b = $('#MaCanbo').val();

	if ($('#contactForm').valid()) {
		const formData = {
			IdCanbo: $('#IdCanbo').val(),
			MaCanbo: $('#MaCanbo').val(),
			TenCanbo: $('#TenCanbo').val(),
			Gioitinh: $('#Gioitinh').val(),
			SoDt: $('#SoDt').val(),
			Mota: $('#Mota').val(),
			Trangthai: $('#Trangthai').val(),
		};

		if (formData.IdCanbo && formData.MaCanbo) {
			editWpCanbo(formData);
		} else {
			addWpCanbo(formData);
		}
	}
}