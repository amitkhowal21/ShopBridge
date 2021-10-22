
using ShopBridge.Model;
using ShopBridge.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.Services.Products.Brand
{
    public class ProductBrandService : IProductBrand
    {
        public List<ProductBrandVM> GetProductBrands(int productId)
        {
            using (var context = new ShopBridgeEntities())
            {
                var productBrands = (from PBM in context.ProductBrandMappings
                                         join PB in context.ProductBrands on PBM.BrandId equals PB.Id
                                         where PB.IsActive == true && PB.IsDeleted == false && PBM.IsActive == true && PBM.IsDeleted == false
                                         && PBM.ProductId == productId
                                         select new ProductBrandVM
                                         {
                                             Id = PB.Id,
                                             Name = PB.Name
                                         }).ToList();
                return productBrands;
            }
        }

    }
}
