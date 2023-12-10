using BookStore_CNN.Helpers;
using BookStore_CNN.Models;
using BookStore_CNN.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_CNN.ViewComponents
{
    public class CartViewComponent:ViewComponent
    {
        private readonly BookStoreCaratContext _context;

        public CartViewComponent(BookStoreCaratContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>(CartItemSetting.CART_KEY) ?? new List<CartItem>();

            return View("CartPanel", new CartItemModels
            {
                Quantity = cart.Sum(p => p.iSoLuong),
                Total = cart.Sum(p => p.dThanhtien)
            });
        }
    }
}
