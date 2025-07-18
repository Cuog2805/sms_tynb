using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpCanboRepository : BaseRepository<WpCanbo, int>
	{
		public WpCanboRepository(SmsTynContext _context) : base(_context)
		{
		}

		public Task<IQueryable<WpCanbo>> Search(string searchInput)
		{
			var query = context.Set<WpCanbo>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				query = query.Where(
					item => item.TenCanbo.ToLower().Contains(searchInput.Trim().ToLower()) 
					|| item.SoDt.ToLower().Contains(searchInput.Trim().ToLower())
				);
			}

			return Task.FromResult(query);
		}
	}
}
