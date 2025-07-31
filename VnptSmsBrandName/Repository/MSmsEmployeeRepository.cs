using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class MSmsEmployeeRepository : BaseRepository<MSmsEmployee, long>
	{
		public MSmsEmployeeRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}
	}
}
