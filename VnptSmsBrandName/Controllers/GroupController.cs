using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
	[Authorize(Roles = "Admin")]
	public class GroupController : BaseController
	{
		private readonly IMGroupService _mGroupService;
		public GroupController
		(
			UserManager<Users> userManager,
			IHttpContextAccessor httpContextAccessor,
			IMGroupService mGroupService
		) : base(userManager, httpContextAccessor)
		{
			_mGroupService = mGroupService;
		}
		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			var currentUser = await GetCurrentUser();
			SelectList mGroupSelectList = new SelectList(await _mGroupService.GetMGroupList(currentUser.OrganizationId), "GroupId", "Name");
			SelectList statusSelectList = new SelectList(EnumHelper.ToSelectListItem<DeletedEnum>(), "Value", "Text");
			var selectLists = new Dictionary<string, SelectList>
			{
				{ "mGroupSelectList", mGroupSelectList },
				{ "statusSelectList", statusSelectList }
			};
			return selectLists;
		}
		public async Task<IActionResult> Index()
		{
			BaseFormViewModel<MGroup> formViewModel = new BaseFormViewModel<MGroup>()
			{
				Data = new MGroup(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(MGroupSearchViewModel model, Pageable pageable)
		{
			var currentUser = await GetCurrentUser();
			var datas = await _mGroupService.SearchMGroup(model, pageable, currentUser.OrganizationId);
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
			var data = await _mGroupService.GetByIdAndOrgId(id, currentUser.OrganizationId);
			BaseFormViewModel<MGroup> formViewModel = new BaseFormViewModel<MGroup>()
			{
				Data = data ?? new MGroup(),
				SelectLists = await CreateSelectList()
			};
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(MGroup model)
		{
			var currentUser = await GetCurrentUser();
			MGroup result = await _mGroupService.Create(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Thêm mới thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(MGroup model)
		{
			var currentUser = await GetCurrentUser();
			MGroup? result = await _mGroupService.Update(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
		public async Task<IActionResult> Assign()
		{
			BaseFormViewModel<MGroup> formViewModel = new BaseFormViewModel<MGroup>()
			{
				Data = new MGroup(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<JsonResult> LoadDataGroupEmployee(MGroupSearchViewModel model)
		{
			var currentUser = await GetCurrentUser();
			List<MGroupViewModel> result = await _mGroupService.GetAllMGroupEmployees(model, currentUser.OrganizationId);
			return Json(new
			{
				state = "success",
				msg = "LoadDataGroupEmployee thành công!",
				data = result
			});
		}
		[HttpGet]
		public async Task<JsonResult> LoadDetailGroupEmployee(int id)
		{
			var currentUser = await GetCurrentUser();
			MGroupViewModel result = await _mGroupService.GetGroupEmployeeByIdAndOrgId(id, currentUser.OrganizationId);
			return Json(new
			{
				state = "success",
				msg = "LoadDetailGroupEmployee thành công!",
				data = result
			});
		}
		[HttpPost]
		public async Task<JsonResult> Assign(MGroupViewModel model)
		{
			var currentUser = await GetCurrentUser();
			MGroupViewModel result = await _mGroupService.Assign(model, currentUser);
			return Json(new
			{
				state = "success",
				msg = "Gán thành công!",
				data = result
			});
		}
	}
}
