using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deeproxio.Asset.BLL.Contract.Entities
{
    public class Asset
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string StorageId { get; set; }
        public AssetInfo Info { get; set; }
    }
}
