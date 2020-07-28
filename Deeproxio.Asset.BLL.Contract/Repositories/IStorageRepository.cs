using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deeproxio.Asset.BLL.Contract.Repositories
{
    public interface IStorageRepository
    {
        Task GetByIdAsync(string id, Stream blobStream, CancellationToken cancellationToken = default);
        Task<bool> PutAsync(string id, Stream blobStream, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
