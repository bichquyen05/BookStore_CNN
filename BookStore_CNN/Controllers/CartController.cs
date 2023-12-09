using BookStore_CNN.Models;
using BookStore_CNN.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;

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
                ViewBag.TotalItem = data.Sum(p => p.iSoLuong);
                ViewBag.TotalPrv = @String.Format("{0:0,0}", CalculateTotal());
                ViewBag.Discount = @String.Format("{0:0,0}", Discount());
                ViewBag.Total = @String.Format("{0:0,0}", CalculateTotal() - Discount());                
                return data;
            }
        }
        public IActionResult Index()
        {
            ViewBag.TongSoLuong = TotalProduct();
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

        public ActionResult DeleteCart()
        {
            var gioHang = Carts;
            gioHang.Clear();
            HttpContext.Session.Set("GioHang", gioHang);
            return RedirectToAction("Index");
        }
        

        [HttpGet]
        public ActionResult PlaceOrder()
        {
            //kiểm tra đăng nhập
            //if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            var cartitem = Carts;
            if (cartitem.Count == 0)
            {
                TempData["EmptyCart"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm vào giỏ!";
                return RedirectToAction("Index", "Cart");
            }           
            Customer customer = new Customer();

            OrderModels order = new OrderModels
            {
                cart = cartitem,
                customer = customer
            };
            
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PlaceOrder(IFormCollection f)
        {
            Order order = new Order();
            OrderDetail detail = new OrderDetail();
            Customer customer = HttpContext.Session.Get<Customer>("username");
            var cart = Carts;
            order.CustomerId = customer.Id;
            order.Receiver = f["Fullname"];
            order.OrderDate = DateTime.Now;
            order.Address = f["Address"];
            order.Amount = (float)CalculateTotal();
            order.Description = f["Description"];
            _context.Orders.Add(order);
            _context.SaveChanges();
            foreach (var item in cart)
            {                
                {
                    detail.OrderId = order.Id;
                    detail.ProductId = item.idSach;
                    detail.Price = item.dThanhtien;
                    detail.Quantity = item.iSoLuong;
                    detail.Discount = Discount();
                };
                _context.OrderDetails.Add(detail);
                _context.SaveChanges();
                HttpContext.Session.Set("GioHang", null);
            }
            _context.SaveChanges();            
            return RedirectToAction("OrderConfirmation", "Cart");
        }


        public ActionResult OrderConfirmation(IFormCollection f)
        {
            if (string.IsNullOrEmpty(f["Fullname"])|| string.IsNullOrEmpty(f["Address"])|| string.IsNullOrEmpty(f["Phone"]))
            {
                TempData["ErrorOrder"] = "Vui lòng điền đầy đủ thông tin!";
                return RedirectToAction("PlaceOrder");
            }
            return View();
        }
        
        public ActionResult _CartItems()
        {
            var cart = Carts;
            ViewBag.TotalItem = cart.Sum(p => p.iSoLuong);
            ViewBag.Discount = @String.Format("{0:0,0}", Discount());
            ViewBag.Total = @String.Format("{0:0,0}", CalculateTotal() - Discount());
            return PartialView("_CartView");
        }        

        private int TotalProduct()
        {
            int total = 0;
            var cartItems = Carts;
            if (cartItems != null)
            {
               foreach(CartItem item in cartItems)
                {
                    total += item.iSoLuong;
                }
            }
            return total;
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
