using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpSmsRepository : BaseRepository<WpSms, long>
	{
		public WpSmsRepository(SmsTynContext _context) : base(_context)
		{
		}

		public Task<IQueryable<WpSms>> Search(string? searchInput)
		{
			var query = context.Set<WpSms>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				query = query.Where(item => item.Noidung.ToLower().Contains(searchInput.Trim().ToLower()));
			}

			return Task.FromResult(query);
		}
	}
}
