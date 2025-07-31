using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Repository
{
    public class ConfigRepository : BaseRepository<Config, long>
    {
        public ConfigRepository(VnptSmsBrandnameContext _context) : base(_context)
        {
        }
    }
}
