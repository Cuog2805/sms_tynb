using SMS_TYNB.Models;

namespace SMS_TYNB.Service
{
	public interface IWpDanhmucService
	{
		Task<IEnumerable<WpDanhmuc>> GetWpDanhmucByType(string type);
	}
}
