using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Service;
using SMS_TYNB.Service.Implement;
using SMS_TYNB.ViewModel;
using System.Threading.Tasks;

namespace SMS_TYNB.Controllers
{
	[Authorize(Roles = "Admin")]
	public class GroupController : Controller
	{
		private readonly IWpNhomService _wpNhomService;
		private readonly IWpDanhmucService _wpDanhmucService;
		public GroupController(IWpNhomService wpNhomService, IWpDanhmucService wpDanhmucService)
		{
			_wpNhomService = wpNhomService;
			_wpDanhmucService = wpDanhmucService;
		}
		private async Task<Dictionary<string, SelectList>> CreateSelectList()
		{
			SelectList wpNhomSelectList = new SelectList(await _wpNhomService.GetAllWpNhom(), "IdNhom", "TenNhom");
			SelectList wpTrangThaiSelectList = new SelectList(await _wpDanhmucService.GetWpDanhmucByType("TRANGTHAI"), "Value", "TenDanhmuc");
			var selectLists = new Dictionary<string, SelectList>
			{
				{ "wpNhomSelectList", wpNhomSelectList },
				{ "wpTrangThaiSelectList", wpTrangThaiSelectList }
			};
			return selectLists;
		}
		public async Task<IActionResult> Index()
		{
			BaseFormViewModel<WpNhom> formViewModel = new BaseFormViewModel<WpNhom>()
			{
				Data = new WpNhom(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(WpNhomSearchViewModel model, Pageable pageable)
		{
			var datas = await _wpNhomService.SearchWpNhom(model, pageable);
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
			var data = await _wpNhomService.GetById(id);
			BaseFormViewModel<WpNhom> formViewModel = new BaseFormViewModel<WpNhom>()
			{
				Data = data ?? new WpNhom(),
				SelectLists = await CreateSelectList()
			};
			return PartialView("_Form", formViewModel);
		}
		[HttpPost]
		public async Task<JsonResult> Create(WpNhom model)
		{
			WpNhom result = await _wpNhomService.Create(model);
			return Json(new
			{
				state = "success",
				msg = "Thêm mới thành công!",
				data = model
			});
		}
		[HttpPost]
		public async Task<JsonResult> Update(WpNhom model)
		{
			WpNhom result = await _wpNhomService.Update(model);
			return Json(new
			{
				state = "success",
				msg = "Cập nhật thành công!",
				data = model
			});
		}
		public async Task<IActionResult> Assign()
		{
			BaseFormViewModel<WpNhom> formViewModel = new BaseFormViewModel<WpNhom>()
			{
				Data = new WpNhom(),
				SelectLists = await CreateSelectList()
			};
			return View(formViewModel);
		}
		[HttpGet]
		public async Task<JsonResult> LoadDataWpNhomCanbos(WpNhomSearchViewModel model)
		{
			List<WpNhomViewModel> result = await _wpNhomService.GetAllWpNhomCanbos(model);
			return Json(new
			{
				state = "success",
				msg = "LoadDataWpNhomCanbos thành công!",
				data = result
			});
		}
		[HttpGet]
		public async Task<JsonResult> LoadDetailWpNhomCanbos(int id)
		{
			WpNhomViewModel result = await _wpNhomService.GetWpNhomCanbosById(id);
			return Json(new
			{
				state = "success",
				msg = "LoadDetailWpNhomCanbos thành công!",
				data = result
			});
		}
		[HttpPost]
		public async Task<JsonResult> Assign(WpNhomViewModel model)
		{
			WpNhomViewModel result = await _wpNhomService.Assign(model);
			return Json(new
			{
				state = "success",
				msg = "Gán thành công!",
				data = result
			});
		}
	}
}
