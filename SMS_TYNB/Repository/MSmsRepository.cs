using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class MSmsRepository: BaseRepository<MSms, long>
	{
		public MSmsRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public Task<IQueryable<MSms>> Search(string? searchInput, long orgId)
		{
			var query = context.Set<MSms>().AsQueryable().Where(item => item.IdOrganization == orgId);

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				var pattern = "%" + searchInput.Trim().ToLower() + "%";

				query = query.Where(item =>
					EF.Functions.Like(item.Content, pattern)
				);
			}

			return Task.FromResult(query);
		}
	}
}
