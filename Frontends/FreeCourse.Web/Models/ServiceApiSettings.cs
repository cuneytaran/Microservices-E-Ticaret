using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models
{
    public class ServiceApiSettings
    {
        public string IdentityBaseUri { get; set; }//identity adresi
        public string GatewayBaseUri { get; set; }//Gateway adresi
        public string PhotoStockUri { get; set; }//fotoğrafları alacağım url

        public ServiceApi Catalog { get; set; }//appsettings.json da Path adında adresi kayıtlı

        public ServiceApi PhotoStock { get; set; }

        public ServiceApi Basket { get; set; }

        public ServiceApi Discount { get; set; }

        public ServiceApi Payment { get; set; }
        public ServiceApi Order { get; set; }
    }

    public class ServiceApi
    {
        public string Path { get; set; } //appsettings.json da Path adında adresi kayıtlı
    }
}