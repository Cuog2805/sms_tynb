using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpUsersRepository
	{
		private readonly UserManager<WpUsers> _userManager;

		public WpUsersRepository(UserManager<WpUsers> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IEnumerable<WpUsers>> GetAll()
		{
			return await _userManager.Users.Select(item => new WpUsers()
			{
				Id = item.Id,
				UserName = item.UserName,
				Email = item.Email,
			}).ToListAsync();
		}
	}
}
