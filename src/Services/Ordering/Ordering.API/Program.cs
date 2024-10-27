using Common.Logging;
using Infrastructure.Middlewares;
using Ordering.API.Extensions;
using Ordering.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information(messageTemplate: "Start Ordering Api up");

try
{
    builder.Host.UseSerilog(Serilogger.Configure);
    builder.Host.AddAppConfigurations();

    //// Add services to the container.
    builder.Services.AddInfrastructure(builder.Configuration);

    // configure Mass Transit
    builder.Services.ConfigureMassTransitWithRabbitMq();

    var app = builder.Build();

    app.UseInfrastructure();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitialiseAsync();
        await orderContextSeed.SeedAsync();
    }
    app.UseMiddleware<ErrorWrappingMiddleware>();


    app.Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}