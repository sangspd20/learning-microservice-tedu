using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information(messageTemplate: "Start Inventory Api up");

try
{
    // Add services to the container.

    builder.Services.AddControllers();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, messageTemplate: "Unhandled exception");
}
finally
{
    Log.Information(messageTemplate: "Shut down Inventory Api complete");
    Log.CloseAndFlush();
}
