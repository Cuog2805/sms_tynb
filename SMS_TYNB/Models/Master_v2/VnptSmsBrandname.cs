using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Master;

namespace SMS_TYNB.Models.Master_v2
{
	public class VnptSmsBrandname : DbContext
	{
		public VnptSmsBrandname()
		{
		}
		public VnptSmsBrandname(DbContextOptions<VnptSmsBrandname> options)
			: base(options)
		{
		}
		public virtual DbSet<MEmployee> MEmployee { get; set; }
		public virtual DbSet<MFile> MFile { get; set; }
		public virtual DbSet<MGroup> MGroup { get; set; }
		public virtual DbSet<MGroupEmployee> MGroupEmployee { get; set; }
		public virtual DbSet<MHistory> MHistory { get; set; }
		public virtual DbSet<MSms> MSms { get; set; }
		public virtual DbSet<MSmsEmployee> MSmsEmployee { get; set; }
		public virtual DbSet<MSmsFile> MSmsFile { get; set; }
		public virtual DbSet<Organization> Organization { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=VnptSmsBrandName;Uid=root;Pwd=280503;CharSet=utf8mb4;", ServerVersion.Parse("9.3.0-mysql"));

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
		}
	}
}
