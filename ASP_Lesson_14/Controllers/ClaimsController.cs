using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.DTO.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AspNet.Security.OAuth.GitHub.GitHubAuthenticationConstants;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
namespace ASP_Lesson_14.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly UserManager<ShopUser> userManager;
        private readonly SignInManager<ShopUser> signInManager;
        public ClaimsController(UserManager<ShopUser> userManager, SignInManager<ShopUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View(User.Claims);
        }
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateClaimDTO dto)
        {
            Claim claim = new Claim(dto.ClaimType, dto.ClaimValue, ClaimValueTypes.String);
            ShopUser? shopUser = await userManager.GetUserAsync(User);
            if(shopUser is not null)
            {
                var identityClaimResult = await userManager.AddClaimAsync(shopUser, claim);
                if (identityClaimResult.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(shopUser);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in identityClaimResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(dto);
        }
        public async Task<IActionResult> Delete(string claimInfo)
        {
            string[] claimsData = claimInfo.Split(';');
            string claimType = claimsData[0];
            string claimValueType = claimsData[1];
            string claimValue = claimsData[2];
            Claim? delClaim = User.Claims.Where(c => c.Value == claimValue && c.Type == claimType).FirstOrDefault();
            ShopUser? shopUser = await userManager.GetUserAsync(User);
            if (shopUser != null && delClaim != null)
            {
                var result = await userManager.RemoveClaimAsync(shopUser, delClaim);
                if (result.Succeeded) {
                    await signInManager.RefreshSignInAsync(shopUser);
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
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(string claimInfo)
        {
            string[] claimsData = claimInfo.Split(';');
            string claimType = claimsData[0];
            string claimValue = claimsData[2];
            Claim? claim = User.Claims.Where(c => c.Value == claimValue && c.Type == claimType).FirstOrDefault();
            if (claim == null) return NotFound();
            EditClaimDTO dto = new EditClaimDTO
            {
                OldClaimType = claim.Type,
                OldClaimValue = claim.Value,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
            return View(dto);
        }
            [HttpPost]
        public async Task<IActionResult> Edit(EditClaimDTO dto)
        {
            ShopUser? shopUser = await userManager.GetUserAsync(User);
            if (shopUser == null) return NotFound();
            Claim? oldClaim = (await userManager.GetClaimsAsync(shopUser)).FirstOrDefault(c => c.Value == dto.OldClaimValue && c.Type == dto.OldClaimType);
            if (oldClaim == null) return NotFound();
            if (ModelState.IsValid)
            {
                Claim? newClaim = new Claim(dto.ClaimType, dto.ClaimValue, ClaimValueTypes.String);
                var result = await userManager.ReplaceClaimAsync(shopUser, oldClaim, newClaim);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(shopUser);
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
    }
}
