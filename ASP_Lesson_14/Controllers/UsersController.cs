using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.DTO.User;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ASP_Lesson_14.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ShopUser> userManager;
        public UsersController(UserManager<ShopUser> userManager)
        {
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await userManager.Users.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                ShopUser user = new ShopUser
                {
                    UserName = dto.Login,
                    Email = dto.Email,
                    DateOfBirth = dto.DateOfBirth
                };
                IdentityResult result = await userManager.CreateAsync(user, dto.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(dto);
        }
        public async Task<IActionResult> Edit(string id)
        {
            ShopUser? user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            EditUserDTO dto = new EditUserDTO
            {
                Id = user.Id,
                Login = user.UserName ?? "",
                Email = user.Email ?? "",
                DateOfBirth = user.DateOfBirth
            };
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                ShopUser? user = await userManager.FindByIdAsync(dto.Id);
                if (user == null) return NotFound();
                user.UserName = dto.Login;
                user.Email = dto.Email;
                user.DateOfBirth = dto.DateOfBirth;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(dto);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            ShopUser? user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ChangePassword(string? id)
        {
            ShopUser? user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ChangePasswordDTO dto = new ChangePasswordDTO()
            {
                Id = id,
                Email = user.Email
            };
            return View(dto);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            ShopUser? user = await userManager.FindByIdAsync(dto.Id);
            if (user == null) return NotFound();
            IdentityResult result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(dto);
        }
        public async Task<IActionResult> Details(string? id)
        {
            if (id == "") return NotFound();
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return View(user);

        }
    }
}
