using System;
using Minio;
using Minio.Exceptions;
using Polly;
using RestSharp;

namespace Deeproxio.Asset.DAL.Configuration
{
    internal static class StorageRetryPolicy
    {
        private const int DEFAULT_RETRY_COUNT = 3;
        private static readonly TimeSpan defaultRetryInterval = TimeSpan.FromMilliseconds(250);
        private static readonly TimeSpan defaultMaxRetryInterval = TimeSpan.FromSeconds(15);

        public static TimeSpan CalcBackoff(int attempt)
        {
            var deviation = 0.8 + new Random(Environment.TickCount).NextDouble() * 0.4;
            var scale = (Math.Pow(2.0, attempt) - 1.0) * deviation;

            var result = TimeSpan.FromMilliseconds(defaultRetryInterval.TotalMilliseconds * scale);

            return result < defaultMaxRetryInterval ? result : defaultMaxRetryInterval;
        }

        public static PolicyBuilder<IRestResponse> CreatePolicyBuilder()
        {
            return Policy<IRestResponse>
                .Handle<ConnectionException>()
                .Or<InternalClientException>(ex => ex.Message.StartsWith("Storage Client Error"));
        }

        public static AsyncPolicy<IRestResponse> GetDefaultRetryPolicy() =>
            GetDefaultRetryPolicy(DEFAULT_RETRY_COUNT);

        public static AsyncPolicy<IRestResponse> GetDefaultRetryPolicy(
            int retryCount) =>
            CreatePolicyBuilder()
                .WaitAndRetryAsync(
                    retryCount,
                    i => CalcBackoff(i));

        public static RetryPolicyHandlingDelegate AsRetryDelegate(this AsyncPolicy<IRestResponse> policy) =>
            policy == null
                ? (RetryPolicyHandlingDelegate)null
                : async executeCallback => await policy.ExecuteAsync(executeCallback);

        public static MinioClient WithRetryPolicy(this MinioClient client, AsyncPolicy<IRestResponse> policy) =>
            client.WithRetryPolicy(policy.AsRetryDelegate());
    }
}
