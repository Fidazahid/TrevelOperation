using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TravelOperation.Core.Data;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Services;

public class EmployeeService : IEmployeeService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(TravelDbContext context, ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<Employee?> GetEmployeeByEmailAsync(string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email && e.IsActive);
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        employee.CreatedAt = DateTime.UtcNow;
        employee.ModifiedAt = DateTime.UtcNow;
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created employee: {Name} ({Email})", employee.Name, employee.Email);
        return employee;
    }

    public async Task<Employee> UpdateEmployeeAsync(Employee employee)
    {
        employee.ModifiedAt = DateTime.UtcNow;
        
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated employee: {EmployeeId}", employee.EmployeeId);
        return employee;
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        var employee = await GetEmployeeByIdAsync(employeeId);
        if (employee != null)
        {
            employee.IsActive = false;
            employee.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deactivated employee: {EmployeeId}", employeeId);
        }
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department)
    {
        return await _context.Employees
            .Where(e => e.Department == department && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(string role)
    {
        return await _context.Employees
            .Where(e => e.Role == role && e.IsActive)
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
}

public class TripRequestService : ITripRequestService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<TripRequestService> _logger;
    private readonly IApprovalService _approvalService;

    public TripRequestService(TravelDbContext context, ILogger<TripRequestService> logger, IApprovalService approvalService)
    {
        _context = context;
        _logger = logger;
        _approvalService = approvalService;
    }

    public async Task<IEnumerable<TripRequest>> GetAllTripRequestsAsync()
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Expenses)
            .Include(t => t.Approvals)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TripRequest?> GetTripRequestByIdAsync(int tripRequestId)
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Expenses)
            .Include(t => t.Approvals)
            .FirstOrDefaultAsync(t => t.TripRequestId == tripRequestId);
    }

    public async Task<IEnumerable<TripRequest>> GetTripRequestsByEmployeeAsync(int employeeId)
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Expenses)
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripRequest>> GetTripRequestsByStatusAsync(string status)
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Expenses)
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<TripRequest> CreateTripRequestAsync(TripRequest tripRequest)
    {
        tripRequest.Status = "Draft";
        tripRequest.CreatedAt = DateTime.UtcNow;
        tripRequest.ModifiedAt = DateTime.UtcNow;
        
        _context.TripRequests.Add(tripRequest);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created trip request: {TripRequestId} for employee {EmployeeId}", 
            tripRequest.TripRequestId, tripRequest.EmployeeId);
        return tripRequest;
    }

    public async Task<TripRequest> UpdateTripRequestAsync(TripRequest tripRequest)
    {
        tripRequest.ModifiedAt = DateTime.UtcNow;
        
        _context.TripRequests.Update(tripRequest);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated trip request: {TripRequestId}", tripRequest.TripRequestId);
        return tripRequest;
    }

    public async Task DeleteTripRequestAsync(int tripRequestId)
    {
        var tripRequest = await GetTripRequestByIdAsync(tripRequestId);
        if (tripRequest != null)
        {
            _context.TripRequests.Remove(tripRequest);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted trip request: {TripRequestId}", tripRequestId);
        }
    }

    public async Task<TripRequest> SubmitTripRequestAsync(int tripRequestId)
    {
        var tripRequest = await GetTripRequestByIdAsync(tripRequestId);
        if (tripRequest == null)
            throw new InvalidOperationException($"Trip request {tripRequestId} not found");

        if (tripRequest.Status != "Draft")
            throw new InvalidOperationException($"Trip request {tripRequestId} is not in draft status");

        tripRequest.Status = "Submitted";
        tripRequest.SubmittedAt = DateTime.UtcNow;
        tripRequest.ModifiedAt = DateTime.UtcNow;

        // Create manager approval request
        if (!string.IsNullOrEmpty(tripRequest.Employee.ManagerEmail))
        {
            var manager = await _context.Employees.FirstOrDefaultAsync(e => e.Email == tripRequest.Employee.ManagerEmail);
            if (manager != null)
            {
                var approval = new Approval
                {
                    TripRequestId = tripRequestId,
                    ApprovedByEmployeeId = manager.EmployeeId,
                    Role = "Manager",
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };
                await _approvalService.CreateApprovalAsync(approval);
            }
        }

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Submitted trip request: {TripRequestId}", tripRequestId);
        return tripRequest;
    }

    public async Task<TripRequest> ApproveTripRequestAsync(int tripRequestId, int approverEmployeeId, string comments = "")
    {
        var tripRequest = await GetTripRequestByIdAsync(tripRequestId);
        if (tripRequest == null)
            throw new InvalidOperationException($"Trip request {tripRequestId} not found");

        var pendingApproval = await _context.Approvals
            .FirstOrDefaultAsync(a => a.TripRequestId == tripRequestId && 
                                    a.ApprovedByEmployeeId == approverEmployeeId && 
                                    a.Status == "Pending");

        if (pendingApproval == null)
            throw new InvalidOperationException($"No pending approval found for trip request {tripRequestId}");

        pendingApproval.Status = "Approved";
        pendingApproval.Comments = comments;
        pendingApproval.ApprovedDate = DateTime.UtcNow;
        pendingApproval.ModifiedAt = DateTime.UtcNow;

        // Check if all required approvals are complete
        var allApprovals = await _context.Approvals
            .Where(a => a.TripRequestId == tripRequestId)
            .ToListAsync();

        var managerApproved = allApprovals.Any(a => a.Role == "Manager" && a.Status == "Approved");
        var financeApproved = allApprovals.Any(a => a.Role == "Finance" && a.Status == "Approved");

        if (pendingApproval.Role == "Manager" && managerApproved)
        {
            // Create finance approval if needed for high-value trips
            if (tripRequest.EstimatedCost > 1000)
            {
                var financeOfficer = await _context.Employees.FirstOrDefaultAsync(e => e.Role == "Finance");
                if (financeOfficer != null)
                {
                    var financeApproval = new Approval
                    {
                        TripRequestId = tripRequestId,
                        ApprovedByEmployeeId = financeOfficer.EmployeeId,
                        Role = "Finance",
                        Status = "Pending",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };
                    await _approvalService.CreateApprovalAsync(financeApproval);
                }
            }
            else
            {
                tripRequest.Status = "Approved";
                tripRequest.ApprovedAt = DateTime.UtcNow;
            }
        }
        else if (pendingApproval.Role == "Finance" && managerApproved && financeApproved)
        {
            tripRequest.Status = "Approved";
            tripRequest.ApprovedAt = DateTime.UtcNow;
        }

        tripRequest.ModifiedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Approved trip request: {TripRequestId} by {ApproverEmployeeId}", 
            tripRequestId, approverEmployeeId);
        return tripRequest;
    }

    public async Task<TripRequest> RejectTripRequestAsync(int tripRequestId, int approverEmployeeId, string comments = "")
    {
        var tripRequest = await GetTripRequestByIdAsync(tripRequestId);
        if (tripRequest == null)
            throw new InvalidOperationException($"Trip request {tripRequestId} not found");

        var pendingApproval = await _context.Approvals
            .FirstOrDefaultAsync(a => a.TripRequestId == tripRequestId && 
                                    a.ApprovedByEmployeeId == approverEmployeeId && 
                                    a.Status == "Pending");

        if (pendingApproval == null)
            throw new InvalidOperationException($"No pending approval found for trip request {tripRequestId}");

        pendingApproval.Status = "Rejected";
        pendingApproval.Comments = comments;
        pendingApproval.ApprovedDate = DateTime.UtcNow;
        pendingApproval.ModifiedAt = DateTime.UtcNow;

        tripRequest.Status = "Rejected";
        tripRequest.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Rejected trip request: {TripRequestId} by {ApproverEmployeeId}", 
            tripRequestId, approverEmployeeId);
        return tripRequest;
    }

    public async Task<IEnumerable<TripRequest>> GetPendingApprovalsForManagerAsync(string managerEmail)
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Approvals)
            .Where(t => t.Employee.ManagerEmail == managerEmail && 
                       t.Approvals.Any(a => a.Role == "Manager" && a.Status == "Pending"))
            .OrderByDescending(t => t.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TripRequest>> GetPendingApprovalsForFinanceAsync()
    {
        return await _context.TripRequests
            .Include(t => t.Employee)
            .Include(t => t.Approvals)
            .Where(t => t.Approvals.Any(a => a.Role == "Finance" && a.Status == "Pending"))
            .OrderByDescending(t => t.SubmittedAt)
            .ToListAsync();
    }
}

public class ExpenseService : IExpenseService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<ExpenseService> _logger;
    private readonly IPolicyService _policyService;

    public ExpenseService(TravelDbContext context, ILogger<ExpenseService> logger, IPolicyService policyService)
    {
        _context = context;
        _logger = logger;
        _policyService = policyService;
    }

    public async Task<IEnumerable<Expense>> GetAllExpensesAsync()
    {
        return await _context.Expenses
            .Include(e => e.Employee)
            .Include(e => e.TripRequest)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int expenseId)
    {
        return await _context.Expenses
            .Include(e => e.Employee)
            .Include(e => e.TripRequest)
            .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);
    }

    public async Task<IEnumerable<Expense>> GetExpensesByTripRequestAsync(int tripRequestId)
    {
        return await _context.Expenses
            .Include(e => e.Employee)
            .Where(e => e.TripRequestId == tripRequestId)
            .OrderBy(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetExpensesByEmployeeAsync(int employeeId)
    {
        return await _context.Expenses
            .Include(e => e.TripRequest)
            .Where(e => e.EmployeeId == employeeId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        expense.CreatedAt = DateTime.UtcNow;
        expense.ModifiedAt = DateTime.UtcNow;
        expense.Status = "Draft";
        
        // Validate against policy
        var isValid = await ValidateExpenseAgainstPolicyAsync(expense);
        if (!isValid)
        {
            _logger.LogWarning("Expense exceeds policy limits: {Category} ${Amount}", 
                expense.Category, expense.Amount);
        }
        
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created expense: {ExpenseId} for trip {TripRequestId}", 
            expense.ExpenseId, expense.TripRequestId);
        return expense;
    }

    public async Task<Expense> UpdateExpenseAsync(Expense expense)
    {
        expense.ModifiedAt = DateTime.UtcNow;
        
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated expense: {ExpenseId}", expense.ExpenseId);
        return expense;
    }

    public async Task DeleteExpenseAsync(int expenseId)
    {
        var expense = await GetExpenseByIdAsync(expenseId);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted expense: {ExpenseId}", expenseId);
        }
    }

    public async Task<bool> ValidateExpenseAgainstPolicyAsync(Expense expense)
    {
        return await _policyService.ValidateExpenseAgainstPolicyAsync(
            expense.Category, expense.Amount);
    }

    public async Task<IEnumerable<Expense>> GetExpensesByStatusAsync(string status)
    {
        return await _context.Expenses
            .Include(e => e.Employee)
            .Include(e => e.TripRequest)
            .Where(e => e.Status == status)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalExpensesForTripAsync(int tripRequestId)
    {
        return await _context.Expenses
            .Where(e => e.TripRequestId == tripRequestId)
            .SumAsync(e => e.AmountUSD);
    }
}

public class PolicyService : IPolicyService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<PolicyService> _logger;

    public PolicyService(TravelDbContext context, ILogger<PolicyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Policy>> GetAllPoliciesAsync()
    {
        return await _context.Policies
            .Where(p => p.IsActive)
            .OrderBy(p => p.Category)
            .ToListAsync();
    }

    public async Task<Policy?> GetPolicyByIdAsync(int policyId)
    {
        return await _context.Policies
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);
    }

    public async Task<IEnumerable<Policy>> GetPoliciesByCategoryAsync(string category)
    {
        return await _context.Policies
            .Where(p => p.Category == category && p.IsActive)
            .OrderBy(p => p.Period)
            .ToListAsync();
    }

    public async Task<Policy> CreatePolicyAsync(Policy policy)
    {
        policy.CreatedAt = DateTime.UtcNow;
        policy.ModifiedAt = DateTime.UtcNow;
        
        _context.Policies.Add(policy);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created policy: {Category} - ${MaxAmount}", 
            policy.Category, policy.MaxAmount);
        return policy;
    }

    public async Task<Policy> UpdatePolicyAsync(Policy policy)
    {
        policy.ModifiedAt = DateTime.UtcNow;
        
        _context.Policies.Update(policy);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated policy: {PolicyId}", policy.PolicyId);
        return policy;
    }

    public async Task DeletePolicyAsync(int policyId)
    {
        var policy = await GetPolicyByIdAsync(policyId);
        if (policy != null)
        {
            policy.IsActive = false;
            policy.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deactivated policy: {PolicyId}", policyId);
        }
    }

    public async Task<bool> ValidateExpenseAgainstPolicyAsync(string category, decimal amount, string period = "Daily")
    {
        var maxAmount = await GetMaxAllowedAmountAsync(category, period);
        return amount <= maxAmount;
    }

    public async Task<decimal> GetMaxAllowedAmountAsync(string category, string period = "Daily")
    {
        var policy = await _context.Policies
            .FirstOrDefaultAsync(p => p.Category == category && p.Period == period && p.IsActive);
        
        return policy?.MaxAmount ?? 0;
    }
}

public class ApprovalService : IApprovalService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(TravelDbContext context, ILogger<ApprovalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Approval>> GetAllApprovalsAsync()
    {
        return await _context.Approvals
            .Include(a => a.TripRequest)
            .Include(a => a.ApprovedBy)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Approval?> GetApprovalByIdAsync(int approvalId)
    {
        return await _context.Approvals
            .Include(a => a.TripRequest)
            .Include(a => a.ApprovedBy)
            .FirstOrDefaultAsync(a => a.ApprovalId == approvalId);
    }

    public async Task<IEnumerable<Approval>> GetApprovalsByTripRequestAsync(int tripRequestId)
    {
        return await _context.Approvals
            .Include(a => a.ApprovedBy)
            .Where(a => a.TripRequestId == tripRequestId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Approval>> GetApprovalsByApproverAsync(int approverEmployeeId)
    {
        return await _context.Approvals
            .Include(a => a.TripRequest)
            .ThenInclude(t => t.Employee)
            .Where(a => a.ApprovedByEmployeeId == approverEmployeeId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Approval> CreateApprovalAsync(Approval approval)
    {
        approval.CreatedAt = DateTime.UtcNow;
        approval.ModifiedAt = DateTime.UtcNow;
        
        _context.Approvals.Add(approval);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created approval: {ApprovalId} for trip {TripRequestId}", 
            approval.ApprovalId, approval.TripRequestId);
        return approval;
    }

    public async Task<Approval> UpdateApprovalAsync(Approval approval)
    {
        approval.ModifiedAt = DateTime.UtcNow;
        
        _context.Approvals.Update(approval);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated approval: {ApprovalId}", approval.ApprovalId);
        return approval;
    }

    public async Task<IEnumerable<Approval>> GetPendingApprovalsAsync()
    {
        return await _context.Approvals
            .Include(a => a.TripRequest)
            .ThenInclude(t => t.Employee)
            .Include(a => a.ApprovedBy)
            .Where(a => a.Status == "Pending")
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Approval>> GetPendingApprovalsForEmployeeAsync(int employeeId)
    {
        return await _context.Approvals
            .Include(a => a.TripRequest)
            .ThenInclude(t => t.Employee)
            .Where(a => a.ApprovedByEmployeeId == employeeId && a.Status == "Pending")
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }
}

public class UserService : IUserService
{
    private readonly TravelDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(TravelDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Employee)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.CreatedAt = DateTime.UtcNow;
        user.ModifiedAt = DateTime.UtcNow;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created user: {Username} for employee {EmployeeId}", 
            user.Username, user.EmployeeId);
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        user.ModifiedAt = DateTime.UtcNow;
        
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated user: {UserId}", user.UserId);
        return user;
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user != null)
        {
            user.IsActive = false;
            user.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deactivated user: {UserId}", userId);
        }
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        if (user == null) return false;
        
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        if (user == null) return null;
        
        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return user;
        }
        
        return null;
    }

    public async Task ChangePasswordAsync(int userId, string newPassword)
    {
        var user = await GetUserByIdAsync(userId);
        if (user != null)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Changed password for user: {UserId}", userId);
        }
    }
}