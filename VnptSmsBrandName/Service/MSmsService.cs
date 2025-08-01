using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using VnptSmsBrandName.ViewModel;
using static VnptSmsBrandName.ViewModel.ApiModel.SmsApiViewModel;

namespace VnptSmsBrandName.Service
{
	public class MSmsService: IMSmsService
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
			MSmsRepository mSmsRepository,
			MSmsEmployeeRepository mSmsEmployeeRepository,
			WpUsersRepository wpUsersRepository,
			MEmployeeRepository mEmployeeRepository,
			MGroupRepository mGroupRepository,
			MFileRepository mFileRepository,
			MSmsFileRepository smsFileRepository,
			IMFileService mFileService,
			ISmsConfigService smsConfigService
		)
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
					CreatedBy = user.UserName,
					CreatedAt = DateTime.Now,
					OrganizationId = user.OrganizationId,
				};

				wpSms = await _mSmsRepository.Create(wpSms);
				int errorCount = 0;

				// Gửi tin nhắn
				var smsConfig = _smsConfigService.GetSmsConfigByOrgIdAndActive(user.OrganizationId);

				// Xử lý file đính kèm
				var fileUrls = await HandleFileAttachments(fileDinhKem, selectedFileIds, user, wpSms.SmsId, smsConfig.Domain);
				var noidungGui = model.Content + " " + fileUrls;

				if (smsConfig?.Id > 0 && model.Employees.Any())
				{
					foreach (var canbo in model.Employees)
					{
						try
						{
							var cb = _mEmployeeRepository.FindById(canbo.EmployeeId ?? 0).Result;
							if (cb != null && cb.EmployeeId > 0)
							{
								var res = SmsHelper.SendSms(smsConfig, noidungGui, cb.PhoneNumberSend);
								await SendMessageToCanbo(canbo, wpSms.SmsId, res, user);
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
				wpSms = await _mSmsRepository.Update(wpSms.SmsId, wpSms);

				var result = new MSmsViewModel()
				{
					SmsId = wpSms.SmsId,
					Content = wpSms.Content,
					ContentSend = noidungGui,
					CreatorId = user.Id,
					CreatedBy = wpSms.CreatedBy,
					CreatedAt = wpSms.CreatedAt,
					NumberMessages = wpSms.NumberMessages,
					NumberMessageError = wpSms.NumberMessageError
				};

				return ServiceResult<MSmsViewModel>.Success(result);
			}
			catch (Exception ex)
			{
				// Câp nhật lại số lượng tin nhắn lỗi trong trường hợp có lỗi xảy ra
				if (wpSms != null)
				{
					try
					{
						wpSms.NumberMessageError = model.Employees?.Count ?? 0;
						await _mSmsRepository.Update(wpSms.SmsId, wpSms);
					}
					catch
					{
						// bỏ qua lỗi cập nhật số lượng tin nhắn lỗi
					}
				}

				return ServiceResult<MSmsViewModel>.Failure($"Lỗi khi send message: {ex.Message}");
			}
		}

		private async Task<string> HandleFileAttachments(List<IFormFile> fileDinhKem, List<long> selectedFileIds, Users user, long smsId, string domain)
		{
			List<string> fileUrls = new List<string>();

			// Xử lý files đính kèm từ input
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

			// Xử lý files đính kèm từ selectedFileIds
			if (selectedFileIds != null && selectedFileIds.Count > 0)
			{
				var createdFiles = await _mFileService.CreateFromFileExisted(selectedFileIds, user, smsId);
				fileUrls.AddRange(createdFiles.Select(f => f.FileUrl));
			}

			return string.Join(" ", fileUrls.Select(f => (domain + f).Replace("\\", "/").Replace("//", "/")));
		}

		private async Task SendMessageToCanbo(MEmployeeViewModel canbo, long smsId, SmsRes res, Users creator)
		{
			if (canbo != null && canbo.EmployeeId.HasValue && canbo.IdGroup.HasValue)
			{
				MSmsEmployee wpSmsCanbo = new MSmsEmployee()
				{
					SmsId = smsId,
					EmployeeId = canbo.EmployeeId.Value,
					GroupId = canbo.IdGroup.Value,
					REQID = res.RPLY.REQID,
					name = res.RPLY.name,
					ERROR = res.RPLY.ERROR,
					ERROR_DESC = res.RPLY.ERROR_DESC,
					OrganizationId = creator.OrganizationId
				};
				AuditHelper.SetCreateAudit(wpSmsCanbo, creator);
				await _mSmsEmployeeRepository.Create(wpSmsCanbo);
			}
		}

		public async Task<PageResult<MSmsViewModel>> SearchMessage(MSmsSearchViewModel model, Pageable pageable, long orgId)
		{
			IQueryable<MSms> baseQuery = await _mSmsRepository.Search(model.searchInput, orgId);

			if(model.dateFrom != null || model.dateTo != null)
			{
				baseQuery = baseQuery.Where(s => s.CreatedAt >= model.dateFrom && s.CreatedAt <= model.dateTo);
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

			var userList = await _wpUsersRepository.GetAll();
			var fileList = _mFileRepository.GetAllByOrgId(orgId);
			var smsFileList = _smsFileRepository.GetAllByOrgId(orgId);
			var smsCanboList = _mSmsEmployeeRepository.GetAllByOrgId(orgId);
			var canboList = _mEmployeeRepository.GetAllByOrgId(orgId);
			var nhomList = _mFileRepository.GetAllByOrgId(orgId);

			var wpSmsViewModel = from s in wpSmsPage
								 join u in userList on s.CreatedBy equals u.UserName
								 select new MSmsViewModel
								 {
									 SmsId = s.SmsId,
									 Content = s.Content,
									 CreatorId = u.Id,
									 CreatedBy = s.CreatedBy,
									 CreatedAt = s.CreatedAt,
									 NumberMessages = s.NumberMessages,
									 NumberMessageError = s.NumberMessageError,
									 // sub query cho file
									 FileAttach = (from sf in smsFileList
													join f in fileList on sf.FileId equals f.FileId
													where sf.SmsId == s.SmsId && (f.FileId == model.FileId || model.FileId == null)
													select f).ToList(),
								 };

			return new PageResult<MSmsViewModel>
			{
				Data = wpSmsViewModel,
				Total = total
			};
		}

		public async Task<MSmsViewModel?> GetSmsEmployeesById(MSmsSearchViewModel model, long orgId)
		{
			if (model == null || model.SmsId <= 0)
				return null;
			var wpSms = await _mSmsRepository.FindById(model.SmsId.Value);
			if (wpSms == null)
				return null;

			var canbos = _mEmployeeRepository.GetAllByOrgId(orgId).Where(item => item.EmployeeId == model.EmployeeId || model.EmployeeId == null);
			var nhoms = _mGroupRepository.GetAllByOrgId(orgId).Where(item => item.GroupId == model.GroupId || model.GroupId == null);
			var smsCanbos = _mSmsEmployeeRepository.GetAllByOrgId(orgId);

			var smsViewModel = new MSmsViewModel
			{
				SmsId = wpSms.SmsId,
				Content = wpSms.Content,
				EmployeesView = (from sc in smsCanbos.Where(sc => sc.SmsId == wpSms.SmsId)
								join c in canbos on sc.EmployeeId equals c.EmployeeId
								join n in nhoms on sc.GroupId equals n.GroupId
								select new MEmployeeMessageStatisticalViewModel
								{
									EmployeeId = c.EmployeeId,
									Name = c.Name,
									PhoneNumber = c.PhoneNumber,
									GroupId = n.GroupId,
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
		Task<PageResult<MSmsViewModel>> SearchMessage(MSmsSearchViewModel model, Pageable pageable, long orgId);
		Task<MSmsViewModel?> GetSmsEmployeesById(MSmsSearchViewModel id, long orgId);
	}
}
