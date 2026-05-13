using System.Security.Claims;

using FinalProjectManager.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FinalProjectManager.Web.Identity;

public class ApplicationUserClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public ApplicationUserClaimsFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("IsApproved", user.IsApproved.ToString().ToLower()));
        if (!string.IsNullOrEmpty(user.FullName))
            identity.AddClaim(new Claim("FullName", user.FullName));
        return identity;
    }
}
