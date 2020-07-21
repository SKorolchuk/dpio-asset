using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Repositories;
using Deeproxio.Asset.DAL.Contracts;
using Microsoft.Extensions.Logging;

namespace Deeproxio.Asset.DAL.Repositories
{
    public class StorageRepository : IStorageRepository
    {
        private readonly IStorageContext _context;
        private readonly IStorageSettings _settings;
        private readonly ILogger<StorageRepository> _logger;

        public StorageRepository(IStorageContext context, IStorageSettings settings, ILogger<StorageRepository> logger)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _context = context;
            _settings = settings;
            _logger = logger;
        }

        public async Task<bool> Delete(string id, CancellationToken cancellationToken = default)
        {
            await EnsureBucketExists(cancellationToken);

            try
            {
                await _context
                    .StorageObjects
                    .RemoveObjectAsync(_settings.BucketName, id, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Storage Client Delete Operation Warning");

                return false;
            }
        }

        public async Task GetById(string id, Stream blobStream, CancellationToken cancellationToken = default)
        {
            await EnsureBucketExists(cancellationToken);

            await _context
                .StorageObjects
                .GetObjectAsync(
                    _settings.BucketName,
                    id,
                    async (stream) =>
                    {
                        await stream.CopyToAsync(blobStream);
                    },
                    cancellationToken: cancellationToken
                );
        }

        public async Task<bool> Put(string id, Stream blobStream, CancellationToken cancellationToken = default)
        {
            await EnsureBucketExists(cancellationToken);

            await _context
                .StorageObjects
                .PutObjectAsync(
                    _settings.BucketName,
                    id,
                    blobStream,
                    blobStream.Length,
                    cancellationToken: cancellationToken
                );

            return true;
        }

        private async Task EnsureBucketExists(CancellationToken cancellationToken = default)
        {
            var bucketExists = await _context.BucketObjects.BucketExistsAsync(_settings.BucketName, cancellationToken);

            if (!bucketExists)
            {
                await _context.BucketObjects.MakeBucketAsync(_settings.BucketName, cancellationToken: cancellationToken);
            }
        }
    }
}
