using Microsoft.AspNetCore.Components;
using TravelOperation.Core.Models.Entities;
using TravelOperation.Core.Services.Interfaces;
using System.Text;
using AuditLogEntity = TravelOperation.Core.Models.Entities.AuditLog;

namespace TrevelOperation.RazorLib.Pages.Settings;

public partial class AuditVerification
{
    private bool isRunning = false;
    private string statusMessage = "";
    private int testsRun = 0;
    private int testsPassed = 0;
    private int testsFailed = 0;
    private int auditEntriesCreated = 0;

    private List<TestResult> testResults = new();
    private List<AuditLogEntity> recentAuditLogs = new();
    private List<string> testTransactionIds = new();
    private List<int> testTripIds = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadRecentAuditLogs();
    }

    private async Task RunAllTests()
    {
        isRunning = true;
        statusMessage = "🔄 Running all tests...";
        StateHasChanged();

        ClearResults();

        await RunTransactionTests();
        await Task.Delay(500); // Brief pause between test suites
        await RunTripTests();

        statusMessage = testsRun == testsPassed 
            ? $"✅ All tests passed! ({testsPassed}/{testsRun})"
            : $"⚠️ Tests completed with {testsFailed} failures ({testsPassed}/{testsRun} passed)";

        isRunning = false;
        await LoadRecentAuditLogs();
        StateHasChanged();
    }

    private async Task RunTransactionTests()
    {
        isRunning = true;
        statusMessage = "🔄 Running transaction tests...";
        StateHasChanged();

        // Test 1: CREATE Transaction
        await TestCreateTransaction();

        // Test 2: UPDATE Transaction
        await TestUpdateTransaction();

        // Test 3: DELETE Transaction
        await TestDeleteTransaction();

        statusMessage = "✅ Transaction tests completed";
        isRunning = false;
        await LoadRecentAuditLogs();
        StateHasChanged();
    }

    private async Task RunTripTests()
    {
        isRunning = true;
        statusMessage = "🔄 Running trip tests...";
        StateHasChanged();

        // Test 1: CREATE Trip
        await TestCreateTrip();

        // Test 2: UPDATE Trip
        await TestUpdateTrip();

        // Test 3: DELETE Trip
        await TestDeleteTrip();

        statusMessage = "✅ Trip tests completed";
        isRunning = false;
        await LoadRecentAuditLogs();
        StateHasChanged();
    }

    private async Task TestCreateTransaction()
    {
        var testName = "Create Transaction";
        var testId = $"TEST-CREATE-{DateTime.Now.Ticks}";

        try
        {
            // Get categories and sources
            var categories = await LookupService.GetCategoriesAsync();
            var category = categories.FirstOrDefault();
            var sources = await LookupService.GetSourcesAsync();
            var source = sources.FirstOrDefault();

            // Create test transaction
            var transaction = new Transaction
            {
                TransactionId = testId,
                SourceId = source?.SourceId ?? 1,
                Email = "audit.test@test.com",
                TransactionDate = DateTime.Now.Date,
                AuthorizationDate = DateTime.Now.Date,
                TransactionType = "Purchase",
                CategoryId = category?.CategoryId ?? 1,
                Vendor = "Audit Test Vendor",
                Currency = "USD",
                Amount = 100.00m,
                AmountUSD = 100.00m,
                IsValid = false,
                DataValidation = true
            };

            await TransactionService.CreateTransactionAsync(transaction);
            testTransactionIds.Add(testId);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Transaction", testId);
            var auditLogged = auditLogs.Any(a => a.Action.Contains("Add"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "CREATE",
                EntityType = "Transaction",
                Success = auditLogged,
                AuditLogged = auditLogged,
                Details = auditLogged 
                    ? $"✓ Audit log created successfully\nTransaction ID: {testId}\nAudit entries found: {auditLogs.Count()}"
                    : $"✗ No audit log found for created transaction\nTransaction ID: {testId}"
            };

            AddTestResult(result);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "CREATE",
                EntityType = "Transaction",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private async Task TestUpdateTransaction()
    {
        var testName = "Update Transaction";
        
        if (!testTransactionIds.Any())
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Transaction",
                Success = false,
                AuditLogged = false,
                Details = "⚠️ No test transaction available. Run CREATE test first."
            });
            return;
        }

        var testId = testTransactionIds.First();

        try
        {
            var transaction = await TransactionService.GetTransactionByIdAsync(testId);
            if (transaction == null)
            {
                AddTestResult(new TestResult
                {
                    TestName = testName,
                    Operation = "UPDATE",
                    EntityType = "Transaction",
                    Success = false,
                    AuditLogged = false,
                    Details = $"✗ Transaction not found: {testId}"
                });
                return;
            }

            // Update transaction
            transaction.Amount = 150.00m;
            transaction.AmountUSD = 150.00m;
            transaction.Vendor = "Audit Test Vendor - UPDATED";

            await TransactionService.UpdateTransactionAsync(transaction);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Transaction", testId);
            var updateLog = auditLogs.FirstOrDefault(a => a.Action.Contains("Modif"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Transaction",
                Success = updateLog != null,
                AuditLogged = updateLog != null,
                Details = updateLog != null
                    ? $"✓ Audit log created successfully\nTransaction ID: {testId}\nOld values recorded: {!string.IsNullOrEmpty(updateLog.OldValue)}\nNew values recorded: {!string.IsNullOrEmpty(updateLog.NewValue)}"
                    : $"✗ No audit log found for updated transaction\nTransaction ID: {testId}\nTotal audit entries: {auditLogs.Count()}"
            };

            AddTestResult(result);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Transaction",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private async Task TestDeleteTransaction()
    {
        var testName = "Delete Transaction";
        
        if (!testTransactionIds.Any())
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Transaction",
                Success = false,
                AuditLogged = false,
                Details = "⚠️ No test transaction available. Run CREATE test first."
            });
            return;
        }

        var testId = testTransactionIds.First();

        try
        {
            await TransactionService.DeleteTransactionAsync(testId);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Transaction", testId);
            var deleteLog = auditLogs.FirstOrDefault(a => a.Action.Contains("Delet"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Transaction",
                Success = deleteLog != null,
                AuditLogged = deleteLog != null,
                Details = deleteLog != null
                    ? $"✓ Audit log created successfully\nTransaction ID: {testId}\nOld values recorded: {!string.IsNullOrEmpty(deleteLog.OldValue)}"
                    : $"✗ No audit log found for deleted transaction\nTransaction ID: {testId}\nTotal audit entries: {auditLogs.Count()}"
            };

            AddTestResult(result);
            testTransactionIds.Remove(testId);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Transaction",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private async Task TestCreateTrip()
    {
        var testName = "Create Trip";

        try
        {
            // Get required lookups
            var purposes = await LookupService.GetPurposesAsync();
            var tripTypes = await LookupService.GetTripTypesAsync();
            var statuses = await LookupService.GetStatusesAsync();
            var validationStatuses = await LookupService.GetValidationStatusesAsync();

            var trip = new Trip
            {
                TripName = "Audit Test Trip",
                Email = "audit.test@test.com",
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(2),
                Duration = 3,
                Country1 = "USA",
                City1 = "New York",
                PurposeId = purposes.FirstOrDefault()?.PurposeId ?? 1,
                TripTypeId = tripTypes.FirstOrDefault()?.TripTypeId ?? 1,
                StatusId = statuses.FirstOrDefault()?.StatusId ?? 1,
                ValidationStatusId = validationStatuses.FirstOrDefault()?.ValidationStatusId ?? 1,
                IsManual = true
            };

            var createdTrip = await TripService.CreateTripAsync(trip);
            testTripIds.Add(createdTrip.TripId);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Trip", createdTrip.TripId.ToString());
            var auditLogged = auditLogs.Any(a => a.Action.Contains("Add"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "CREATE",
                EntityType = "Trip",
                Success = auditLogged,
                AuditLogged = auditLogged,
                Details = auditLogged
                    ? $"✓ Audit log created successfully\nTrip ID: {createdTrip.TripId}\nAudit entries found: {auditLogs.Count()}"
                    : $"✗ No audit log found for created trip\nTrip ID: {createdTrip.TripId}"
            };

            AddTestResult(result);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "CREATE",
                EntityType = "Trip",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private async Task TestUpdateTrip()
    {
        var testName = "Update Trip";
        
        if (!testTripIds.Any())
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Trip",
                Success = false,
                AuditLogged = false,
                Details = "⚠️ No test trip available. Run CREATE test first."
            });
            return;
        }

        var testId = testTripIds.First();

        try
        {
            var trip = await TripService.GetTripByIdAsync(testId);
            if (trip == null)
            {
                AddTestResult(new TestResult
                {
                    TestName = testName,
                    Operation = "UPDATE",
                    EntityType = "Trip",
                    Success = false,
                    AuditLogged = false,
                    Details = $"✗ Trip not found: {testId}"
                });
                return;
            }

            // Update trip
            trip.TripName = "Audit Test Trip - UPDATED";
            trip.Duration = 4;

            await TripService.UpdateTripAsync(trip);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Trip", testId.ToString());
            var updateLog = auditLogs.FirstOrDefault(a => a.Action.Contains("Modif"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Trip",
                Success = updateLog != null,
                AuditLogged = updateLog != null,
                Details = updateLog != null
                    ? $"✓ Audit log created successfully\nTrip ID: {testId}\nOld values recorded: {!string.IsNullOrEmpty(updateLog.OldValue)}\nNew values recorded: {!string.IsNullOrEmpty(updateLog.NewValue)}"
                    : $"✗ No audit log found for updated trip\nTrip ID: {testId}\nTotal audit entries: {auditLogs.Count()}"
            };

            AddTestResult(result);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "UPDATE",
                EntityType = "Trip",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private async Task TestDeleteTrip()
    {
        var testName = "Delete Trip";
        
        if (!testTripIds.Any())
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Trip",
                Success = false,
                AuditLogged = false,
                Details = "⚠️ No test trip available. Run CREATE test first."
            });
            return;
        }

        var testId = testTripIds.First();

        try
        {
            await TripService.DeleteTripAsync(testId);

            // Wait a moment for audit log to be written
            await Task.Delay(200);

            // Check if audit log was created
            var auditLogs = await AuditService.GetAuditHistoryAsync("Trip", testId.ToString());
            var deleteLog = auditLogs.FirstOrDefault(a => a.Action.Contains("Delet"));

            var result = new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Trip",
                Success = deleteLog != null,
                AuditLogged = deleteLog != null,
                Details = deleteLog != null
                    ? $"✓ Audit log created successfully\nTrip ID: {testId}\nOld values recorded: {!string.IsNullOrEmpty(deleteLog.OldValue)}"
                    : $"✗ No audit log found for deleted trip\nTrip ID: {testId}\nTotal audit entries: {auditLogs.Count()}"
            };

            AddTestResult(result);
            testTripIds.Remove(testId);
        }
        catch (Exception ex)
        {
            AddTestResult(new TestResult
            {
                TestName = testName,
                Operation = "DELETE",
                EntityType = "Trip",
                Success = false,
                AuditLogged = false,
                Details = $"✗ Exception: {ex.Message}"
            });
        }
    }

    private void AddTestResult(TestResult result)
    {
        testResults.Add(result);
        testsRun++;
        
        if (result.Success)
            testsPassed++;
        else
            testsFailed++;

        if (result.AuditLogged)
            auditEntriesCreated++;

        StateHasChanged();
    }

    private void ClearResults()
    {
        testResults.Clear();
        testsRun = 0;
        testsPassed = 0;
        testsFailed = 0;
        auditEntriesCreated = 0;
        statusMessage = "";
    }

    private async Task CleanupTestData()
    {
        isRunning = true;
        statusMessage = "🧹 Cleaning up test data...";
        StateHasChanged();

        try
        {
            // Delete test transactions
            foreach (var id in testTransactionIds.ToList())
            {
                try
                {
                    await TransactionService.DeleteTransactionAsync(id);
                    testTransactionIds.Remove(id);
                }
                catch { /* Already deleted */ }
            }

            // Delete test trips
            foreach (var id in testTripIds.ToList())
            {
                try
                {
                    await TripService.DeleteTripAsync(id);
                    testTripIds.Remove(id);
                }
                catch { /* Already deleted */ }
            }

            statusMessage = "✅ Test data cleaned up successfully";
        }
        catch (Exception ex)
        {
            statusMessage = $"❌ Cleanup error: {ex.Message}";
        }

        isRunning = false;
        await LoadRecentAuditLogs();
        StateHasChanged();
    }

    private async Task LoadRecentAuditLogs()
    {
        try
        {
            var logs = await AuditService.SearchAuditLogsAsync(
                searchTerm: "audit.test@test.com",
                startDate: DateTime.Now.AddHours(-1)
            );
            recentAuditLogs = logs.ToList();
        }
        catch
        {
            recentAuditLogs.Clear();
        }
    }

    private string GetActionBadgeClass(string action)
    {
        return action.ToLower() switch
        {
            var a when a.Contains("add") || a.Contains("create") => "badge-success",
            var a when a.Contains("modif") || a.Contains("edit") || a.Contains("update") => "badge-warning",
            var a when a.Contains("delet") => "badge-error",
            var a when a.Contains("restore") => "badge-info",
            _ => "badge-ghost"
        };
    }

    private class TestResult
    {
        public string TestName { get; set; } = "";
        public string Operation { get; set; } = "";
        public string EntityType { get; set; } = "";
        public bool Success { get; set; }
        public bool AuditLogged { get; set; }
        public string Details { get; set; } = "";
    }
}
