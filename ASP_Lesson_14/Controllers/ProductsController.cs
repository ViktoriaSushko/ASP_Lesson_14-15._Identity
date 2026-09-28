using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.Data;
using ASP_Lesson_14.Models.DTO.Product;
using ASP_Lesson_14.Models.ViewModels.Product;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASP_Lesson_14.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IMapper mapper;

        public ProductsController(ShopDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(await shopDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
           
            CreateProductVM productVM = new CreateProductVM()
            {
                Brands = new SelectList(_context.Brands, "Id", "BrandName"),
                Category = new SelectList(_context.Categories, "Id", "CategoryName")
            };
            return View(productVM);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                Product createProduct = mapper.Map<Product>(product);
                _context.Products.Add(createProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CreateProductVM productVM = new CreateProductVM()
            {
                Brands = new SelectList(_context.Brands, "Id", "BrandName", product.BrandId),
                Category = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId),
                Product = product
            };

            return View(productVM);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            CreateProductVM productVM = new CreateProductVM()
            {
                Brands = new SelectList(_context.Brands, "Id", "BrandName", product.BrandId),
                Category = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId),
                Product = mapper.Map<ProductDTO>(product)
            };
            return View(productVM);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDTO product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Product editProduct = mapper.Map<Product>(product);
                try
                {
                    _context.Update(editProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            CreateProductVM productVM = new CreateProductVM()
            {
                Brands = new SelectList(_context.Brands, "Id", "BrandName", product.BrandId),
                Category = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId),
                Product = mapper.Map<ProductDTO>(product)
            };
            return View(productVM);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
