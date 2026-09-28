using ASP_Lesson_14.Models.DTO.Product;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP_Lesson_14.Models.ViewModels.Product
{
    public class CreateProductVM
    {
        public SelectList Brands { get; set; } = default!;
        public SelectList Category { get; set; } = default!;
        public ProductDTO Product { get; set; } = default!;
    }
}
