using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Entities;
using Deeproxio.Asset.BLL.Contract.Services;

namespace Deeproxio.Asset.BLL.Services
{
    public class AssetService : IAssetService
    {
        public Task<Contract.Entities.Asset> GetById(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssetInfo> GetInfoById(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Put(Contract.Entities.Asset assetModel, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Put(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> PutMetadata(string id, AssetInfo assetInfoModel, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
