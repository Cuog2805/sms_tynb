using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Helper;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpCanboRepository : BaseRepository<WpCanbo, int>
	{
		public WpCanboRepository(SmsTynContext _context) : base(_context)
		{
		}

		public Task<IQueryable<WpCanbo>> Search(string? searchInput)
		{
			var query = context.Set<WpCanbo>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				var pattern = "%" + searchInput.Trim().ToLower() + "%";

				query = query.Where(item =>
					EF.Functions.Like(item.TenCanbo, pattern)
					|| EF.Functions.Like(item.SoDt, pattern)
				);
			}

			return Task.FromResult(query);
		}
	}
}
