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
	}
}
