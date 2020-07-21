using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Deeproxio.Asset.DAL.Contracts;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.DAL.Data;
using Deeproxio.Asset.DAL.Repositories;

namespace Deeproxio.Asset.DAL.Configuration
{
    public class DependencyModule
    {
        public void RegisterTypes(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AssetsDatabaseSettings>(configuration.GetSection(nameof(AssetsDatabaseSettings)));
            services.AddSingleton<IAssetsDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<AssetsDatabaseSettings>>().Value);

            services.Configure<StorageSettings>(configuration.GetSection(nameof(StorageSettings)));
            services.AddSingleton<IStorageSettings>(sp =>
                sp.GetRequiredService<IOptions<StorageSettings>>().Value);

            services.AddTransient<IAssetDataContext, AssetDataContext>();
            services.AddTransient<IAssetRepository, AssetRepository>();

            services.AddTransient<IStorageContext, StorageContext>();
            services.AddTransient<IStorageRepository, StorageRepository>();
        }
    }
}
