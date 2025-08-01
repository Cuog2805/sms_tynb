using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VnptSmsBrandName.Models.Identity;

namespace VnptSmsBrandName.Controllers
{
	public class BaseController : Controller
	{
		private readonly UserManager<Users> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public BaseController(UserManager<Users> userManager, IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<Users?> GetCurrentUser()
		{
			var httpContext = _httpContextAccessor.HttpContext;
			if (httpContext?.User?.Identity?.IsAuthenticated == true)
			{
				return await _userManager.GetUserAsync(httpContext.User);
			}
			return null;
		}
	}
}
