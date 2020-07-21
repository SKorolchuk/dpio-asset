
namespace Deeproxio.Asset.DAL.Contracts
{
    public interface IAssetsDatabaseSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
