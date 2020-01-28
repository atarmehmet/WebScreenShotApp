using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ScreenShotDAL.Entity
{
    public class ScreenShotData
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Url { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; } 
        public string ContentImage { get; set; } 
    }
}
