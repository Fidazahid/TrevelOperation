using TravelOperation.Core.Models;
using System.Collections.Concurrent;
using TravelOperation.Core.Data;
using Microsoft.EntityFrameworkCore;
using AuthUser = TravelOperation.Core.Models.User;

namespace TravelOperation.Core.Services
{
    /// <summary>
    /// Shared static user store for authentication with database persistence
    /// Used by both AuthenticationService and UserManagementService
    /// </summary>
    public static class UserStore
    {
        private static readonly ConcurrentDictionary<string, AuthUser> _users = new();
        private static TravelDbContext? _dbContext;

        public static void Initialize(TravelDbContext dbContext)
        {
            _dbContext = dbContext;
            LoadUsersFromDatabase();
            Console.WriteLine($"[UserStore] Initialized with {_users.Count} users from database");
        }

        private static void LoadUsersFromDatabase()
        {
            if (_dbContext == null) return;

            try
            {
                var users = _dbContext.AuthUsers.ToList();
                _users.Clear();
                foreach (var user in users)
                {
                    _users.TryAdd(user.Email.ToLower(), user);
                }
                Console.WriteLine($"[UserStore] Loaded {users.Count} active users from database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserStore] Error loading users from database: {ex.Message}");
            }
        }

        public static AuthUser? GetUserByEmailAndPassword(string email, string password)
        {
            var normalizedEmail = email.ToLower().Trim();
            if (_users.TryGetValue(normalizedEmail, out var user))
            {
                if (user.Password == password)
                {
                    return user;
                }
            }
            return null;
        }

        public static AuthUser? GetUserByEmail(string email)
        {
            var normalizedEmail = email.ToLower().Trim();
            if (_users.TryGetValue(normalizedEmail, out var user))
            {
                return user;
            }
            return null;
        }

        public static List<AuthUser> GetAllActiveUsers()
        {
            return _users.Values.OrderBy(u => u.Department).ThenBy(u => u.LastName).ToList();
        }

        public static bool AddUser(AuthUser user)
        {
            var normalizedEmail = user.Email.ToLower().Trim();
            user.Email = normalizedEmail;
            
            bool added = _users.TryAdd(normalizedEmail, user);
            
            if (added)
            {
                Console.WriteLine($"[UserStore] ✅ User added to memory: {user.Email}");
                
                // Save to database
                if (_dbContext != null)
                {
                    try
                    {
                        _dbContext.AuthUsers.Add(user);
                        _dbContext.SaveChanges();
                        Console.WriteLine($"[UserStore] ✅ User saved to database: {user.Email}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UserStore] ❌ Error saving user to database: {ex.Message}");
                        // Remove from memory if database save failed
                        _users.TryRemove(normalizedEmail, out _);
                        return false;
                    }
                }
            }
            
            return added;
        }

        public static bool UpdateUser(AuthUser user)
        {
            var normalizedEmail = user.Email.ToLower().Trim();
            if (_users.TryGetValue(normalizedEmail, out var existingUser))
            {
                // Update the existing user reference
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;
                existingUser.Department = user.Department;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                
                // Save to database
                if (_dbContext != null)
                {
                    try
                    {
                        var dbUser = _dbContext.AuthUsers.FirstOrDefault(u => u.Email == normalizedEmail);
                        if (dbUser != null)
                        {
                            dbUser.Password = user.Password;
                            dbUser.Role = user.Role;
                            dbUser.Department = user.Department;
                            dbUser.FirstName = user.FirstName;
                            dbUser.LastName = user.LastName;
                            _dbContext.SaveChanges();
                            Console.WriteLine($"[UserStore] ✅ User updated in database: {user.Email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UserStore] ❌ Error updating user in database: {ex.Message}");
                        return false;
                    }
                }
                
                return true;
            }
            return false;
        }

        public static bool DeactivateUser(string email)
        {
            var normalizedEmail = email.ToLower().Trim();
            if (_users.TryGetValue(normalizedEmail, out var user))
            {
                // Remove from memory (soft delete)
                _users.TryRemove(normalizedEmail, out _);
                
                // Delete from database
                if (_dbContext != null)
                {
                    try
                    {
                        var dbUser = _dbContext.AuthUsers.FirstOrDefault(u => u.Email == normalizedEmail);
                        if (dbUser != null)
                        {
                            _dbContext.AuthUsers.Remove(dbUser);
                            _dbContext.SaveChanges();
                            Console.WriteLine($"[UserStore] ✅ User deleted from database: {email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[UserStore] ❌ Error deleting user from database: {ex.Message}");
                        return false;
                    }
                }
                
                return true;
            }
            return false;
        }

        public static bool IsEmailAvailable(string email)
        {
            var normalizedEmail = email.ToLower().Trim();
            return !_users.ContainsKey(normalizedEmail);
        }
    }
}
