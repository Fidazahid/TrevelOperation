using Microsoft.AspNetCore.Components;
using TravelOperation.Core.Services;
using TravelOperation.Core.Models;

namespace TrevelOperation.RazorLib.Components
{
    public abstract class AuthorizedPageBase : ComponentBase
    {
        [Inject] protected IAuthenticationService AuthService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected bool IsLoading { get; set; } = true;
        protected bool IsAuthorized { get; set; } = false;
        protected User? CurrentUser { get; set; }
        protected string ErrorMessage { get; set; } = "";

        // Override these in derived classes
        protected virtual List<string> RequiredRoles => new();
        protected virtual string? RequiredRole => null;
        protected virtual bool AllowAllAuthenticatedUsers => false;

        protected override async Task OnInitializedAsync()
        {
            await CheckAuthorizationAsync();
            
            if (IsAuthorized)
            {
                await OnAuthorizedInitializeAsync();
            }
            
            IsLoading = false;
            StateHasChanged();
        }

        protected virtual async Task OnAuthorizedInitializeAsync()
        {
            // Override in derived classes for authorized initialization logic
            await Task.CompletedTask;
        }

        private async Task CheckAuthorizationAsync()
        {
            try
            {
                var isAuthenticated = await AuthService.IsAuthenticatedAsync();
                
                if (!isAuthenticated)
                {
                    Navigation.NavigateTo("/login");
                    return;
                }

                CurrentUser = await AuthService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    Navigation.NavigateTo("/login");
                    return;
                }

                // If allowing all authenticated users, authorize immediately
                if (AllowAllAuthenticatedUsers)
                {
                    IsAuthorized = true;
                    return;
                }

                // Build list of required roles
                var requiredRoles = new List<string>(RequiredRoles);
                if (!string.IsNullOrEmpty(RequiredRole))
                {
                    requiredRoles.Add(RequiredRole);
                }

                // If no specific roles required, allow all authenticated users
                if (requiredRoles.Count == 0)
                {
                    IsAuthorized = true;
                    return;
                }

                // Check if user has required role
                IsAuthorized = requiredRoles.Contains(CurrentUser.Role);
                
                if (!IsAuthorized)
                {
                    ErrorMessage = $"Access denied. Required role: {string.Join(", ", requiredRoles)}. Your role: {CurrentUser.Role}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AuthorizedPageBase: Error checking authorization: {ex.Message}");
                ErrorMessage = "Authentication error occurred.";
                Navigation.NavigateTo("/login");
            }
        }

        protected bool IsInRole(params string[] roles)
        {
            return CurrentUser != null && roles.Contains(CurrentUser.Role);
        }

        protected bool IsFinanceUser()
        {
            return IsInRole("Finance");
        }

        protected bool IsOwner()
        {
            return IsInRole("Owner");
        }

        protected bool IsEmployee()
        {
            return IsInRole("Employee");
        }

        protected bool CanAccessUserManagement()
        {
            return IsFinanceUser();
        }

        protected bool CanAccessAllData()
        {
            return IsFinanceUser();
        }

        protected bool CanAccessDepartmentData()
        {
            return IsFinanceUser() || IsOwner();
        }
    }
}