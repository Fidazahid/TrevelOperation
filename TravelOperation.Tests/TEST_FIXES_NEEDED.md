# Unit Test Compilation Fixes Required

## Overview
The unit test project has been created with 65 comprehensive tests across 5 test files. However, there are **25 compilation errors** that need to be fixed before tests can run.

## Summary of Errors

### 1. DateFormattingTests.cs (1 error)
**Error:** Missing required parameter 'settingsService'
```
error CS7036: There is no argument given that corresponds to the required parameter 
'settingsService' of 'MessageTemplateService.MessageTemplateService(ISettingsService)'
```

**Fix Required:**
Add mock for ISettingsService:
```csharp
var mockSettings = new Mock<ISettingsService>();
var service = new MessageTemplateService(mockSettings.Object);
```

---

### 2. CategoryMappingTests.cs (3 errors)

**Error 1:** Missing logger parameter
```
error CS7036: There is no argument given that corresponds to the required parameter 
'logger' of 'CsvImportService.CsvImportService(TravelDbContext, ILogger<CsvImportService>)'
```

**Error 2:** Missing using directive for Category
```
error CS0246: The type or namespace name 'Category' could not be found
```

**Error 3:** Wrong argument type for AddRange
```
error CS1503: Argument 1: cannot convert from 'System.Collections.Generic.List<Category>' 
to 'TravelOperation.Core.Models.Lookup.Category'
```

**Fix Required:**
```csharp
// Add using directive
using TravelOperation.Core.Models.Lookup;

// Add logger mock
var mockLogger = new Mock<ILogger<CsvImportService>>();
var service = new CsvImportService(context, mockLogger.Object);

// Fix AddRange call - categories is already a list
await context.Categories.AddRangeAsync(categories);
```

---

### 3. TaxCalculationServiceTests.cs (9 errors)

**Error 1:** Missing logger parameter
```
error CS7036: There is no argument given that corresponds to the required parameter 
'logger' of 'TaxCalculationService.TaxCalculationService(TravelDbContext, ILogger<TaxCalculationService>)'
```

**Error 2:** TaxRule doesn't exist
```
error CS0246: The type or namespace name 'TaxRule' could not be found
```
**Note:** Should use `Tax` entity instead

**Error 3-5:** Missing using directives
```
error CS0246: The type or namespace name 'Category' could not be found
error CS0246: The type or namespace name 'CabinClass' could not be found
```

**Error 6-8:** Wrong AddRange calls
```
error CS1503: Argument 1: cannot convert from 'System.Collections.Generic.List<Category>'
error CS1503: Argument 1: cannot convert from 'System.Collections.Generic.List<CabinClass>'
```

**Error 9-12:** Non-existent properties on TaxExposureResult
```
error CS1061: 'TaxExposureResult' does not contain a definition for 'HasMealsIssue'
error CS1061: 'TaxExposureResult' does not contain a definition for 'HasLodgingIssue'
error CS1061: 'TaxExposureResult' does not contain a definition for 'HasPremiumCabinClass'
```

**Actual TaxExposureResult properties:**
- `TripId` (int)
- `TripName` (string)
- `MealsExposure` (decimal) - Use `result.MealsExposure > 0` instead of `HasMealsIssue`
- `LodgingExposure` (decimal) - Use `result.LodgingExposure > 0` instead of `HasLodgingIssue`
- `TotalTaxExposure` (decimal)
- `HasPremiumAirfare` (bool) - Correct name!
- `PremiumCabinClasses` (List<string>)
- `AppliedTaxSettings` (Tax?)
- `TaxNote` (string?)

**Fix Required:**
```csharp
// Add using directives
using TravelOperation.Core.Models.Lookup;
using TravelOperation.Core.Models.Entities;
using Microsoft.Extensions.Logging;

// Add logger mock
var mockLogger = new Mock<ILogger<TaxCalculationService>>();
var taxService = new TaxCalculationService(context, mockLogger.Object);

// Change TaxRule to Tax
var tax = new Tax
{
    TaxId = 1,
    FiscalYear = 2024,
    Country = "United States",
    Subsidiary = "WSC IL",
    MealsCap = 50m,
    LodgingCap = 200m,
    TaxShield = 0.17m
};

// Fix AddRange calls (remove extra list wrapping)
await context.Categories.AddRangeAsync(categories);
await context.CabinClasses.AddRangeAsync(cabinClasses);
await context.Taxes.AddRangeAsync(tax); // Single entity, use AddAsync or AddRange with array

// Change assertions
result.MealsExposure.Should().BeGreaterThan(0); // Instead of HasMealsIssue.Should().BeTrue()
result.LodgingExposure.Should().BeGreaterThan(0); // Instead of HasLodgingIssue.Should().BeTrue()
result.HasPremiumAirfare.Should().BeTrue(); // This is correct!
result.PremiumCabinClasses.Should().Contain("Business"); // Use this instead of HasPremiumCabinClass
```

---

### 4. SplitServiceTests.cs (12 errors)

**Error 1-2:** Missing using directives
```
error CS0246: The type or namespace name 'Category' could not be found
error CS0246: The type or namespace name 'Source' could not be found
```

**Error 3-7:** Missing userId parameter (5 occurrences)
```
error CS7036: There is no argument given that corresponds to the required parameter 
'userId' of 'SplitService.ApplySplitAsync(string, List<SplitItem>, string)'
```

**Actual signature:**
```csharp
Task<ServiceResult<List<Transaction>>> ApplySplitAsync(
    string transactionId, 
    List<SplitItem> splitItems, 
    string userId
)
```

**Error 8-9:** Non-existent properties on SplitSuggestion
```
error CS1061: 'SplitSuggestion' does not contain a definition for 'ParticipantCount'
error CS1061: 'SplitSuggestion' does not contain a definition for 'SuggestedSplitAmount'
```

**Fix Required:**
```csharp
// Add using directives
using TravelOperation.Core.Models.Lookup;

// Add userId parameter to all ApplySplitAsync calls
var result = await splitService.ApplySplitAsync(
    "T001", 
    splitItems, 
    "test@example.com"  // ADD THIS
);

// Remove non-existent property assertions
// suggestion.ParticipantCount.Should().Be(3); // REMOVE
// suggestion.SuggestedSplitAmount.Should().Be(100m); // REMOVE

// Instead, verify the SplitItem collection properties
suggestion.Splits.Should().HaveCount(3);
suggestion.Splits.Sum(s => s.Amount).Should().Be(300m);
```

---

## Quick Fix Checklist

### File: DateFormattingTests.cs
- [ ] Add `var mockSettings = new Mock<ISettingsService>();`
- [ ] Pass `mockSettings.Object` to MessageTemplateService constructor

### File: CategoryMappingTests.cs
- [ ] Add `using TravelOperation.Core.Models.Lookup;`
- [ ] Add `var mockLogger = new Mock<ILogger<CsvImportService>>();`
- [ ] Pass `mockLogger.Object` to CsvImportService constructor
- [ ] Fix AddRange call: remove extra list wrapper

### File: TaxCalculationServiceTests.cs
- [ ] Add `using TravelOperation.Core.Models.Lookup;`
- [ ] Add `using TravelOperation.Core.Models.Entities;`
- [ ] Add `using Microsoft.Extensions.Logging;`
- [ ] Add `var mockLogger = new Mock<ILogger<TaxCalculationService>>();`
- [ ] Pass `mockLogger.Object` to TaxCalculationService constructor
- [ ] Change all `TaxRule` to `Tax`
- [ ] Fix AddRange calls: remove extra list wrappers
- [ ] Change `result.HasMealsIssue` to `result.MealsExposure.Should().BeGreaterThan(0)`
- [ ] Change `result.HasLodgingIssue` to `result.LodgingExposure.Should().BeGreaterThan(0)`
- [ ] Change `result.HasPremiumCabinClass` to `result.HasPremiumAirfare`
- [ ] Add assertions for `result.PremiumCabinClasses` list

### File: SplitServiceTests.cs
- [ ] Add `using TravelOperation.Core.Models.Lookup;`
- [ ] Add userId parameter to all 5 ApplySplitAsync calls (use "test@example.com")
- [ ] Remove `suggestion.ParticipantCount` assertions
- [ ] Remove `suggestion.SuggestedSplitAmount` assertions
- [ ] Add assertions for `suggestion.Splits` collection instead

### File: AmountCalculationTests.cs
- [ ] ✅ No errors - ready to run!

---

## Commands to Run After Fixes

1. **Build tests:**
   ```powershell
   dotnet build TravelOperation.Tests/TravelOperation.Tests.csproj
   ```

2. **Run tests:**
   ```powershell
   dotnet test TravelOperation.Tests/TravelOperation.Tests.csproj
   ```

3. **Run with detailed output:**
   ```powershell
   dotnet test TravelOperation.Tests/TravelOperation.Tests.csproj --verbosity detailed
   ```

4. **Run specific test class:**
   ```powershell
   dotnet test --filter "FullyQualifiedName~AmountCalculationTests"
   ```

---

## Expected Outcome After Fixes

- **65 unit tests** should compile successfully
- All tests should pass (assuming service implementations are correct)
- Test coverage includes:
  - Tax calculations with various scenarios
  - Date and amount formatting
  - Category mapping from CSV
  - Transaction splitting logic
  - Currency conversions and calculations

---

## Next Steps (Items 59-60)

After unit tests are fixed and passing:

1. **Item 59: Integration Tests**
   - Create `TravelOperation.IntegrationTests` project
   - Test full workflows (CSV import → trip creation → validation)
   - Test database transactions and rollbacks
   - Test service interactions

2. **Item 60: UI Tests**
   - Create UI test project with bUnit
   - Test Blazor component rendering
   - Test user interactions (clicks, form submissions)
   - Test navigation and routing
   - Test data binding and validation

---

**Last Updated:** October 24, 2025  
**Status:** Ready for fixes - all errors documented with solutions
