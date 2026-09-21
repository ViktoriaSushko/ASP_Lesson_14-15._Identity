using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models.DTO.User
{
    public class ChangePasswordDTO
    {
        public string Id { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = default;
        [Display(Name ="Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = default;
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = default;
    }
}
