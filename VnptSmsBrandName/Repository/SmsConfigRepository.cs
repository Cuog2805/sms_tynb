using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.ViewModel;

namespace VnptSmsBrandName.Repository
{
    public class SmsConfigRepository : BaseRepository<SmsConfig, long>
    {
        public SmsConfigRepository(VnptSmsBrandnameContext _context) : base(_context)
        {
        }

		public SmsConfig? GetSmsConfigByOrgIdAndActive(long orgId)
		{
			return context.SmsConfig
				.FirstOrDefault(x => x != null && x.OrganizationId == orgId && x.IsDeleted == 0);
		}
		public Task<IQueryable<SmsConfig>> Search(SmsConfigSearchViewModel model, long orgId)
		{
			var query = context.Set<SmsConfig>().AsQueryable().Where(item => item.OrganizationId == orgId);

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
