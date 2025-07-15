using SMS_TYNB.Common;
using SMS_TYNB.Models;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public interface IWpSmsService
	{
		Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, WpUsers user);
		Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable);
	}
}
