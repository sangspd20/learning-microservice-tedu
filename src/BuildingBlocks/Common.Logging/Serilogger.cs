using Microsoft.Extensions.Hosting;
using Serilog;

namespace Common.Logging
{
    public static class Serilogger
    {
        // implement elastic search
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
            (context, configuration) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(oldValue:".", newValue:"-");
            var enviromentName = context.HostingEnvironment.EnvironmentName ?? "Development";

            configuration
                .WriteTo.Debug()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty(name: "Enviroment", enviromentName)
                .Enrich.WithProperty(name: "Application", applicationName)
                .ReadFrom.Configuration(context.Configuration);

        };
    }
}
