using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookStore_CNN.Models;
using BookStore_CNN.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using X.PagedList;

namespace BookStore_CNN.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BookStoreCaratContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductsController(BookStoreCaratContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var bookStoreCaratContext = _context.Products.Include(p => p.Category).Include(p => p.Supplier);
            return View(await bookStoreCaratContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            Product product = new Product();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadFile(product);
                product.Image = uniqueFileName;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadFile(product);
                    product.Image = uniqueFileName;
                    _context.Update(product);
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
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
            if (_context.Products == null)
            {
                return Problem("Entity set 'BookStoreCaratContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       public IActionResult ProductByCategories(int? loai, int? page)
        {
            var products = _context.Products.AsQueryable();
            if (loai.HasValue)
            {
                products = products.Where(p => p.CategoryId == loai.Value);
            }

            var result = products.Select(p => new ProductVM
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image ?? "",
                Price = p.Price ?? 0,
            });
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;            
            PagedList<ProductVM> lst = new PagedList<ProductVM>(result, pageNumber, pageSize);
            return View(lst);
        }

        public IActionResult Search(string? keyword)
        {
            var products = _context.Products.AsQueryable();
            if (keyword != null)
            {
                products = products.Where(p => p.Name.Contains(keyword));
            }

            var result = products.Select(p => new ProductVM
            {
                Id = p.Id,
                Name = p.Name,
                Image= p.Image ?? "",
                Price = p.Price ?? 0,
            }).ToList();
            return View(result);
        }

        public IActionResult ProductDetails(int? idSP)
        {
            var product = _context.Products.SingleOrDefault(p=>p.Id==idSP.Value);

            var category = _context.Categories.SingleOrDefault(c => c.Id == product.CategoryId);
            ViewBag.loai = category.Name;

            var suppliers = _context.Suppliers.SingleOrDefault(s => s.Id == product.SupplierId);
            ViewBag.nhaXB = suppliers.Name;

            var relatedProducts = _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
            .Take(4)
            .ToList();

            ViewData["RelatedProducts"] = relatedProducts;

            return View(product);
        }
        

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string UploadFile(Product product)
        {
            string uniqueFileName = null;
            if(product.productFile != null)
            {
                var myPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "BookImage");
                if (!Directory.Exists(myPath))
                {
                    Directory.CreateDirectory(myPath);
                }
                var fullPath = Path.Combine(myPath, product.productFile.FileName);
                using (var file = new FileStream(fullPath, FileMode.Create))
                {
                    product.productFile.CopyTo(file);
                }
                uniqueFileName = Path.GetFileName(fullPath);                
            }
            return uniqueFileName;
        }



        
    }
}
