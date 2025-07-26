$(document).ready(function () {
    $("#previewFileTemplateBtn").on("click", function () {
        readFile();
    });
});

var configFileTemplate = {
    headers: [],
    range: 1
};

var dataPreview = [];

function createImportFileConfig(options = {}) {
    configFileTemplate = {
        headers: options.headers || [],
        range: options.range || 1
    };
}

function readFile() {
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

                console.log(jsonData);

                if (jsonData.length > 0) {
                    dataPreview = jsonData;
                    displayDataPreview(dataPreview);
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

function displayDataPreview(items) {
    const $thead = $('<thead></thead>');
    const $theadRow = $('<tr></tr>');

    // Tạo header
    configFileTemplate.headers.forEach(header => {
        $theadRow.append($('<th></th>').text(header));
    });
    $thead.append($theadRow);

    // Tạo body
    const $tbody = $('<tbody></tbody>');
    items.forEach(row => {
        const $tr = $('<tr></tr>');
        configFileTemplate.headers.forEach(header => {
            $tr.append($('<td></td>').text(row[header] || ''));
        });
        $tbody.append($tr);
    });

    $('#previewDataImportTable').empty().append($thead).append($tbody);
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
                required: true
            },
        },
        messages: {
            'FileImport': {
                required: "Vui lòng chọn file"
            },
        },
        errorClass: "text-danger",
        errorElement: "div",
    });
}