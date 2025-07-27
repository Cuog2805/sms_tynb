using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using System.Threading.Tasks;

namespace SMS_TYNB.Service.Implement
{
    public class ConfigService : IConfigService
    {
        private readonly ConfigRepository _configRepository;
        public ConfigService(ConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }
        public async Task<List<Config>> GetAll()
        {
            var data = await _configRepository.GetAll();
            return data.ToList();
        }
        public async Task<Config> Create(Config obj)
        {
            Config newObj = await _configRepository.Create(obj);
            return obj;
        }
        public async Task<Config?> Update(Config obj)
        {
            Config? newObj = await _configRepository.Update(obj.Id, obj);
            return newObj;
        }
        public async Task Delete(Config obj)
        {
            await _configRepository.Delete(obj.Id);
        }
        public async Task<Config?> FindByKey(string key)
        {
            var result = await _configRepository.Query().Where(t => t.Key == key).FirstOrDefaultAsync();
            return result;
        }
    }
}
