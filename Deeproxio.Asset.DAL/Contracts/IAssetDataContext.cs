using MongoDB.Driver;

namespace Deeproxio.Asset.DAL.Contracts
{
    public interface IAssetDataContext
    {
        IMongoCollection<BLL.Contract.Entities.Asset> Assets { get; }
    }
}
