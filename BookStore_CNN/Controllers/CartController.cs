using BookStore_CNN.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore_CNN.Controllers
{
    public class CartController : Controller
    {
        private readonly BookStoreCaratContext _context;
        public CartController (BookStoreCaratContext context)
        {
            _context = context;
        }        

        public List<CartItem> Carts
        {
            get
            {
                var data = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();
                ViewBag.Discount = Discount();
                ViewBag.Total = CalculateTotal()-Discount();              
                return data;
            }
        }
        public IActionResult Index()
        {
            return View(Carts);
        }

        public IActionResult AddToCart(int Id, int qty)
        {
            var gioHang = Carts;
            var item = gioHang.SingleOrDefault(p => p.idSach == Id);
            if (item != null)
            {
                item.iSoLuong += qty;
            }
            else
            {
                var product = _context.Products.SingleOrDefault(p => p.Id == Id);
                if (product != null)
                {
                    item = new CartItem
                    {
                        idSach = Id,
                        tenSach = product.Name,
                        tacGia = product.TacGia,
                        giaBan = product.Price ?? 0,
                        hinhAnh = product.Image,
                        iSoLuong = qty
                    };
                    gioHang.Add(item);
                }
            }
            HttpContext.Session.Set("GioHang", gioHang);
            return RedirectToAction("Index");
        }
        
        public ActionResult RemoveCartItem( int itemid)
        {            
            var gioHang = Carts;
            var item = gioHang.SingleOrDefault(p => p.idSach == itemid);
            if (item != null)
            {
                var product = _context.Products.SingleOrDefault(p => p.Id == itemid);
                gioHang.Remove(item);
            }
            HttpContext.Session.Set("GioHang", gioHang);
            return RedirectToAction("Index");
        }

        public ActionResult UpdateCartItem(int itemid)
        {
            var gioHang = Carts;
            var item = gioHang.SingleOrDefault(p => p.idSach == itemid);
            if (item != null)
            {
                int quantity;
                if (int.TryParse(Request.Form["quantity"], out quantity))
                {
                    item.iSoLuong = quantity;
                }
            }
            HttpContext.Session.Set("GioHang", gioHang);
            return RedirectToAction("Index");
        }




        private double CalculateTotal()
        {
            double tt = 0;
            List<CartItem> data = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (data != null)
            {
                foreach (CartItem item in data)
                {
                    tt += item.dThanhtien;
                }
            }            
           return tt;
        }

        private double Discount()
        {
            double total = CalculateTotal();
            double discountAmount = 0.0;
            if (total > 500000)
            {
                discountAmount = total * 0.1;
            }
            return discountAmount;
        }


    }
}
