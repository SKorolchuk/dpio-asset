using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Entities;

namespace Deeproxio.Asset.BLL.Contract.Services
{
    public interface IAssetService
    {
        Task<bool> PutAsync(Entities.Asset assetModel, Stream blobStream, CancellationToken cancellationToken);
        Task<Entities.Asset> GetByIdAsync(string id, Stream blobStream, CancellationToken cancellationToken);
        Task<AssetInfo> GetInfoByIdAsync(string id, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
        Task<bool> PutMetadataAsync(string id, AssetInfo assetInfoModel, CancellationToken cancellationToken);
    }
}
