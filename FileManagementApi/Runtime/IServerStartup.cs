using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Deeproxio.Infrastructure.Runtime
{
	public interface IServerStartup
	{
		IConfiguration Configuration { get; }
		void ConfigureServices(IServiceCollection services);
		void Configure(IApplicationBuilder app, IHostingEnvironment env);
	}
}