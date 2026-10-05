namespace ASP_Lesson_14.Models.ViewModels.Cart
{
    public class CartIndexVM
    {
        public IEnumerable<CartItem> CartItems { get; set; } = default!;
        public decimal TotalPrice { get; set; }
        public string? ReturnUrl { get; set; }=default!;
    }
}
