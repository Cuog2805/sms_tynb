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
