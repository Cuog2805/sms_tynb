using SMS_TYNB.Common;
using SMS_TYNB.Models.Master;
using SMS_TYNB.ViewModel;

namespace SMS_TYNB.Service
{
	public interface IWpNhomService
	{
		Task<IEnumerable<WpNhom>> GetAllWpNhom();
		Task<IEnumerable<WpNhom>> GetAllWpNhomById(int id);
		Task<PageResult<WpNhomViewModel>> SearchWpNhom(WpNhomSearchViewModel model, Pageable pageable);
		Task<WpNhomViewModel> GetWpNhomCanbosById(int id);
		Task<WpNhom?> GetById(int id);
		Task<WpNhom> Create(WpNhom model);
		Task<WpNhom?> Update(WpNhom model);
		Task Delete(WpNhom model);
		Task<WpNhomViewModel> Assign(WpNhomViewModel model);
		Task<List<WpNhomViewModel>> GetAllWpNhomCanbos(WpNhomSearchViewModel model);
	}
}
