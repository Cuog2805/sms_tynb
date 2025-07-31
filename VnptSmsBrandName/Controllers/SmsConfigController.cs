using Microsoft.AspNetCore.Mvc;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
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
				msg = "Thêm m?i thành công!",
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
				msg = "C?p nh?t thành công!",
				data = model
			});
		}
	}
}
