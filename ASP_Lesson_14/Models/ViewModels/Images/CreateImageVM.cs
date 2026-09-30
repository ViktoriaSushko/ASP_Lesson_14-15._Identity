using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP_Lesson_14.Models.ViewModels.Images
{
    public class CreateImageVM
    {
        public SelectList Categories { set; get; } = default!;
    }
}
