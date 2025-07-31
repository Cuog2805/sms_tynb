using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class WpUsersRepository
	{
		private readonly UserManager<Users> _userManager;

		public WpUsersRepository(UserManager<Users> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IEnumerable<Users>> GetAll()
		{
			return await _userManager.Users.Select(item => new Users()
			{
				Id = item.Id,
				UserName = item.UserName,
				Email = item.Email,
			}).ToListAsync();
		}
	}
}
