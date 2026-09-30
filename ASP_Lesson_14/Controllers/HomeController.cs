using ASP_Lesson_14.Models;
using ASP_Lesson_14.Models.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ASP_Lesson_14.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShopDbContext _context;
        

        public HomeController(ShopDbContext context)
        {
            _context = context;
           
        }
        public async Task<IActionResult> Index()
        {
            var products = _context.Products.Include(p => p.Brand).Include(p => p.Category).Include(p => p.Images);
            return View(await products.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
