using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpSmsCanboRepository : BaseRepository<WpSmsCanbo, long>
	{
		public WpSmsCanboRepository(SmsTynContext _context) : base(_context)
		{
		}
	}
}
