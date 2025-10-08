using TravelOperation.Core.Models;

namespace TravelOperation.Core.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string email, string password);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetCurrentUserEmailAsync();
        Task<string?> GetCurrentUserRoleAsync();
        Task<string?> GetCurrentUserDepartmentAsync();
        Task<string?> GetCurrentUserFullNameAsync();
        Task<User?> GetCurrentUserAsync();
        event Action<bool>? AuthenticationStateChanged;
    }
}