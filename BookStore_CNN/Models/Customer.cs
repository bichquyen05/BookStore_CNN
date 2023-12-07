using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore_CNN.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        [Key]

        [BindNever]
        public string Id { get; set; }
        [BindNever]  
        
        public string Password { get; set; } 

        [Required(ErrorMessage = "Tên không được để trống")]
        [Display(Name = "Họ và tên")]
        public string Fullname { get; set; } 

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Vui lòng nhập đúng email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [BindNever]
        public string Photo { get; set; } = null!;
        [BindNever]
        public bool Activated { get; set; }

        [Required(ErrorMessage = "SĐT không được để trống")]
        [Phone(ErrorMessage = "Vui lòng nhập đúng SĐT")]
        [MaxLength(10, ErrorMessage = "Số điện thoại phải có 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; } 

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; } 

        public virtual ICollection<Order> Orders { get; set; }
    }
}
