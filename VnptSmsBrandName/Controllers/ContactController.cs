using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
{
	[Authorize(Roles = "Admin")]
	public class ContactController : BaseController
	{
		private readonly IMEmployeeService _employeeService;
		public ContactController
		(
			IMEmployeeService employeeService, 
			IHttpContextAccessor httpContextAccessor, 
			UserManager<Users> userManager
		) : base(userManager, httpContextAccessor)
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
			var currentUser = await GetCurrentUser();
			var datas = await _employeeService.SearchMEmployee(model, pageable, currentUser.OrganizationId);
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
			var currentUser = await GetCurrentUser();
			var data = await _employeeService.GetByIdAndOrgId(id, currentUser.OrganizationId);
			BaseFormViewModel<MEmployee> formViewModel = await CreateFormViewModel(data);
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(MEmployee model)
		{
			var currentUser = await GetCurrentUser();
			MEmployee result = await _employeeService.Create(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Thêm mới thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(MEmployee model)
		{
			var currentUser = await GetCurrentUser();
			MEmployee? result = await _employeeService.Update(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Import(List<MEmployee> model)
		{
			var currentUser = await GetCurrentUser();
			MEmployeeCreateRangeViewModel data = await _employeeService.CreateMulti(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Import thành công!",
				data = data
			});
		}
	}
}
