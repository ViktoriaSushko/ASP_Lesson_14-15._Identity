using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models.Data
{
    public class ShopUser:IdentityUser
    {
        [Display(Name ="Date of birthday: ")]
        public DateTime DateOfBirth { set; get; }
    }
}
