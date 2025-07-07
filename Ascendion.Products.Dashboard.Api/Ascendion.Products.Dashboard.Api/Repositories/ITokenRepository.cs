using Microsoft.AspNetCore.Identity;

namespace Ascendion.Products.Dashboard.Repositories;

public interface ITokenRepository
{
    string CreateJWTToken(IdentityUser user, List<string> roles);
}
