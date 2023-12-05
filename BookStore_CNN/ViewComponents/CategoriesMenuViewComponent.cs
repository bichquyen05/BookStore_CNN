using BookStore_CNN.Models;
using BookStore_CNN.Repository;
using BookStore_CNN.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace BookStore_CNN.ViewComponents
{
    public class CategoriesMenuViewComponent : ViewComponent
    {
        private readonly BookStoreCaratContext _context;

        public CategoriesMenuViewComponent(BookStoreCaratContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.Select(c => new MenuCategories
            {
                Id= c.Id,
                Name= c.Name,
            });
            return View(categories);
        }
    }
}
