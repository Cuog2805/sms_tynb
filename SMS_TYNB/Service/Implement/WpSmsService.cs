using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service.Implement
{
	public class WpSmsService : IWpSmsService
	{
		private readonly WpSmsRepository _wpSmsRepository;
		private readonly WpSmsCanboRepository _wpSmsCanboRepository;
		private readonly IWpFileService _wpFileService;
		private readonly WpUsersRepository _wpUsersRepository;
		public WpSmsService
		(
			WpSmsRepository wpSmsRepository,
			WpSmsCanboRepository wpSmsCanboRepository,
			IWpFileService wpFileService,
			WpUsersRepository wpUsersRepository
		)
		{
			_wpSmsRepository = wpSmsRepository;
			_wpSmsCanboRepository = wpSmsCanboRepository;
			_wpFileService = wpFileService;
			_wpUsersRepository = wpUsersRepository;
		}
		public async Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, WpUsers user)
		{
			WpSms wpSms = new WpSms()
			{
				Noidung = model.Noidung,
				Ngaygui = DateTime.Now,
				IdNguoigui = user.Id,
				SoTn = model.WpCanbos.Count
			};
			wpSms = await _wpSmsRepository.Create(wpSms);

			// Xử lý file đính kèm
			if (fileDinhKem != null && fileDinhKem.Count > 0)
			{
				foreach (var file in fileDinhKem)
				{
					if (file.Length > 0)
					{
						var wpFile = await FileUpload.SaveFile(file, user, "wp_sms", (int)wpSms.IdSms);
						await _wpFileService.Create(wpFile);
					}
				}
			}

			// Xử lý gửi tin nhắn cho cán bộ
			foreach (var item in model.WpCanbos)
			{
				if (item.IdNhom.HasValue)
				{
					WpSmsCanbo wpSmsCanbo = new WpSmsCanbo()
					{
						IdSms = wpSms.IdSms,
						IdCanbo = item.IdCanbo,
						IdNhom = item.IdNhom.Value
					};
					await _wpSmsCanboRepository.Create(wpSmsCanbo);
				}
			}
		}
		public async Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable)
		{
			IQueryable<WpSms> wpSms = await _wpSmsRepository.Search(model.searchInput);

			IEnumerable<WpSms> wpSmsPage = await _wpSmsRepository.GetPagination(wpSms, pageable);

			var wpFileList = await _wpFileService.GetByBangLuuFile("wp_sms");
			var wpUserList = await _wpUsersRepository.GetAll();

			var wpSmsViewModel = from wps in wpSmsPage
								 join wpf in wpFileList on wps.IdSms equals wpf.BangLuuFileId
								 join wpu in wpUserList on wps.IdNguoigui equals wpu.Id
								 where (wps.Ngaygui >= model.dateFrom && wps.Ngaygui <= model.dateTo)
								 group new { wps, wpf , wpu } by new { wps, wpu } into wpsGroup
								 select new WpSmsViewModel
								 {
									 IdSms = wpsGroup.Key.wps.IdSms,
									 Noidung = wpsGroup.Key.wps.Noidung,
									 FileDinhKem = wpsGroup.Select(x => x.wpf).ToList(),
									 IdNguoigui = wpsGroup.Key.wps.IdNguoigui,
									 TenNguoigui = wpsGroup.Key.wpu.UserName,
									 Ngaygui = wpsGroup.Key.wps.Ngaygui,
									 SoTn = wpsGroup.Key.wps.SoTn
								 };


			int total = await wpSms.CountAsync();

			return new PageResult<WpSmsViewModel>
			{
				Data = wpSmsViewModel,
				Total = total,
			};
		}

	}
}
