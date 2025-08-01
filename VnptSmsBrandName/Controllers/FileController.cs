using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Service;

namespace VnptSmsBrandName.Controllers
{
	public class FileController : BaseController
	{
		private readonly IMFileService _mFileService;
		public FileController
		(
			IMFileService mFileService, 
			IHttpContextAccessor httpContextAccessor, 
			UserManager<Users> userManager
		)
		: base(userManager, httpContextAccessor)
		{
			_mFileService = mFileService;

		}
		[HttpGet]
		public async Task<IActionResult> LoadData(string searchInput, Pageable pageable)
		{
			var currentUser = await GetCurrentUser();
			var datas = await _mFileService.SearchFile(searchInput, pageable, currentUser.OrganizationId);
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
			var currentUser = await GetCurrentUser();
			var datas = await _mFileService.GetAllFileHistory(id, currentUser.OrganizationId);
			return Json(new
			{
				state = "success",
				msg = "LoadFileChangeHistory thành công!",
				content = datas
			});
		}
	}
}
