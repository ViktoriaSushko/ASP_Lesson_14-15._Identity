namespace ASP_Lesson_14.Models.ViewModels.Cart
{
    public class CartIndexVM
    {
        public int Id { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public string? ReturnUrl { get; set; }=default!;
    }
}
