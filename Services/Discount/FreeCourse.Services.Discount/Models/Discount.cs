using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Models
{
    [Dapper.Contrib.Extensions.Table("discount")] //Posgre sql tablosuna oluştururken maplemek için  kullanıyoruz. 
    public class Discount
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rate { get; set; }//indirim kuponu oranı
        public string Code { get; set; }//indirim kodu
        public DateTime CreatedTime { get; set; }
    }
}