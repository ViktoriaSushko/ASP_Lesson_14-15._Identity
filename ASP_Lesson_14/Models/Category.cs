using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Lesson_14.Models
{
    public class Category
    {
        public int Id { set; get; }
        [Display(Name ="Category name: ")]
        [Required]
        public string CategoryName { set; get; }
        [Display(Name ="Parent category")]
        public int? ParentCategoryId { set; get; }
        [ForeignKey(nameof(ParentCategoryId))]
        public Category? ParentCategory { set; get; }
        public ICollection<Category>? ChildCategories { set; get; }
        public ICollection<Product>? Products { set; get; } 
    }
}
