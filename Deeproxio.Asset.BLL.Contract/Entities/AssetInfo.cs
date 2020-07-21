using System.Collections.Generic;

namespace Deeproxio.Asset.BLL.Contract.Entities
{
    public class AssetInfo
    {
        public string Name { get; set; }
        public string StorePrefix { get; set; }
        public string BlobExtension { get; set; }
        public string BlobMimeType { get; set; }
        public AssetType MediaType { get; set; }
        Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
