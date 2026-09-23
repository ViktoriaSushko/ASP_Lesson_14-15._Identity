namespace ASP_Lesson_14.Models.ViewModels.Roles
{
    public class ChangeRoleViewModel
    {
        public string Id { get; set; } = default;
        public string? Email { get; set; }
        public IEnumerable<string>? AllRoles { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public IList<string>? UserRoles { get; set; } = default;
    }
}
