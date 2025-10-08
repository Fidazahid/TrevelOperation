using TravelOperation.Core.Models;

namespace TravelOperation.Core.Services
{
    public interface IUserManagementService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string email);
        Task<List<string>> GetDepartmentsAsync();
        Task<List<string>> GetRolesAsync();
        Task<bool> IsEmailAvailableAsync(string email);
    }
}