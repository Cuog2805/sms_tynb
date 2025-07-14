using SMS_TYNB.Models;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpFileRepository : BaseRepository<WpFile, long>
	{
		public WpFileRepository(SmsTynContext _context) : base(_context)
		{
		}

	}
}
