using Deeproxio.Asset.DAL.Contracts;
using MongoDB.Driver;

namespace Deeproxio.Asset.DAL.Data
{
    internal class AssetDataContext : IAssetDataContext
    {
        public AssetDataContext(IAssetsDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            Assets = database.GetCollection<BLL.Contract.Entities.Asset>(settings.CollectionName);
        }

        public IMongoCollection<BLL.Contract.Entities.Asset> Assets { get; }
    }
}
