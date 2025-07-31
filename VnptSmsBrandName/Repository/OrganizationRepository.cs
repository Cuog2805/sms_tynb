using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class OrganizationRepository: BaseRepository<Organization, long>
	{
		public OrganizationRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}
	}
}
