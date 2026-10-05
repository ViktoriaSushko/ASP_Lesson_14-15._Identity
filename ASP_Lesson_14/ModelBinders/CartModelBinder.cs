using ASP_Lesson_14.Extentsions;
using ASP_Lesson_14.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ASP_Lesson_14.ModelBinders
{
    public class CartModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
           if(bindingContext==null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            string sessioKey = "Cart";
            IEnumerable<CartItem>? cartItems = null;
            cartItems=bindingContext.HttpContext.Session.Get<IEnumerable<CartItem>>(sessioKey);
            if(cartItems == null)
            {
                cartItems = new List<CartItem>();
                bindingContext.HttpContext.Session.Set(sessioKey, cartItems);
            }
            Cart cart = new Cart(cartItems);
            bindingContext.Result = ModelBindingResult.Success(cart);
            return Task.CompletedTask;
        }
    }
}
