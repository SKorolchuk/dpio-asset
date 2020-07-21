using Minio;

namespace Deeproxio.Asset.DAL.Contracts
{
    public interface IStorageContext
    {
        IObjectOperations StorageObjects { get; }

        IBucketOperations BucketObjects { get; }
    }
}
