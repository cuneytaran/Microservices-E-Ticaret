using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Services
{
    //Redis ile bağlantı kurmakla sorumlu olan yer.

    //Dependencies den AddReferance kısmından Sharedi referans vermeyi unutma
    //Properties klasöründeki lauchSettings.json da ayarlamayı yap.
    //NudgetPackages den StackExchange.Redis yükle
    //appsettings.json içinde redise nasıl bağlanacağını belirledik.
    //settings klasörün içinde redis ayarları yaptık.
    //startup tarafında redisi tanımlama yaptım.

    public class RedisService
    {
        private readonly string _host;//adresi

        private readonly int _port;//portu

        private ConnectionMultiplexer _ConnectionMultiplexer; //ConnectionMultiplexer = RedisExchange den gelen bir sınıf.Nudget gelen.

        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect() => _ConnectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");//redis bağlantısı

        public IDatabase GetDb(int db = 1) => _ConnectionMultiplexer.GetDatabase(db);//int db = 1 yapmasının sebebi Redis içinde birden fazla veritabanı varsa ayırmak için. biri test veritabanı diğeri canlı vs...
                                                                                     //GetDb rasgele isim verdik.
                                                                                     //startup da tanımlamayı yapmayı unutma
    }
}