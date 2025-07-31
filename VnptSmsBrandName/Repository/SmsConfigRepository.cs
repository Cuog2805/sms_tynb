using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Repository
{
    public class SmsConfigRepository : BaseRepository<SmsConfig, long>
    {
        public SmsConfigRepository(VnptSmsBrandnameContext _context) : base(_context)
        {
        }
		public SmsConfig? GetSmsConfigActive(bool isActive) {
            return context.SmsConfig.FirstOrDefault(x => x != null && x.IsActive == isActive);
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
