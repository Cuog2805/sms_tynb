using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Helper
{
	public class FileUpload
	{
		public static async Task<WpFile> SaveFile(IFormFile file, WpUsers creator, string tableName, int tableId, string subFolder = "upload")
		{
			if (file == null || file.Length == 0)
				throw new ArgumentException("File không hợp lệ");

			// Tạo thư mục upload nếu chưa tồn tại
			var subFolderUser = Path.Combine(subFolder, creator.UserName, DateTime.Now.ToString("ddMMyyyy"));
			var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subFolderUser);
			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}

			// Validate file
			var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".doc", ".docx" };
			var fileExtension = Path.GetExtension(file.FileName).ToLower();

			if (!allowedExtensions.Contains(fileExtension))
			{
				throw new Exception($"{fileExtension} không hợp lệ");
			}

			// Tạo tên file
			var fileName = file.FileName.Replace(" ", "_");
			var filePath = Path.Combine(uploadPath, fileName);

			// Lưu file
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return new WpFile
			{
				TenFile = fileName,
				FileUrl = "/" + Path.Combine(subFolderUser, fileName).Replace("\\", "/"),
				Type = file.ContentType,
				BangLuuFile = tableName,
				BangLuuFileId = tableId,
			};
		}

		public static void DeleteFile(string fileUrl)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileUrl))
				{
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileUrl.TrimStart('/'));
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error DeleteFile: {ex.Message}");
			}
		}

		public static string GenerateFileName(string originalFileName)
		{
			var fileExtension = Path.GetExtension(originalFileName);
			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

			fileNameWithoutExtension = fileNameWithoutExtension.Replace(" ", "_");

			return $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{Guid.NewGuid().ToString("N")[..4]}_{fileNameWithoutExtension}{fileExtension}";
		}

	}

}
