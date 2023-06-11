using FreeCourse.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IUserService
    {
        //statup da tanımlamayı unutma.
        Task<UserViewModel> GetUser();//endpoint ten gelecek olan modeli karşılayacak.
    }
}