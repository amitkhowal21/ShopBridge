
using ShopBridge.ViewModels.Common;
using ShopBridge.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopBridge.Services.Products.Stock
{
    public interface IProductStock
    {
        /// <summary>
        /// This method will return product details with tags and categories
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>product details view model</returns>
        Task<BaseResponseModel<ProductVM>> GetProductById(int id);
        /// <summary>
        /// Save product
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return primary key of the product</returns>
        Task<BaseResponseModel<int>> SaveProduct(ProductStockVM model);
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<BaseResponseModel<bool>> Delete(ProductDeleteVM model);
        /// <summary>
        /// Get list of products
        /// </summary>
        /// <param name="searchVM"></param>
        /// <returns>Products</returns>
        Task<BaseResponseModel<List<ProductListVM>>> List(ProductSearchVM searchVM);
    }
}
