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
		sort: ''
	};

	$.ajax({
		url: '/SmsConfig/LoadData',
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
			console.log(xhr);
			alertify.set('notifier', 'position', 'top-center');
			alertify.error('Đã có lỗi xảy ra');
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
					<td>${item.LabelId}</td>
					<td>${item.ContractId}</td>
					<td>${item.TemplateId}</td>
					<td>${item.IsTelCoSub}</td>
					<td>${item.AgentId}</td>
					<td>${item.ApiUser}</td>
					<td>${item.ApiPass}</td>
					<td>${item.UserName}</td>
					<td>${item.DataCoding}</td>
					<td>${item.SaleOrderId}</td>
					<td>${item.PakageId}</td>
					<td>${item.UrlSms}</td>
					<td>${item.IssEnSms}</td>
                    <td class="text-center">
                        <button 
                            class="btn btn-sm btn-primary" 
                            type="button"  
                            data-bs-toggle="modal" 
                            data-bs-target="#data-form" 
                            onclick="startEdit(${item.Id})"
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
	$('#smsConfigTableBody').html(tableHtml);
}

function loadDetail(id) {
	if (id) {
		$.ajax({
			url: '/SmsConfig/LoadDetail',
			type: 'GET',
			data: { id: id },
			success: function (response) {
				$('#data-form').html(response);
				formState.isEditing = true;
				formState.currentEditId = id;
				$("#data-form-header").html("Cập nhật sms config");
			},
			error: function () {
				alertify.set('notifier', 'position', 'top-center');
				alertify.error('Đã có lỗi xảy ra');
			}
		});
	}
}

function addSmsConfig(formData) {
	$.ajax({
		url: '/SmsConfig/Create',
		type: 'POST',
		data: formData,
		dataType: "json",
		success: function (response) {
			if (response.state === 'success') {
				alertify.set('notifier', 'position', 'top-center');
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
			alertify.error('Đã có lỗi xảy ra');
		}
	});
}

function editSmsConfig(formData) {
	$.ajax({
		url: '/SmsConfig/Update',
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
	$('#smsConfigForm').validate({
		rules: {
			LabelId: {
				required: true
			},
			ContractId: {
				required: true
			},
			TemplateId: {
				required: true
			},
			IsTelCoSub: {
				required: true
			},
			AgentId: {
				required: true
			},
			ApiUser: {
				required: true
			},
			ApiPass: {
				required: true
			},
			UserName: {
				required: true
			},
			DataCoding: {
				required: true
			},
			SaleOrderId: {
				required: true
			},
			PakageId: {
				required: true
			},
			UrlSms: {
				required: true
			},
		},
		messages: {
			LabelId: {
				required: "LabelId không được để trống!"
			},
			ContractId: {
				required: "ContractId không được để trống!"
			},
			TemplateId: {
				required: "TemplateId không được để trống!"
			},
			IsTelCoSub: {
				required: "IsTelCoSub không được để trống!"
			},
			AgentId: {
				required: "AgentId không được để trống!"
			},
			ApiUser: {
				required: "ApiUser không được để trống!"
			},
			ApiPass: {
				required: "ApiPass không được để trống!"
			},
			UserName: {
				required: "UserName không được để trống!"
			},
			DataCoding: {
				required: "DataCoding không được để trống!"
			},
			SaleOrderId: {
				required: "SaleOrderId không được để trống!"
			},
			PakageId: {
				required: "PakageId không được để trống!"
			},
			UrlSms: {
				required: "UrlSms không được để trống!"
			},
		},
		errorClass: "text-danger",
		errorElement: "div"
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
	$("#data-form-header").html("Thêm sms config");

	$('#LabelId').focus();
}

function clearForm() {
	$('#LabelId').val('');
	$('#ContractId').val('');
	$('#TemplateId').val('');
	$('#IsTelCoSub').val('');
	$('#AgentId').val('');
	$('#ApiUser').val('');
	$('#ApiPass').val('');
	$('#UserName').val('');
	$('#DataCoding').val('');
	$('#SaleOrderId').val('');
	$('#PakageId').val('');
	$('#UrlSms').val('');
	$('#IssEnSms').prop('checked', false);

	// Reset validation
	if ($('#smsConfigForm').data('validator')) {
		$('#smsConfigForm').data('validator').resetForm();
	}
	$('#smsConfigForm').find('.text-danger').remove();
	$('#smsConfigForm').find('.is-invalid').removeClass('is-invalid');
}

function submitForm() {
	if (!$('#smsConfigForm').data('validator')) {
		initFormValidate();
	}

	if ($('#smsConfigForm').valid()) {
		const formData = {
			Id: $("#Id").val(),
			LabelId: $('#LabelId').val(),
			ContractId: $('#ContractId').val(),
			TemplateId: $('#TemplateId').val(),
			IsTelCoSub: $('#IsTelCoSub').val(),
			AgentId: $('#AgentId').val(),
			ApiUser: $('#ApiUser').val(),
			ApiPass: $('#ApiPass').val(),
			UserName: $('#UserName').val(),
			DataCoding: $('#DataCoding').val(),
			SaleOrderId: $('#SaleOrderId').val(),
			PakageId: $('#PakageId').val(),
			UrlSms: $('#UrlSms').val(),
			IssEnSms: $('#IssEnSms').prop('checked')
		};

		if (formState.isEditing && formState.currentEditId) {
			editSmsConfig(formData);
		} else {
			addSmsConfig(formData);
		}
	}
}