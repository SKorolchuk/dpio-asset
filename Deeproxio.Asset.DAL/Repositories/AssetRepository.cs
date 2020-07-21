using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.DAL.Contracts;
using MongoDB.Driver;

namespace Deeproxio.Asset.DAL.Repositories
{
    public class AssetRepository : IAssetRepository
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

        public async Task Create(BLL.Contract.Entities.Asset asset, CancellationToken cancellationToken = default)
        {
            await _context.Assets.InsertOneAsync(asset, new InsertOneOptions(), cancellationToken);
        }

        public async Task<bool> Delete(string id, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BLL.Contract.Entities.Asset> filter =
                Builders<BLL.Contract.Entities.Asset>.Filter.Eq(asset => asset.Id, id);

            DeleteResult result = await _context
                .Assets
                .DeleteOneAsync(filter, cancellationToken);

            return result.IsAcknowledged
                && result.DeletedCount > 0;
        }

        public async Task<IEnumerable<BLL.Contract.Entities.Asset>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _context
                .Assets
                .Find(asset => true)
                .ToListAsync(cancellationToken);
        }

        public async Task<BLL.Contract.Entities.Asset> GetById(string id, CancellationToken cancellationToken = default)
        {
            return await _context
                .Assets
                .Find(asset => asset.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> Update(BLL.Contract.Entities.Asset asset, CancellationToken cancellationToken = default)
        {
            var updateResult = await _context
                .Assets
                .ReplaceOneAsync(filter: item => item.Id == asset.Id, replacement: asset, cancellationToken: cancellationToken);

            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }
    }
}
