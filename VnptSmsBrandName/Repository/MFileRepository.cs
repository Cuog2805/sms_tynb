using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class MFileRepository : BaseRepository<MFile, long>
	{
		public MFileRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public Task<IQueryable<MFile>> Search(string? searchInput, long orgId)
		{
			var query = context.Set<MFile>().AsQueryable().Where(item => item.IdOrganization == orgId);

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				query = query.Where(
					item => item.Name.ToLower().Contains(searchInput.Trim().ToLower())
				);
			}

			return Task.FromResult(query);
		}

		public virtual async Task<IEnumerable<MFile>> GetByIdFiles(List<long> ids)
		{
			var query = context.Set<MFile>().AsQueryable();

			if (ids != null && ids.Any())
			{
				query = query.Where(item => ids.Contains(item.IdFile));
			}

			return await query.ToListAsync();
		}
	}
}
