using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public interface IWpCanboService
	{
		Task<IEnumerable<WpCanbo>> GetAllWpCanbo();
		Task<PageResult<WpCanboViewModel>> SearchWpCanbo(WpCanboSearchViewModel model, Pageable pageable);
		Task<WpCanbo?> GetById(int wpCanboId);
		Task<WpCanbo> Create(WpCanbo wpCanbo);
		Task<WpCanbo?> Update(WpCanbo wpCanbo);
		Task Delete(WpCanbo wpCanbo);
	}
}
