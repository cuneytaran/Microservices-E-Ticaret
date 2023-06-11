using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Models
{
    //Mongo db için
    public class Course
    {
        [BsonId]//id olarak algılamasını sağlıyoruz
        [BsonRepresentation(BsonType.ObjectId)]//id üretiyoruz
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]//decimal tipinde veriyi tut
        public decimal Price { get; set; }

        public string UserId { get; set; }
        public string Picture { get; set; }

        [BsonRepresentation(BsonType.DateTime)]//tarih tipinde veriyi tut
        public DateTime CreatedTime { get; set; }

        public Feature Feature { get; set; }//Feature model ile bağlıyoruz

        [BsonRepresentation(BsonType.ObjectId)]
        public string CategoryId { get; set; }

        [BsonIgnore]//Mongodb veritabanına bunu kaydetme
        public Category Category { get; set; }
    }
}