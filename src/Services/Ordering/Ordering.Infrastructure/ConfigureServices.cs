
using Contracts.Common.Interfaces;
using Contracts.Services;
using Infrastructure.Common;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Interfaces;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;


namespace Ordering.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        //if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
        //    throw new ArgumentNullException("Connection string is not configured.");

        services.AddDbContext<OrderContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString"),
               builder =>
                    builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
        });

        services.AddScoped<OrderContextSeed>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));

        return services;
    }
}