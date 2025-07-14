using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpNhomRepository : BaseRepository<WpNhom, int>
	{
		public WpNhomRepository(SmsTynContext _context) : base(_context)
		{
		}
		public Task<IQueryable<WpNhom>> Search(string searchInput)
		{
			var query = context.Set<WpNhom>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				query = query.Where(item => item.TenNhom.Contains(searchInput));
			}

			return Task.FromResult(query);
		}
	}
}
