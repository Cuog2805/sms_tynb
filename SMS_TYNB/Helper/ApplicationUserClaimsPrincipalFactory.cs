using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SMS_TYNB.Models.Identity;
using System.Security.Claims;

namespace SMS_TYNB.Helper
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<WpUsers, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<WpUsers> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options
            ) : base(userManager, roleManager, options) { }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(WpUsers user)
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
