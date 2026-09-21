using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Lesson_14.Models
{
    public class Product
    {
        public int Id { set; get; }
        [Display(Name = "Product name: ")]
        [Required]
        public string ProductName { set; get; }
        public string? Description {set;get;}
        public int BrandId { set; get; }
        [ForeignKey(nameof(BrandId))]
        public Brand Brand { set; get; } = default!;
        public int CategoryId { set; get; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { set; get; } = default!;
        public ICollection<Images>? Images { set; get; } 
    }
}
