using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API.Extensions;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Customer.API.Repositories;
using Customer.API.Services.Interfaces;
using Customer.API.Services;
using Infrastructure.Common;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information(messageTemplate: "Start Customer Api up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();

    // Add services to the container.
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.MapGet("/", () => $"Welcome to {builder.Environment.ApplicationName}!");
    app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomersAsync());
    app.MapGet("/api/customers/{username}", async (string username,ICustomerService customerService) => await customerService.GetCustomerByUsernameAsync(username));

    app.UseInfrastructure();

    app.SeedCustomerData()
      .Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information(messageTemplate: "Shut down Customer Api complete");
    Log.CloseAndFlush();
}
