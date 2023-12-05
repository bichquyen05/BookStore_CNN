using BookStore_CNN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace BookStore_CNN.Controllers
{
    public class HomeController : Controller
    {
        BookStoreCaratContext _context=new BookStoreCaratContext();
        private readonly ILogger<HomeController> _logger;
        private ProductsController ProductsController { get; set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;            
        }

        public IActionResult Index()
        {         
            var featuredProducts = _context.Products.Where(p => (bool)p.Special).OrderByDescending(p => p.CreatedDate).Take(8).ToList();

            var latestProducts = _context.Products.OrderByDescending(p => p.CreatedDate).Take(12).ToList();                                      

            ViewData["FeaturedProducts"] = featuredProducts;
            ViewData["LatestProducts"] = latestProducts;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Shop(int? page)
        {           
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var products = _context.Products.AsNoTracking().OrderBy(p => p.Name);
            PagedList<Product> lst = new PagedList<Product>(products, pageNumber, pageSize);            
            return View(lst);
        }

        public IActionResult ProductByCategory(int id, int? page)
        {
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var products = _context.Products.AsNoTracking().Where(x => x.Id == id).OrderBy(p => p.Name);
            PagedList<Product> lst = new PagedList<Product>(products, pageNumber, pageSize);
            return View(lst);
            //List<Product> products = _context.Products.Where(x => x.Id == id).OrderBy(x => x.Name).ToList();            
            //return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
