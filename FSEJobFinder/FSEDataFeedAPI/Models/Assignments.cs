using FSEDataFeed;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FSEDataFeedAPI.Models
{
#pragma warning disable CS1591
#pragma warning disable IDE1006
    public class Assignments
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string aircraft { get; set; } = null!;
        public List<Assignment> jobs { get; set; } = null!;
        public DateTime createdAt { get; set; } = DateTime.Now;
        public DateTime updatedAt { get; set; } = DateTime.Now;
    }
#pragma warning restore CS1591
#pragma warning restore IDE1006
}
