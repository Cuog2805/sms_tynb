using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Helper;
using SMS_TYNB.Models;
using SMS_TYNB.Service;
using SMS_TYNB.ViewModel;
using System.Threading.Tasks;

namespace SMS_TYNB.Controllers
{
	public class MessageController : Controller
	{
		private readonly IWpSmsService _wpSmsService;
		public MessageController(IWpSmsService wpSmsService) 
		{
			_wpSmsService = wpSmsService;
		}
		public async Task<IActionResult> Index()
		{
			BaseFormViewModel<WpSms> formViewModel = new BaseFormViewModel<WpSms>();
			return View(formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> SendMessage([FromForm] WpSmsViewModel model, List<IFormFile> fileDinhKem)
		{
			await _wpSmsService.SendMessage(model, fileDinhKem);
			return Json(new
			{
				state = "success",
				msg = "Gửi tin nhắn thành công!",
				data = new { },
			});
		}
		//[HttpPost]
		//public async Task<IActionResult> UploadFile(IFormFile file)
		//{
		//	if (file == null || file.Length == 0)
		//		return BadRequest("File không hợp lệ.");

		//	var rootPath = Directory.GetCurrentDirectory();
		//	var fileUrl = await FileUpload.UploadFileAsync(file, rootPath);

		//	if (fileUrl != null)
		//	{
		//		var sms = new WpSms
		//		{
		//			FileDinhKem = fileUrl
		//		};

		//		_context.WpSms.Add(sms);
		//		await _context.SaveChangesAsync();

		//		return Json(new { success = true, fileUrl });
		//	}

		//	return BadRequest("Upload thất bại.");
		//}

	}
}
