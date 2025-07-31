using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
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
