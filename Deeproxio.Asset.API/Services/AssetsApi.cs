using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Deeproxio.Asset.API.Services
{
    public class AssetsApi : AssetService.AssetServiceBase
    {
        private readonly ILogger<AssetsApi> _logger;

        public AssetsApi(ILogger<AssetsApi> logger)
        {
            _logger = logger;
        }

        public override Task<StatusResponse> Upload(Asset asset, ServerCallContext context)
        {
            _logger.LogInformation(nameof(Upload));

            return Task.FromResult(new StatusResponse
            {
                Code = 0,
                Message = ""
            });
        }
    }
}
