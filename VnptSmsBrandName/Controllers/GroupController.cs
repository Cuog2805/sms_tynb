using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Common.Enum;
using VnptSmsBrandName.Helper;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.ViewModel;
using System.Threading.Tasks;

namespace VnptSmsBrandName.Controllers
{
	[Authorize(Roles = "Admin")]
	public class GroupController : Controller
	{
		private readonly IMGroupService _mGroupService;
		public GroupController(IMGroupService mGroupService)
		{
			_mGroupService = mGroupService;
		}
		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			SelectList mGroupSelectList = new SelectList(await _mGroupService.GetAllMGroup(), "IdGroup", "Name");
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
			var datas = await _mGroupService.SearchMGroup(model, pageable);
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
			var data = await _mGroupService.GetById(id);
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
			MGroup result = await _mGroupService.Create(model);
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
			MGroup? result = await _mGroupService.Update(model);
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
			List<MGroupViewModel> result = await _mGroupService.GetAllMGroupEmployees(model);
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
			MGroupViewModel result = await _mGroupService.GetGroupEmployeeById(id);
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
			MGroupViewModel result = await _mGroupService.Assign(model);
			return Json(new
			{
				state = "success",
				msg = "Gán thành công!",
				data = result
			});
		}
	}
}
