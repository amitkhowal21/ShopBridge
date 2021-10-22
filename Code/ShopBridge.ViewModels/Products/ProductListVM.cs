
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopBridge.BuldingBlocks;
namespace ShopBridge.ViewModels.Products
{
    public class ProductListVM
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public decimal LowPrice { get; set; }
        public decimal HighPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsFinished { get; set; } = false;
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
