using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service.Implement
{
	public class WpFileService : IWpFileService
	{
		private readonly IWebHostEnvironment _environment;
		private readonly WpFileRepository _wpFileRepository;
		public WpFileService(WpFileRepository wpFileRepository, IWebHostEnvironment environment)
		{
			_environment = environment;
			_wpFileRepository = wpFileRepository;
		}

		public virtual async Task<IEnumerable<WpFile>> GetByBangLuuFile(string tableName)
		{
			IEnumerable<WpFile> wpFiles = await _wpFileRepository.GetByBangLuuFile(tableName);
			return wpFiles;
		}

		public async Task<PageResult<WpFile>> GetAllWpFile(string searchInput, Pageable pageable)
		{
			IQueryable<WpFile> wpFiles = await _wpFileRepository.Search(searchInput);

			var wpFilesPage = await _wpFileRepository.GetPagination(wpFiles, pageable);

			int total = await wpFiles.CountAsync();

			return new PageResult<WpFile>
			{
				Data = wpFilesPage,
				Total = total,
			};
		}
		public async Task<WpFile> Create(WpFile model)
		{
			WpFile wpFile = await _wpFileRepository.Create(model);
			return wpFile;
		}

		public async Task Delete(WpFile model)
		{
			this.DeleteFile(model.FileUrl);
			await _wpFileRepository.Delete(model.IdFile);
		}

		public async Task<WpFile?> GetById(int id)
		{
			WpFile? wpFile = await _wpFileRepository.FindById(id);
			return wpFile;
		}

		public async Task<WpFile?> Update(WpFile model)
		{
			WpFile? wpFile = await _wpFileRepository.Update(model.IdFile, model);
			return wpFile;
		}

		public async Task SaveFile(IFormFile file, WpUsers creator, string tableName, long tableId, string subFolder = "upload")
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không hợp lệ");

			// Tạo thư mục upload nếu chưa tồn tại
			var subFolderUser = Path.Combine(subFolder, creator.Id, DateTime.Now.ToString("ddMMyyyy"));
			var uploadPath = Path.Combine(_environment.WebRootPath, subFolderUser);
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
			fileName = CommonHelper.RemoveUnicodeMark(fileName);
			fileName = CommonHelper.RemoveSign4VietnameseString(fileName);
			var filePath = Path.Combine(uploadPath, fileName);

			// Lưu file
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Lưu thông tin file vào DB
			var fileSave = new WpFile
			{
				TenFile = fileName,
				FileUrl = "/" + Path.Combine(subFolderUser, fileName).Replace("\\", "/"),
				DuoiFile = fileExtension,
				Type = file.ContentType,
				BangLuuFile = tableName,
				BangLuuFileId = tableId,
			};

			await Create(fileSave);
		}

		public async Task CreateFromFileExisted(List<long> selectedFileIds, WpUsers creator, string tableName, long tableId)
		{
			if (selectedFileIds != null && selectedFileIds.Any())
			{
				var existingFiles = await _wpFileRepository.GetByIdFiles(selectedFileIds);

				foreach (var existingFile in existingFiles)
				{
					var newFile = CopyFileToNewLocation(existingFile, creator, tableName, tableId);
					await Create(newFile);
				}
			}
		}

		private WpFile CopyFileToNewLocation(WpFile originalFile, WpUsers creator, string tableName, long tableId, string subFolder = "upload")
		{
			// Tạo thư mục mới
			var subFolderUser = Path.Combine(subFolder, creator.Id, DateTime.Now.ToString("ddMMyyyy"));
			var destinationPath = Path.Combine(_environment.WebRootPath, subFolderUser);

			if (!Directory.Exists(destinationPath))
			{
				Directory.CreateDirectory(destinationPath);
			}

			// Lấy đường dẫn file gốc
			var originalFilePath = Path.Combine(_environment.WebRootPath, originalFile.FileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

			if (!File.Exists(originalFilePath))
			{
				throw new Exception($"File gốc không tồn tại: {originalFile.TenFile}");
			}

			var newFilePath = Path.Combine(destinationPath, originalFile.TenFile);

			// Copy file
			if(originalFilePath != newFilePath) File.Copy(originalFilePath, newFilePath);

			return new WpFile
			{
				TenFile = originalFile.TenFile,
				FileUrl = "/" + Path.Combine(subFolderUser, originalFile.TenFile).Replace("\\", "/"),
				DuoiFile = originalFile.DuoiFile,
				Type = originalFile.Type,
				BangLuuFile = tableName,
				BangLuuFileId = tableId,
			};
		}

		public async Task UpdateContentFile(IFormFile file, WpFile oldFile, WpUsers modifier)
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không hợp lệ");

			// Validate file extension của file mới
			var allowedExtensions = new[] { oldFile.DuoiFile };
			var newFileExtension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(newFileExtension))
			{
				throw new Exception($"{newFileExtension} không hợp lệ");
			}

			try
			{
				var oldFileRelativePath = oldFile.FileUrl.TrimStart('/');
				var oldFilePath = Path.Combine(_environment.WebRootPath, oldFileRelativePath);

				// Backup file cũ
				string backupFilePath = null;
				if (File.Exists(oldFilePath))
				{
					backupFilePath = oldFilePath + ".bak";
					if (File.Exists(backupFilePath))
					{
						File.Delete(backupFilePath);
					}
					File.Move(oldFilePath, backupFilePath);
				}

				var directory = Path.GetDirectoryName(oldFilePath);
				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}

				// Lưu file mới với tên của file cũ
				using (var stream = new FileStream(oldFilePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				// Xóa backup file
				if (!string.IsNullOrEmpty(backupFilePath) && File.Exists(backupFilePath))
				{
					File.Delete(backupFilePath);
				}
			}
			catch (Exception ex)
			{
				var oldFileRelativePath = oldFile.FileUrl.TrimStart('/');
				var oldFilePath = Path.Combine(_environment.WebRootPath, oldFileRelativePath);
				var backupFilePath = oldFilePath + ".bak";

				if (File.Exists(backupFilePath))
				{
					if (File.Exists(oldFilePath))
					{
						File.Delete(oldFilePath);
					}
					File.Move(backupFilePath, oldFilePath);
				}

				throw new Exception($"Lỗi khi cập nhật file: {ex.Message}");
			}
		}

		public void DeleteFile(string fileUrl)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileUrl))
				{
					var filePath = Path.Combine(_environment.WebRootPath, fileUrl.TrimStart('/'));
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error: {ex.Message}");
			}
		}

	}
}
