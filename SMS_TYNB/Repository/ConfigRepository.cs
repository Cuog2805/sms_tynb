using SMS_TYNB.Models.Master;
using TodoApi.Repository;

namespace SMS_TYNB.Repository
{
    public class ConfigRepository : BaseRepository<Config, long>
    {
        public ConfigRepository(SmsTynContext _context) : base(_context)
        {
        }
    }
}
