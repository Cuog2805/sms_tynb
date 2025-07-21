$(document).ready(function () {
    loadData();
    $("#searchInput").on("input", function () {
        currentPagination.pageNumber = 1;
        loadData();
    })

    $("#btnSearchByDate").on("click", function () {
        if (!$('#searchByDateForm').data('validator')) {
            initFormValidate();
        }

        if ($('#searchByDateForm').valid()) {
            currentPagination.pageNumber = 1;
            loadData();
        }
    })

    $("#pageSize").on("change", function () {
        currentPagination.pageNumber = 1;
        currentPagination.pageSize = parseInt($(this).val());
        loadData();
    })

    // xử lý file upload modal
    $('#editFileModal').on('hidden.bs.modal', function () {
        $('#newFileInput').val('');
    });

    $('#uploadNewFileBtn').on('click', function () {
        uploadNewFile();
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

let currentEditingFile = {
    IdFile: '',
    FileUrl: '',
    TenFile: '',
    DuoiFile: '',   
    BangLuuFileId: ''
};

function loadData() {
    // xử lý input date
    const dateFromStr = $("#dateFrom").val();
    const dateToStr = $("#dateTo").val();
    const dateFrom = formatDateTime(new Date(dateFromStr + 'T00:00:00'));
    const dateTo = formatDateTime(new Date(dateToStr + 'T23:59:59'));

    //const dateFrom = $("#dateFrom").val();
    //const dateTo = $("#dateTo").val();

    let searchInput = $('#searchInput').val();
    let pageable = {
        pageNumber: currentPagination.pageNumber,
        pageSize: currentPagination.pageSize,
        sort: ''
    };

    $.ajax({
        url: '/Message/LoadData',
        type: 'GET',
        data: $.param({
            'model.searchInput': searchInput,
            'model.dateFrom': dateFrom,
            'model.dateTo': dateTo,
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
            let fileHtml = '';
            if (item.FileDinhKem && Array.isArray(item.FileDinhKem)) {
                fileHtml = item.FileDinhKem.map(file => {
                    if (file) {
                        return `
                        <div class="position-relative d-inline-block m-1">
                            <a class="text-primary text-decoration-underline cursor-pointer" onclick="toggleFileActions(this)">
                                ${file.TenFile}
                            </a>
                            <div class="card shadow-sm p-2 position-absolute top-100 start-0 d-none" style="z-index: 1000; min-width: 150px;">
                                <div class="mb-2">
                                    <a class="btn btn-primary btn-sm w-100 small" href="${file.FileUrl}" target="_blank">
                                        Xem
                                    </a>
                                </div>
                                <div class="mb-2">
                                    <button class="btn btn-primary btn-sm w-100 small" 
                                            onclick="editFile('${file.IdFile}', '${file.TenFile}','${file.DuoiFile}','${file.FileUrl}','${file.BangLuuFileId}')">
                                        Sửa
                                    </button>
                                </div>
                            </div>
                        </div>
                        `;
                    }
                }).join('<br/>');
            } else {
                fileHtml = 'Không có file';
            }
            tableHtml += `
                <tr id="row-${item.IdSms}">
                    <td class="text-center">${startIndex + index + 1}</td>
                    <td>${item.Noidung}</td>
                    <td>${fileHtml}</td>
                    <td>${item.TenNguoigui}</td>
                    <td>${item.Ngaygui.replace("T", " ")}</td>
                    <td>${item.SoTn}</td>
                    <td>${0}</td>
                </tr>
            `;
        });
    } else {
        tableHtml = `
            <tr>
                <td colspan="7" class="text-center text-muted">
                    <i class="fas fa-info-circle"></i>
                    Không có dữ liệu
                </td>
            </tr>
        `;
    }
    $('#messaegStatisticalTableBody').html(tableHtml);
}

function toggleFileActions(element) {
    const actions = element.nextElementSibling;
    const isVisible = !actions.classList.contains('d-none');

    document.querySelectorAll('.file-actions').forEach(el => {
        el.classList.add('d-none');
    });

    if (isVisible) {
        actions.classList.add('d-none');
    } else {
        actions.classList.remove('d-none');

        setTimeout(() => {
            const closeHandler = (e) => {
                if (!actions.contains(e.target) && !element.contains(e.target)) {
                    actions.classList.add('d-none');
                    document.removeEventListener('click', closeHandler);
                }
            };
            document.addEventListener('click', closeHandler);
        }, 100);
    }
}

function editFile(IdFile, TenFile, DuoiFile, FileUrl, SmsId) {
    currentEditingFile = {
        IdFile: IdFile,
        TenFile: TenFile,
        DuoiFile: DuoiFile,
        FileUrl: FileUrl,
        BangLuuFileId: SmsId
    };

    // Reset form
    $('#newFileInput').val('');

    $('#editFileModal').modal('show');
}

function uploadNewFile() {
    if (!$('#uploadFileModalForm').data('validator')) {
        initFormValidate();
    }

    if ($('#uploadFileModalForm').valid()) {


        const formData = new FormData();
        //model WpFile - thông tin file ban đầu
        const oldFile = {
            IdFile: currentEditingFile.IdFile,
            FileUrl: currentEditingFile.FileUrl,
            TenFile: currentEditingFile.TenFile,
            DuoiFile: currentEditingFile.DuoiFile,
            BangLuuFileId: currentEditingFile.BangLuuFileId
        };
        for (const key in oldFile) {
            formData.append(`oldFile.${key}`, oldFile[key]);
        }
        //file đính kèm mới
        const file = $('#newFileInput')[0].files[0];
        formData.append('fileDinhKem', file);

        $.ajax({
            url: '/Message/MessageUpdateFile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.state === 'success') {
                    alertify.success(res.msg);
                    $('#newFileInput').val('');
                    $('#editFileModal').modal('hide');
                } else {
                    alertify.error(res.msg);
                }
            },
            error: function (xhr) {
                console.log("xhr", xhr);
                alertify.error("Lỗi khi gửi tin nhắn");
            }
        });
    }
}

//form action
function initFormValidate() {
    $('#searchByDateForm').validate({
        rules: {
            'dateFrom': {
                required: true
            },
            'dateTo': {
                required: true
            },
        },
        messages: {
            'dateFrom': {
                required: "Vui lòng nhập từ ngày"
            },
            'dateTo': {
                required: "Vui lòng nhập đến ngày"
            },
        },
        errorClass: "text-danger",
        errorElement: "div",
    });

    $('#uploadFileModalForm').validate({
        rules: {
            'newFileInput': {
                required: true,
                extension: currentEditingFile.DuoiFile.slice(1),
                filesize: 5 * 1024 * 1024
            },
        },
        messages: {
            'newFileInput': {
                required: "Vui lòng nhập file mới",
                extension: "Chỉ chấp nhận định dạng file cũ: " + currentEditingFile.DuoiFile,
                filesize: "Kích thước mỗi tệp không được vượt quá 5MB"
            },
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}