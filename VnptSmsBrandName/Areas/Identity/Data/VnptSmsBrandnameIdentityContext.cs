using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VnptSmsBrandName.Models.Identity;

namespace VnptSmsBrandName.Data;

public class VnptSmsBrandnameIdentityContext : IdentityDbContext<Users>
{
    public VnptSmsBrandnameIdentityContext(DbContextOptions<VnptSmsBrandnameIdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
		builder.Entity<Users>().ToTable("users");
		builder.Entity<IdentityRole>().ToTable("roles");
		builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
		builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
		builder.Entity<IdentityUserLogin<string>>().ToTable("user_login");
		builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
		builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
	}
}
