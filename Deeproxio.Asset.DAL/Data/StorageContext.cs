using System.Net;
using Deeproxio.Asset.DAL.Configuration;
using Deeproxio.Asset.DAL.Contracts;
using Minio;

namespace Deeproxio.Asset.DAL.Data
{
    public class StorageContext : IStorageContext
    {
        public StorageContext(IStorageSettings storageSettings)
        {
            var retryPolicy = StorageRetryPolicy.GetDefaultRetryPolicy();

            var client = new MinioClient(
                    storageSettings.EndpointUrl,
                    storageSettings.AccessKey,
                    storageSettings.SecretKey
                ).WithRetryPolicy(retryPolicy);

            if (storageSettings.EnableHTTPS)
            {
                StorageObjects = client.WithSSL();
            }
            else
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                       (sender, certificate, chain, sslPolicyErrors) => true;

                StorageObjects = client;
            }
        }

        public IObjectOperations StorageObjects { get; }
    }
}
