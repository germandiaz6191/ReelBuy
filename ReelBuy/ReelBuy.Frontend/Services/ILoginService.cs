namespace ReelBuy.Frontend.Services;

public interface ILoginService
{
    Task LoginAsync(string token);

    Task LogoutAsync();
}