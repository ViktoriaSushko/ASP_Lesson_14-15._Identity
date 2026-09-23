using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_Lesson_14.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ShopUser> userManager;
        private readonly SignInManager<ShopUser> signInManager;
        public AccountController(UserManager<ShopUser> userManager, SignInManager<ShopUser> singInManager)
        {
            this.userManager = userManager;
            this.signInManager = singInManager;
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
                    await signInManager.SignInAsync(user, isPersistent: false);
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
                    var result = await signInManager.PasswordSignInAsync(user, lvm.Password, lvm.RememberMe, false);
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
            await signInManager.SignOutAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
                return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action("GoogleResponse"));
            return new ChallengeResult("Google", properties);
        }
        public IActionResult GitHubLogin()
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("GitHub", Url.Action("GoogleResponse"));
            return new ChallengeResult("GitHub", properties);
        }
       
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string? returnUrl)
        {
            ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
                return RedirectToAction("Login");
            var signInResult = await signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false);
            string?[] userInfo = new[] {
            externalLoginInfo.Principal?.FindFirst(ClaimTypes.Name)?.Value,
            externalLoginInfo.Principal?.FindFirst(ClaimTypes.Email)?.Value};
            if (signInResult.Succeeded)
                return View(userInfo);        
            ShopUser? user = await userManager.FindByNameAsync(userInfo[0]);
            if (user == null)
            {
                user = new ShopUser
                {
                    UserName = externalLoginInfo.Principal?.FindFirst(ClaimTypes.Name)?.Value,
                    Email = externalLoginInfo.Principal?.FindFirst(ClaimTypes.Email)?.Value
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return RedirectToAction("AccessDenied");
            }
            var addLoginRes = await userManager.AddLoginAsync(user, externalLoginInfo);
            if (addLoginRes.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                return View(userInfo);
            }
            return RedirectToAction("AccessDenied");
        }
        public IActionResult AccessDenied() => View();
    }
}
