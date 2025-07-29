using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;
using System.Collections.Generic;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;

namespace SMS_TYNB.Service.Implement
{
	public class WpSmsService : IWpSmsService
	{
		private readonly WpSmsRepository _wpSmsRepository;
		private readonly WpSmsCanboRepository _wpSmsCanboRepository;
		private readonly WpUsersRepository _wpUsersRepository;
		private readonly WpCanboRepository _wpCanboRepository;
		private readonly WpNhomCanboRepository _wpNhomCanboRepository;
		private readonly WpNhomRepository _wpNhomRepository;
		private readonly WpFileRepository _wpFileRepository;
		private readonly IWpFileService _wpFileService;
		private readonly ISmsConfigService _smsConfigService;
		private readonly ILogger<WpSmsService> _logger;
		private readonly WpSmsFileRepository _wpSmsFileRepository;

		public WpSmsService
		(
			WpSmsRepository wpSmsRepository,
			WpSmsCanboRepository wpSmsCanboRepository,
			IWpFileService wpFileService,
			WpUsersRepository wpUsersRepository,
			ILogger<WpSmsService> logger,
			ISmsConfigService smsConfigService,
			WpCanboRepository wpCanboRepository,
			WpNhomCanboRepository wpNhomCanboRepository,
			WpNhomRepository wpNhomRepository,
			WpFileRepository wpFileRepository,
			WpSmsFileRepository wpSmsFileRepository
		)
		{
			_wpSmsRepository = wpSmsRepository;
			_wpSmsCanboRepository = wpSmsCanboRepository;
			_wpFileService = wpFileService;
			_wpUsersRepository = wpUsersRepository;
			_logger = logger;
			_smsConfigService = smsConfigService;
			_wpCanboRepository = wpCanboRepository;
			_wpNhomCanboRepository = wpNhomCanboRepository;
			_wpNhomRepository = wpNhomRepository;
			_wpFileRepository = wpFileRepository;
			_wpSmsFileRepository = wpSmsFileRepository;
		}

		public async Task<ServiceResult<WpSmsViewModel>> SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user)
		{
			// validate input
			if (user == null)
				return ServiceResult<WpSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Lỗi khi thông tin người dùng!");

			if (string.IsNullOrEmpty(model.Noidung))
				return ServiceResult<WpSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Không có nội dung tin nhắn!");

			if (model.WpCanbos.Count == 0)
				return ServiceResult<WpSmsViewModel>.Failure("Lỗi khi xử lý SendMessage: Chưa chọn người nhận tin nhắn!");

			WpSms wpSms = null;
			try
			{
				wpSms = new WpSms()
				{
					Noidung = model.Noidung,
					Ngaygui = DateTime.Now,
					IdNguoigui = user.Id,
					SoTn = model.WpCanbos.Count,
					SoTnLoi = 0
				};

				wpSms = await _wpSmsRepository.Create(wpSms);
				int errorCount = 0;

				// Gửi tin nhắn
				var smsConfig = _smsConfigService.GetSmsConfigActive(true);

				// Xử lý file đính kèm
				var fileUrls = await HandleFileAttachments(fileDinhKem, selectedFileIds, user, wpSms.IdSms, smsConfig.Domain);
				var noidungGui = model.Noidung + " " + fileUrls;

				if (smsConfig?.Id > 0 && model.WpCanbos.Any())
				{
					foreach (var canbo in model.WpCanbos)
					{
						try
						{
							var cb = _wpCanboRepository.FindById(canbo.IdCanbo ?? 0).Result;
							if (cb != null && cb.IdCanbo > 0)
							{
								var res = SmsHelper.SendSms(smsConfig, noidungGui, cb.SoDTGui);
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
					errorCount = model.WpCanbos?.Count ?? 0;
				}

				wpSms.SoTnLoi = errorCount;
				wpSms = await _wpSmsRepository.Update(wpSms.IdSms, wpSms);

				var result = new WpSmsViewModel()
				{
					IdSms = wpSms.IdSms,
					Noidung = wpSms.Noidung,
					NoidungGui = noidungGui,
					IdNguoigui = wpSms.IdNguoigui,
					TenNguoigui = user.UserName,
					Ngaygui = wpSms.Ngaygui,
					SoTn = wpSms.SoTn,
					SoTnLoi = wpSms.SoTnLoi
				};

				return ServiceResult<WpSmsViewModel>.Success(result);
			}
			catch (Exception ex)
			{
				// Cập nhật lỗi nếu wpSms đã được tạo
				if (wpSms != null)
				{
					try
					{
						wpSms.SoTnLoi = model.WpCanbos?.Count ?? 0;
						await _wpSmsRepository.Update(wpSms.IdSms, wpSms);
					}
					catch
					{
						// bỏ qua lỗi cập nhật
					}
				}

				return ServiceResult<WpSmsViewModel>.Failure($"Lỗi khi send message: {ex.Message}");
			}
		}

		private async Task<string> HandleFileAttachments(List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user, long smsId, string domain)
		{
			List<string> fileUrls = new List<string>();

			// Xử lý file đính kèm mới
			if (fileDinhKem != null && fileDinhKem.Count > 0)
			{
				foreach (var file in fileDinhKem)
				{
					if (file.Length > 0)
					{
						var savedFile = await _wpFileService.SaveFile(file, user, smsId);
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
				var createdFiles = await _wpFileService.CreateFromFileExisted(selectedFileIds, user, smsId);
				fileUrls.AddRange(createdFiles.Select(f => f.FileUrl));
			}

			return string.Join(" ", fileUrls.Select(f => (domain + f).Replace("\\", "/").Replace("//", "/")));
		}

		private async Task SendMessageToCanbo(WpCanboViewModel canbo, long smsId, SmsRes res)
		{
			if (canbo != null && canbo.IdCanbo.HasValue && canbo.IdNhom.HasValue)
			{
				WpSmsCanbo wpSmsCanbo = new WpSmsCanbo()
				{
					IdSms = smsId,
					IdCanbo = canbo.IdCanbo.Value,
					IdNhom = canbo.IdNhom.Value,
					REQID = res.RPLY.REQID,
					name = res.RPLY.name,
					ERROR = res.RPLY.ERROR,
					ERROR_DESC = res.RPLY.ERROR_DESC
				};
				await _wpSmsCanboRepository.Create(wpSmsCanbo);
			}
		}

		public async Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable)
		{
			IQueryable<WpSms> baseQuery = await _wpSmsRepository.Search(model.searchInput);

			if (model.Trangthai.HasValue)
			{
				if(model.Trangthai.Value == 1)
				{
					baseQuery = baseQuery.Where(wps => wps.SoTnLoi == 0);
				}
				else if (model.Trangthai.Value == 0)
				{
					baseQuery = baseQuery.Where(wps => wps.SoTnLoi > 0);
				}
			}


			var total = await baseQuery.CountAsync();
			var wpSmsPage = await _wpSmsRepository.GetPagination(baseQuery, pageable);

			if (!wpSmsPage.Any())
			{
				return new PageResult<WpSmsViewModel>
				{
					Data = new List<WpSmsViewModel>(),
					Total = 0
				};
			}

			var wpFileList = await _wpFileRepository.GetAll();
			var wpSmsFileList = await _wpSmsFileRepository.GetAll();
			var wpUserList = await _wpUsersRepository.GetAll();
			var wpSmsCanboList = await _wpSmsCanboRepository.GetAll();
			var wpCanboList = await _wpCanboRepository.GetAll();
			var wpNhomList = await _wpNhomRepository.GetAll();

			var wpSmsViewModel = from wps in wpSmsPage
								 join wpu in wpUserList on wps.IdNguoigui equals wpu.Id
								 select new WpSmsViewModel
								 {
									 IdSms = wps.IdSms,
									 Noidung = wps.Noidung,
									 IdNguoigui = wps.IdNguoigui,
									 TenNguoigui = wpu.UserName,
									 Ngaygui = wps.Ngaygui,
									 SoTn = wps.SoTn,
									 SoTnLoi = wps.SoTnLoi,
									 // sub query cho file
									 //FileDinhKem = wpFileList.Where(f => f.BangLuuFileId == wps.IdSms && (f.IdFile == model.IdFile || model.IdFile == null)).ToList(),
									 FileDinhKem = (from wpsf in wpSmsFileList
													join wpf in wpFileList on wpsf.IdFile equals wpf.IdFile
													where wpsf.IdSms == wps.IdSms && (wpf.IdFile == model.IdFile || model.IdFile == null)
													select wpf).ToList(),
									 // sub query cho cán bộ
									 WpCanbosView = (from wpsc in wpSmsCanboList.Where(sc => sc.IdSms == wps.IdSms)
												 join wpc in wpCanboList on wpsc.IdCanbo equals wpc.IdCanbo
												 join wpn in wpNhomList on wpsc.IdNhom equals wpn.IdNhom
												 where (wpsc.IdCanbo == model.IdCanBo || model.IdCanBo == null)
												 && (wpsc.IdNhom == model.IdNhom || model.IdNhom == null)
												 select new WpCanboMessageStatisticalViewModel
												 {
													 IdCanbo = wpc.IdCanbo,
													 TenCanbo = wpc.TenCanbo,
													 SoDt = wpc.SoDt,
													 IdNhom = wpn.IdNhom,
													 TenNhom = wpn.TenNhom,
													 ERROR = wpsc.ERROR,
													 ERROR_DESC = wpsc.ERROR_DESC
												 }).ToList()
								 };

			return new PageResult<WpSmsViewModel>
			{
				Data = wpSmsViewModel,
				Total = total
			};
		}

		public async Task UpdateFile(WpFile oldFile, IFormFile fileDinhKem)
		{
			await _wpFileService.UpdateContentFile(fileDinhKem, oldFile, new WpUsers());
		}
	}
}