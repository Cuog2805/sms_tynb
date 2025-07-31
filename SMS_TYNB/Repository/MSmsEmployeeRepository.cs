using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class MSmsEmployeeRepository : BaseRepository<MSmsEmployee, long>
	{
		public MSmsEmployeeRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}
	}
}
