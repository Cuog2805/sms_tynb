using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class MGroupEmployeeRepository : BaseRepository<MGroupEmployee, long>
	{
		public MGroupEmployeeRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public async Task<IEnumerable<MGroupEmployee>> FindByGroupId(long id)
		{
			IEnumerable<MGroupEmployee> groupEmployees = context.Set<MGroupEmployee>().Where(item => item.IdGroup == id);
			return groupEmployees;
		}

		public async Task DeleteByGroupId(long id)
		{
			IEnumerable<MGroupEmployee> groupEmployees = await FindByGroupId(id);
			context.Set<MGroupEmployee>().RemoveRange(groupEmployees);
		}
	}
}
