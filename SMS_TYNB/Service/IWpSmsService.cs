using SMS_TYNB.Models;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public interface IWpSmsService
	{
		Task SendMessage(WpSmsViewModel model, List<IFormFile> fileDinhKem);
	}
}
