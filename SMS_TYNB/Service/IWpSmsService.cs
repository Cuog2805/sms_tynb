using SMS_TYNB.Common;
using SMS_TYNB.Models;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public interface IWpSmsService
	{
		Task<WpSmsViewModel> SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds, WpUsers user);
		Task<PageResult<WpSmsViewModel>> SearchMessage(WpSmsSearchViewModel model, Pageable pageable);
		Task UpdateFile(WpFile oldFile, IFormFile fileDinhKem);
	}
}
