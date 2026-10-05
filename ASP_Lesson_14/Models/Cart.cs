namespace ASP_Lesson_14.Models
{
    public class Cart
    {
        protected ICollection<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> CartItems => items;
        public Cart(IEnumerable<CartItem> items)
        {
            this.items = new List<CartItem>(items);
        }
        public void AddToCart(CartItem item)
        {
            var cartItem = items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (cartItem != null) cartItem.Count += item.Count;
            else items.Add(item);
        }
        public decimal GetTotalPrice()
        {
            return items.Sum(i => i.TotalPrice);
        }
     
        public bool RemoveFromCart(CartItem item) => RemoveFromCart(item.ProductId);

        public bool RemoveFromCart(int id)
        {
            CartItem? cartItem = items.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null)
            {
                items.Remove(cartItem);
                return true;
            }
            return false;
        }

        public void IncCount(int id)
        {
            CartItem? cartItem = items.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null) cartItem.Count++;   
        }

        public void DecCount(int id)
        {
            CartItem? cartItem = items.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null)
            {
                if (cartItem.Count > 1) cartItem.Count--;
                else RemoveFromCart(cartItem);
            }
        }
    }
}
