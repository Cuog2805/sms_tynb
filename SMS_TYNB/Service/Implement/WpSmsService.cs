using SMS_TYNB.Helper;
using SMS_TYNB.Models;
using SMS_TYNB.Repository;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service.Implement
{
	public class WpSmsService : IWpSmsService
	{
		private readonly WpSmsRepository _wpSmsRepository;
		private readonly WpSmsCanboRepository _wpSmsCanboRepository;
		private readonly IWpFileService _wpFileService;
		public WpSmsService
		(
			WpSmsRepository wpSmsRepository,
			WpSmsCanboRepository wpSmsCanboRepository,
			IWpFileService wpFileService
		)
		{
			_wpSmsRepository = wpSmsRepository;
			_wpSmsCanboRepository = wpSmsCanboRepository;
			_wpFileService = wpFileService;
		}
		public async Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem)
		{
			WpSms wpSms = new WpSms()
			{
				Noidung = model.Noidung,
				Ngaygui = DateTime.Now,
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
						var wpFile = await FileUpload.SaveFile(file, "Wp_sms", (int)wpSms.IdSms);
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
	}
}
