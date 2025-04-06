using System.Security.Claims;

namespace ReelBuy.Frontend.Services;

public interface ILoginService
{
    Task LoginAsync(string token);

    Task LogoutAsync();

    IEnumerable<Claim> ParseClaimsFromJWT(string token);
}