using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Deeproxio.Asset.BLL.Contract.Repositories
{
    public interface IStorageRepository
    {
        Task GetById(string id, Stream blobStream, CancellationToken cancellationToken = default);
        Task<bool> Put(string id, Stream blobStream, CancellationToken cancellationToken = default);
        Task<bool> Delete(string id, CancellationToken cancellationToken = default);
    }
}
