using Microsoft.AspNetCore.Identity;

namespace FreeCourse.IdentityServer.Models
{
    // user için extra bir özellik ekleme istiyorsak buraya yazacağız.
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}