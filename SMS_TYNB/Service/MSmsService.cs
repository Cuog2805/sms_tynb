using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;

namespace SMS_TYNB.Service
{
	public class MSmsService: BaseService, IMSmsService
	{
		private readonly MSmsRepository _mSmsRepository;
		private readonly MSmsEmployeeRepository _mSmsEmployeeRepository;
		private readonly WpUsersRepository _wpUsersRepository;
		private readonly MEmployeeRepository _mEmployeeRepository;
		private readonly MGroupRepository _mGroupRepository;
		private readonly MFileRepository _mFileRepository;
		private readonly MSmsFileRepository _smsFileRepository;
		private readonly IMFileService _mFileService;
		private readonly ISmsConfigService _smsConfigService;

		public MSmsService
		(
			ICurrentUserService currentUserService,
			MSmsRepository mSmsRepository,
			MSmsEmployeeRepository mSmsEmployeeRepository,
			WpUsersRepository wpUsersRepository,
			MEmployeeRepository mEmployeeRepository,
			MGroupRepository mGroupRepository,
			MFileRepository mFileRepository,
			MSmsFileRepository smsFileRepository,
			IMFileService mFileService,
			ISmsConfigService smsConfigService
		) : base(currentUserService)
		{
			_mSmsRepository = mSmsRepository;
			_mSmsEmployeeRepository = mSmsEmployeeRepository;
			_wpUsersRepository = wpUsersRepository;
			_mEmployeeRepository = mEmployeeRepository;
			_mGroupRepository = mGroupRepository;
			_mFileRepository = mFileRepository;
			_smsFileRepository = smsFileRepository;
			_mFileService = mFileService;
			_smsConfigService = smsConfigService;
		}
		public async Task<ServiceResult<MSmsViewModel>> SendMessage(MSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, Users user)
		{
			// validate input
			if (user == null)
				return ServiceResult<MSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Lỗi khi thông tin người dùng!");

			if (string.IsNullOrEmpty(model.Content))
				return ServiceResult<MSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Không có nội dung tin nhắn!");

			if (model.Employees.Count == 0)
				return ServiceResult<MSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Chưa chọn người nhận tin nhắn!");

			MSms wpSms = null;
			try
			{
				wpSms = new MSms()
				{
					Content = model.Content,
					NumberMessages = model.Employees.Count,
					NumberMessageError = 0,
					CreateBy = user.UserName,
					CreateAt = DateTime.Now,
					IdOrganization = user.OrgId,
				};

				wpSms = await _mSmsRepository.Create(wpSms);
				int errorCount = 0;

				// Gửi tin nhắn
				var smsConfig = _smsConfigService.GetSmsConfigActive(true);

				// Xử lý file đính kèm
				var fileUrls = await HandleFileAttachments(fileDinhKem, selectedFileIds, user, wpSms.IdSms, smsConfig.Domain);
				var noidungGui = model.Content + " " + fileUrls;

				if (smsConfig?.Id > 0 && model.Employees.Any())
				{
					foreach (var canbo in model.Employees)
					{
						try
						{
							var cb = _mEmployeeRepository.FindById(canbo.IdEmployee ?? 0).Result;
							if (cb != null && cb.IdEmployee > 0)
							{
								var res = SmsHelper.SendSms(smsConfig, noidungGui, cb.PhoneNumberSend);
								await SendMessageToCanbo(canbo, wpSms.IdSms, res);
								if (!(res?.RPLY?.ERROR == "0"))
									errorCount++;
							}
						}
						catch (Exception)
						{
							errorCount++;
						}
					}
				}
				else
				{
					errorCount = model.Employees?.Count ?? 0;
				}

				wpSms.NumberMessageError = errorCount;
				wpSms = await _mSmsRepository.Update(wpSms.IdSms, wpSms);

				var result = new MSmsViewModel()
				{
					IdSms = wpSms.IdSms,
					Content = wpSms.Content,
					ContentSend = noidungGui,
					CreatorId = user.Id,
					CreateBy = wpSms.CreateBy,
					CreateAt = wpSms.CreateAt,
					NumberMessages = wpSms.NumberMessages,
					NumberMessageError = wpSms.NumberMessageError
				};

				return ServiceResult<MSmsViewModel>.Success(result);
			}
			catch (Exception ex)
			{
				// Cập nhật lỗi nếu wpSms đã được tạo
				if (wpSms != null)
				{
					try
					{
						wpSms.NumberMessageError = model.Employees?.Count ?? 0;
						await _mSmsRepository.Update(wpSms.IdSms, wpSms);
					}
					catch
					{
						// bỏ qua lỗi cập nhật
					}
				}

				return ServiceResult<MSmsViewModel>.Failure($"Lỗi khi send message: {ex.Message}");
			}
		}

		private async Task<string> HandleFileAttachments(List<IFormFile> fileDinhKem, List<long> selectedFileIds, Users user, long smsId, string domain)
		{
			List<string> fileUrls = new List<string>();

			// Xử lý file đính kèm mới
			if (fileDinhKem != null && fileDinhKem.Count > 0)
			{
				foreach (var file in fileDinhKem)
				{
					if (file.Length > 0)
					{
						var savedFile = await _mFileService.SaveFile(file, user, smsId);
						if (savedFile != null)
						{
							fileUrls.Add(savedFile.FileUrl);
						}
					}
				}
			}

			// Xử lý files đã chọn từ selectedFileIds
			if (selectedFileIds != null && selectedFileIds.Count > 0)
			{
				var createdFiles = await _mFileService.CreateFromFileExisted(selectedFileIds, user, smsId);
				fileUrls.AddRange(createdFiles.Select(f => f.FileUrl));
			}

			return string.Join(" ", fileUrls.Select(f => (domain + f).Replace("\\", "/").Replace("//", "/")));
		}

		private async Task SendMessageToCanbo(MEmployeeViewModel canbo, long smsId, SmsRes res)
		{
			if (canbo != null && canbo.IdEmployee.HasValue && canbo.IdGroup.HasValue)
			{
				MSmsEmployee wpSmsCanbo = new MSmsEmployee()
				{
					IdSms = smsId,
					IdEmployee = canbo.IdEmployee.Value,
					IdGroup = canbo.IdGroup.Value,
					REQID = res.RPLY.REQID,
					name = res.RPLY.name,
					ERROR = res.RPLY.ERROR,
					ERROR_DESC = res.RPLY.ERROR_DESC
				};
				await _mSmsEmployeeRepository.Create(wpSmsCanbo);
			}
		}

		public async Task<PageResult<MSmsViewModel>> SearchMessage(MSmsSearchViewModel model, Pageable pageable)
		{
			var user = await _currentUserService.GetCurrentUser();
			IQueryable<MSms> baseQuery = await _mSmsRepository.Search(model.searchInput, user.OrgId);

			if(model.dateFrom != null || model.dateTo != null)
			{
				baseQuery = baseQuery.Where(s => s.CreateAt >= model.dateFrom && s.CreateAt <= model.dateTo);
			}

			if (model.Status.HasValue)
			{
				if (model.Status.Value == 1)
				{
					baseQuery = baseQuery.Where(s => s.NumberMessageError == 0);
				}
				else if (model.Status.Value == 0)
				{
					baseQuery = baseQuery.Where(s => s.NumberMessageError > 0);
				}
			}


			var total = await baseQuery.CountAsync();
			var wpSmsPage = await _mSmsRepository.GetPagination(baseQuery, pageable);

			if (!wpSmsPage.Any())
			{
				return new PageResult<MSmsViewModel>
				{
					Data = new List<MSmsViewModel>(),
					Total = 0
				};
			}

			var fileList = _mFileRepository.Query();
			var smsFileList = _smsFileRepository.Query();
			var userList = await _wpUsersRepository.GetAll();
			var smsCanboList = _mSmsEmployeeRepository.Query();
			var canboList = _mEmployeeRepository.Query();
			var nhomList = _mFileRepository.Query();

			var wpSmsViewModel = from s in wpSmsPage
								 join u in userList on s.CreateBy equals u.UserName
								 select new MSmsViewModel
								 {
									 IdSms = s.IdSms,
									 Content = s.Content,
									 CreatorId = u.Id,
									 CreateBy = s.CreateBy,
									 CreateAt = s.CreateAt,
									 NumberMessages = s.NumberMessages,
									 NumberMessageError = s.NumberMessageError,
									 // sub query cho file
									 FileAttach = (from sf in smsFileList
													join f in fileList on sf.IdFile equals f.IdFile
													where sf.IdSms == s.IdSms && (f.IdFile == model.IdFile || model.IdFile == null)
													select f).ToList(),
								 };

			return new PageResult<MSmsViewModel>
			{
				Data = wpSmsViewModel,
				Total = total
			};
		}

		public async Task<MSmsViewModel?> GetSmsEmployeesById(MSmsSearchViewModel model)
		{
			if (model == null || model.IdSms <= 0)
				return null;
			var wpSms = await _mSmsRepository.FindById(model.IdSms.Value);
			if (wpSms == null)
				return null;

			var canbos = _mEmployeeRepository.Query().Where(item => item.IdEmployee == model.IdEmployee || model.IdEmployee == null);
			var nhoms = _mGroupRepository.Query().Where(item => item.IdGroup == model.IdGroup || model.IdGroup == null);
			var smsCanbos = _mSmsEmployeeRepository.Query();

			var smsViewModel = new MSmsViewModel
			{
				IdSms = wpSms.IdSms,
				Content = wpSms.Content,
				EmployeesView = (from sc in smsCanbos.Where(sc => sc.IdSms == wpSms.IdSms)
								join c in canbos on sc.IdEmployee equals c.IdEmployee
								join n in nhoms on sc.IdGroup equals n.IdGroup
								select new MEmployeeMessageStatisticalViewModel
								{
									IdEmployee = c.IdEmployee,
									Name = c.Name,
									PhoneNumber = c.PhoneNumber,
									IdGroup = n.IdGroup,
									GroupName = n.Name,
									ERROR = sc.ERROR,
									ERROR_DESC = sc.ERROR_DESC
								}).ToList()
			};

			return smsViewModel;
		}

	}

	public interface IMSmsService
	{
		Task<ServiceResult<MSmsViewModel>> SendMessage(MSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, Users user);
		Task<PageResult<MSmsViewModel>> SearchMessage(MSmsSearchViewModel model, Pageable pageable);
		Task<MSmsViewModel?> GetSmsEmployeesById(MSmsSearchViewModel id);
	}
}
