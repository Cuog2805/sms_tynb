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
	});

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
		sort: 'TenCanbo'
	};

	$.ajax({
		url: '/Contact/LoadData',
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
		error: function () {
			alert("Lỗi khi load dữ liệu");
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
                    <td>${item.TenCanbo}</td>
                    <td>${item.SoDt}</td>
                    <td>${item.Gioitinh}</td>
                    <td>${item.Mota || ''}</td>
					<td>${item.Trangthai}</td>
                    <td class="text-center">
                        <button 
                            class="btn btn-sm btn-primary" 
                            type="button"  
                            data-bs-toggle="modal" 
                            data-bs-target="#data-form" 
                            onclick="startEdit(${item.IdCanbo})"
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
                <td colspan="6" class="text-center text-muted">
                    Không có dữ liệu
                </td>
            </tr>
        `;
	}
	$('#contactTableBody').html(tableHtml);
}

function loadDetail(id) {
	if (id) {
		$.ajax({
			url: '/Contact/LoadDetail',
			type: 'GET',
			data: { id: id },
			success: function (response) {
				$('#data-form').html(response);
				formState.isEditing = true;
				formState.currentEditId = id;
				$("#data-form-header").html("Cập nhật cán bộ");
			},
			error: function (xhr, error) {
				console.log("xhr", xhr);
				console.log("error", error);
				alertify.error("Lỗi khi load thông tin chi tiết");
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
				formState.currentEditId = null;

				loadData();
				$('#data-form').modal('hide');
			} else {
				alertify.set('notifier', 'position', 'top-center');
				alertify.error(response.msg || 'Đã có lỗi xảy ra');
			}
		},
		error: function () {
			alertify.set('notifier', 'position', 'top-center');
			alertify.error('Có lỗi xảy ra khi thêm dữ liệu');
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
				alertify.set('notifier', 'position', 'top-center');
				alertify.success(response.msg || 'Cập nhật thành công');

				// Reset form state
				formState.isEditing = false;
				formState.currentEditId = null;

				loadData();
				$('#data-form').modal('hide');
			} else {
				alertify.set('notifier', 'position', 'top-center');
				alertify.error(response.msg || 'Đã có lỗi xảy ra');
			}
		},
		error: function () {
			alertify.set('notifier', 'position', 'top-center');
			alertify.error('Có lỗi xảy ra khi cập nhật dữ liệu');
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

function startEdit(id) {
	formState.isEditing = true;
	formState.currentEditId = id;
	loadDetail(id);
}

function beforeAdd() {
	formState.isEditing = false;
	formState.currentEditId = null;
	clearForm();
	$("#data-form-header").html("Thêm cán bộ");

	$('#TenCanbo').focus();
}

function clearForm() {
	$('#IdCanbo').val('');
	$('#MaCanbo').val('');
	$('#TenCanbo').val('');
	$('#SoDt').val('');
	$('#Mota').val('');

	// Reset validation
	if ($('#contactForm').data('validator')) {
		$('#contactForm').data('validator').resetForm();
	}
	$('#contactForm').find('.text-danger').remove();
	$('#contactForm').find('.is-invalid').removeClass('is-invalid');
}

function submitForm() {
	if (!$('#contactForm').data('validator')) {
		initFormValidate();
	}

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

		if (formState.isEditing && formState.currentEditId) {
			editWpCanbo(formData);
		} else {
			addWpCanbo(formData);
		}
	}
}