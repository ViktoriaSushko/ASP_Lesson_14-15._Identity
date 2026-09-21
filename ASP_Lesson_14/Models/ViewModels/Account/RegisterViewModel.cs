using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string Login { get; set; } = default;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default;
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm password:")]
        [Compare(nameof(Password),ErrorMessage ="Passwords should match!")]
        public string ConfirmPassword { get; set; } = default;
    }
}
