using Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information(messageTemplate: "Start Hangfire Api up");

try
{
    // Add services to the container.

    builder.Services.AddControllers();
    //builder.Services.AddSwaggerGen();

    var app = builder.Build();

    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

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
    Log.Information(messageTemplate: "Shut down Hangfire Api complete");
    Log.CloseAndFlush();
}
