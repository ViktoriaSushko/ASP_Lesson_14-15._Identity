using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Lesson_14.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ShopUser> userManager;
        private readonly SignInManager<ShopUser> singInManager;
        public AccountController(UserManager<ShopUser> userManager, SignInManager<ShopUser> singInManager)
        {
            this.userManager = userManager;
            this.singInManager = singInManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await userManager.Users.ToListAsync());
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                ShopUser user = new ShopUser
                {
                    UserName = vm.Login,
                    Email = vm.Email,
                    DateOfBirth = vm.DateOfBirth
                };
                IdentityResult result = await userManager.CreateAsync(user, vm.Password);
                if (result.Succeeded)
                {
                    await singInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(vm);
                    }
                }
            }
            return View(vm);
        }
        public IActionResult Login(string? returnUrl)
        {
            LoginViewModel lvm = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(lvm);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                ShopUser? user = await userManager.FindByEmailAsync(lvm.Email);
                if (user != null)
                {
                    var result = await singInManager.PasswordSignInAsync(user, lvm.Password, lvm.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(lvm.ReturnUrl) && Url.IsLocalUrl(lvm.ReturnUrl))
                        {
                            return Redirect(lvm.ReturnUrl);
                        }
                        else
                            return RedirectToAction("Index", "Account");
                        
                    }
                    ModelState.AddModelError(string.Empty, "Incorrect login or password!");
                   
                }
            }
            return View(lvm);
        }
        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl)
        {
            await singInManager.SignOutAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
