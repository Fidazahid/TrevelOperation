using TravelOperation.Core.Services;
using TravelOperation.Core.Models;
using TravelOperation.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TravelOperation.Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IAuthenticationService _authService;
        private readonly TravelDbContext _context;
        
        public UserManagementService(IAuthenticationService authService, TravelDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            // Only Finance role can view all users
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            if (currentUserRole != "Finance")
            {
                throw new UnauthorizedAccessException("Only Finance managers can view all users.");
            }

            return await _context.AuthUsers
                .Where(u => u.IsActive)
                .OrderBy(u => u.Department)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            var currentUserEmail = await _authService.GetCurrentUserEmailAsync();
            
            // Finance can view any user, others can only view themselves
            if (currentUserRole != "Finance" && currentUserEmail != email)
            {
                throw new UnauthorizedAccessException("You can only view your own profile.");
            }

            return await _context.AuthUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            // Only Finance role can create users
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            if (currentUserRole != "Finance")
            {
                throw new UnauthorizedAccessException("Only Finance managers can create users.");
            }

            // Validate email uniqueness
            if (!await IsEmailAvailableAsync(user.Email))
            {
                return false;
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.Email) || 
                string.IsNullOrWhiteSpace(user.Password) || 
                string.IsNullOrWhiteSpace(user.Role) || 
                string.IsNullOrWhiteSpace(user.Department) ||
                string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName))
            {
                return false;
            }

            // Set defaults
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            user.Email = user.Email.ToLower().Trim();

            _context.AuthUsers.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            var currentUserEmail = await _authService.GetCurrentUserEmailAsync();
            
            // Finance can update any user, others can only update themselves (limited fields)
            if (currentUserRole != "Finance" && currentUserEmail != user.Email)
            {
                throw new UnauthorizedAccessException("You can only update your own profile.");
            }

            var existingUser = await _context.AuthUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());
            
            if (existingUser == null)
            {
                return false;
            }

            // Update fields based on role
            if (currentUserRole == "Finance")
            {
                // Finance can update all fields except CreatedDate
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
                existingUser.Department = user.Department;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.IsActive = user.IsActive;
            }
            else
            {
                // Non-Finance users can only update their own password and name
                existingUser.Password = user.Password;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            // Only Finance role can delete users
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            if (currentUserRole != "Finance")
            {
                throw new UnauthorizedAccessException("Only Finance managers can delete users.");
            }

            var user = await _context.AuthUsers
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            
            if (user == null)
            {
                return false;
            }

            // Don't allow deleting yourself
            var currentUserEmail = await _authService.GetCurrentUserEmailAsync();
            if (email.Equals(currentUserEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("You cannot delete your own account.");
            }

            // Soft delete - just mark as inactive
            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetDepartmentsAsync()
        {
            await Task.CompletedTask; // For consistency with async pattern
            
            return new List<string>
            {
                "Sales",
                "IT", 
                "HR",
                "Operations",
                "Development",
                "Marketing",
                "Legal",
                "Support", 
                "Finance",
                "Executive"
            };
        }

        public async Task<List<string>> GetRolesAsync()
        {
            await Task.CompletedTask; // For consistency with async pattern
            
            return new List<string>
            {
                "Employee",
                "Owner", 
                "Finance"
            };
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var normalizedEmail = email.ToLower().Trim();
            return !await _context.AuthUsers
                .AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.IsActive);
        }
    }
}