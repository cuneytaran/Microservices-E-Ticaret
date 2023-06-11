using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using Mass = MassTransit;//namespace isim verdik. Çünkü MassTransit ile aynı isimde bir nesne var.Mass olarak kullanacağız.
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Shared.Messages;
using GreenPipes.Caching;

namespace FreeCourse.Services.Catalog.Services
{
    //RabbitMQ için Nudget Packages manager console içinde MassTransit.AspNetCore ve MassTransit.RabbitMQ kurduk
    //startup tarafında ayar yapmamız gerekiyor.

    //services.AddMassTransit(x =>
    //{
    //    // Default Port : 5672
    //    x.UsingRabbitMq((context, cfg) =>
    //    {
    //        cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
    //        {
    //            host.Username("guest");
    //            host.Password("guest");
    //        });
    //    });
    //});

    //appsettings.json kısmında RabbitMQUrl tanımlaması yapmamız gerekiyor. 
    //"RabbitMQUrl": "localhost",
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection; //Mongodb tanımlıyoruz.
        private readonly IMongoCollection<Category> _categoryCollection; 
        private readonly IMapper _mapper;
        private readonly Mass.IPublishEndpoint _publishEndpoint;//RabbitMQ için tanımladık. Event yapıyoruz. Send yerine Publish kullanıyoruz.Çoklu microservice kullanıyorsak Publish kullanıyoruz.

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionString); //mongodb veritabanına bağlantı yolunu oluşturuyoruz.

            var database = client.GetDatabase(databaseSettings.DatabaseName); //veritabanına bağlanıyoruz

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName); //Course ve collectiona bağlanıyoruz

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName); //Category ve collectiona bağlanıyoruz
            
            _mapper = mapper; //mapper ayağa kaldırıyoruz

            _publishEndpoint = publishEndpoint;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync(); //course => true tüm kategorileri bana ver.

            if (courses.Any())
            {
                foreach (var course in courses) //monogoda join olmadığı için. tek tek categoryleri çağırıyoruz.
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();//eğer course yoksa boş liste dön.
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);//course boş ise faille beraber 404 hatası dönüyoruz.
            }
            course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);//_mapper.Map<CourseDto>(course) course içindeki verileri mapper ile CourseDto içine aktarma işlemi
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);

            newCourse.CreatedTime = DateTime.Now;
            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto) //NoContent dememiz sebebi update edilen listeye dönmeye gerek yok.
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);//FindOneAndReplaceAsync bul ve değiştir. bulamazsa null dönecek

            if (result == null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }

            await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent { CourseId = updateCourse.Id, UpdatedName = courseUpdateDto.Name });//Publish methodunu kullanıyoruz çünkü çoklu microservice kullanıyoruz.
            //kuyruğa id ve name bilgisi gönderiyoruz. diğer servislerde bu kuyruğu dinleyecekler. değişiklik olduğunda kendilerinde güncelleyecekler.
            //Order ve Basket servislerinde bu kuyruğu dinleyeceğiz.
            

            return Response<NoContent>.Success(204);//204 update başarılı kodu
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);//DeleteOneAsync tek bir datayı silme işlemi. 

            if (result.DeletedCount > 0)//eğerki resultun DeleteCount 0 dan büyükse gerçekten silmiş demektir.
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}