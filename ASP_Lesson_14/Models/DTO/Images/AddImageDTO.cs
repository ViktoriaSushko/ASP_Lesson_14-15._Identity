using Microsoft.AspNetCore.Mvc;

namespace ASP_Lesson_14.Models.DTO.Images
{
    public class AddImageDTO 
    {
     public IFormFile[] Photos { set; get; } = default!;
        public int ProductId { set; get; }
    }
}
