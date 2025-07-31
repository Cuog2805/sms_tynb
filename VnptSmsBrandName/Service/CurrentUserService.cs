using Microsoft.AspNetCore.Identity;
using VnptSmsBrandName.Models.Identity;

namespace VnptSmsBrandName.Service
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly UserManager<Users> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(
			UserManager<Users> userManager,
			IHttpContextAccessor httpContextAccessor)
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

	public interface ICurrentUserService
	{
		Task<Users?> GetCurrentUser();
	}
}
