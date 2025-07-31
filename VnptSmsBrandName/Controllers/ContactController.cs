using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ContactController : Controller
	{
		private readonly IMEmployeeService _employeeService;
		public ContactController(IMEmployeeService employeeService) 
		{
			_employeeService = employeeService;
		}
		private async Task<BaseFormViewModel<MEmployee>> CreateFormViewModel(MEmployee? model = null)
		{
			SelectList statusSelectList = new SelectList(EnumHelper.ToSelectListItem<DeletedEnum>(), "Value", "Text");
			SelectList genderSelectList = new SelectList(EnumHelper.ToSelectListItem<GenderEnum>(), "Value", "Text");
			var selectLists = new Dictionary<string, SelectList>
			{
				{ "statusSelectList", statusSelectList },
				{ "genderSelectList", genderSelectList }
			};
			BaseFormViewModel<MEmployee> formViewModel = new BaseFormViewModel<MEmployee>()
			{
				Data = model ??= new MEmployee(),
				SelectLists = selectLists
			};
			return formViewModel;
		}
		public async Task<IActionResult> Index() 
		{
			BaseFormViewModel<MEmployee> formViewModel = await CreateFormViewModel();
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(MEmployeeSearchViewModel model, Pageable pageable)
		{
			var datas = await _employeeService.SearchMEmployee(model, pageable);
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
			var data = await _employeeService.GetById(id);
			BaseFormViewModel<MEmployee> formViewModel = await CreateFormViewModel(data);
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(MEmployee model)
		{
			MEmployee result = await _employeeService.Create(model);
			return Json(new
			{
				state = "success",
				msg = "Thêm m?i thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(MEmployee model)
		{
			MEmployee? result = await _employeeService.Update(model);
			return Json(new
			{
				state = "success",
				msg = "C?p nh?t thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Import(List<MEmployee> model)
		{
			MEmployeeCreateRangeViewModel data = await _employeeService.CreateMulti(model);
			return Json(new
			{
				state = "success",
				msg = "Import thành công!",
				data = data
			});
		}
	}
}
