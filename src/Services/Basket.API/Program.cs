using Basket.API.Extensions;
using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information(messageTemplate: "Start Basket Api up");

try
{
    // Add services to the container.
    builder.Host.AddAppConfigurations();

    //// Add services to the container.
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddConfigurationSettings(builder.Configuration);
    // configure Mass Transit
    builder.Services.ConfigureMassTransitWithRabbitMq();
    var app = builder.Build();

    //// Configure the HTTP request pipeline.
    app.UseInfrastructure();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, messageTemplate: "Unhandled exception");
}
finally
{
    Log.Information(messageTemplate: "Shut down Basket Api complete");
    Log.CloseAndFlush();
}
