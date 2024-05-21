using DAL.DBContext;
using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.CashierMain;
using Entities.ModuleSpecificModels.CashierMain;
using Helpers.ApiHelpers;
using Helpers.CommonHelpers.Enums;
using Helpers.ConversionHelpers.IServices;
using Irony.Parsing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ConversionHelpers.Services
{
    public class CalculationHelper : ICalculationHelper
    {

        private readonly IConfiguration _configuration;
        private readonly ICashierMainServicesDAL _cashierMainServicesDAL;

        public CalculationHelper(IConfiguration configuration,  ICashierMainServicesDAL cashierMainServicesDAL)
        {
            _configuration = configuration;
            _cashierMainServicesDAL = cashierMainServicesDAL;   
        }

       


        public async Task<List<ProductPointOfSaleEntity>?> CalculateDiscountsForProducts(List<ProductPointOfSaleEntity>? data)
        {
            List<ProductPointOfSaleEntity>? result = new List<ProductPointOfSaleEntity>();

            
            try
            {
                List<ProductPointOfSaleEntity>? ApiProductList = new List<ProductPointOfSaleEntity>();
                ApiProductList = data;

                var productIdsList = data?.Select(x => x.ProductId)?.ToList();
                if (productIdsList == null)
                {
                    productIdsList = new List<int>();
                }

                List<DiscountedProductsTemp>? tempDiscountedProducts = new List<DiscountedProductsTemp>();

                //--Just create dummy list and set product id in the list tempDiscountedProducts
                foreach (var productId in productIdsList)
                {
                    tempDiscountedProducts.Add(new DiscountedProductsTemp
                    {
                        ProductId = productId,
                        DiscountId = 0,
                        DiscountedPrice = 0,
                        ProductActualPrice = 0,
                        IsDiscountCalculated = false,
                        CouponCode = ""
                    });
                }

                #region product based discount info
                //--Get product based discount info
                string? ProductIdsForProductBased = string.Join(",", tempDiscountedProducts?.Where(x=>x.IsDiscountCalculated == false && x.DiscountedPrice == 0)?.Select(p=>p.ProductId)?.ToList() ?? new List<int>()); 
                List<ProductBasedDiscountInfo>? productBasedDiscountInfo = await _cashierMainServicesDAL.GetProductDiscountInfoProductBasedDAL(ProductIdsForProductBased);
                if (productBasedDiscountInfo != null && productBasedDiscountInfo.Count() > 0)
                {
                    // Perform the update operation on product based
                    if (tempDiscountedProducts != null)
                    {
                        foreach (var tempProduct in tempDiscountedProducts?.Where(p => p.IsDiscountCalculated == false))
                        {
                            var cteProduct = productBasedDiscountInfo?.FirstOrDefault(p => p.ProductId == tempProduct.ProductId);
                            if (cteProduct != null)
                            {
                                tempProduct.DiscountedPrice = (cteProduct.Price) - (cteProduct.TotalDiscount);
                                tempProduct.ProductActualPrice = cteProduct.Price;
                                tempProduct.IsDiscountCalculated = true;
                                tempProduct.DiscountId = tempProduct.DiscountId;
                                tempProduct.CouponCode = tempProduct.CouponCode;
                            }
                        }
                    }
                 
                }
                #endregion

                #region category based discount info
                //--Get categories based discount info
                string? ProductIdsForCategoryBased = string.Join(",", tempDiscountedProducts?.Where(x => x.IsDiscountCalculated == false && x.DiscountedPrice == 0)?.Select(p => p.ProductId)?.ToList() ?? new List<int>());
                List<CategoryBasedDiscountInfo>? categoryBasedDiscountInfos = await _cashierMainServicesDAL.GetProductDiscountInfoCategoryBasedDAL(ProductIdsForProductBased);
                if (categoryBasedDiscountInfos != null && categoryBasedDiscountInfos.Count() > 0)
                {
                    // Perform the update operation on category based
                    if (tempDiscountedProducts != null)
                    {
                        foreach (var tempProduct in tempDiscountedProducts?.Where(p => p.IsDiscountCalculated == false))
                        {
                            var cteCategory = categoryBasedDiscountInfos?.FirstOrDefault(p => p.ProductId == tempProduct.ProductId);
                            if (cteCategory != null)
                            {
                                tempProduct.DiscountedPrice = (cteCategory.Price) - (cteCategory.TotalDiscount);
                                tempProduct.ProductActualPrice = cteCategory.Price;
                                tempProduct.IsDiscountCalculated = true;
                                tempProduct.DiscountId = tempProduct.DiscountId;
                                tempProduct.CouponCode = tempProduct.CouponCode;
                            }
                        }
                    }
                    
                }
                #endregion

                //--Create final output for response
                var ApiProductListFurtherCalculation = tempDiscountedProducts;
                if (ApiProductListFurtherCalculation?.Count() > 0)
                {
                    foreach (var discountProduct in ApiProductListFurtherCalculation)
                    {
                        if (discountProduct != null)
                        {
                            ApiProductList?.Where(p => p.ProductId == discountProduct.ProductId).Select(prdObject => { prdObject.DiscountId = discountProduct.DiscountId; return prdObject; }).ToList();
                            ApiProductList?.Where(p => p.ProductId == discountProduct.ProductId).Select(prdObject => { prdObject.DiscountedPrice = discountProduct.DiscountedPrice; return prdObject; }).ToList();
                            ApiProductList?.Where(p => p.ProductId == discountProduct.ProductId).Select(prdObject => { prdObject.IsDiscountCalculated = discountProduct.IsDiscountCalculated; return prdObject; }).ToList();
                            ApiProductList?.Where(p => p.ProductId == discountProduct.ProductId).Select(prdObject => { prdObject.CouponCode = discountProduct.CouponCode; return prdObject; }).ToList();

                        }

                    }
                    result = ApiProductList;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;
        }



    }
}
