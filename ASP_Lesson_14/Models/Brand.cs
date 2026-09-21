using System.ComponentModel.DataAnnotations;

namespace ASP_Lesson_14.Models
{
    public class Brand
    {
        public int Id { set; get; }
        [Display(Name = "Brand name: ")]
        [Required]
        public string BrandName { set; get; }
        [Required]
        public string? Country { set; get; }
        public ICollection<Product> Products { set; get; }

    }
}
