using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deeproxio.Asset.BLL.Contract.Repositories
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Entities.Asset>> GetAll(CancellationToken cancellationToken = default);
        Task<Entities.Asset> GetById(string id, CancellationToken cancellationToken = default);
        Task<bool> Create(Entities.Asset asset, CancellationToken cancellationToken = default);
        Task<bool> Update(Entities.Asset asset, CancellationToken cancellationToken = default);
        Task<bool> Delete(string id, CancellationToken cancellationToken = default);
    }
}
