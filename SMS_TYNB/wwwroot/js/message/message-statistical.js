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
});

let currentPagination = {
    pageNumber: 1,
    pageSize: 10
};

let paginationData = {
    total: 0,
    data: []
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

function displayItems(items, pageNumber, pageSize) {
    let tableHtml = '';
    const startIndex = (pageNumber - 1) * pageSize;

    if (items && items.length > 0) {
        items.forEach((item, index) => {
            let fileHtml = '';
            if (item.FileDinhKem && Array.isArray(item.FileDinhKem)) {
                fileHtml = item.FileDinhKem.map(file => {
                    if (file) {
                        return `<a href="${file.FileUrl}" target="_blank">${file.TenFile}</a>`;
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
}