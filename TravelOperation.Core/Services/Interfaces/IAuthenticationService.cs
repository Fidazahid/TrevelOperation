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
        event Action<bool>? AuthenticationStateChanged;
    }
}