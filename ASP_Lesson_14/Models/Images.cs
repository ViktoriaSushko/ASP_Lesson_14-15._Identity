using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_Lesson_14.Models
{
    public class Images
    {
        public int Id { set; get; }
        public byte[]? ImageData { set; get; } 
        public int ProductId { set; get; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { set; get; } = default!;
    }
}
