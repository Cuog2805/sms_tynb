using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Identity;

namespace SMS_TYNB.Data;

public class VnptSmsBrandnameIdentityContext : IdentityDbContext<WpUsers>
{
    public VnptSmsBrandnameIdentityContext(DbContextOptions<VnptSmsBrandnameIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
		builder.Entity<WpUsers>().ToTable("users");
		builder.Entity<IdentityRole>().ToTable("roles");
		builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
		builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
		builder.Entity<IdentityUserLogin<string>>().ToTable("user_login");
		builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
		builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
	}
}
