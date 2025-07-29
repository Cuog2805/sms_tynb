$(document).ready(function () {
    $("#previewFileTemplateBtn").on("click", function () {
        readFile();
    });

    //phân trang bảng preview
    const previewDataImportPaginationId = $("#previewDataImportPagination").attr("id");
    $("#pageSize-" + previewDataImportPaginationId).on("change", function () {
        dataPreviewPagination.pageNumber = 1;
        dataPreviewPagination.pageSize = parseInt($(this).val());
        displayDataPreview();
    });

    $('#inputFileTemplateModal').on('hidden.bs.modal', function (e) {

    });
});

var configFileTemplate = {
    headers: [],
    headersLabel: [],
    range: 1
};


let dataPreviewPagination = {
    pageNumber: 1,
    pageSize: 10
};

var dataPreview = [];

function createImportFileConfig(options = {}) {
    configFileTemplate = {
        headers: options.headers || [],
        headersLabel: options.headersLabel || [],
        range: options.range || 1
    };
}

function exportExcel(data, fileName = 'export.xlsx') {
    const exportData = [];

    exportData.push(configFileTemplate.headersLabel);

    if (data && data.length > 0) {
        data.forEach(item => {
            const row = [];
            configFileTemplate.headers.forEach(header => {
                let value = item[header];

                if (value === null || value === undefined) {
                    value = '';
                } else if (typeof value === 'object') {
                    value = JSON.stringify(value);
                } else {
                    value = value.toString();
                }

                row.push(value);
            });
            exportData.push(row);
        });
    }

    const ws = XLSX.utils.aoa_to_sheet(exportData);

    autoResizeColumns(ws, exportData);

    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');

    XLSX.writeFile(wb, fileName);
}

function autoResizeColumns(ws, data) {
    const colWidths = [];

    data.forEach(row => {
        row.forEach((cell, colIndex) => {
            const cellLength = cell ? cell.toString().length : 0;
            colWidths[colIndex] = Math.max(colWidths[colIndex] || 0, cellLength);
        });
    });

    // Thiết lập column widths
    ws['!cols'] = colWidths.map(width => ({
        wch: Math.min(Math.max(width + 2, 10), 50)
    }));
}

function readFile(callback) {
    if (!$('#inputFileTemplateModalForm').data('validator')) {
        initImportFileFormValidate();
    }
    if ($("#inputFileTemplateModalForm").valid()) {
        const file = $("#FileImport")[0].files[0];
        if (!file) {
            alertify.error("Vui lòng chọn file");
            return;
        }
        const reader = new FileReader();
        reader.onload = function (event) {
            try {
                const data = event.target.result;
                const workbook = XLSX.read(data, { type: 'binary' });
                const firstSheetName = workbook.SheetNames[0];
                const worksheet = workbook.Sheets[firstSheetName];
                const options = {
                    range: configFileTemplate.range,
                    defval: ""
                };
                if (configFileTemplate.headers && configFileTemplate.headers.length > 0) {
                    options.header = configFileTemplate.headers;
                } else {
                    options.header = 1;
                }
                const jsonData = XLSX.utils.sheet_to_json(worksheet, options);

                if (jsonData.length > 0) {
                    // Loại bỏ bản ghi trùng nhau
                    let uniqueData = removeDuplicates(jsonData);
                    uniqueData = removeDuplicatesByFields(uniqueData, ['SoDt']);

                    dataPreview = uniqueData;
                    displayDataPreview();

                    if (jsonData.length > uniqueData.length) {
                        alertify.success(`Đã bỏ ${jsonData.length - uniqueData.length} bản ghi trùng.`);
                    }

                    // Gọi callback với dữ liệu đã loại bỏ trùng
                    if (typeof callback === 'function') {
                        callback(uniqueData);
                    }
                } else {
                    alertify.error("Không có dữ liệu trong file");
                }
            } catch (error) {
                console.error('Error reading Excel file:', error);
                alertify.error("Có lỗi khi đọc file Excel");
            }
        };
        reader.onerror = function () {
            alertify.error("Có lỗi khi đọc file");
        };
        reader.readAsBinaryString(file);
    }
}

function removeDuplicates(data) {
    const seen = new Set();
    return data.filter(item => {
        const key = JSON.stringify(item);
        if (seen.has(key)) {
            return false;
        }
        seen.add(key);
        return true;
    });
}

/**
 * 
 * @param {list} data - Data
 * @param {list} fields - danh sách tên trường
 * @returns {list} Data đã loại bỏ trùng theo fields
 */
function removeDuplicatesByFields(data, fields) {
    const seen = new Set();
    return data.filter(item => {
        const key = fields.map(field => item[field]).join(',');
        if (seen.has(key)) {
            return false;
        }
        seen.add(key);
        return true;
    });
}

function displayDataPreview() {
    if (dataPreview && dataPreview.length > 0) {
        const $thead = $('<thead></thead>');
        const $theadRow = $('<tr></tr>');
        // Tạo header
        configFileTemplate.headersLabel.forEach(header => {
            $theadRow.append($('<th></th>').text(header));
        });
        $thead.append($theadRow);

        // Tạo body
        const $tbody = $('<tbody></tbody>');
        const startIndex = (dataPreviewPagination.pageNumber - 1) * dataPreviewPagination.pageSize;
        const endIndex = startIndex + dataPreviewPagination.pageSize;
        const currentPageItems = dataPreview.slice(startIndex, endIndex);

        currentPageItems.forEach(row => {
            const $tr = $('<tr></tr>');
            configFileTemplate.headers.forEach(header => {
                $tr.append($('<td></td>').text(row[header] || ''));
            });
            $tbody.append($tr);
        });

        $('#previewDataImportTable').empty().append($thead).append($tbody);

        // Tạo phân trang
    }
    else {
        $('#previewDataImportTable').empty();
    }
    CreatePaginationMinimal(
        dataPreview.length,
        dataPreviewPagination.pageNumber,
        dataPreviewPagination.pageSize,
        $("#previewDataImportPagination"),
        loadDataPreviewPage
    );
}

function clearDataPreView() {
    // Reset modal
    $("#FileImport").val('');
    dataPreview = [];
    dataPreviewPagination.pageNumber = 1;
    $("#pagination-info-previewDataImportPagination").empty();
    displayDataPreview();
    $('#inputFileTemplateModal').modal('hide');
}

function loadDataPreviewPage(pageNumber) {
    dataPreviewPagination.pageNumber = pageNumber;
    displayDataPreview();
}

function downloadTemplate(filename) {
    fetch(`/DataTransport/DownloadSample?filename=${filename}`)
        .then(response => {
            if (!response.ok) {
                alertify.error("Có lỗi khi tải file");
                throw new Error("Có lỗi khi tải file");
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error:', error);
            alertify.error("Có lỗi khi tải file");
        });
}

function initImportFileFormValidate() {
    $('#inputFileTemplateModalForm').validate({
        rules: {
            'FileImport': {
                required: true,
                extension: "xls|xlsx",
            },
        },
        messages: {
            'FileImport': {
                required: "Vui lòng chọn file",
                extension: "Chọn file định dạng .xls, .xlsx",
            },
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}