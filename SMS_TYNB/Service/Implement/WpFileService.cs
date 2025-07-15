using SMS_TYNB.Helper;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;

namespace SMS_TYNB.Service.Implement
{
	public class WpFileService : IWpFileService
	{
		private readonly WpFileRepository _wpFileRepository;
		public WpFileService(WpFileRepository wpFileRepository)
		{
			_wpFileRepository = wpFileRepository;
		}

		public virtual async Task<IEnumerable<WpFile>> GetByBangLuuFile(string tableName)
		{
			IEnumerable<WpFile> wpFiles = await _wpFileRepository.GetByBangLuuFile(tableName);
			return wpFiles;
		}

		public async Task<IEnumerable<WpFile>> GetAll()
		{
			IEnumerable<WpFile> wpFiles = await _wpFileRepository.GetAll();
			return wpFiles;
		}
		public async Task<WpFile> Create(WpFile model)
		{
			WpFile wpFile = await _wpFileRepository.Create(model);
			return wpFile;
		}

		public async Task Delete(WpFile model)
		{
			FileUpload.DeleteFile(model.FileUrl);
			await _wpFileRepository.Delete(model.IdFile);
		}

		public async Task<WpFile?> GetById(int id)
		{
			WpFile? wpFile = await _wpFileRepository.FindById(id);
			return wpFile;
		}

		public async Task<WpFile?> Update(WpFile model)
		{
			WpFile? wpFile = await _wpFileRepository.Update(model.IdFile, model);
			return wpFile;
		}
	}
}
