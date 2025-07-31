using Microsoft.AspNetCore.Mvc;
using VnptSmsBrandName.Service;
using VnptSmsBrandName.Common;
using System.Threading.Tasks;

namespace VnptSmsBrandName.Controllers
{
	public class FileController : Controller
	{
		private readonly IMFileService _mFileService;
		public FileController(IMFileService mFileService)
		{
			_mFileService = mFileService;
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(string searchInput, Pageable pageable)
		{
			var datas = await _mFileService.SearchFile(searchInput, pageable);
			return Json(new
			{
				state = "success",
				msg = "LoadData thành công!",
				content = datas
			});
		}
		[HttpGet]
		public async Task<IActionResult> LoadFileChangeHistory(long id)
		{
			var datas = await _mFileService.GetAllFileHistory(id);
			return Json(new
			{
				state = "success",
				msg = "LoadFileChangeHistory thành công!",
				content = datas
			});
		}
	}
}
