using AutoMapper;

namespace Deeproxio.Asset.API.Mapping
{
    public class AssetsProfile : Profile
    {
        public AssetsProfile()
        {
            CreateMap<Asset, BLL.Contract.Entities.Asset>()
                .ForMember(entity => entity.Id, source => source.MapFrom(rpcModel => rpcModel.Id))
                .ForMember(entity => entity.Info, source => source.MapFrom(rpcModel => rpcModel.Info));

            CreateMap<BLL.Contract.Entities.Asset, Asset>()
                .ForMember(rpcModel => rpcModel.Id, source => source.MapFrom(entity => entity.Id))
                .ForMember(rpcModel => rpcModel.Info, source => source.MapFrom(entity => entity.Info));

            CreateMap<AssetInfo, BLL.Contract.Entities.AssetInfo>()
                .ForMember(entity => entity.Name, source => source.MapFrom(rpcModel => rpcModel.Name))
                .ForMember(entity => entity.StorePrefix, source => source.MapFrom(rpcModel => rpcModel.StorePrefix))
                .ForMember(entity => entity.BlobExtension, source => source.MapFrom(rpcModel => rpcModel.BlobExtension))
                .ForMember(entity => entity.BlobMimeType, source => source.MapFrom(rpcModel => rpcModel.BlobMimeType))
                .ForMember(entity => entity.MediaType, source => source.MapFrom(rpcModel => (BLL.Contract.Entities.AssetType)(int)rpcModel.MediaType))
                .ForMember(entity => entity.Metadata, source => source.MapFrom(rpcModel => rpcModel.Metadata));

            CreateMap<BLL.Contract.Entities.AssetInfo, AssetInfo>()
                .ForMember(rpcModel => rpcModel.Name, source => source.MapFrom(entity => entity.Name))
                .ForMember(rpcModel => rpcModel.StorePrefix, source => source.MapFrom(entity => entity.StorePrefix))
                .ForMember(rpcModel => rpcModel.BlobExtension, source => source.MapFrom(entity => entity.BlobExtension))
                .ForMember(rpcModel => rpcModel.BlobMimeType, source => source.MapFrom(entity => entity.BlobMimeType))
                .ForMember(rpcModel => rpcModel.MediaType, source => source.MapFrom(entity => (AssetType)(int)entity.MediaType))
                .ForMember(rpcModel => rpcModel.Metadata, source => source.MapFrom(entity => entity.Metadata));
        }
    }
}
