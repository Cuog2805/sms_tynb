using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using VnptSmsBrandName.ViewModel;
using System.Threading.Tasks;

namespace VnptSmsBrandName.Service
{
	public class MFileService: IMFileService
	{
		private readonly IWebHostEnvironment _environment;
		private readonly MFileRepository _mFileRepository;
		private readonly MSmsFileRepository _mSmsFileRepository;
		private readonly MHistoryRepository _mHistoryRepository;
		public MFileService
		(
			IWebHostEnvironment environment,
			MFileRepository mFileRepository,
			MSmsFileRepository mSmsFileRepository,
			MHistoryRepository mHistoryRepository
		)
		{
			_environment = environment;
			_mFileRepository = mFileRepository;
			_mSmsFileRepository = mSmsFileRepository;
			_mHistoryRepository = mHistoryRepository;
		}

		public async Task<IEnumerable<MFile>> GetAllFile(long orgId)
		{
			IEnumerable<MFile> files = _mFileRepository.Query().Where(item => item.OrganizationId == orgId);
			return files;
		}

		public async Task<PageResult<MFile>> SearchFile(string searchInput, Pageable pageable, long orgId)
		{
			IQueryable<MFile> files = await _mFileRepository.Search(searchInput, orgId);

			var filesPage = await _mFileRepository.GetPagination(files, pageable);

			int total = await files.CountAsync();

			return new PageResult<MFile>
			{
				Data = filesPage,
				Total = total,
			};
		}
		public async Task<MFile> Create(MFile model, Users user)
		{
			AuditHelper.SetCreateAudit(model, user);
			MFile file = await _mFileRepository.Create(model);

			// Luu l?ch s? t?o file
			var history = new MHistory
			{
				OrganizationId = file.OrganizationId,
				TableName = "m_file",
				RecordId = file.FileId,
				Action = "CREATE",
				CreatedBy = file.CreatedBy,
				CreatedAt = file.CreatedAt,
			};
			await _mHistoryRepository.Create(history);

			return file;
		}

		public async Task<MFile> SaveFile(IFormFile file, Users creator, long smsId, string subFolder = "upload")
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không hợp lệ");

			// Tạo thư mục upload nếu chưa tồn tại
			var subFolderUser = Path.Combine(subFolder, creator.OrganizationId.ToString(), creator.Id, DateTime.Now.ToString("ddMMyyyy"));
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

			// Luu thông tin file vào DB
			var fileSave = new MFile
			{
				Name = fileName,
				FileUrl = "/" + Path.Combine(subFolderUser, fileName).Replace("\\", "/"),
				Type = file.ContentType,
			};

			fileSave = await Create(fileSave, creator);

			// Luu thông tin SmsFile
			var smsfile = new MSmsFile()
			{
				SmsId = smsId,
				FileId = fileSave.FileId,
				OrganizationId = creator.OrganizationId,
			};
			AuditHelper.SetCreateAudit(smsfile, creator);
			await _mSmsFileRepository.Create(smsfile);

			return fileSave;
		}
		public async Task<IEnumerable<MFile>> CreateFromFileExisted(List<long> selectedFileIds, Users creator, long smsId)
		{
			var linkedFiles = new List<MFile>();
			if (selectedFileIds != null && selectedFileIds.Any())
			{
				var existingFiles = await _mFileRepository.GetByIdFiles(selectedFileIds);
				foreach (var existingFile in existingFiles)
				{
					// kiểm tra xem file đã được liên kết với SMS chưa
					var existingSmsFile = _mSmsFileRepository.GetBySmsIdAndFileIdAndOrgId(smsId, existingFile.FileId, creator.OrganizationId);
					if (existingSmsFile == null)
					{
						var smsFile = new MSmsFile
						{
							SmsId = smsId,
							FileId = existingFile.FileId,
							OrganizationId = creator.OrganizationId
						};
						AuditHelper.SetCreateAudit(smsFile, creator);
						await _mSmsFileRepository.Create(smsFile);
					}

					linkedFiles.Add(existingFile);
				}
			}
			return linkedFiles;
		}

		public async Task UpdateContentFile(IFormFile file, long oldFileId, Users user)
		{
			if (file == null || file.Length == 0)
				throw new Exception("File không họp lệ");

			var oldFile = await _mFileRepository.FindById(oldFileId);
			if (oldFile == null)
				throw new Exception("File không tồn tại");

			string fileExtension = Path.GetExtension(oldFile.Name);
			// Validate file extension của file mới
			var allowedExtensions = new[] { fileExtension };
			var newFileExtension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(newFileExtension))
			{
				throw new Exception($"{newFileExtension} không hợp lệ");
			}

			try
			{
				var oldFileRelativePath = oldFile.FileUrl.TrimStart('/');
				var oldFilePath = Path.Combine(_environment.WebRootPath, oldFileRelativePath);

				// Backup file cu
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

				// Luu file m?i v?i tên c?a file cu
				using (var stream = new FileStream(oldFilePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				AuditHelper.SetUpdateAudit(oldFile, user);
				await _mFileRepository.Update(oldFile.FileId, oldFile);
				// Lưu lịch sử thay đổi file
				var history = new MHistory
				{
					OrganizationId = oldFile.OrganizationId,
					TableName = "m_file",
					RecordId = oldFile.FileId,
					Action = "UPDATE",
					CreatedBy = user.UserName,
					CreatedAt = DateTime.Now,
				};
				await _mHistoryRepository.Create(history);

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

		public async Task<MFileViewModel> GetAllFileHistory(long id, long orgId)
		{
			MFile? mFile = await _mFileRepository.FindByIdAndOrgId(id, orgId);
			if(mFile == null)
			{
				return new MFileViewModel() { };
			}
			return new MFileViewModel()
			{
				FileId = mFile.FileId,
				Name = mFile.Name,
				History = await _mHistoryRepository.GetByIdRecordAndTableName(id, "m_file")
			};
		}
	}

	public interface IMFileService
	{
		Task<MFile> Create(MFile model, Users user);
		Task<IEnumerable<MFile>> GetAllFile(long orgId);
		Task<PageResult<MFile>> SearchFile(string searchInput, Pageable pageable, long orgId);
		Task<MFile> SaveFile(IFormFile file, Users creator, long smsId, string subFolder = "upload");
		Task<IEnumerable<MFile>> CreateFromFileExisted(List<long> selectedFileIds, Users creator, long smsId);
		Task UpdateContentFile(IFormFile file, long oldFileId, Users user);
		Task<MFileViewModel> GetAllFileHistory(long id, long orgId);
	}
}
