using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FreeCourse.Shared.Dtos
{
    public class Response<T> //Tek bir DTO nesnesi dönecek. Liste değil.kısıtlama yapmamak için where kullanmıyoruz.
    {
        public T Data { get; set; }

        [JsonIgnore]//Kendi içinde kullanmak için. Yani response içinde olmasına gerek yok.Yani gelen verinin içinde olmasına gerek yok.
        public int StatusCode { get; set; }

        [JsonIgnore]//başarılımı değilmi olduğunu öğrenmek için
        public bool IsSuccessful { get; set; }

        public List<string> Errors { get; set; }//hata listesini dönecek. çünkü birden fazla hata olabilir.

        // Static Factory Method. Özellikle nesne oluşturmada büyük kolaylık sağlar.
        public static Response<T> Success(T data, int statusCode) //başarılı ve data alıyorsa ise
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Success(int statusCode) // başarılı olabilir ama data almayabilir ise
        {
            return new Response<T> { Data = default(T), StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Fail(List<string> errors, int statusCode)//başarısız ama hataları alabiliyorsa ise

        {
            return new Response<T>
            {
                Errors = errors,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }

        public static Response<T> Fail(string error, int statusCode)//başarısız ama hata datası almayacak ise
        {
            return new Response<T> { Errors = new List<string>() { error }, StatusCode = statusCode, IsSuccessful = false };
        }
    }
}