using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VnptSmsBrandName.Models.Identity;
using System.Security.Claims;

namespace VnptSmsBrandName.Helper
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Users, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options
            ) : base(userManager, roleManager, options) { }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Users user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("FullName",
                user.FullName
            ));
            identity.AddClaim(new Claim("UserRole",
                user.UserRole
            ));
            identity.AddClaim(new Claim("Id",
                user.Id
            ));
            return identity;
        }
    }
}
