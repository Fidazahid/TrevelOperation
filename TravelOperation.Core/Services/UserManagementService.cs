using TravelOperation.Core.Services;
using TravelOperation.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace TravelOperation.Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IAuthenticationService _authService;
        
        // In-memory storage for now - replace with database later
        private static List<User> _users = new();
        
        public UserManagementService(IAuthenticationService authService)
        {
            _authService = authService;
            InitializeUsers();
        }

        private void InitializeUsers()
        {
            if (_users.Any()) return;

            // Initialize with existing mock users from AuthenticationService
            _users = new List<User>
            {
                // FINANCE MANAGERS
                new User { Email = "admin@corporate.com", Password = "admin123", Role = "Finance", Department = "Finance", FirstName = "Admin", LastName = "User", IsActive = true },
                new User { Email = "martina.popinsk@wsc.com", Password = "finance123", Role = "Finance", Department = "Finance", FirstName = "Martina", LastName = "Popinsk", IsActive = true },
                new User { Email = "maayan.chesler@wsc.com", Password = "finance123", Role = "Finance", Department = "Finance", FirstName = "Maayan", LastName = "Chesler", IsActive = true },
                
                // DEPARTMENT OWNERS
                new User { Email = "sales.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "Sales", FirstName = "Sales", LastName = "Manager", IsActive = true },
                new User { Email = "it.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "IT", FirstName = "IT", LastName = "Manager", IsActive = true },
                new User { Email = "hr.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "HR", FirstName = "HR", LastName = "Manager", IsActive = true },
                new User { Email = "ops.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "Operations", FirstName = "Operations", LastName = "Manager", IsActive = true },
                new User { Email = "dev.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "Development", FirstName = "Development", LastName = "Manager", IsActive = true },
                new User { Email = "marketing.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "Marketing", FirstName = "Marketing", LastName = "Manager", IsActive = true },
                new User { Email = "legal.counsel@wsc.com", Password = "manager123", Role = "Owner", Department = "Legal", FirstName = "Legal", LastName = "Counsel", IsActive = true },
                new User { Email = "support.manager@wsc.com", Password = "manager123", Role = "Owner", Department = "Support", FirstName = "Support", LastName = "Manager", IsActive = true },
                
                // EMPLOYEES (sample)
                new User { Email = "john.doe@wsc.com", Password = "emp123", Role = "Employee", Department = "Sales", FirstName = "John", LastName = "Doe", IsActive = true },
                new User { Email = "jane.smith@wsc.com", Password = "emp123", Role = "Employee", Department = "IT", FirstName = "Jane", LastName = "Smith", IsActive = true },
                new User { Email = "bob.wilson@wsc.com", Password = "emp123", Role = "Employee", Department = "HR", FirstName = "Bob", LastName = "Wilson", IsActive = true },
                new User { Email = "alice.johnson@wsc.com", Password = "emp123", Role = "Employee", Department = "Operations", FirstName = "Alice", LastName = "Johnson", IsActive = true },
                new User { Email = "charlie.brown@wsc.com", Password = "emp123", Role = "Employee", Department = "Development", FirstName = "Charlie", LastName = "Brown", IsActive = true },
                
                // EXECUTIVES
                new User { Email = "ceo@wsc.com", Password = "exec123", Role = "Finance", Department = "Executive", FirstName = "Chief", LastName = "Executive", IsActive = true },
                new User { Email = "cfo@wsc.com", Password = "exec123", Role = "Finance", Department = "Executive", FirstName = "Chief", LastName = "Financial", IsActive = true },
                new User { Email = "cto@wsc.com", Password = "exec123", Role = "Finance", Department = "Executive", FirstName = "Chief", LastName = "Technology", IsActive = true }
            };
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            // Only Finance role can view all users
            var currentUserRole = await _authService.GetCurrentUserRoleAsync();
            if (currentUserRole != "Finance")
            {
                throw new UnauthorizedAccessException("Only Finance managers can view all users.");
            }

            return _users.Where(u => u.IsActive).OrderBy(u => u.Department).ThenBy(u => u.LastName).ToList();
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

            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.IsActive);
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

            _users.Add(user);
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

            var existingUser = _users.FirstOrDefault(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase));
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

            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
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
            await Task.CompletedTask; // For consistency with async pattern
            
            return !_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.IsActive);
        }
    }
}