using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;
using SMS_TYNB.Service.Implement;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ContactController : Controller
	{
		private readonly IWpCanboService _wpCanboService;
		private readonly IWpDanhmucService _wpDanhmucService;
		public ContactController(IWpCanboService wpCanboService, IWpDanhmucService wpDanhmucService) 
		{
			_wpCanboService = wpCanboService;
			_wpDanhmucService = wpDanhmucService;
		}
		private async Task<BaseFormViewModel<WpCanbo>> CreateFormViewModel(WpCanbo? model = null)
		{
			SelectList wpTrangThaiSelectList = new SelectList(await _wpDanhmucService.GetWpDanhmucByType("TRANGTHAI"), "Value", "TenDanhmuc");
			SelectList wpGioiTinhSelectList = new SelectList(await _wpDanhmucService.GetWpDanhmucByType("GIOITINH"), "Value", "TenDanhmuc");
			var selectLists = new Dictionary<string, SelectList>
			{
				{ "wpTrangThaiSelectList", wpTrangThaiSelectList },
				{ "wpGioiTinhSelectList", wpGioiTinhSelectList }
			};
			BaseFormViewModel<WpCanbo> formViewModel = new BaseFormViewModel<WpCanbo>()
			{
				Data = model ??= new WpCanbo(),
				SelectLists = selectLists
			};
			return formViewModel;
		}
		public async Task<IActionResult> Index() 
		{
			BaseFormViewModel<WpCanbo> formViewModel = await CreateFormViewModel();
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(WpCanboSearchViewModel model, Pageable pageable)
		{
			var datas = await _wpCanboService.SearchWpCanbo(model, pageable);
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
			var data = await _wpCanboService.GetById(id);
			BaseFormViewModel<WpCanbo> formViewModel = await CreateFormViewModel(data);
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(WpCanbo model)
		{
			WpCanbo result = await _wpCanboService.Create(model);
			return Json(new
			{
				state = "success",
				msg = "Thêm mới thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(WpCanbo model)
		{
			WpCanbo result = await _wpCanboService.Update(model);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
	}
}
