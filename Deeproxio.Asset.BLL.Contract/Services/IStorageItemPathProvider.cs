namespace Deeproxio.Asset.BLL.Contract.Services
{
    public interface IStorageItemPathProvider
    {
        string GeneratePath(string storePrefix);
    }
}
