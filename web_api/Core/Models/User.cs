using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace web_api.Core.Models
{
    public record User
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("Email")]
        public string Email { get; set; } = String.Empty;
        
        [BsonElement("Password")]
        public string Password { get; set; } = String.Empty;
        //public Guid Id { get; init; }
        //public string Email { get; set; } = String.Empty;
        //public string Password { get; set; }
        //public DateTimeOffset CreatedDate { get; init; }
    }
    public record UserLogin
    {
        [BsonElement("Email")]
        public string Email { get; set; } = String.Empty;
        [BsonElement("Password")]
        public string Password { get; set; } = String.Empty;
    }
}
