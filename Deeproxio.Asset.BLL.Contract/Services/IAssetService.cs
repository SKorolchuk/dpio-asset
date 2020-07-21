using System.Threading;
using System.Threading.Tasks;

namespace Deeproxio.Asset.BLL.Contract.Services
{
    public interface IAssetService
    {
        Task<bool> Upload(Entities.Asset assetModel, CancellationToken cancellationToken);
    }
}
