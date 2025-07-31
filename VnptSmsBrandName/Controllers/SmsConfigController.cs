using Microsoft.AspNetCore.Mvc;
using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Controllers
{
	public class SmsConfigController : Controller
	{
		private readonly ISmsConfigService _smsConfigService;
		public SmsConfigController(ISmsConfigService smsConfigService) 
		{ 
			_smsConfigService = smsConfigService;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> LoadData(SmsConfigSearchViewModel model, Pageable pageable)
		{
			var datas = await _smsConfigService.SearchSmsConfig(model, pageable);
			return Json(new
			{
				state = "success",
				msg = "LoadData thành công!",
				content = datas
			});
		}
		[HttpGet]
		public async Task<IActionResult> LoadDetail(int id)
		{
			var data = await _smsConfigService.GetById(id);
			return PartialView("_Form", data);
		}
		[HttpPost]
		public async Task<JsonResult> Create(SmsConfig model)
		{
			SmsConfig result = await _smsConfigService.Create(model);
			return Json(new
			{
				state = "success",
				msg = "Thêm mới thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(SmsConfig model)
		{
			SmsConfig? result = await _smsConfigService.Update(model);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
	}
}
