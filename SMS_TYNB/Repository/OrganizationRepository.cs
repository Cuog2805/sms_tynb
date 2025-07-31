using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class OrganizationRepository: BaseRepository<Organization, long>
	{
		public OrganizationRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}
	}
}
