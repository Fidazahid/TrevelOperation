using Microsoft.JSInterop;
using TravelOperation.Core.Models;

namespace TravelOperation.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IJSRuntime _jsRuntime;
        private bool _isAuthenticated = false;
        private string? _currentUserEmail;
        private string? _currentUserRole;
        private string? _currentUserDepartment;
        
        public event Action<bool>? AuthenticationStateChanged;

        public AuthenticationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            
            // Always start unauthenticated to show login page
            _isAuthenticated = false;
            _currentUserEmail = null;
            _currentUserRole = null;
            _currentUserDepartment = null;
            Console.WriteLine("AuthenticationService initialized - starting unauthenticated");
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"LoginAsync called with email: '{email}', password: '{password}'");
                
                // Mock users for testing - replace with actual database/API authentication
                var mockUsers = new Dictionary<string, (string Password, string Role, string Department)>
                {
                    // FINANCE MANAGERS (1-2 users with full access to everything)
                    { "admin@noras.com", ("admin123", "Finance", "Finance") },
                    { "martina.popinsk@wsc.com", ("finance123", "Finance", "Finance") },
                    { "maayan.chesler@wsc.com", ("finance123", "Finance", "Finance") },
                    
                    // DEPARTMENT OWNERS (5-10 users with department-only access)
                    // Sales Department
                    { "sales.manager@wsc.com", ("manager123", "Owner", "Sales") },
                    { "sales.director@wsc.com", ("manager123", "Owner", "Sales") },
                    
                    // IT Department
                    { "it.manager@wsc.com", ("manager123", "Owner", "IT") },
                    { "tech.lead@wsc.com", ("manager123", "Owner", "IT") },
                    
                    // HR Department
                    { "hr.manager@wsc.com", ("manager123", "Owner", "HR") },
                    { "hr.director@wsc.com", ("manager123", "Owner", "HR") },
                    
                    // Operations Department
                    { "ops.manager@wsc.com", ("manager123", "Owner", "Operations") },
                    { "operations.head@wsc.com", ("manager123", "Owner", "Operations") },
                    
                    // Development Department
                    { "dev.manager@wsc.com", ("manager123", "Owner", "Development") },
                    { "engineering.lead@wsc.com", ("manager123", "Owner", "Development") },
                    
                    // Marketing Department
                    { "marketing.manager@wsc.com", ("manager123", "Owner", "Marketing") },
                    
                    // EMPLOYEES (50-200+ users with personal-only access)
                    // Sales Department Employees
                    { "john.doe@wsc.com", ("emp123", "Employee", "Sales") },
                    { "sarah.wilson@wsc.com", ("emp123", "Employee", "Sales") },
                    { "mike.johnson@wsc.com", ("emp123", "Employee", "Sales") },
                    { "lisa.brown@wsc.com", ("emp123", "Employee", "Sales") },
                    { "david.miller@wsc.com", ("emp123", "Employee", "Sales") },
                    { "emma.davis@wsc.com", ("emp123", "Employee", "Sales") },
                    { "alex.garcia@wsc.com", ("emp123", "Employee", "Sales") },
                    { "kelly.martinez@wsc.com", ("emp123", "Employee", "Sales") },
                    { "ryan.anderson@wsc.com", ("emp123", "Employee", "Sales") },
                    { "nicole.taylor@wsc.com", ("emp123", "Employee", "Sales") },
                    
                    // IT Department Employees
                    { "jane.smith@wsc.com", ("emp123", "Employee", "IT") },
                    { "tom.wilson@wsc.com", ("emp123", "Employee", "IT") },
                    { "amy.chen@wsc.com", ("emp123", "Employee", "IT") },
                    { "carlos.rodriguez@wsc.com", ("emp123", "Employee", "IT") },
                    { "priya.patel@wsc.com", ("emp123", "Employee", "IT") },
                    { "james.lee@wsc.com", ("emp123", "Employee", "IT") },
                    { "maria.gonzalez@wsc.com", ("emp123", "Employee", "IT") },
                    { "kevin.kim@wsc.com", ("emp123", "Employee", "IT") },
                    { "jessica.wang@wsc.com", ("emp123", "Employee", "IT") },
                    { "daniel.park@wsc.com", ("emp123", "Employee", "IT") },
                    
                    // HR Department Employees
                    { "bob.wilson@wsc.com", ("emp123", "Employee", "HR") },
                    { "jennifer.clark@wsc.com", ("emp123", "Employee", "HR") },
                    { "robert.lewis@wsc.com", ("emp123", "Employee", "HR") },
                    { "michelle.white@wsc.com", ("emp123", "Employee", "HR") },
                    { "steven.hall@wsc.com", ("emp123", "Employee", "HR") },
                    { "laura.green@wsc.com", ("emp123", "Employee", "HR") },
                    
                    // Operations Department Employees
                    { "alice.johnson@wsc.com", ("emp123", "Employee", "Operations") },
                    { "chris.adams@wsc.com", ("emp123", "Employee", "Operations") },
                    { "sandra.baker@wsc.com", ("emp123", "Employee", "Operations") },
                    { "mark.evans@wsc.com", ("emp123", "Employee", "Operations") },
                    { "patricia.hill@wsc.com", ("emp123", "Employee", "Operations") },
                    { "william.scott@wsc.com", ("emp123", "Employee", "Operations") },
                    { "nancy.carter@wsc.com", ("emp123", "Employee", "Operations") },
                    { "joseph.mitchell@wsc.com", ("emp123", "Employee", "Operations") },
                    
                    // Development Department Employees
                    { "charlie.brown@wsc.com", ("emp123", "Employee", "Development") },
                    { "anna.thompson@wsc.com", ("emp123", "Employee", "Development") },
                    { "brian.moore@wsc.com", ("emp123", "Employee", "Development") },
                    { "rachel.jackson@wsc.com", ("emp123", "Employee", "Development") },
                    { "andrew.martin@wsc.com", ("emp123", "Employee", "Development") },
                    { "stephanie.lee@wsc.com", ("emp123", "Employee", "Development") },
                    { "derek.wright@wsc.com", ("emp123", "Employee", "Development") },
                    { "vanessa.lopez@wsc.com", ("emp123", "Employee", "Development") },
                    { "nathan.harris@wsc.com", ("emp123", "Employee", "Development") },
                    { "crystal.clark@wsc.com", ("emp123", "Employee", "Development") },
                    
                    // Marketing Department Employees
                    { "karen.walker@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "gary.young@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "helen.king@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "paul.wright@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "diane.lopez@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "roger.hill@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "julie.green@wsc.com", ("emp123", "Employee", "Marketing") },
                    { "terry.adams@wsc.com", ("emp123", "Employee", "Marketing") },
                    
                    // Additional Departments
                    // Finance Department Employees (non-managers)
                    { "susan.financial@wsc.com", ("emp123", "Employee", "Finance") },
                    { "peter.accountant@wsc.com", ("emp123", "Employee", "Finance") },
                    { "linda.analyst@wsc.com", ("emp123", "Employee", "Finance") },
                    
                    // Legal Department
                    { "legal.counsel@wsc.com", ("manager123", "Owner", "Legal") },
                    { "attorney.smith@wsc.com", ("emp123", "Employee", "Legal") },
                    { "paralegal.jones@wsc.com", ("emp123", "Employee", "Legal") },
                    
                    // Customer Support
                    { "support.manager@wsc.com", ("manager123", "Owner", "Support") },
                    { "agent.one@wsc.com", ("emp123", "Employee", "Support") },
                    { "agent.two@wsc.com", ("emp123", "Employee", "Support") },
                    { "agent.three@wsc.com", ("emp123", "Employee", "Support") },
                    
                    // Executive Team
                    { "ceo@wsc.com", ("exec123", "Finance", "Executive") },
                    { "cfo@wsc.com", ("exec123", "Finance", "Executive") },
                    { "cto@wsc.com", ("exec123", "Finance", "Executive") }
                };

                var normalizedEmail = email.ToLower().Trim();
                Console.WriteLine($"Normalized email: '{normalizedEmail}'");
                Console.WriteLine($"Available users: {string.Join(", ", mockUsers.Keys)}");
                
                if (mockUsers.TryGetValue(normalizedEmail, out var user) && user.Password == password)
                {
                    Console.WriteLine($"User found and password matches. Role: {user.Role}, Department: {user.Department}");
                    
                    // Store user session in memory first (fallback if localStorage fails)
                    _currentUserEmail = normalizedEmail;
                    _currentUserRole = user.Role;
                    _currentUserDepartment = user.Department;
                    _isAuthenticated = true;
                    
                    try
                    {
                        // Try to store in localStorage
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", normalizedEmail);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", user.Role);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userDepartment", user.Department);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "loginTime", DateTimeOffset.UtcNow.ToString("O"));
                        Console.WriteLine("localStorage updated successfully");
                    }
                    catch (Exception jsEx)
                    {
                        Console.WriteLine($"localStorage error (continuing with in-memory session): {jsEx.Message}");
                    }
                    
                    AuthenticationStateChanged?.Invoke(true);
                    
                    Console.WriteLine("Login successful!");
                    return true;
                }

                Console.WriteLine("User not found or password doesn't match");
                return false;
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
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userDepartment");
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginTime");
                
                _isAuthenticated = false;
                AuthenticationStateChanged?.Invoke(false);
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
                
                // For development/testing, always start unauthenticated
                // Remove this block in production
                if (_isAuthenticated == false && string.IsNullOrEmpty(_currentUserEmail))
                {
                    Console.WriteLine("First check - ensuring clean state by clearing localStorage");
                    try
                    {
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userDepartment");
                        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "loginTime");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error clearing localStorage: {ex.Message}");
                    }
                    Console.WriteLine("Returning false for clean start");
                    return false;
                }
                
                // Check in-memory first
                if (_isAuthenticated && !string.IsNullOrEmpty(_currentUserEmail))
                {
                    Console.WriteLine($"Returning true from in-memory: {_currentUserEmail}");
                    return true;
                }
                
                try
                {
                    // Fallback to localStorage
                    var userEmail = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");
                    var loginTime = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "loginTime");
                    
                    Console.WriteLine($"localStorage check - userEmail: {userEmail}, loginTime: {loginTime}");
                    
                    if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(loginTime))
                    {
                        Console.WriteLine("No user data in localStorage, returning false");
                        _isAuthenticated = false;
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

                    // Restore in-memory state from localStorage
                    _currentUserEmail = userEmail;
                    _currentUserRole = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
                    _currentUserDepartment = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userDepartment");
                    _isAuthenticated = true;
                    
                    Console.WriteLine($"Session restored from localStorage: {_currentUserEmail}");
                    return true;
                }
                catch (Exception jsEx)
                {
                    Console.WriteLine($"localStorage access error: {jsEx.Message}");
                    // If localStorage isn't available, fall back to in-memory check
                    _isAuthenticated = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication check error: {ex.Message}");
                _isAuthenticated = false;
                return false;
            }
        }

        public async Task<string?> GetCurrentUserEmailAsync()
        {
            try
            {
                // Check in-memory first
                if (!string.IsNullOrEmpty(_currentUserEmail))
                {
                    return _currentUserEmail;
                }
                
                // Fallback to localStorage if authenticated
                if (await IsAuthenticatedAsync())
                {
                    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user email error: {ex.Message}");
                return _currentUserEmail;
            }
        }

        public async Task<string?> GetCurrentUserRoleAsync()
        {
            try
            {
                if (await IsAuthenticatedAsync())
                {
                    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get user role error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> GetCurrentUserDepartmentAsync()
        {
            try
            {
                if (await IsAuthenticatedAsync())
                {
                    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userDepartment");
                }
                return null;
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
                if (!string.IsNullOrEmpty(email))
                {
                    // Extract name from email (e.g., john.doe@wsc.com -> John Doe)
                    var namePart = email.Split('@')[0];
                    var nameParts = namePart.Split('.');
                    if (nameParts.Length >= 2)
                    {
                        return $"{CapitalizeFirst(nameParts[0])} {CapitalizeFirst(nameParts[1])}";
                    }
                    return CapitalizeFirst(namePart);
                }
                return null;
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
                var role = await GetCurrentUserRoleAsync();
                var department = await GetCurrentUserDepartmentAsync();
                var fullName = await GetCurrentUserFullNameAsync();
                
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role))
                {
                    // Extract first and last names
                    var nameParts = fullName?.Split(' ') ?? Array.Empty<string>();
                    var firstName = nameParts.Length > 0 ? nameParts[0] : "";
                    var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                    
                    return new User
                    {
                        Email = email,
                        Role = role,
                        Department = department ?? "",
                        FirstName = firstName,
                        LastName = lastName,
                        IsActive = true
                    };
                }
                return null;
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