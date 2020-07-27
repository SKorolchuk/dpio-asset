using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Deeproxio.Asset.BLL.Contract.Repositories
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Entities.Asset>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Entities.Asset> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> CreateAsync(Entities.Asset asset, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Entities.Asset asset, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
