using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;//Mongodb tanımlıyoruz.

        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);//mongodb veritabanına bağlantı yolunu oluşturuyoruz.

            var database = client.GetDatabase(databaseSettings.DatabaseName);//veritabanına bağlanıyoruz

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);//Category ve collectiona bağlanıyoruz
            _mapper = mapper;
        }

        //GetAll Async
        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryCollection.Find(category => true).ToListAsync();//category => true tüm kategorileri bana ver.

            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);//List<CategoryDto>>(categories) categoriesdeki bilgileri mapper ile CategoryDto ya aktar
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);//Map<Category>(categoryDto) categoryDto daki verileri Category clasına aktarma işlemi mapper ile.
            await _categoryCollection.InsertOneAsync(category);//InsertOneAsync tek veri girişi yapmak. yani liste değil

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();

            if (category == null)
            {
                return Response<CategoryDto>.Fail("Category not found", 404);
            }

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }
    }
}