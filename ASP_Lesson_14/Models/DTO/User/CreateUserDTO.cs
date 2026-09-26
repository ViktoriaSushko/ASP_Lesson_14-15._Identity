using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models.DTO.User
{
    public class CreateUserDTO
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
        //add Role
        public string Role { get; set; }
    }
}
