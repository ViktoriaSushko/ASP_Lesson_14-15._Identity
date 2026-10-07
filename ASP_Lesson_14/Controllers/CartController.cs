using ASP_Lesson_14.Extentsions;
using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.ViewModels.Cart;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Text;
using ASP_Lesson_14.Services;
namespace ASP_Lesson_14.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly ILogger<CartController> _logger;
        private readonly IEmailSenderCart emailSender;
        private readonly UserManager<ShopUser> userManager;
        private readonly SignInManager<ShopUser> signInManager;
        public CartController(ShopDbContext context, ILogger<CartController> logger, IEmailSenderCart emailSender, UserManager<ShopUser> userManager, SignInManager<ShopUser> signInManager)
        {
            _context = context;
            _logger = logger;
            this.emailSender = emailSender;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        //public IActionResult Index(string? returnUrl)
          public IActionResult Index(Cart cart,string? returnUrl)//modelBinders->CartModelBinder->CartModelBinderProvider
        {
            //1 Cart cart = GetCart();
            CartIndexVM cartIndexVM = new CartIndexVM()
            {
                CartItems = cart.CartItems,
                TotalPrice = cart.GetTotalPrice(),
                ReturnUrl = returnUrl ?? Url.Action("Index", "Home")
            };
            return View(cartIndexVM);
        }
        public IActionResult SetCountry()
        {
            HttpContext.Session.SetString("Country", "Ukraine");
            ViewBag.Country = HttpContext.Session.GetString("Country");
            return View();
        }
        public IActionResult GetCountry()
        {
            var country = HttpContext.Session.GetString("Country") ?? "Session key wasnot set";
            return View(model: country);
        }
        private Cart GetCart()
        {
            IEnumerable<CartItem>? cartItems = HttpContext.Session.Get<IEnumerable<CartItem>>("Cart");
            if (cartItems == null)
            {
                cartItems = new List<CartItem>();
                HttpContext.Session.Set("Cart", cartItems);
            }

            return new Cart(cartItems);
        }


        private void SetCart(Cart cart)
        {
            HttpContext.Session.Set("Cart", cart.CartItems);
        }
        //public async Task<IActionResult> AddToCart(int id, string? returnUrl)
             public async Task<IActionResult> AddToCart(int id,Cart cart, string? returnUrl)//modelBinders->CartModelBinder->CartModelBinderProvider
        {
            //1 Cart cart = GetCart();
            Product? product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await _context.Entry(product).Collection(p => p.Images!).LoadAsync();
          
            cart.AddToCart(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = 1
            });
            SetCart(cart);
            return Redirect(returnUrl);
        }
        [HttpPost]
        //public IActionResult RemoveFromCart(int? id, string? returnUrl)
        public IActionResult RemoveFromCart(int? id,Cart cart, string? returnUrl)// modelBinders->CartModelBinder->CartModelBinderProvider
        {
            if (id == null)
            {
                return NotFound();
            }
            //1 Cart cart = GetCart();
            cart.RemoveFromCart(id.Value);
            SetCart(cart);
            return RedirectToAction("Index", new { returnUrl });
        }
        [HttpPost]
        //public IActionResult IncCount(int id)
        public IActionResult IncCount(int id, Cart cart)
        {
            //1 Cart cart = GetCart();
            CartItem? item = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (item == null)
            {
                return NotFound();
            }           
            cart.IncCount(id);
            SetCart(cart);
            return Ok(new { item.Count, item.TotalPrice });
        }
        [HttpPost]
        //public IActionResult DecCount(int id)
        public IActionResult DecCount(int id, Cart cart)
        {           
            //Cart cart = GetCart();
            if (!cart.CartItems.Any(i => i.ProductId == id)) return NotFound();
            cart.DecCount(id);
            SetCart(cart);        
            var after = cart.CartItems.FirstOrDefault(i => i.ProductId == id);
            return Ok(new { Count = after?.Count ?? 0, TotalPrice = after?.TotalPrice ?? 0m });
        }
        [HttpPost]
        //public IActionResult getTotalPrice()
            public IActionResult getTotalPrice(Cart cart)
        {
            //1 Cart? cart = GetCart();

            if (cart == null)
            {
                return Ok(new { TotalPrice = 0 });
            }

            return Ok(new
            {
                TotalPrice = cart.GetTotalPrice()
            });
        }
        // Отдаёт первую картинку товара (в сессии картинки не храним)
        public async Task<IActionResult> ProductImage(int id)
        {
            var data = await _context.Products
                .Where(p => p.Id == id)
                .SelectMany(p => p.Images!)
                .Select(i => i.ImageData)
                .FirstOrDefaultAsync();

            if (data == null || data.Length == 0)
                return Redirect("/images/no-img.jpg");

            return File(data, "image/jpeg");   // браузер сам определит реальный формат
        }
        [HttpPost]
        public async Task<IActionResult> Buy(int cartId,string? returnUrl)
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Login", "Account");
            }
            var cart = GetCart();
            if (!cart.CartItems.Any())
            {
                TempData["OrderMessage"] = "Your cart is empty. Please add items to your cart before placing an order.";
                TempData["OrderMessageType"] = "warning";
                return RedirectToAction("Index", "Home");
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["OrderMessage"] = "Не вдалося отримати інформацію про користувача";
                TempData["OrderMessageType"] = "danger";
                return RedirectToAction("Login", "Account");
            }
            var email = user.Email;
            if(string.IsNullOrWhiteSpace(email))
            {
                TempData["OrderMessage"] = "Не вдалося отримати електронну пошту користувача";
                TempData["OrderMessageType"] = "danger";
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var body = new StringBuilder();
                body.AppendLine($"<h2>Вітаємо, {user.UserName}!</h2>");
                body.Append("<table style='border-collapse: collapse; width: 100%;'>");
                body.Append("<tr style='background-color: #f2f2f2;'><th style='border: 1px solid #ddd; padding: 8px;'>Product</th><th style='border: 1px solid #ddd; padding: 8px;'>Price</th><th style='border: 1px solid #ddd; padding: 8px;'>Quantity</th><th style='border: 1px solid #ddd; padding: 8px;'>Total</th></tr>");
                foreach (var item in cart.CartItems)
                {
                    body.Append("<tr>");
                    body.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Name}</td>");
                    body.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Price:C}</td>");
                    body.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Count}</td>");
                    body.Append($"<td style='border: 1px solid #ddd; padding: 8px;'>{item.TotalPrice:C}</td>");
                    body.Append("</tr>");
                }
                body.Append("<tr style='background-color: #f2f2f2;'><td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right;'><strong>Total:</strong></td><td style='border: 1px solid #ddd; padding: 8px;'><strong>" + cart.GetTotalPrice().ToString("C") + "</strong></td></tr>");
                body.Append("</table>");
                await emailSender.SendAsync(
                    from: email,
                    to: email,
                    subject: $"Замовлення від {DateTime.Now:g}",
                    body: body.ToString()
                    );
                SetCart(new Cart(new List<CartItem>()));
                TempData["OrderMessage"] = "Замовлення успішно оформлено!";
                TempData["OrderMessageType"] = "success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не вдалося відправити електронний лист для користувача {Email}", email);
                TempData["OrderMessage"] = "Помилка при оформленні замовлення.";
                TempData["OrderMessageType"] = "danger";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
