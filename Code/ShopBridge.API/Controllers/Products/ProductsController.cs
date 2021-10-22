using ShopBridge.API.Infrastructure.Filters;
using ShopBridge.BuldingBlocks;
using ShopBridge.BuldingBlocks.AppEnums;
using ShopBridge.BuldingBlocks.Extensions;
using ShopBridge.Services.Products.Stock;
using ShopBridge.ViewModels;
using ShopBridge.ViewModels.Common;
using ShopBridge.ViewModels.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ShopBridge.API.Controllers.Products
{
    [CustomAuthorizationFilter]
    [RoutePrefix("api/v1/products")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductStock _productStock;
        public ProductsController(IProductStock productStock)
        {
            _productStock = productStock;
        }

        [HttpGet]
        [Route("get/{id}")]
        public async Task<BaseResponseModel<ProductVM>> GetById(int id)
        {
            var response = new BaseResponseModel<ProductVM>();
            if (id <= 0)
            {
                response.Status = (int)ResponseStatus.Fail;
                response.Message = Messages.Productd_Invalid_ProductId;
                return response;
            }
            response = await _productStock.GetProductById(id);
            return response;
        }

        [HttpPost]
        [Route("save")]
        [ModelValidatoryFilter]
        public async Task<BaseResponseModel<int>> Save(ProductStockVM model)
        {
            var response = new BaseResponseModel<int>();
            response = await _productStock.SaveProduct(model);
            return response;
        }

        [HttpPost]
        [Route("list")]
        public async Task<BaseResponseModel<List<ProductListVM>>> List([FromBody]ProductSearchVM searchVM)
        {
            searchVM = searchVM ?? new ProductSearchVM();
            var response = new BaseResponseModel<List<ProductListVM>>();
            response = await _productStock.List(searchVM);
            if (response.Data.IsNullOrEmpty())
            {
                response.Message = Messages.Common_No_Record_Found;
            }
            return response;
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<BaseResponseModel<bool>> Delete(ProductDeleteVM model)
        {
            var response = new BaseResponseModel<bool>();
            response = await _productStock.Delete(model);
            return response;
        }
    }
}
