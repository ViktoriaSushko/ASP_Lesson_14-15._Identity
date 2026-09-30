using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.DTO.Images;
using ASP_Lesson_14.Models.ViewModels.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_Lesson_14.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ShopDbContext _context;
        public ImagesController(ShopDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var images = _context.Images.Include(i => i.Product);
            return View(await images.ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Products.Include(p => p.Category).Select(p => p.Category).Distinct().ToListAsync();
            CreateImageVM imageVM = new CreateImageVM
            {
                Categories = new SelectList(categories, nameof(Category.Id), nameof(Category.CategoryName))
            };
            return View(imageVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AddImageDTO imageDTO)
        {
            if (ModelState.IsValid)
            {
                if (imageDTO.Photos.Any())
                {
                    foreach (var photo in imageDTO.Photos)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await photo.CopyToAsync(ms);
                            ms.Seek(0, SeekOrigin.Begin);
                            Images image = new Images
                            {
                                ImageData = ms.ToArray(),
                                ProductId = imageDTO.ProductId
                            };
                            _context.Images.Add(image);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
              
                return RedirectToAction(nameof(Index));
            }
            var categories = await _context.Products.Include(p => p.Category).Select(p => p.Category).Distinct().ToListAsync();
            CreateImageVM imageVM = new CreateImageVM
            {
                Categories = new SelectList(categories, nameof(Category.Id), nameof(Category.CategoryName))
            };
            return View(imageVM);
        }
        [HttpPost]
        public async Task<IActionResult> GetBrandsByCategory(int id)
        {
            var brands = await _context.Products.Include(p => p.Brand).Where(p => p.CategoryId == id).Select(p => p.Brand).Distinct().ToListAsync();
            return PartialView(brands);
        }
        [HttpPost]
        public async Task<IActionResult> GetProducts([FromBody] FindProductDTO findProductDTO)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == findProductDTO.CategoryId && p.BrandId == findProductDTO.BrandId)
                .ToListAsync();
            return PartialView(products);
        }
    }
}
