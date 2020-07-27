using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Deeproxio.Asset.BLL.Contract.Services;
using Google.Protobuf;
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

            using var blobStream = new MemoryStream(asset.Blob.ToByteArray());

            if (await _assetService.PutAsync(assetModel, blobStream, context.CancellationToken))
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
            using var blobStream = new MemoryStream();

            var assetModel = await _assetService.GetByIdAsync(request.Id, blobStream, context.CancellationToken);

            var asset = _mapper.Map<Asset>(assetModel);
            asset.Blob = ByteString.CopyFrom(blobStream.ToArray());

            return asset;
        }

        public override async Task<AssetInfo> Info(AssetRequest request, ServerCallContext context)
        {
            var assetModel = await _assetService.GetInfoByIdAsync(request.Id, context.CancellationToken);

            return _mapper.Map<AssetInfo>(assetModel);
        }

        public override async Task<StatusResponse> Delete(AssetRequest request, ServerCallContext context)
        {
            if (await _assetService.DeleteAsync(request.Id, context.CancellationToken))
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

            if (await _assetService.PutMetadataAsync(request.Id, assetInfoModel, context.CancellationToken))
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
