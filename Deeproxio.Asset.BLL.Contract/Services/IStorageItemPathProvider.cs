using System;
using System.Collections.Generic;
using System.Text;

namespace Deeproxio.Asset.BLL.Contract.Services
{
    public interface IStorageItemPathProvider
    {
        string GeneratePath(string storePrefix);
    }
}
