using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.DAL.Contracts;
using MongoDB.Driver;

namespace Deeproxio.Asset.DAL.Repositories
{
    internal class AssetRepository : IAssetRepository
    {
        private readonly IAssetDataContext _context;

        public AssetRepository(IAssetDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
        }

        public async Task<bool> CreateAsync(BLL.Contract.Entities.Asset asset, CancellationToken cancellationToken = default)
        {
            if (asset == null)
            {
                return false;
            }

            await _context.Assets.InsertOneAsync(asset, new InsertOneOptions(), cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BLL.Contract.Entities.Asset> filter =
                Builders<BLL.Contract.Entities.Asset>.Filter.Eq(asset => asset.Id, id);

            DeleteResult result = await _context
                .Assets
                .DeleteOneAsync(filter, cancellationToken);

            return result.IsAcknowledged;
        }

        public async Task<IEnumerable<BLL.Contract.Entities.Asset>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _context
                .Assets
                .FindAsync(asset => true, cancellationToken: cancellationToken);

            return result.ToList();
        }

        public async Task<BLL.Contract.Entities.Asset> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var result = await _context
                .Assets
                .FindAsync(asset => asset.Id == id, cancellationToken: cancellationToken);

            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(BLL.Contract.Entities.Asset asset, CancellationToken cancellationToken = default)
        {
            if (asset == null)
            {
                return false;
            }

            var updateResult = await _context
                .Assets
                .ReplaceOneAsync(filter: item => item.Id == asset.Id, replacement: asset, cancellationToken: cancellationToken);

            return updateResult.IsAcknowledged;
        }
    }
}
