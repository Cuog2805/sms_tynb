using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpNhomCanboRepository : BaseRepository<WpNhomCanbo, int>
	{
		public WpNhomCanboRepository(SmsTynContext _context) : base(_context)
		{
		}
		public async Task<IEnumerable<WpNhomCanbo>> FindByWpNhomId(int id)
		{
			IEnumerable<WpNhomCanbo> wpNhomCanbos = context.Set<WpNhomCanbo>().Where(item => item.IdNhom == id);
			return wpNhomCanbos;
		}

		public async Task DeleteByWpNhomId(int id)
		{
			IEnumerable<WpNhomCanbo> wpNhomCanbos = await FindByWpNhomId(id);
			context.Set<WpNhomCanbo>().RemoveRange(wpNhomCanbos);
		}
	}
}
