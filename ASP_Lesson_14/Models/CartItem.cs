namespace ASP_Lesson_14.Models
{
    public class CartItem
    {
        //public Product Product { get; set; }=default!;
        public int ProductId { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice => Price * Count;
    }
}
