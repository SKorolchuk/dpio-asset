using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Entities;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.BLL.Contract.Services;

namespace Deeproxio.Asset.BLL.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IStorageItemPathProvider _storageItemPathProvider;

        public AssetService(IAssetRepository assetRepository, IStorageRepository storageRepository, IStorageItemPathProvider storageItemPathProvider)
        {
            _assetRepository = assetRepository;
            _storageRepository = storageRepository;
            _storageItemPathProvider = storageItemPathProvider;
        }

        public async Task<Contract.Entities.Asset> GetById(string id, Stream blobStream, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetById(id, cancellationToken);

            if (asset == null)
            {
                return null;
            }

            await _storageRepository.GetById(asset.StorageId, blobStream, cancellationToken);

            return asset;
        }

        public async Task<AssetInfo> GetInfoById(string id, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetById(id, cancellationToken);

            if (asset == null)
            {
                return null;
            }

            return asset.Info;
        }

        public async Task<bool> Put(Contract.Entities.Asset assetModel, Stream blobStream, CancellationToken cancellationToken)
        {
            var existingAsset = await _assetRepository.GetById(assetModel.Id, cancellationToken);

            if (existingAsset != null)
            {
                assetModel.StorageId = existingAsset.StorageId;

                if (!await _assetRepository.Update(assetModel, cancellationToken))
                {
                    return false;
                }

                return await _storageRepository.Put(assetModel.StorageId, blobStream, cancellationToken);
            }

            assetModel.StorageId = _storageItemPathProvider.GeneratePath(assetModel.Info.StorePrefix);

            if (!await _storageRepository.Put(assetModel.StorageId, blobStream, cancellationToken))
            {
                return false;
            }

            return await _assetRepository.Create(assetModel, cancellationToken);
        }

        public async Task<bool> PutMetadata(string id, AssetInfo assetInfoModel, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetById(id, cancellationToken);

            if (asset == null)
            {
                return false;
            }

            asset.Info = assetInfoModel;

            return await _assetRepository.Update(asset, cancellationToken);
        }

        public async Task<bool> Delete(string id, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetById(id, cancellationToken);

            if (asset == null)
            {
                return false;
            }

            if (!await _assetRepository.Delete(id, cancellationToken))
            {
                return false;
            }

            return await _storageRepository.Delete(asset.StorageId, cancellationToken);
        }
    }
}
