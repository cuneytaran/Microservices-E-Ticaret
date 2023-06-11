using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Services
{
    //token içinden userid sini otomatik almak için yapıyoruz
    public interface ISharedIdentityService
    {
        public string GetUserId { get; }
    }
}