using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_CNN.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;
        public string? TacGia { get; set; }
        public double? Price { get; set; }

        public string? Image { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? SupplierId { get; set; }
        public int? Quantity { get; set; }
        public double? Discount { get; set; }
        public bool? Special { get; set; }
        public DateTime? CreatedDate { get; set; }


        [NotMapped]
        public IFormFile productFile { get; set; }

        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
