﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Deeproxio.Asset.BLL.Contract.Entities;

namespace Deeproxio.Asset.BLL.Contract.Services
{
    public interface IAssetService
    {
        Task<bool> Put(Entities.Asset assetModel, Stream blobStream, CancellationToken cancellationToken);
        Task<Entities.Asset> GetById(string id, Stream blobStream, CancellationToken cancellationToken);
        Task<AssetInfo> GetInfoById(string id, CancellationToken cancellationToken);
        Task<bool> Delete(string id, CancellationToken cancellationToken);
        Task<bool> PutMetadata(string id, AssetInfo assetInfoModel, CancellationToken cancellationToken);
    }
}
