using ReelBuy.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using ReelBuy.Shared.DTOs;

namespace ReelBuy.Backend.UnitsOfWork.Interfaces;

public interface IUsersUnitOfWork
{
    Task<SignInResult> LoginAsync(LoginDTO model);

    Task LogoutAsync();

    Task<User> GetUserAsync(string email);

    Task<IdentityResult> AddUserAsync(User user, string password);

    Task CheckRoleAsync(string roleName);

    Task AddUserToRoleAsync(User user, string roleName);

    Task<bool> IsUserInRoleAsync(User user, string roleName);
}