
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopBridge.BuldingBlocks;
using ShopBridge.BuldingBlocks.AppEnums;
using ShopBridge.BuldingBlocks.Extensions;
using ShopBridge.Model;
using ShopBridge.Services.Common;
using ShopBridge.Services.Products.Brand;
using ShopBridge.ViewModels.Common;
using ShopBridge.ViewModels.Products;

namespace ShopBridge.Services.Products.Stock
{
    public class ProductStockService : IProductStock
    {
        #region Fields
        private readonly IProductBrand _productBrand;
        #endregion

        #region Ctor
        public ProductStockService(IProductBrand productBrand)
        {
            _productBrand = productBrand;
        }
        #endregion
        /// <summary>
        /// This method will return product details with tags and brands
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>product details view model</returns>
        public async Task<BaseResponseModel<ProductVM>> GetProductById(int id)
        {
            var response = new BaseResponseModel<ProductVM>();
            using (var context = new ShopBridgeEntities())
            {
                var product = await context.Products.FirstOrDefaultAsync(a => a.Id == id && a.IsDeleted == false);
                response.Data = EntityMapper.Map<ProductVM>(product);
                if (response.Data == null)
                {
                    response.Status = (int)ResponseStatus.Fail;
                    response.Message = Messages.Product_NotFound;
                    return response;
                }
                //GET PRODUCT TAGS
                response.Data.ProductTags = await context.ProductTags.Where(a => a.ProductId == id && a.IsActive == true && a.IsDeleted == false).Select(a => a.TagName).ToListAsync();
                //GET PRODUCT BRANDS
                //response.Data.Brands = _productBrand.GetProductBrands(id);

            }
            return response;
        }
        /// <summary>
        /// Save product
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return primary key of the product</returns>
        public async Task<BaseResponseModel<int>> SaveProduct(ProductStockVM model)
        {
            return model.Id == 0 ? await InsertProduct(model) : await UpdateProduct(model);
        }
        private async Task<BaseResponseModel<int>> InsertProduct(ProductStockVM model)
        {
            var response = new BaseResponseModel<int>();
            using (var context = new ShopBridgeEntities())
            {
                // CHECK PRODUCT BUSINESS VALIDATION HERE...
                var checkValidity = ValidateProduct(context, model);
                if (!checkValidity.IsSuccess)
                    return checkValidity;

                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var productEntity = EntityMapper.Map<Product>(model);
                        productEntity.IsDeleted = false;
                        productEntity.IsActive = true;
                        productEntity.CreatedDate = System.DateTime.Now;
                        context.Products.Add(productEntity);
                        await context.SaveChangesAsync();
                        model.Id = productEntity.Id;// GET NEWLY CREATED ID HERE....
                        //SAVE PRODUCT OTHER DETAILS
                        await SaveProductOtherDetails(context, model);

                        transaction.Commit();
                        response.Data = model.Id;
                        response.Message = Messages.Product_Saved_Successfully;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogError(ex);
                        response.Status = (int)ResponseStatus.Fail;
                        response.Message = ex.Message;
                        transaction.Rollback();
                    }
                }
            }
            return response;
        }

        private async Task<BaseResponseModel<int>> UpdateProduct(ProductStockVM model)
        {
            var response = new BaseResponseModel<int>();
            //CHECK PRODUCT EXISTS
            var product = await GetProductById(model.Id);
            if (product.Data == null)
            {
                response.Status = (int)ResponseStatus.Fail;
                response.Message = Messages.Product_NotFound;
                return response;
            }

            using (var context = new ShopBridgeEntities())
            {
                // CHECK PRODUCT BUSINESS VALIDATION HERE...
                var checkValidity = ValidateProduct(context, model);
                if (!checkValidity.IsSuccess)
                    return checkValidity;

                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var productEntity = EntityMapper.Map<Product>(model);
                        productEntity.IsDeleted = product.Data.IsDeleted;
                        productEntity.IsActive = product.Data.IsActive;
                        productEntity.CreatedBy = product.Data.CreatedBy;
                        productEntity.CreatedDate = product.Data.CreatedDate;
                        productEntity.ModifiedDate = System.DateTime.Now;
                        context.Products.Add(productEntity);
                        context.Entry(productEntity).State = System.Data.Entity.EntityState.Modified;
                        await context.SaveChangesAsync();
                        //SAVE PRODUCT OTHER DETAILS
                        await SaveProductOtherDetails(context, model);

                        transaction.Commit();
                        response.Data = model.Id;
                        response.Message = Messages.Product_Saved_Successfully;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.LogError(ex);
                        response.Status = (int)ResponseStatus.Fail;
                        response.Message = ex.Message;
                        transaction.Rollback();
                    }
                }
            }
            return response;
        }
        private async Task SaveProductOtherDetails(ShopBridgeEntities context, ProductStockVM model)
        {
            // INSERT PRODUCT TAGS
            await SaveProductTags(context, model);
            // SAVE PRODUCT BRAND MAPPING
            await SaveProductBrand(context, model);
            // SAVE PRODUCT STOCK MOVEMENT
            await SaveProductStockMovement(context, model);
        }
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BaseResponseModel<bool>> Delete(ProductDeleteVM model)
        {
            var response = new BaseResponseModel<bool> { Data = true };
            using (var context = new ShopBridgeEntities())
            {
                var paramProductId = new SqlParameter { ParameterName = "@ProductId", Value = model.Id };
                var paramDeletedBy = new SqlParameter { ParameterName = "@DeletedBy", Value = model.DeletedBy };
                response = await context.Database.SqlQuery<BaseResponseModel<bool>>("EXEC DeleteProduct @ProductId,@DeletedBy",
                        paramProductId, paramDeletedBy).FirstOrDefaultAsync();
            }
            return response;
        }
        /// <summary>
        /// Get list of products
        /// </summary>
        /// <param name="searchVM"></param>
        /// <returns>Products</returns>
        public async Task<BaseResponseModel<List<ProductListVM>>> List(ProductSearchVM searchVM)
        {
            //WE SHOULD PREFER COMPLEX SEARCHING WITH PROCEDURE INSTEAD OF LINQ
            var response = new BaseResponseModel<List<ProductListVM>>();
            using (var context = new ShopBridgeEntities())
            {
                var query = context.Products.AsQueryable();
                query = query.Where(p => !p.IsDeleted);
                if (searchVM != null)
                {
                    //ACTIVE ONLY
                    if (searchVM.OnlyActive == true)
                    {
                        query = query.Where(p => p.IsActive);
                    }
                    //FINISHED ONLY
                    if (searchVM.OnlyFinished == true)
                    {
                        query = query.Where(p => p.IsFinished == true);
                    }
                    // CHECK PRODUCT NAME
                    if (!string.IsNullOrWhiteSpace(searchVM.Name))
                    {
                        query = query.Where(p => p.Name.Contains(searchVM.Name));
                    }
                    //SEARCHING BY KEYWORD
                    if (!string.IsNullOrWhiteSpace(searchVM.Keywords))
                    {
                        query = from p in query
                                where (p.Name.Contains(searchVM.Keywords)) ||
                                      (p.SKU.Contains(searchVM.Keywords)) ||
                                      (p.ShortDescription.Contains(searchVM.Keywords)) ||
                                      (p.FullDescription.Contains(searchVM.Keywords))

                                select p;
                    }
                }
                query = query.OrderBy(p => p.Name);
                var products = new PagedList<Product>(query, searchVM);
                response.Data = EntityMapper.Map<List<ProductListVM>>(products.List);
                response.TotalItems = products.ItemCount;
                return response;
            }
        }

        #region PRRODUCT TAGS
        /// <summary>
        /// THIS METHOD WILL DELETE/SAVE THE PRODUCT TAGS
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        private async Task SaveProductTags(ShopBridgeEntities context, ProductStockVM model)
        {
            // RETURN IF NO TAG FOUND
            if (model.ProductTagsList.IsNullOrEmpty())
                return;

            // MANAGE REMOVED TAGS FROM USER AND DELETED IN DB
            var existingTags = context.ProductTags.Where(a => a.ProductId == model.Id && a.IsActive == true && a.IsDeleted == false).ToList();
            var deletedTags = existingTags.Where(a => !model.ProductTagsList.Contains(a.TagName)).ToList();
            deletedTags?.ForEach(a =>
            {
                a.IsActive = false;
                a.IsDeleted = true;
                a.ModifiedBy = model.ModifiedBy;
                a.ModifiedDate = DateTime.Now;
            });
            // FILTER NEW TAGS AND INSERT
            var newTags = model.ProductTagsList?.Except(existingTags.Select(a => a.TagName).ToArray()).ToList();
            var tags = new List<ProductTag>();
            foreach (var tagName in newTags)
            {
                tags.Add(new ProductTag
                {
                    ProductId = model.Id,
                    TagName = tagName,
                    IsDeleted = false,
                    IsActive = true,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = System.DateTime.Now
                });
            };
            context.ProductTags.AddRange(tags);
            await context.SaveChangesAsync();
        }
        #endregion

        #region SAVE PRODUCT BRAND
        private async Task SaveProductBrand(ShopBridgeEntities context, ProductStockVM model)
        {
            // WILL BE IMPLEMENTED
        }
        #endregion

        #region SAVE PRODUCT STOCK Movement
        private async Task SaveProductStockMovement(ShopBridgeEntities context, ProductStockVM model)
        {
            //WILL BE IMPLEMENTED
        }
        #endregion

        #region VALIDATIONS
        private BaseResponseModel<int> ValidateProduct(ShopBridgeEntities context, ProductStockVM model)
        {
            var response = new BaseResponseModel<int>();
            // CHECK SKU(STOCK KEEPING UNIT) AREADY EXISTS
            if (!string.IsNullOrEmpty(model.SKU) && context.Products.Any(a => a.SKU.ToLower().Trim() == model.SKU.ToLower().Trim() && (model.Id != a.Id) && a.IsDeleted == false))
            {
                response.Status = (int)ResponseStatus.Fail;
                response.Message = Messages.Product_SKU_Already_Exists;
            }
            // CHECK OTHER VALIDATION HERE...
            return response;
        }
        #endregion

    }
}
