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
	public class MFileService: BaseService, IMFileService
	{
		private readonly IWebHostEnvironment _environment;
		private readonly MFileRepository _mFileRepository;
		private readonly MSmsFileRepository _mSmsFileRepository;
		private readonly MHistoryRepository _mHistoryRepository;
		public MFileService
		(
			ICurrentUserService currentUserService,
			IWebHostEnvironment environment,
			MFileRepository mFileRepository,
			MSmsFileRepository mSmsFileRepository,
			MHistoryRepository mHistoryRepository
		) : base(currentUserService)
		{
			_environment = environment;
			_mFileRepository = mFileRepository;
			_mSmsFileRepository = mSmsFileRepository;
			_mHistoryRepository = mHistoryRepository;
		}

		public async Task<IEnumerable<MFile>> GetAllFile()
		{
			var user = await _currentUserService.GetCurrentUser();
			IEnumerable<MFile> files = _mFileRepository.Query().Where(item => item.IdOrganization == user.OrgId);
			return files;
		}

		public async Task<PageResult<MFile>> SearchFile(string searchInput, Pageable pageable)
		{
			var user = await _currentUserService.GetCurrentUser();
			IQueryable<MFile> files = await _mFileRepository.Search(searchInput, user.OrgId);

			var filesPage = await _mFileRepository.GetPagination(files, pageable);

			int total = await files.CountAsync();

			return new PageResult<MFile>
			{
				Data = filesPage,
				Total = total,
			};
		}
		public async Task<MFile> Create(MFile model)
		{
			await SetCreateAudit(model);
			MFile file = await _mFileRepository.Create(model);

			// Luu l?ch s? t?o file
			var history = new MHistory
			{
				IdOrganization = file.IdOrganization,
				TableName = "m_file",
				IdRecord = file.IdFile,
				Action = "CREATE",
				CreatedBy = file.CreateBy,
				CreatedAt = file.CreateAt,
			};
			await _mHistoryRepository.Create(history);

			return file;
		}

		public async Task<MFile> SaveFile(IFormFile file, Users creator, long smsId, string subFolder = "upload")
		{
			if (file == null || file.Length == 0)
				throw new Exception("File kh�ng h?p l?");

			// T?o thu m?c upload n?u chua t?n t?i
			var subFolderUser = Path.Combine(subFolder, DateTime.Now.ToString("ddMMyyyy"));
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
				throw new Exception($"{fileExtension} kh�ng h?p l?");
			}

			// T?o t�n file
			var fileName = file.FileName.Replace(" ", "_");
			fileName = CommonHelper.RemoveUnicodeMark(fileName);
			fileName = CommonHelper.RemoveSign4VietnameseString(fileName);
			var filePath = Path.Combine(uploadPath, fileName);

			// Luu file
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Luu th�ng tin file v�o DB
			var fileSave = new MFile
			{
				Name = fileName,
				FileUrl = "/" + Path.Combine(subFolderUser, fileName).Replace("\\", "/"),
				Type = file.ContentType,
			};

			fileSave = await Create(fileSave);

			// Luu th�ng tin SmsFile
			var smsfile = new MSmsFile()
			{
				IdSms = smsId,
				IdFile = fileSave.IdFile,
			};
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
					// Ki?m tra xem li�n k?t d� t?n t?i chua d? tr�nh tr�ng l?p
					var existingSmsFile = _mSmsFileRepository.GetBySmsIdAndFileId(smsId, existingFile.IdFile);
					if (existingSmsFile == null)
					{
						var smsFile = new MSmsFile
						{
							IdSms = smsId,
							IdFile = existingFile.IdFile
						};

						await _mSmsFileRepository.Create(smsFile);
					}

					linkedFiles.Add(existingFile);
				}
			}
			return linkedFiles;
		}

		public async Task UpdateContentFile(IFormFile file, long oldFileId)
		{
			if (file == null || file.Length == 0)
				throw new Exception("File kh�ng h?p l?");

			var oldFile = await _mFileRepository.FindById(oldFileId);
			if (oldFile == null)
				throw new Exception("File kh�ng t?n t?i");

			string fileExtension = Path.GetExtension(oldFile.Name);
			// Validate file extension c?a file m?i
			var allowedExtensions = new[] { fileExtension };
			var newFileExtension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(newFileExtension))
			{
				throw new Exception($"{newFileExtension} kh�ng h?p l?");
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

				// Luu file m?i v?i t�n c?a file cu
				using (var stream = new FileStream(oldFilePath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				await SetUpdateAudit(oldFile);
				await _mFileRepository.Update(oldFile.IdFile, oldFile);
				// Luu l?ch s? thay d?i file
				var user = await _currentUserService.GetCurrentUser();
				var history = new MHistory
				{
					IdOrganization = oldFile.IdOrganization,
					TableName = "m_file",
					IdRecord = oldFile.IdFile,
					Action = "UPDATE",
					CreatedBy = user.UserName,
					CreatedAt = DateTime.Now,
				};
				await _mHistoryRepository.Create(history);

				// X�a backup file
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

				throw new Exception($"L?i khi c?p nh?t file: {ex.Message}");
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

		public async Task<MFileViewModel> GetAllFileHistory(long id)
		{
			MFile? mFile = await _mFileRepository.FindById(id);
			if(mFile == null)
			{
				return new MFileViewModel() { };
			}
			return new MFileViewModel()
			{
				IdFile = mFile.IdFile,
				Name = mFile.Name,
				History = await _mHistoryRepository.GetByIdRecordAndTableName(id, "m_file")
			};
		}
	}

	public interface IMFileService
	{
		Task<MFile> Create(MFile model);
		Task<IEnumerable<MFile>> GetAllFile();
		Task<PageResult<MFile>> SearchFile(string searchInput, Pageable pageable);
		Task<MFile> SaveFile(IFormFile file, Users creator, long smsId, string subFolder = "upload");
		Task<IEnumerable<MFile>> CreateFromFileExisted(List<long> selectedFileIds, Users creator, long smsId);
		Task UpdateContentFile(IFormFile file, long oldFileId);
		Task<MFileViewModel> GetAllFileHistory(long id);
	}
}
