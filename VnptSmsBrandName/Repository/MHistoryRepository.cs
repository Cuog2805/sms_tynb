using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
	public class MHistoryRepository : BaseRepository<MHistory, long>
	{
		public MHistoryRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}
		public virtual async Task<IEnumerable<MHistory>> GetByIdRecordAndTableName(long idrecord, string tablename)
		{
			var query = context.Set<MHistory>().AsQueryable();

			if (idrecord > 0)
			{
				query = query.Where(item => item.IdRecord == idrecord);
			}

			if (!string.IsNullOrEmpty(tablename))
			{
				query = query.Where(item => item.TableName == tablename);
			}

			return await query.OrderByDescending(item => item.CreatedAt).ToListAsync();
		}
	}
}
