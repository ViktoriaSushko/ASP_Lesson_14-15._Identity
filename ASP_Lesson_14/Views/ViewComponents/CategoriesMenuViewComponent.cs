using ASP_Lesson_14.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_Lesson_14.Views.ViewComponents
{
    public class CategoriesMenuViewComponent:ViewComponent
    {
        private readonly ShopDbContext context;
        public CategoriesMenuViewComponent(ShopDbContext context)
        {
            this.context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await context.Categories.Where(t => t.ParentCategoryId == null).Include(t => t.ChildCategories!).ThenInclude(t => t.ChildCategories).ToListAsync();
            return View(categories);
        }
    }
}
