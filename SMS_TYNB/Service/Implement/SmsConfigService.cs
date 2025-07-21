using SMS_TYNB.Models.Master;
using SMS_TYNB.Repository;
using System.Threading.Tasks;

namespace SMS_TYNB.Service.Implement
{
    public class SmsConfigService: ISmsConfigService
    {
        private readonly SmsConfigRepository _smsConfigRepository;
        public SmsConfigService(SmsConfigRepository smsConfigRepository)
        {
            _smsConfigRepository = smsConfigRepository;
        }
        public async Task<List<SmsConfig>> GetAll()
        {
            var data = await _smsConfigRepository.GetAll();
            return data.ToList();
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
}
