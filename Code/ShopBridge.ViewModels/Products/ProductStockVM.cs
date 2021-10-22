
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
    public class ProductStockVM : BaseEntityVM
    {
        public int Id { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Common_Limit_Error")]
        public string SKU { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Product_ProductName_Required")]
        [StringLength(250, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Common_Limit_Error")]
        public string Name { get; set; }
        [StringLength(500, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Common_Limit_Error")]
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Product_Low_Price_Required")]
        public decimal LowPrice { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Product_Price_Zero_Validation")]
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Product_High_Price_Required")]
        public decimal HighPrice { get; set; }
        [Required(ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = "Product_Quantity_Required")]
        public int Quantity { get; set; }
        public bool IsFinished { get; set; } = false;
       

        //OTHER DATA INFO
        public List<int> Brands { get; set; }
        public List<string> ProductTagsList { get; set; }
    }
}
