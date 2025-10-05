using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Models;
using TravelOperation.Core.Data;

namespace TravelOperation.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly TravelDbContext _context;
        
        public event Action<bool>? AuthenticationStateChanged;

        public AuthenticationService(IJSRuntime jsRuntime, TravelDbContext context)
        {
            _jsRuntime = jsRuntime;
            _context = context;
            Console.WriteLine("AuthenticationService initialized");
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"LoginAsync called with email: '{email}'");
                
                var normalizedEmail = email.ToLower().Trim();
                Console.WriteLine($"Normalized email: '{normalizedEmail}'");
                
                // Query user from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => 
                        u.Email.ToLower() == normalizedEmail && 
                        u.IsActive);
                
                if (user == null)
                {
                    Console.WriteLine("User not found in database");
                    return false;
                }
                
                // Verify password (Note: In production, use proper password hashing like BCrypt)
                if (user.Password != password)
                {
                    Console.WriteLine("Password doesn't match");
                    return false;
                }
                
                Console.WriteLine($"User authenticated. Role: {user.Role}, Department: {user.Department}");
                
                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                // Store only email in localStorage for session tracking
                try
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", normalizedEmail);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "loginTime", DateTimeOffset.UtcNow.ToString("O"));
                    Console.WriteLine("Session stored in localStorage");
                }
                catch (Exception jsEx)
                {
                    Console.WriteLine($"localStorage error: {jsEx.Message}");
                }
                
                AuthenticationStateChanged?.Invoke(true);
                
                Console.WriteLine("Login successful!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginTime");
                
                AuthenticationStateChanged?.Invoke(false);
                Console.WriteLine("User logged out");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                Console.WriteLine("IsAuthenticatedAsync called");
                
                // Get user email from localStorage (session tracking only)
                string? userEmail = null;
                string? loginTime = null;
                
                try
                {
                    userEmail = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");
                    loginTime = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "loginTime");
                }
                catch (Exception jsEx)
                {
                    Console.WriteLine($"localStorage access error: {jsEx.Message}");
                    return false;
                }
                
                Console.WriteLine($"localStorage check - userEmail: {userEmail}, loginTime: {loginTime}");
                
                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(loginTime))
                {
                    Console.WriteLine("No user session in localStorage");
                    return false;
                }

                // Check if session is expired (24 hours)
                if (DateTimeOffset.TryParse(loginTime, out var loginDateTime))
                {
                    var sessionDuration = DateTimeOffset.UtcNow - loginDateTime;
                    if (sessionDuration.TotalHours > 24)
                    {
                        Console.WriteLine("Session expired, logging out");
                        await LogoutAsync();
                        return false;
                    }
                }

                // Verify user still exists and is active in database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userEmail.ToLower() && u.IsActive);
                
                if (user == null)
                {
                    Console.WriteLine("User not found or inactive in database, logging out");
                    await LogoutAsync();
                    return false;
                }
                
                Console.WriteLine($"User authenticated from database: {user.Email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication check error: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetCurrentUserEmailAsync()
        {
            try
            {
                if (!await IsAuthenticatedAsync())
                {
                    return null;
                }
                
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user email error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetCurrentUserRoleAsync()
        {
            try
            {
                var email = await GetCurrentUserEmailAsync();
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                
                // Query role from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
                return user?.Role;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user role error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetCurrentUserIdAsync()
        {
            try
            {
                var email = await GetCurrentUserEmailAsync();
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                
                // Query UserId from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
                return user?.UserId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user ID error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetCurrentUserDepartmentAsync()
        {
            try
            {
                var email = await GetCurrentUserEmailAsync();
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                
                // Query department from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
                return user?.Department;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user department error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetCurrentUserFullNameAsync()
        {
            try
            {
                var email = await GetCurrentUserEmailAsync();
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                
                // Query name from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
                if (user != null && !string.IsNullOrEmpty(user.FirstName))
                {
                    return $"{user.FirstName} {user.LastName}".Trim();
                }
                
                // Fallback: Extract name from email
                var namePart = email.Split('@')[0];
                var nameParts = namePart.Split('.');
                if (nameParts.Length >= 2)
                {
                    return $"{CapitalizeFirst(nameParts[0])} {CapitalizeFirst(nameParts[1])}";
                }
                return CapitalizeFirst(namePart);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user full name error: {ex.Message}");
                return null;
            }
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            try
            {
                var email = await GetCurrentUserEmailAsync();
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                
                // Query full user details from database
                var user = await _context.AuthUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get current user error: {ex.Message}");
                return null;
            }
        }

        private string CapitalizeFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
    }
}