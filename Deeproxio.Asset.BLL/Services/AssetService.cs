using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Entities;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.BLL.Contract.Services;

namespace Deeproxio.Asset.BLL.Services
{
    internal class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly IStorageItemPathProvider _storageItemPathProvider;

        public AssetService(IAssetRepository assetRepository, IStorageRepository storageRepository, IStorageItemPathProvider storageItemPathProvider)
        {
            if (assetRepository == null)
            {
                throw new ArgumentNullException(nameof(assetRepository));
            }
            if (storageRepository == null)
            {
                throw new ArgumentNullException(nameof(storageRepository));
            }
            if (storageItemPathProvider == null)
            {
                throw new ArgumentNullException(nameof(storageItemPathProvider));
            }

            _assetRepository = assetRepository;
            _storageRepository = storageRepository;
            _storageItemPathProvider = storageItemPathProvider;
        }

        public async Task<Contract.Entities.Asset> GetByIdAsync(string id, Stream blobStream, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

            if (asset == null)
            {
                return null;
            }

            await _storageRepository.GetByIdAsync(asset.StorageId, blobStream, cancellationToken);

            return asset;
        }

        public async Task<AssetInfo> GetInfoByIdAsync(string id, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

            if (asset == null)
            {
                return null;
            }

            return asset.Info;
        }

        public async Task<bool> PutAsync(Contract.Entities.Asset assetModel, Stream blobStream, CancellationToken cancellationToken)
        {
            var existingAsset = await _assetRepository.GetByIdAsync(assetModel.Id, cancellationToken);

            if (existingAsset != null)
            {
                assetModel.StorageId = existingAsset.StorageId;

                if (!await _assetRepository.UpdateAsync(assetModel, cancellationToken))
                {
                    return false;
                }

                return await _storageRepository.PutAsync(assetModel.StorageId, blobStream, cancellationToken);
            }

            assetModel.StorageId = _storageItemPathProvider.GeneratePath(assetModel.Info.StorePrefix);

            if (!await _storageRepository.PutAsync(assetModel.StorageId, blobStream, cancellationToken))
            {
                return false;
            }

            return await _assetRepository.CreateAsync(assetModel, cancellationToken);
        }

        public async Task<bool> PutMetadataAsync(string id, AssetInfo assetInfoModel, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

            if (asset == null)
            {
                return false;
            }

            asset.Info = assetInfoModel;

            return await _assetRepository.UpdateAsync(asset, cancellationToken);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

            if (asset == null)
            {
                return false;
            }

            if (!await _assetRepository.DeleteAsync(id, cancellationToken))
            {
                return false;
            }

            return await _storageRepository.DeleteAsync(asset.StorageId, cancellationToken);
        }
    }
}
