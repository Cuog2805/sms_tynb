namespace VnptSmsBrandName.Service
{
	public class FileDownloadResult
	{
		public byte[] FileContent { get; set; }
		public string ContentType { get; set; }
		public string FileName { get; set; }
	}
	public class DataTransportService : IDataTransportService
	{
		private readonly IWebHostEnvironment _environment;
		public DataTransportService(IWebHostEnvironment environment)
		{
			_environment = environment;
		}
		public async Task<FileDownloadResult> DownloadSampleExcel(string fileName, string folderPath = "template")
		{
			var filePath = Path.Combine(_environment.WebRootPath, folderPath, fileName);
			// Logic to download the Excel file
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Không tìm thấy '{fileName}' trong thư mục '{folderPath}'.");
			}

			// Read file content
			var fileBytes = await File.ReadAllBytesAsync(filePath);
			var contentType = GetContentType(fileName);

			var file = new FileDownloadResult
			{
				FileContent = fileBytes,
				ContentType = contentType,
				FileName = fileName
			};
			return file;
		}

		private static string GetContentType(string fileName)
		{
			var extension = Path.GetExtension(fileName).ToLowerInvariant();

			return extension switch
			{
				".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				".xls" => "application/vnd.ms-excel",
				".csv" => "text/csv",
				".pdf" => "application/pdf",
				".doc" => "application/msword",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				".txt" => "text/plain",
				_ => "application/octet-stream"
			};
		}
	}

	public interface IDataTransportService
	{
		Task<FileDownloadResult> DownloadSampleExcel(string fileName, string folderPath);
	}
}
