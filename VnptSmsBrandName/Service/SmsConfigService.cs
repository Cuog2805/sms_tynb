using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Common;
using VnptSmsBrandName.Models.Master;
using VnptSmsBrandName.Repository;
using VnptSmsBrandName.ViewModel;
using System.Threading.Tasks;

namespace VnptSmsBrandName.Service
{
    public class SmsConfigService : ISmsConfigService
    {
        private readonly SmsConfigRepository _smsConfigRepository;

        public SmsConfigService(SmsConfigRepository smsConfigRepository)
        {
            _smsConfigRepository = smsConfigRepository;
        }

        public async Task<PageResult<SmsConfig>> SearchSmsConfig(SmsConfigSearchViewModel model, Pageable pageable)
        {
            IQueryable<SmsConfig> smsConfigs = await _smsConfigRepository.Search(model);
            var wpCanbosPage = await _smsConfigRepository.GetPagination(smsConfigs, pageable);

            int total = await smsConfigs.CountAsync();

            return new PageResult<SmsConfig>
            {
                Data = wpCanbosPage,
                Total = total,
            };
        }

        public async Task<List<SmsConfig>> GetAll()
        {
            var data = await _smsConfigRepository.GetAll();
            return data.ToList();
        }

        public SmsConfig GetSmsConfigActive(bool isActive)
        {
            var data = _smsConfigRepository.GetSmsConfigActive(isActive) ?? new SmsConfig();
            return data;
        }

        public async Task<SmsConfig?> GetById(int id)
        {
            SmsConfig? data = await _smsConfigRepository.FindById(id);
            return data;
        }

        public async Task<SmsConfig> Create(SmsConfig obj)
        {
            SmsConfig newObj = await _smsConfigRepository.Create(obj);
            return obj;
        }

        public async Task<SmsConfig?> Update(SmsConfig obj)
        {
            SmsConfig? newObj = await _smsConfigRepository.Update(obj.Id, obj);
            return newObj;
        }

        public async Task Delete(SmsConfig obj)
        {
            await _smsConfigRepository.Delete(obj.Id);
        }
    }

	public interface ISmsConfigService
	{
		Task Delete(SmsConfig obj);
		Task<SmsConfig?> Update(SmsConfig obj);
		Task<SmsConfig> Create(SmsConfig obj);
		Task<SmsConfig?> GetById(int id);
		Task<List<SmsConfig>> GetAll();
		SmsConfig GetSmsConfigActive(bool isActive);
		Task<PageResult<SmsConfig>> SearchSmsConfig(SmsConfigSearchViewModel model, Pageable pageable);

	}
}
