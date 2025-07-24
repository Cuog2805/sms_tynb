$(document).ready(function () {
	// xử lý file upload modal
	let debounceTimer;
	$("#searchInput").on("input", function () {
		clearTimeout(debounceTimer);

		debounceTimer = setTimeout(function () {
			currentPagination.pageNumber = 1;
			loadData();
		}, 300);
	});

	$('#inputFileModal').on('shown.bs.modal', function () {
		LoadFileInput();
    });

    $("#uploadNewFileBtn").on("click", function () {
		$('#inputFileModal').modal('hide');
		displaySelectedFiles();
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
	let searchInput = $('#searchInput').val();
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
			alertify.set('notifier', 'position', 'top-center');
			alertify.error('Đã có lỗi xảy ra');
		}
	});
}

function loadPage(pageNumber) {
	currentFilePagination.pageNumber = pageNumber;
	LoadFileInput();
}

function displayFiles(items) {
    $("#fileInputList").empty();
    if (items && items.length > 0) {
        items.forEach((item, index) => {
			const isSelected = selectedFiles.some(selected => selected.IdFile === item.IdFile);
            const row = $("<div>").addClass("checkbox-item").append(
                $("<div>").addClass("form-check").append(
                    $("<input>")
                        .addClass("form-check-input")
                        .attr("type", "checkbox")
						.attr("id", `check-${item.IdFile}`)
                        .prop("checked", isSelected)
                        .data("item", item)
                        .on("change", function () {
                            handleItemSelect($(this));
                        }),
                    $("<label>")
                        .addClass("form-check-label")
						.attr("for", `check-${item.IdFile}`)
						.html(`${item.IdFile}-${item.TenFile}`)
                )
            );
            $("#fileInputList").append(row);
        });
    } else {
        $("#fileInputList").append($("<div>").append("Không có dữ liệu"));
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
                        <span class="file-name">${file.IdFile}-${file.TenFile}</span>
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
		if (!selectedFiles.some(selected => selected.IdFile === item.IdFile)) {
			selectedFiles.push(item);
		}
	} else {
		selectedFiles = selectedFiles.filter(selected => selected.IdFile !== item.IdFile);
	}
}