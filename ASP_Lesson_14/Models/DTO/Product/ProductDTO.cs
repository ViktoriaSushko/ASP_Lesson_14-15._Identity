using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models.DTO.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Brand")]
        public int BrandId { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
