using Microsoft.AspNetCore.Identity;
using SMS_TYNB.Models.Identity;

namespace SMS_TYNB.Service
{
	public class CurrentUserService : ICurrentUserService
	{
		private readonly UserManager<WpUsers> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(
			UserManager<WpUsers> userManager,
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<WpUsers?> GetCurrentUser()
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
		Task<WpUsers?> GetCurrentUser();
	}
}
