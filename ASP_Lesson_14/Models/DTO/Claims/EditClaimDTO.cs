namespace ASP_Lesson_14.Models.DTO.Claims
{
    public class EditClaimDTO
    {     
        public string OldClaimType { get; set; } = default!;
        public string OldClaimValue { get; set; } = default!;
        public string ClaimType { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
    }
}
