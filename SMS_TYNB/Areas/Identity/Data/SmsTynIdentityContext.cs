using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SMS_TYNB.Models.Identity;

namespace SMS_TYNB.Data;

public class SmsTynIdentityContext : IdentityDbContext<WpUsers>
{
    public SmsTynIdentityContext(DbContextOptions<SmsTynIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
		builder.Entity<WpUsers>().ToTable("wp_users");
		builder.Entity<IdentityRole>().ToTable("wp_roles");
		builder.Entity<IdentityUserRole<string>>().ToTable("wp_user_roles");
		builder.Entity<IdentityUserClaim<string>>().ToTable("wp_user_claims");
		builder.Entity<IdentityUserLogin<string>>().ToTable("wp_user_login");
		builder.Entity<IdentityRoleClaim<string>>().ToTable("wp_role_claims");
		builder.Entity<IdentityUserToken<string>>().ToTable("wp_user_tokens");
	}
}
