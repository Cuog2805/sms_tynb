using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;
using static SMS_TYNB.ViewModel.ApiModel.SmsApiViewModel;

namespace SMS_TYNB.Service.Implement
{
	public class WpSmsService : IWpSmsService
	{
		private readonly WpSmsRepository _wpSmsRepository;
		private readonly WpSmsCanboRepository _wpSmsCanboRepository;
		private readonly WpUsersRepository _wpUsersRepository;
		private readonly IWpFileService _wpFileService;
		private readonly ISmsConfigService _smsConfigService;
		private readonly ILogger<WpSmsService> _logger;
		public WpSmsService
		(
			WpSmsRepository wpSmsRepository,
			WpSmsCanboRepository wpSmsCanboRepository,
			IWpFileService wpFileService,
			WpUsersRepository wpUsersRepository,
			ILogger<WpSmsService> logger,
			ISmsConfigService smsConfigService
		)
		{
			_wpSmsRepository = wpSmsRepository;
			_wpSmsCanboRepository = wpSmsCanboRepository;
			_wpFileService = wpFileService;
			_wpUsersRepository = wpUsersRepository;
			_logger = logger;
			_smsConfigService = smsConfigService;
		}
		public async Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user)
		{
			WpSms wpSms = new WpSms()
			{
				Noidung = model.Noidung,
				Ngaygui = DateTime.Now,
				IdNguoigui = user.Id,
				SoTn = model.WpCanbos.Count,
				SoTnLoi = 0
			};
			wpSms = await _wpSmsRepository.Create(wpSms);

			int errorCount = 0;
			int successCount = 0;


			// Xử lý file đính kèm
			await HandleFileAttachments(fileDinhKem, selectedFileIds, user, wpSms.IdSms);

			// Gửi tin nhắn qua dịch vụ SMS
			var smsConfig = _smsConfigService.GetSmsConfigActive(true);
			if (smsConfig?.Id > 0)
			{
				var phoneNumbers = model.WpCanbos.Select(c => c.SoDTGui)
												.Where(s => !string.IsNullOrEmpty(s))
												.ToList();

				if (phoneNumbers.Count > 0)
				{
					phoneNumbers.ForEach(phoneNumber =>
					{
						if(!string.IsNullOrEmpty(phoneNumber))
						{
							var res = SmsHelper.SendSms(smsConfig, model.Noidung ?? " ", phoneNumber);

							if (res.RPLY.ERROR == "0")
							{
								successCount += 1;
							}
							else
							{
								errorCount += 1;
							}
						}
					});
				}
			}
			else
			{
				errorCount = model.WpCanbos.Count;
				successCount = 0;
			}

			// Xử lý gán tin nhắn cho cán bộ
			var sendTasks = model.WpCanbos.Where(item => item.IdNhom.HasValue)
										  .Select(item => SendMessageToCanbo(item, wpSms.IdSms));
			await Task.WhenAll(sendTasks);

			// Cập nhật số liệu thống kê
			wpSms.SoTn = successCount;
			wpSms.SoTnLoi = errorCount;
			await _wpSmsRepository.Update(wpSms.IdSms, wpSms);
		}

		private async Task HandleFileAttachments(List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user, long smsId)
		{
			try
			{
				// Xử lý file đính kèm mới
				if (fileDinhKem != null && fileDinhKem.Count > 0)
				{
					var uploadTasks = fileDinhKem.Where(file => file.Length > 0)
											   .Select(file => _wpFileService.SaveFile(file, user, "wp_sms", smsId));
					await Task.WhenAll(uploadTasks);
				}

				// Xử lý files đã chọn từ selectedFileIds
				if (selectedFileIds != null && selectedFileIds.Count > 0)
				{
					await _wpFileService.CreateFromFileExisted(selectedFileIds, user, "wp_sms", smsId);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error HandleFileAttachments for SMS {SmsId}", smsId);
			}
		}

		private async Task SendMessageToCanbo(WpCanboViewModel canbo, long smsId)
		{
			if (canbo != null && canbo.IdCanbo.HasValue && canbo.IdNhom.HasValue)
			{
				WpSmsCanbo wpSmsCanbo = new WpSmsCanbo()
				{
					IdSms = smsId,
					IdCanbo = canbo.IdCanbo.Value,
					IdNhom = canbo.IdNhom.Value
				};
				await _wpSmsCanboRepository.Create(wpSmsCanbo);
			}
		}
		public async Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable)
		{
			IQueryable<WpSms> wpSms = await _wpSmsRepository.Search(model.searchInput);

			if (model.dateFrom.HasValue && model.dateTo.HasValue)
			{
				wpSms = wpSms.Where(wps => wps.Ngaygui >= model.dateFrom && wps.Ngaygui <= model.dateTo);
			}

			int total = await wpSms.CountAsync();

			IEnumerable<WpSms> wpSmsPage = await _wpSmsRepository.GetPagination(wpSms, pageable);

			var wpFileList = await _wpFileService.GetByBangLuuFile("wp_sms");
			var wpUserList = await _wpUsersRepository.GetAll();

			var wpSmsViewModel = from wps in wpSmsPage
								 join wpf in wpFileList on wps.IdSms equals wpf.BangLuuFileId into wpsWithFileGroup
								 from gwpf in wpsWithFileGroup.DefaultIfEmpty()
								 join wpu in wpUserList on wps.IdNguoigui equals wpu.Id
								 group new { wps, gwpf, wpu } by new { wps, wpu } into wpsGroup
								 select new WpSmsViewModel
								 {
									 IdSms = wpsGroup.Key.wps.IdSms,
									 Noidung = wpsGroup.Key.wps.Noidung,
									 FileDinhKem = wpsGroup.Where(x => x.gwpf != null).Select(x => x.gwpf).ToList(),
									 IdNguoigui = wpsGroup.Key.wps.IdNguoigui,
									 TenNguoigui = wpsGroup.Key.wpu.UserName,
									 Ngaygui = wpsGroup.Key.wps.Ngaygui,
									 SoTn = wpsGroup.Key.wps.SoTn,
									 SoTnLoi = wpsGroup.Key.wps.SoTnLoi
								 };

			return new PageResult<WpSmsViewModel>
			{
				Data = wpSmsViewModel,
				Total = total,
			};
		}

		public async Task UpdateFile(WpFile oldFile, IFormFile fileDinhKem)
		{
			await _wpFileService.UpdateContentFile(fileDinhKem, oldFile, new WpUsers());
		}

	}
}
