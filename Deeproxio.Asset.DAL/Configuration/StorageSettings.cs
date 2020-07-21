using Deeproxio.Asset.DAL.Contracts;

namespace Deeproxio.Asset.DAL.Configuration
{
    public class StorageSettings : IStorageSettings
    {
        public string EndpointUrl { get; set; }
        public string BucketName { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public bool EnableHTTPS { get; set; }
    }
}
