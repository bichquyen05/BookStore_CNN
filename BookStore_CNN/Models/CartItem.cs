using System.ComponentModel.DataAnnotations;

namespace BookStore_CNN.Models
{
    public class CartItem
    {
        BookStoreCaratContext data = new BookStoreCaratContext();
        [Display(Name = "Mã sách")]
        public int idSach { get; set; }

        [Display(Name = "Tên sách")]
        public string tenSach { get; set; }

        [Display(Name = "Tác giả")]
        public string tacGia { get; set; }

        [Display(Name = "Hình ảnh")]
        public string hinhAnh { get; set; }

        [Display(Name = "Giá bán")]
        public Double giaBan { get; set; }

        [Display(Name = "Số lượng")]
        public int iSoLuong { get; set; }

        [Display(Name = "Thành tiền")]
        public Double dThanhtien => iSoLuong * giaBan;
             
    }
}
