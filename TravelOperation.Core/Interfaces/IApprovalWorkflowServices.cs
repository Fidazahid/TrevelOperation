using TravelOperation.Core.Models.Entities;

namespace TravelOperation.Core.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int employeeId);
    Task<Employee?> GetEmployeeByEmailAsync(string email);
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<Employee> UpdateEmployeeAsync(Employee employee);
    Task DeleteEmployeeAsync(int employeeId);
    Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string department);
    Task<IEnumerable<Employee>> GetEmployeesByRoleAsync(string role);
}

public interface ITripRequestService
{
    Task<IEnumerable<TripRequest>> GetAllTripRequestsAsync();
    Task<TripRequest?> GetTripRequestByIdAsync(int tripRequestId);
    Task<IEnumerable<TripRequest>> GetTripRequestsByEmployeeAsync(int employeeId);
    Task<IEnumerable<TripRequest>> GetTripRequestsByStatusAsync(string status);
    Task<TripRequest> CreateTripRequestAsync(TripRequest tripRequest);
    Task<TripRequest> UpdateTripRequestAsync(TripRequest tripRequest);
    Task DeleteTripRequestAsync(int tripRequestId);
    Task<TripRequest> SubmitTripRequestAsync(int tripRequestId);
    Task<TripRequest> ApproveTripRequestAsync(int tripRequestId, int approverEmployeeId, string comments = "");
    Task<TripRequest> RejectTripRequestAsync(int tripRequestId, int approverEmployeeId, string comments = "");
    Task<IEnumerable<TripRequest>> GetPendingApprovalsForManagerAsync(string managerEmail);
    Task<IEnumerable<TripRequest>> GetPendingApprovalsForFinanceAsync();
}

public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int expenseId);
    Task<IEnumerable<Expense>> GetExpensesByTripRequestAsync(int tripRequestId);
    Task<IEnumerable<Expense>> GetExpensesByEmployeeAsync(int employeeId);
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task<Expense> UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(int expenseId);
    Task<bool> ValidateExpenseAgainstPolicyAsync(Expense expense);
    Task<IEnumerable<Expense>> GetExpensesByStatusAsync(string status);
    Task<decimal> GetTotalExpensesForTripAsync(int tripRequestId);
}

public interface IPolicyService
{
    Task<IEnumerable<Policy>> GetAllPoliciesAsync();
    Task<Policy?> GetPolicyByIdAsync(int policyId);
    Task<IEnumerable<Policy>> GetPoliciesByCategoryAsync(string category);
    Task<Policy> CreatePolicyAsync(Policy policy);
    Task<Policy> UpdatePolicyAsync(Policy policy);
    Task DeletePolicyAsync(int policyId);
    Task<bool> ValidateExpenseAgainstPolicyAsync(string category, decimal amount, string period = "Daily");
    Task<decimal> GetMaxAllowedAmountAsync(string category, string period = "Daily");
}

public interface IApprovalService
{
    Task<IEnumerable<Approval>> GetAllApprovalsAsync();
    Task<Approval?> GetApprovalByIdAsync(int approvalId);
    Task<IEnumerable<Approval>> GetApprovalsByTripRequestAsync(int tripRequestId);
    Task<IEnumerable<Approval>> GetApprovalsByApproverAsync(int approverEmployeeId);
    Task<Approval> CreateApprovalAsync(Approval approval);
    Task<Approval> UpdateApprovalAsync(Approval approval);
    Task<IEnumerable<Approval>> GetPendingApprovalsAsync();
    Task<IEnumerable<Approval>> GetPendingApprovalsForEmployeeAsync(int employeeId);
}

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(User user, string password);
    Task<User> UpdateUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task<bool> ValidatePasswordAsync(string username, string password);
    Task<User?> AuthenticateAsync(string username, string password);
    Task ChangePasswordAsync(int userId, string newPassword);
}