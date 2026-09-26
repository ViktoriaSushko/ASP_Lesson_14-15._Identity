using ASP_Lesson_14.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ASP_Lesson_14.Models.DTO.Claims.CustomerPolicies
{
    public class AllowedAgeRequirement : IAuthorizationRequirement
    {
        public int MinAge { get; init; }
        public AllowedAgeRequirement(int minAge=18)
        {
            this.MinAge = minAge;
        }
    }
    public class AllowedAgeHandler : AuthorizationHandler<AllowedAgeRequirement>
    {
        private readonly UserManager<ShopUser> userManager;
        public AllowedAgeHandler(UserManager<ShopUser> userManager)
        {
            this.userManager = userManager;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,AllowedAgeRequirement requirement)
        {
            if(context is not null&& context.User.Identity is not null)
            {
                ShopUser? shopUser = await userManager.GetUserAsync(context.User);
                if (shopUser != null)
                {
                    int totalAge = DateTime.Now.Year - shopUser.DateOfBirth.Year;
                    if (totalAge >= requirement.MinAge)
                        context.Succeed(requirement);
                    else
                        context.Fail();
                    await Task.CompletedTask;
                }
            }
        }
    }
}
