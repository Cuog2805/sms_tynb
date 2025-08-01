$(document).ready(function () {
	// xử lý file upload modal
	let debounceTimer;
	$("#searchFileInput").on("input", function () {
		clearTimeout(debounceTimer);

		debounceTimer = setTimeout(function () {
			currentFilePagination.pageNumber = 1;
			LoadFileInput();
		}, 300);
	});

	$('#inputFileModal').on('shown.bs.modal', function () {
		LoadFileInput();
    });

	$("#uploadNewFileBtn").on("click", function () {
		if (!$('#uploadFileModalForm').data('validator')) {
			initFileFormValidate();
		}

		if ($("#uploadFileModalForm").valid()) {
			$('#inputFileModal').modal('hide');
			displaySelectedFiles();
		}
    });
});

let currentFilePagination = {
	pageNumber: 1,
	pageSize: 10
};

let paginationFileData = {
	total: 0,
	data: []
};

let selectedFiles = [];

function LoadFileInput() {
	let searchInput = $('#searchFileInput').val();
	let pageable = {
		pageNumber: currentFilePagination.pageNumber,
		pageSize: currentFilePagination.pageSize,
		sort: ''
	};

	$.ajax({
		url: '/File/LoadData',
		type: 'GET',
		data: $.param({
			'searchInput': searchInput,
			'pageable.PageNumber': pageable.pageNumber,
			'pageable.PageSize': pageable.pageSize,
			'pageable.Sort': pageable.sort
		}),
		success: function (response) {
			if (response.state === "success") {
				paginationFileData.total = response.content.Total;
				paginationFileData.data = response.content.Data;

				currentFilePagination.pageNumber = pageable.pageNumber;
				currentFilePagination.pageSize = pageable.pageSize;

				displayFiles(paginationFileData.data);
				CreatePagination(paginationFileData.total, currentFilePagination.pageNumber, currentFilePagination.pageSize, $("#pagination"));
			}
		},
		error: function (xhr) {
			console.log("xhr", xhr);
			alertify.error('Đã có lỗi xảy ra');
		}
	});
}

function loadPage(pageNumber) {
	currentFilePagination.pageNumber = pageNumber;
	LoadFileInput();
}

function displayFiles(items) {
    const container = $("#fileInputList");
    container.empty();

    if (items && items.length > 0) {
        let html = `
            <table class="table table-hover">
                <tbody>
        `;

        items.forEach(item => {
            const isChecked = selectedFiles.some(selected => selected.FileId === item.FileId) ? "checked" : "";

            html += `
                <tr>
                    <td>
                        <input type="checkbox" class="form-check-input" id="check-${item.FileId}" ${isChecked}>
                    </td>
                    <td>
                        <label class="form-check-label" for="check-${item.FileId}">
                            ${item.Name}
                        </label>
                    </td>
                    <td>
                        <a href="${item.FileUrl}" target="_blank" class="btn btn-link btn-sm">
                            <i class="fas fa-external-link-alt"></i> Xem nội dung
                        </a>
                    </td>
                </tr>
            `;
        });

        html += `
                </tbody>
            </table>
        `;

        container.html(html);

        items.forEach(item => {
            $(`#check-${item.FileId}`).data("item", item).on("change", function () {
                handleItemSelect($(this));
            });
        });

    } else {
        container.html(`<div class="alert alert-info text-center">Không có dữ liệu</div>`);
    }
}


function displaySelectedFiles() {
	const filesList = $('#selectedFilesList').empty();

	let html = '';
	//file từ input tab
	const fileInput = $("#FileDinhKem")[0];
    if (fileInput && fileInput.files && fileInput.files.length > 0) {

        for (let i = 0; i < fileInput.files.length; i++) {
            const file = fileInput.files[i];

            html += `
                <div class="selected-file-item d-flex align-items-center justify-content-between mb-2 p-2 border rounded bg-white">
                    <div class="d-flex align-items-center">
                        <i class="bi bi-folder me-2"></i>
                        <span class="file-name">${file.name}</span>
                    </div>
                </div>
            `;
		}
    }

	//file từ media tab
    if (selectedFiles && selectedFiles.length > 0) {
        selectedFiles.forEach((file, index) => {
            html += `
                <div class="selected-file-item d-flex align-items-center justify-content-between mb-2 p-2 border rounded bg-white">
                    <div class="d-flex align-items-center">
                        <i class="bi bi-folder me-2"></i>
                        <span class="file-name">${file.Name}</span>
                    </div>
                </div>
            `;
        });
    }
    filesList.html(html);
}

function clearSelectedFiles() {
	$('#selectedFilesList').empty();
	$("#FileDinhKem").val('');
	selectedFiles = [];
}

function handleItemSelect(checkbox) {
	const item = checkbox.data("item");
	const isChecked = checkbox.prop("checked");

	if (isChecked) {
        if (!selectedFiles.some(selected => selected.FileId === item.FileId)) {
			selectedFiles.push(item);
		}
	} else {
        selectedFiles = selectedFiles.filter(selected => selected.FileId !== item.FileId);
	}
}

function initFileFormValidate() {
    $('#uploadFileModalForm').validate({
        rules: {
            'FileDinhKem': {
                extension: "doc|docx|pdf|png|jpg",
                filesize: 5 * 1024 * 1024
            }
        },
        messages: {
            'FileDinhKem': {
                extension: "Chỉ chấp nhận .doc, .docx, .pdf, .png, .jpg",
                filesize: "Kích thước mỗi tệp không được vượt quá 5MB"
            }
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}