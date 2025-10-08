using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TravelOperation.Core.Data;
using TravelOperation.Core.Services;
using TravelOperation.Core.Services.Interfaces;
using TravelOperation.Core.Interfaces;
using TravelOperation.Core.Interceptors;
using TrevelOperation.Service;

namespace TrevelOperation;

public static class Startup
{
    public static IServiceProvider? Services { get; private set; }

    public static void Init()
    {
        var host = Host.CreateDefaultBuilder()
                       .ConfigureServices(WireupServices)
                       .Build();
        Services = host.Services;
        
        EnsureDatabaseCreated();
    }

    private static void WireupServices(IServiceCollection services)
    {
        services.AddWpfBlazorWebView();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        services.AddScoped<AuditInterceptor>();
        
        services.AddDbContext<TravelDbContext>((serviceProvider, options) =>
            options.UseSqlite("Data Source=TravelExpense.db")
                   .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>()));
            
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITripService, TripService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<ITaxCalculationService, TaxCalculationService>();
        services.AddScoped<IMessageTemplateService, MessageTemplateService>();
        services.AddScoped<ICsvImportService, CsvImportService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IMatchingService, MatchingService>();
        services.AddScoped<ISplitService, SplitService>();
        services.AddScoped<IExportService, ExportService>();
    }
    
    private static void EnsureDatabaseCreated()
    {
        using var scope = Services?.CreateScope();
        var context = scope?.ServiceProvider.GetRequiredService<TravelDbContext>();
        context?.Database.EnsureCreated();
    }
}
