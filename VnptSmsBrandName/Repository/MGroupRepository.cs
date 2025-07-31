using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class MGroupRepository : BaseRepository<MGroup, long>
	{
		public MGroupRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public Task<IQueryable<MGroup>> Search(string? searchInput, long orgId)
		{
			var query = context.Set<MGroup>().AsQueryable().Where(item => item.IdOrganization == orgId);

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				var pattern = "%" + searchInput.Trim().ToLower() + "%";

				query = query.Where(item =>
					EF.Functions.Like(item.Name.ToLower(), pattern)
				);
			}

			return Task.FromResult(query);
		}
	}
}
