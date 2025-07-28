using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
	public class WpSmsFileRepository : BaseRepository<WpSmsFile, long>
	{
		public WpSmsFileRepository(SmsTynContext context) : base(context)
		{
		}

		public WpSmsFile? GetBySmsIdAndFileId(long smsId, long fileId)
		{
			return context.Set<WpSmsFile>().FirstOrDefault(file => file.IdSms == smsId && file.IdFile == fileId);
		}
	}
}
