namespace Deeproxio.Asset.DAL.Contracts
{
    public interface IStorageSettings
    {
        string EndpointUrl { get; set; }
        string BucketName { get; set; }
        string AccessKey { get; set; }
        string SecretKey { get; set; }
        bool EnableHTTPS { get; set; }
    }
}
