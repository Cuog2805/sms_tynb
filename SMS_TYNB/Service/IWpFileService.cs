using SMS_TYNB.Common;
using SMS_TYNB.Models.Identity;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Service
{
	public interface IWpFileService
	{
		Task<WpFile?> GetById(int id);
		Task<WpFile> Create(WpFile model);
		Task<WpFile?> Update(WpFile model);
		Task Delete(WpFile model);
		Task<PageResult<WpFile>> GetAllWpFile(string searchInput, Pageable pageable);
		Task<IEnumerable<WpFile>> GetByBangLuuFile(string tableName);
		Task SaveFile(IFormFile file, WpUsers creator, string tableName, long tableId, string subFolder = "upload");
		Task CreateFromFileExisted(List<long> selectedFileIds, WpUsers creator, string tableName, long tableId);
		Task UpdateContentFile(IFormFile file, WpFile oldFile, WpUsers modifier);
	}
}
