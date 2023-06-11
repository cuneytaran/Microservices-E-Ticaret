using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FreeCourse.Services.Catalog.Models
{
    public class Category
    {
        //Mongo db için

        [BsonId]//id olarak algılamasını sağlıyoruz
        [BsonRepresentation(BsonType.ObjectId)]//id üretiyoruz
        public string Id { get; set; }

        public string Name { get; set; }
    }
}