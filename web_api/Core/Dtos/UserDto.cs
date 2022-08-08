using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace web_api.Core.Dtos
{
    public record UserDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("Email")]
        public string email { get; set; } = String.Empty;
    }
}
