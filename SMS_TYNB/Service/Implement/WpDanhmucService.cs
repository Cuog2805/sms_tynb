using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;

namespace SMS_TYNB.Service.Implement
{
	public class WpDanhmucService: IWpDanhmucService
	{
		private readonly WpDanhmucRepository _wpDanhmucRepository;
		public WpDanhmucService(WpDanhmucRepository wpDanhmucRepository) 
		{
			_wpDanhmucRepository = wpDanhmucRepository;
		}

		public async Task<IEnumerable<WpDanhmuc>> GetWpDanhmucByType(string type)
		{
			IEnumerable<WpDanhmuc> wpDanhmucs = await _wpDanhmucRepository.GetByType(type);
			return wpDanhmucs;
		}
	}
}
