using VnptSmsBrandName.Models.Identity;
using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Helper
{
	public static class AuditHelper
	{
		public static void SetCreateAudit<T>(T entity, Users creator) where T : BaseModel
		{
			if (creator != null)
			{
				entity.CreatedBy = creator.UserName;
				entity.CreatedAt = DateTime.Now;
				entity.OrganizationId = creator.OrganizationId;
			}
		}

		public static void SetUpdateAudit<T>(T entity, Users modifier) where T : BaseModel
		{
			if (modifier != null)
			{
				entity.UpdatedBy = modifier.UserName;
				entity.UpdatedAt = DateTime.Now;
			}
		}
	}
}
