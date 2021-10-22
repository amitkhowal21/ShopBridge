
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
    public class ProductBrandVM : BaseEntityVM 
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
