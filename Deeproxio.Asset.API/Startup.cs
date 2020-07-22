using System;
using System.IO;
using AutoMapper;
using Calzolari.Grpc.AspNetCore.Validation;
using Deeproxio.Asset.API.Infrastructure;
using Deeproxio.Asset.API.Services.v1;
using Deeproxio.Asset.API.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using Serilog;

namespace Deeproxio.Asset.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secrets/appsettings.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services
                .AddAutoMapper(typeof(Startup).Assembly)
                .AddCors();

            new DAL.Configuration.DependencyModule().RegisterTypes(services, Configuration);
            new BLL.Configuration.DependencyModule().RegisterTypes(services);

            services.AddGrpc(options =>
            {
                options.EnableMessageValidation();
                options.Interceptors.Add<LoggerInterceptor>();
            });

            services.AddValidator<AssetValidator>();
            services.AddValidator<AssetInfoValidator>();

            services.AddGrpcValidation();

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.Equals("Development", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message"));

            app.UseMetricServer();

            app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<AssetsApi>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response
                        .WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });

                endpoints.MapHealthChecks("/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => true,
                    // The following StatusCodes are the default assignments for
                    // the HealthCheckStatus properties.
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    // The default value is false.
                    AllowCachingResponses = false
                }).WithDisplayName("Asset API Health Check");
            });
        }
    }
}
