
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CategoryController : Controller
{
    private readonly ShopDbContext _context;

    public CategoryController(ShopDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORY
    public async Task<IActionResult> Index()    
    {
        var categories = _context.Categories.Include(c => c.ParentCategory);
        return View(await categories.ToListAsync());
    }

    // GET: CATEGORY/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: CATEGORY/Create
    public async Task<IActionResult> Create()
    {
        var categories = await _context.Categories.Where(c => c.ParentCategoryId == null).ToListAsync();
        ViewData["ParentCategoryId"] = new SelectList(categories, "Id", "CategoryName");
        return View();
    }

    // POST: CATEGORY/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CategoryName,ParentCategoryId,ParentCategory,ChildCategories,Products")] Category category, int[] parentCategoryId)
    {
        if (ModelState.IsValid)
        {
            if (parentCategoryId.Length == 1 && parentCategoryId[0] == 0)
            {
                category.ParentCategoryId = null;
            }
            else
                if (parentCategoryId[parentCategoryId.Length - 1] != 0)
                    category.ParentCategoryId = parentCategoryId[parentCategoryId.Length - 1];
            else
                    category.ParentCategoryId = parentCategoryId[parentCategoryId.Length - 2];
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName",category.ParentCategoryId);
        return View(category);
    }
    public async Task<IActionResult> GetChildCategories(int parentId)
    {
        var childCategories = await _context.Categories.Where(c => c.ParentCategoryId == parentId).ToListAsync();
        return PartialView(childCategories);
    }
    // GET: CATEGORY/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: CATEGORY/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,CategoryName,ParentCategoryId,ParentCategory,ChildCategories,Products")] Category category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: CATEGORYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: CATEGORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int? id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
