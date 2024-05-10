using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using VerificationsProvider.Data.Contexts;
using VerificationsProvider.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("SiliconDatabase")));
        services.AddScoped<IVerificationService, VerificationService>();
        services.AddScoped<IVerificationCleanerService, VerificationCleanerService>();
        services.AddScoped<IValidateVerificationCodeService, ValidateVerificationCodeService>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var migration = context.Database.GetPendingMigrations();
        if (migration != null && migration.Any())
        {
            context.Database.Migrate();
        }
    }
    catch(Exception ex) 
    {
        Debug.WriteLine($"ERROR : VerificationProvider.Program.cs :: {ex.Message}");
    }
 

}
host.Run();
