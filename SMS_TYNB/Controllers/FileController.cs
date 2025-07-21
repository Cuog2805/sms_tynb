using Microsoft.AspNetCore.Mvc;
using SMS_TYNB.Service;
using SMS_TYNB.Common;
using System.Threading.Tasks;

namespace SMS_TYNB.Controllers
{
	public class FileController : Controller
	{
		private readonly IWpFileService _wpFileService;
		public FileController(IWpFileService wpFileService)
		{
			_wpFileService = wpFileService;
		}
		[HttpGet]
		public async Task<IActionResult> LoadData(string searchInput, Pageable pageable)
		{
			var datas = await _wpFileService.GetAllWpFile(searchInput, pageable);
			return Json(new
			{
				state = "success",
				msg = "LoadData thành công!",
				content = datas
			});
		}
	}
}
