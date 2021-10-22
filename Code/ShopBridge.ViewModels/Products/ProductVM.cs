
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopBridge.BuldingBlocks;
using ShopBridge.ViewModels.Common;

namespace ShopBridge.ViewModels.Products
{
    public class ProductVM : BaseEntityVM
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
       
        //OTHER DATA INFO
        public List<ProductBrandVM> Brands { get; set; }
        public List<string> ProductTags { get; set; }
    }
}
