using Deeproxio.Asset.BLL.Contract.Services;
using Deeproxio.Asset.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Deeproxio.Asset.BLL.Configuration
{
    public class DependencyModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddTransient<IStorageItemPathProvider, StorageItemPathProvider>();
            services.AddTransient<IAssetService, AssetService>();
        }
    }
}
