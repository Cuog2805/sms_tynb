using VnptSmsBrandName.Models.Master;

namespace VnptSmsBrandName.Repository
{
	public class MSmsFileRepository: BaseRepository<MSmsFile, long>
	{
		public MSmsFileRepository(VnptSmsBrandnameContext _context) : base(_context)
		{
		}

		public MSmsFile? GetBySmsIdAndFileIdAndOrgId(long smsId, long fileId, long orgId)
		{
			return context.Set<MSmsFile>().FirstOrDefault(file => file.IdSms == smsId && file.IdFile == fileId && file.IdOrganization == orgId);
		}
	}
}
