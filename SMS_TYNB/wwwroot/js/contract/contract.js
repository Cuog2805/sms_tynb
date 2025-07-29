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
		headers: ["TenCanbo", "SoDt", "Gioitinh", "Mota"],
		headersLabel: ["Tên cán bộ", "Số điện thoại", "Giới tính", "Mô tả"],
		range: 1
	});

	//modal thông báo import thành công
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
		return item.TenCanbo && item.TenCanbo.trim() !== '' &&
			item.SoDt && item.SoDt.trim() !== '';
	});

	if (validData.length === 0) {
		alertify.error('Không có dữ liệu hợp lệ để import');
		return;
	}

	const importData = {
		model: validData.map(item => ({
			TenCanbo: item.TenCanbo?.toString().trim() || '',
			SoDt: item.SoDt?.toString().trim() || '',
			Gioitinh: item.Gioitinh?.toString().trim() == 'Nam' ? 1 : 0,
			Mota: item.Mota?.toString().trim() || '',
			Trangthai: 1, // Mặc định active
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
				succeedImportWpCanbo = response.data.WpCanboNew;
				failImportWpCanbo = response.data.WpCanboDupplicate;
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
                    <td>${item.TenCanbo}</td>
                    <td>${item.SoDt}</td>
                    <td>${item.Gioitinh}</td>
                    <td>${item.Mota || ''}</td>
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
                    <td>${item.TenCanbo}</td>
                    <td>${item.SoDt}</td>
                    <td>${item.Gioitinh}</td>
                    <td>${item.Mota}</td>
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
	if (succeedImportWpCanbo && succeedImportWpCanbo.length > 0) {
		$('#succeedImportWpCanboTotal').text(`${succeedImportWpCanbo.length}`);
	}

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

function addWpCanbo(formData) {
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
		error: function () {
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