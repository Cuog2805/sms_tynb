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

	// Modal form
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

	// nhập/xuất dữ liệu
	$("#btnDownloadTemplate").on("click", function () {
		downloadTemplate("template_can_bo.xlsx");
	});

	//modal nhập dữ liệu
	$("#btnImportTemplate").on("click", function () {
		$('#inputFileTemplateModal').modal('show');
	});

	$("#importFileTemplateBtn").on("click", function () {
		readFile(function (data) {
			importWpCanbos(data);
		});
	});

	$("#btnExportTemplate").on("click", function () {
		$.ajax({
			url: '/Contact/LoadData',
			type: 'GET',
			data: $.param({
				'pageable.PageNumber': 1,
				'pageable.PageSize': 999999,
				'pageable.Sort': ''
			}),
			success: function (response) {
				if (response.state === "success") {
					exportExcel(response.content.Data);
				}
			},
			error: function () {
				alertify.error('Có lỗi xảy ra khi load dữ liệu');
			}
		});
	});

	createImportFileConfig({
		headers: ["Name", "PhoneNumber", "Gender", "Description"],
		headersLabel: ["Tên cán bộ", "Số điện thoại", "Giới tính", "Mô tả"],
		range: 1
	});

	//modal thông báo import thành công
	$('#successImportModal').on('hidden.bs.modal', function () {
		failImportWpCanboPagination.pageNumber = 1;
		succeedImportWpCanboPagination.pageNumber = 1;
		loadData();
	});

	const succeedImportWpCanboPaginationId = $("#succeedImportWpCanboPagination").attr("id");
	$("#pageSize-" + succeedImportWpCanboPaginationId).on("change", function () {
		succeedImportWpCanboPagination.pageNumber = 1;
		succeedImportWpCanboPagination.pageSize = parseInt($(this).val());
		displayResultImport();
	});

	const failImportWpCanboPaginationId = $("#failImportWpCanboPagination").attr("id");
	$("#pageSize-" + failImportWpCanboPaginationId).on("change", function () {
		failImportWpCanboPagination.pageNumber = 1;
		failImportWpCanboPagination.pageSize = parseInt($(this).val());
		displayResultImport();
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

//modal thông báo import thành công
let succeedImportWpCanboPagination = {
	pageNumber: 1,
	pageSize: 10
};

var succeedImportWpCanbo = [];

let failImportWpCanboPagination = {
	pageNumber: 1,
	pageSize: 10
};

var failImportWpCanbo = [];

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
		error: function (xhr) {
			console.log('xhr', xhr);
			alertify.error('Có lỗi xảy ra khi load dữ liệu');
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
                    <td>${item.PhoneNumber}</td>
                    <td>${item.Gender}</td>
                    <td>${item.Description || ''}</td>
					<td>${item.IsDeleted}</td>
                    <td class="text-center">
                        <button 
                            class="btn btn-sm btn-primary" 
                            type="button"  
                            data-bs-toggle="modal" 
                            data-bs-target="#data-form" 
                            onclick="startEdit(${item.EmployeeId})"
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

function importWpCanbos(data) {
	const validData = data.filter(item => {
		return item.Name && item.Name.trim() !== '' &&
			item.PhoneNumber && item.PhoneNumber.trim() !== '';
	});

	if (validData.length === 0) {
		alertify.error('Không có dữ liệu hợp lệ để import');
		return;
	}

	const importData = {
		model: validData.map(item => ({
			Name: item.Name?.toString().trim() || '',
			PhoneNumber: item.PhoneNumber?.toString().trim() || '',
			Gender: item.Gender?.toString().trim() == 'Nam' ? 1 : 0,
			Description: item.Description?.toString().trim() || '',
			IsDeleted: 0, // Mặc định active
		}))
	};
	$.ajax({
		url: '/Contact/Import',
		type: 'POST',
		data: importData,
		dataType: "json",
		success: function (response) {
			if (response.state === 'success') {
				alertify.success(response.msg || 'Import thành công');
				// clear modal import
				clearDataPreView()

				//hiển thị kết quả
				$("#successImportModal").modal('show');
				succeedImportWpCanbo = response.data.MEmployeeNew;
				failImportWpCanbo = response.data.MEmployeeDupplicate;
				displayResultImport(response.data);
			} else {
				alertify.error(response.msg || 'Đã có lỗi xảy ra');
			}
		},
		error: function () {
			alertify.error('Có lỗi xảy ra khi Import dữ liệu');
		}
	});
}

function displayResultImport() {
	// Hiển thị danh sách import thành công
	if (succeedImportWpCanbo && succeedImportWpCanbo.length > 0) {
		//phân trang
		const startIndex = (succeedImportWpCanboPagination.pageNumber - 1) * succeedImportWpCanboPagination.pageSize;
		const endIndex = startIndex + succeedImportWpCanboPagination.pageSize;
		const currentPageItems = succeedImportWpCanbo.slice(startIndex, endIndex);

		let succeedTableHtml = '';
		currentPageItems.forEach((item, index) => {
			succeedTableHtml += `
                <tr>
                    <td class="text-center">${index + 1}</td>
                    <td>${item.Name}</td>
                    <td>${item.PhoneNumber}</td>
                    <td>${item.Gender}</td>
                    <td>${item.Description || ''}</td>
                </tr>
            `;
		});
		$('#succeedImportWpCanboTableBody').html(succeedTableHtml);
		$('#succeedImportWpCanboTable').show();
	} else {
		$('#succeedImportWpCanboTable').hide();
	}

	// Hiển thị danh sách import không thành công
	if (failImportWpCanbo && failImportWpCanbo.length > 0) {
		//phân trang
		const startIndex = (failImportWpCanboPagination.pageNumber - 1) * failImportWpCanboPagination.pageSize;
		const endIndex = startIndex + failImportWpCanboPagination.pageSize;
		const currentPageItems = failImportWpCanbo.slice(startIndex, endIndex);

		let failTableHtml = '';
		currentPageItems.forEach((item, index) => {
			failTableHtml += `
                <tr>
                    <td class="text-center">${index + 1}</td>
                    <td>${item.Name}</td>
                    <td>${item.PhoneNumber}</td>
                    <td>${item.Gender}</td>
                    <td>${item.Description || ''}</td>
                </tr>
            `;
		});
		$('#failImportWpCanboTableBody').html(failTableHtml);
		$('#failImportWpCanboTable').show();

		// têm header cho danh sách không thành công
		$('#failImportWpCanboTotal').text(`${failImportWpCanbo.length}`);
	} else {
		$('#failImportWpCanboTable').hide();
	}

	// Thêm header cho danh sách thành công
	$('#succeedImportWpCanboTotal').text(`${succeedImportWpCanbo.length}`);

	// Tạo phân trang
	CreatePaginationMinimal(
		succeedImportWpCanbo.length,
		succeedImportWpCanboPagination.pageNumber,
		succeedImportWpCanboPagination.pageSize,
		$("#succeedImportWpCanboPagination"),
		loadSucceedImportWpCanboPage
	);

	// Tạo phân trang
	CreatePaginationMinimal(
		failImportWpCanbo.length,
		failImportWpCanboPagination.pageNumber,
		failImportWpCanboPagination.pageSize,
		$("#failImportWpCanboPagination"),
		loadFailImportWpCanboPage
	);
}

function loadSucceedImportWpCanboPage(pageNumber) {
	succeedImportWpCanboPagination.pageNumber = pageNumber;
	displayResultImport();
}

function loadFailImportWpCanboPage(pageNumber) {
	failImportWpCanboPagination.pageNumber = pageNumber;
	displayResultImport();
}

function addEmployee(formData) {
	$.ajax({
		url: '/Contact/Create',
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
		error: function (xhr) {
			console.log('xhr', xhr);
			alertify.error('Có lỗi xảy ra khi thêm dữ liệu');
		}
	});
}

function editEmployee(formData) {
	$.ajax({
		url: '/Contact/Update',
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
		error: function (xhr) {
			console.log('xhr', xhr)
			alertify.error('Có lỗi xảy ra khi cập nhật dữ liệu');
		}
	});
}

// form action
function initFormValidate() {
	$('#contactForm').validate({
		rules: {
			'Data.Name': {
				required: true
			},
			'Data.Gender': {
				required: true
			},
			'Data.PhoneNumber': {
				required: true,
                pattern: /^0\d{9,10}$/	
			},
			'Data.IsDeleted': {
				required: true
			},
		},
		messages: {
			'Data.Name': {
				required: "Vui lòng nhập tên cán bộ"
			},
			'Data.Gender': {
				required: "Vui lòng chọn giới tính"
			},
			'Data.PhoneNumber': {
				required: "Vui lòng nhập số điện thoại",
				pattern: "Định dạng số điện thoại không hợp lệ"
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
	$("#data-form-header").html("Thêm cán bộ");

	$('#TenCanbo').focus();
}

function clearForm() {
	$('#EmployeeId').val('');
	$('#OrganizationId').val('');
	$('#CreatedBy').val('');
	$('#CreatedAt').val('');
	$('#Name').val('');
	$('#PhoneNumber').val('');
	$('#Description').val('');

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
			EmployeeId: $('#EmployeeId').val(),
			OrganizationId: $('#OrganizationId').val(),
			CreatedBy: $('#CreatedBy').val(),
			CreatedAt: $('#CreatedAt').val(),
			Name: $('#Name').val(),
			Gender: $('#Gender').val(),
			PhoneNumber: $('#PhoneNumber').val(),
			Description: $('#Description').val(),
			IsDeleted: $('#IsDeleted').val(),
		};

		if (formState.isEditing && formState.currentEditId) {
			editEmployee(formData);
		} else {
			addEmployee(formData);
		}
	}
}