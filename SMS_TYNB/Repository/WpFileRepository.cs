using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models;
using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpFileRepository : BaseRepository<WpFile, long>
	{
		public WpFileRepository(SmsTynContext _context) : base(_context)
		{
		}

		public Task<IQueryable<WpFile>> Search(string? searchInput)
		{
			var query = context.Set<WpFile>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(searchInput))
			{
				query = query.Where(
					item => item.TenFile.ToLower().Contains(searchInput.Trim().ToLower())
				);
			}

			return Task.FromResult(query);
		}

		public virtual async Task<IEnumerable<WpFile>> GetByIdFiles(List<long> ids)
		{
			var query = context.Set<WpFile>().AsQueryable();

			if (ids != null && ids.Any())
			{
				query = query.Where(item => ids.Contains(item.IdFile));
			}

			return await query.ToListAsync();
		}

		public virtual async Task<IEnumerable<WpFile>> GetByBangLuuFile(string tableName)
		{
			var query = context.Set<WpFile>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(tableName))
			{
				query = query.Where(item => item.BangLuuFile == tableName);
			}

			return await query.ToListAsync();
		}
	}
}
