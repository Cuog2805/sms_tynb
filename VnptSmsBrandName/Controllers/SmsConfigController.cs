using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Controllers
{
	public class SmsConfigController : BaseController
	{
		private readonly ISmsConfigService _smsConfigService;
		public SmsConfigController
		(
			ISmsConfigService smsConfigService,
			IHttpContextAccessor httpContextAccessor,
			UserManager<Users> userManager
		): base(userManager, httpContextAccessor)
		{ 
			_smsConfigService = smsConfigService;
		}

		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			SelectList statusSelectList = new SelectList(EnumHelper.ToSelectListItem<DeletedEnum>(), "Value", "Text");
			var selectLists = new Dictionary<string, SelectList>
			{
				{ "statusSelectList", statusSelectList }
			};
			return selectLists;
		}

		public async Task<IActionResult> Index()
		{
			BaseFormViewModel<SmsConfig> formViewModel = new BaseFormViewModel<SmsConfig>()
			{
				Data = new SmsConfig(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
		}

		[HttpGet]
		public async Task<IActionResult> LoadData(SmsConfigSearchViewModel model, Pageable pageable)
		{
			var currentUser = await GetCurrentUser();
			var datas = await _smsConfigService.SearchSmsConfig(model, pageable, currentUser.OrganizationId);
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
			var data = await _smsConfigService.GetByIdAndOrgId(id, currentUser.OrganizationId);
			BaseFormViewModel<SmsConfig> formViewModel = new BaseFormViewModel<SmsConfig>()
			{
				Data = data ?? new SmsConfig(),
				SelectLists = await CreateSelectList()
			};
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(SmsConfig model)
		{
			var currentUser = await GetCurrentUser();
			SmsConfig result = await _smsConfigService.Create(model, currentUser);
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
			var currentUser = await GetCurrentUser();
			SmsConfig? result = await _smsConfigService.Update(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
	}
}
