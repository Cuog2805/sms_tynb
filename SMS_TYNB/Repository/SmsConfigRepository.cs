using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
    public class SmsConfigRepository : BaseRepository<SmsConfig, long>
    {
        public SmsConfigRepository(SmsTynContext _context) : base(_context)
        {
        }

		public Task<IQueryable<SmsConfig>> Search(SmsConfigSearchViewModel model)
		{
			var query = context.Set<SmsConfig>().AsQueryable();

			if (!string.IsNullOrWhiteSpace(model.searchInput))
			{
				query = query.Where(
					item => item.LabelId.ToLower().Contains(model.searchInput.Trim().ToLower())
					|| item.ApiUser.ToLower().Contains(model.searchInput.Trim().ToLower())
				);
			}

			return Task.FromResult(query);
		}
	}
}
