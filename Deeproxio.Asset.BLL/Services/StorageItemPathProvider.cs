using System;
using Deeproxio.Asset.BLL.Contract.Services;

namespace Deeproxio.Asset.BLL.Services
{
    internal class StorageItemPathProvider : IStorageItemPathProvider
    {
        public string GeneratePath(string storePrefix)
        {
            return $"{storePrefix}:{Guid.NewGuid():N}".ToLowerInvariant();
        }
    }
}
