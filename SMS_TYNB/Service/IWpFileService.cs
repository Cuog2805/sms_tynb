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
		Task<IEnumerable<WpFile>> GetAllWpFile();
		Task<PageResult<WpFile>> SearchWpFile(string searchInput, Pageable pageable);
		Task<IEnumerable<WpFile>> GetByBangLuuFile(string tableName);
		Task<WpFile> SaveFile(IFormFile file, WpUsers creator, long smsId, string subFolder = "upload");
		Task<IEnumerable<WpFile>> CreateFromFileExisted(List<long> selectedFileIds, WpUsers creator, long smsId);
		Task UpdateContentFile(IFormFile file, WpFile oldFile, WpUsers modifier);
	}
}
