
using ShopBridge.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.Services.Products.Brand
{
    public interface IProductBrand
    {
        List<ProductBrandVM> GetProductBrands(int productId);
    }
}
