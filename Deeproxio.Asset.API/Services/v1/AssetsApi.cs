using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.Asset.BLL.Contract.Services;
using Grpc.Core;

namespace Deeproxio.Asset.API.Services.v1
{
    public class AssetsApi : AssetService.AssetServiceBase
    {
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;

        public AssetsApi(IAssetService assetService, IMapper mapper)
        {
            _assetService = assetService;
            _mapper = mapper;
        }

        public override async Task<StatusResponse> Upload(Asset asset, ServerCallContext context)
        {
            var assetModel = _mapper.Map<BLL.Contract.Entities.Asset>(asset);

            if (await _assetService.Put(assetModel, context.CancellationToken))
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

        public override async Task<Asset> Download(AssetRequest request, ServerCallContext context)
        {
            var assetModel = await _assetService.GetById(request.Id, context.CancellationToken);

            return _mapper.Map<Asset>(assetModel);
        }

        public override async Task<AssetInfo> Info(AssetRequest request, ServerCallContext context)
        {
            var assetModel = await _assetService.GetInfoById(request.Id, context.CancellationToken);

            return _mapper.Map<AssetInfo>(assetModel);
        }

        public override async Task<StatusResponse> Delete(AssetRequest request, ServerCallContext context)
        {
            if (await _assetService.Put(request.Id, context.CancellationToken))
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = $"Asset Delete - {request.Id} has been completed"
                };
            }
            else
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Asset Delete - {request.Id} error"
                };
            }
        }

        public override async Task<StatusResponse> UpdateMetadata(UpdateMetadataRequest request, ServerCallContext context)
        {
            var assetInfoModel = _mapper.Map<BLL.Contract.Entities.AssetInfo>(request.AssetInfo);

            if (await _assetService.PutMetadata(request.Id, assetInfoModel, context.CancellationToken))
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = $"Asset UpdateMetadata - {request.Id} has been completed"
                };
            }
            else
            {
                return new StatusResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Asset UpdateMetadata - {request.Id} error"
                };
            }
        }
    }
}
