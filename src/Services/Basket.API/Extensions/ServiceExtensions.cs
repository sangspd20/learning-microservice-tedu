


using Basket.API.Repositories.Interfaces;
using Basket.API.Repositories;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Shared.Configurations;
using Infrastructure.Extensions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using EventBus.Messages.IntegrationEvents.Events;

namespace Basket.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSettings);

            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            services.AddSingleton(cacheSettings);

            //var grpcSettings = configuration.GetSection(nameof(GrpcSettings))
            //    .Get<GrpcSettings>();
            //services.AddSingleton(grpcSettings);

            //var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
            //    .Get<BackgroundJobSettings>();
            //services.AddSingleton(backgroundJobSettings);

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddInfrastructureServices();
            services.ConfigureRedis(configuration);

            //services.ConfigureCustomerContext(configuration);
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));


            return services;
        }

        //private static IServiceCollection ConfigureCustomerContext(this IServiceCollection services,
        //IConfiguration configuration)
        //{
        //    var connectionString = configuration.GetConnectionString(name: "DefaultConnectionString");

        //    services.AddDbContext<CustomerContext>(
        //    options => options.UseNpgsql(connectionString));

        //    return services;

        //}

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped<IBasketRepository, BasketRepository>()
            .AddTransient<ISerializeService, SerializeService>();
        }
        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = services.GetOptions<CacheSettings>("CacheSettings");
            if (string.IsNullOrEmpty(settings.ConnectionString))
                throw new ArgumentNullException("Redis Connection string is not configured.");

            //Redis Configuration
            services.AddStackExchangeRedisCache(options => { options.Configuration = settings.ConnectionString; });
        }

        public static void ConfigureMassTransitWithRabbitMq(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>("EventBusSettings");
            if (settings == null || string.IsNullOrEmpty(settings.HostAddress))
                throw new ArgumentNullException("EventBusSettings is not configured properly!");

            var mqConnection = new Uri(settings.HostAddress);

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                });

                config.AddRequestClient<IBasketCheckoutEvent>();
            });

        }


    }
}
