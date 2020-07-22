using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Deeproxio.Asset.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                    .CreateLogger();

                Log.Information("Starting host...");
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.Configure<KestrelServerOptions>(
                        context.Configuration.GetSection("Kestrel"));
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(serverOptions =>
                        {
                            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("HEALTHCHECK_PORT")) &&
                                !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GRPC_PORT")))
                            {
                                int.TryParse(Environment.GetEnvironmentVariable("HEALTHCHECK_PORT"), out var healthCheckPort);

                                serverOptions.ListenAnyIP(healthCheckPort, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1;
                                });

                                int.TryParse(Environment.GetEnvironmentVariable("GRPC_PORT"), out var grpcPort);

                                serverOptions.ListenAnyIP(grpcPort, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http2;
                                });
                            } else {
                                serverOptions.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
                            }
                        })
                        .UseKestrel()
                        .UseStartup<Startup>();
                });
    }
}
