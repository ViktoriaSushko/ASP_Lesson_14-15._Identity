using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ASP_Lesson_14.Controllers
{
    [Authorize(Roles="admin, manager")]
    public class RolesController : Controller
    {
        private readonly UserManager<ShopUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public RolesController(UserManager<ShopUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await roleManager.Roles.Select(t => new RoleViewModel
            {
                Id = t.Id,
                Name = t.Name

            }).ToListAsync();
            return View(roles);
        }
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(string? name)
        {
            if (name is not null)
            {
                IdentityRole role = new IdentityRole { Name = name };
                await roleManager.CreateAsync(role);
                return RedirectToAction("Index");
            }
            return View(new RoleViewModel { Name = name });
        }
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole? role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            RoleViewModel rvm = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(rvm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                IdentityRole? role = await roleManager.FindByIdAsync(rvm.Id);
                if (role == null) return NotFound();
                role.Name = rvm.Name;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(rvm);
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole? role = await roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            await roleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ChangeRoles(string? id)
        {
            ShopUser? user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ChangeRoleViewModel vm = new ChangeRoleViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = await userManager.GetRolesAsync(user),
                AllRoles = roleManager.Roles.Select(r => r.Name)
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRoles(ChangeRoleViewModel vm)
        {
            ShopUser? user = await userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();
            var userRoles = await userManager.GetRolesAsync(user);
            if (ModelState.IsValid)
            {
                var addedRoles = vm.Roles!.Except(userRoles);
                var deletedRoles = userRoles.Except(vm.Roles!);

                await userManager.AddToRolesAsync(user, addedRoles);
                await userManager.RemoveFromRolesAsync(user, deletedRoles);
                return RedirectToAction("Index", "Users");
            }
            var allRoles = roleManager.Roles.Select(r => r.Name);
            vm = new ChangeRoleViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(vm);
        }
    }
}
