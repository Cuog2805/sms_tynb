using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Service
{
    public interface IConfigService
    {
        Task Delete(Config obj);
        Task<Config?> Update(Config obj);
        Task<Config> Create(Config obj);
        Task<List<Config>> GetAll();
        Task<Config?> FindByKey(string key);
    }
}
