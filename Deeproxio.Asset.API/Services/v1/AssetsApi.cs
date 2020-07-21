using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.Asset.BLL.Contract.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Deeproxio.Asset.API.Services.v1
{
    public class AssetsApi : AssetService.AssetServiceBase
    {
        private readonly ILogger<AssetsApi> _logger;
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;

        public AssetsApi(ILogger<AssetsApi> logger, IAssetService assetService, IMapper mapper)
        {
            _logger = logger;
            _assetService = assetService;
            _mapper = mapper;
        }

        public override async Task<StatusResponse> Upload(Asset asset, ServerCallContext context)
        {
            var assetModel = _mapper.Map<BLL.Contract.Entities.Asset>(asset);

            if (await _assetService.Upload(assetModel, context.CancellationToken))
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = $"Asset Upload - {asset.Id} has been completed"
                };
            }
            else
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Asset Upload - {asset.Id} error"
                };
            }
        }

        public override Task<Asset> Download(AssetRequest request, ServerCallContext context)
        {
            return base.Download(request, context);
        }

        public override Task<AssetInfo> Info(AssetRequest request, ServerCallContext context)
        {
            return base.Info(request, context);
        }

        public override Task<StatusResponse> Delete(AssetRequest request, ServerCallContext context)
        {
            return base.Delete(request, context);
        }

        public override Task<StatusResponse> UpdateMetadata(AssetInfo request, ServerCallContext context)
        {
            return base.UpdateMetadata(request, context);
        }
    }
}
