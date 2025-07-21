using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Common;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;
using SMS_TYNB.Service.Implement;
using SMS_TYNB.ViewModel;
using System.Threading.Tasks;

namespace SMS_TYNB.Controllers
{
	[Authorize(Roles = "Admin, User")]
	public class MessageController : Controller
	{
		private readonly IWpSmsService _wpSmsService;
		private readonly UserManager<WpUsers> _userManager;
		public MessageController(IWpSmsService wpSmsService, UserManager<WpUsers> userManager) 
		{
			_wpSmsService = wpSmsService;
			_userManager = userManager;
		}
		public IActionResult SendMessage()
		{
			BaseFormViewModel<WpSms> formViewModel = new BaseFormViewModel<WpSms>();
			return View(formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> SendMessage([FromForm] WpSmsViewModel model, List<IFormFile> fileDinhKem, List<long> selectedFileIds)
		{
			// gán người gửi
			WpUsers? user = await _userManager.GetUserAsync(HttpContext.User);
			if (user != null) await _wpSmsService.SendMessage(model, fileDinhKem, selectedFileIds, user);
			return Json(new
			{
				state = "success",
				msg = "Gửi tin nhắn thành công!",
				data = new { },
			});
		}
		public async Task<IActionResult> MessageStatistical()
		{
			return View();
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(WpSmsSearchViewModel model, Pageable pageable)
		{
			var datas = await _wpSmsService.SearchMessage(model, pageable);
			return Json(new
			{
				state = "success",
				msg = "LoadData thành công!",
				content = datas,
			});
		}

		[HttpPost]
		public async Task<IActionResult> MessageUpdateFile([FromForm]WpFile oldFile, IFormFile fileDinhKem)
		{
			await _wpSmsService.UpdateFile(oldFile, fileDinhKem);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật file thành công!",
				content = new {},
			});
		}

	}
}
