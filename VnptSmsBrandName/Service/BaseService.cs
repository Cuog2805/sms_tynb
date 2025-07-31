using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Service
{
	public abstract class BaseService
	{
		protected readonly ICurrentUserService _currentUserService;

		protected BaseService(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		protected async Task SetCreateAudit<T>(T entity) where T : BaseModel
		{
			var user = await _currentUserService.GetCurrentUser();
			if(user != null)
			{
				entity.CreateBy = user.UserName;
				entity.CreateAt = DateTime.Now;
				entity.IdOrganization = user.OrgId;
			}
		}

		protected async Task SetUpdateAudit<T>(T entity) where T : BaseModel
		{
			var user = await _currentUserService.GetCurrentUser();
			if (user != null)
			{
				entity.UpdatedBy = user.UserName;
				entity.UpdatedAt = DateTime.Now;
			}
		}
	}
}
