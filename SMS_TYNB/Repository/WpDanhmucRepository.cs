using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpDanhmucRepository : BaseRepository<WpDanhmuc, int>
	{
		public WpDanhmucRepository(SmsTynContext _context) : base(_context)
		{
		}

		public async Task<IEnumerable<WpDanhmuc>> GetByType(string type)
		{
			return await context.Set<WpDanhmuc>().Where(item => item.Type == type).ToListAsync();
		}
	}
}
