using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.DBModels;
using Entities.DBModels.Discounts;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Discounts.RequestForms;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class DiscountsServicesDAL : IDiscountsServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public DiscountsServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }


        public async Task<List<DiscountEntity>> GetDiscountsListDAL(DiscountEntity FormData)
        {


            try
            {
                List<DiscountEntity> result = new List<DiscountEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.DiscountId > 0)
                    {
                        SearchParameters.Append("AND MTBL.DiscountId =  @0 ", FormData.DiscountId);
                    }

                    if (FormData.DiscountTypeId > 0)
                    {
                        SearchParameters.Append("AND dt.DiscountTypeId =  @0 ", FormData.DiscountTypeId);
                    }

                    if (FormData.IsActiveSelected != null && (FormData.IsActiveSelected == 1 || FormData.IsActiveSelected == 0))
                    {
                        SearchParameters.Append("AND dt.IsActive =  @0 ", FormData.IsActiveSelected);
                    }

                    if (!String.IsNullOrEmpty(FormData.Title))
                    {
                        SearchParameters.Append("AND MTBL.Title LIKE  @0", "%" + FormData.Title + "%");
                    }

                    if (!String.IsNullOrEmpty(FormData.CouponCode))
                    {
                        SearchParameters.Append("AND MTBL.CouponCode LIKE  @0", "%" + FormData.CouponCode + "%");
                    }


                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.* ,dt.DiscountTypeName , DUH.TotalUsage AS TotalUsage ")
                      .From(" Discounts MTBL")
                      .InnerJoin("DiscountTypes dt").On("MTBL.DiscountTypeId=dt.DiscountTypeId")
                      .Append("OUTER APPLY(")
                      .Append("select count(*) as TotalUsage from DiscountUsageHistory DUH where DUH.DiscountID=MTBL.DiscountID")
                      .Append(") DUH")
                      .Where("MTBL.DiscountTypeId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CreatedOn DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<DiscountEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<DiscountTypeEntity>> GetDiscountTypesListDAL(DiscountTypeEntity FormData)
        {

            List<DiscountTypeEntity> result = new List<DiscountTypeEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.DiscountTypeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.DiscountTypeId =  @0 ", FormData.DiscountTypeId);
                    }


                    if (!String.IsNullOrEmpty(FormData.DiscountTypeName))
                    {
                        SearchParameters.Append("AND MTBL.DiscountTypeName LIKE  @0", "%" + FormData.DiscountTypeName + "%");
                    }




                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" DiscountTypes MTBL")
                      .Where("MTBL.DiscountTypeId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.DiscountTypeId ASC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<DiscountTypeEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception ex)
                {

                    throw;
                }

            }

        }

        public async Task<List<DiscountProductsMappingEntity>> GetDiscountProductsMappingListDAL(DiscountProductsMappingEntity FormData)
        {

            List<DiscountProductsMappingEntity> result = new List<DiscountProductsMappingEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");




                    if (FormData.ProductId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ProductId =  @0", FormData.ProductId);
                    }


                    if (!String.IsNullOrEmpty(FormData.ProductName))
                    {
                        SearchParameters.Append("AND P.ProductName LIKE  @0", "%" + FormData.ProductName + "%");
                    }

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords, MTBL.DiscountProductMappingID, MTBL.DiscountID, MTBL.ProductID,P.ProductName")
                      .From(" DiscountProductsMapping MTBL")
                      .InnerJoin("Products p").On("MTBL.ProductID = p.ProductID")
                      .Where("MTBL.Discountid = @0", FormData.DiscountId)
                      .Append(SearchParameters)
                     .OrderBy("MTBL.DiscountProductMappingID ASC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<DiscountProductsMappingEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }


        public async Task<List<ProductEntity>> GetProductsListForDiscountDAL(ProductEntity FormData)
        {

            List<ProductEntity> result = new List<ProductEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductEntity>(@";EXEC [dbo].[Sp_GetProductsListForDiscount] @ProductId,@ProductName,@CategoryId,@PageNo,@PageSize",
                          new
                          {
                              ProductId = FormData.ProductId,
                              ProductName = FormData.ProductName,
                              CategoryId = FormData.CategoryId,
                              PageNo = FormData.PageNo,
                              PageSize = FormData.PageSize,

                          }).ToList();


                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }


        public async Task<ServicesResponse>? InsertUpdateDiscountDAL(DiscountRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    dbConnection.Open();

                    dbConnection.Execute("Sp_InsertUpdateDiscount",
                        new
                        {
                            DiscountId = FormData.DiscountId,
                            Title = FormData.Title,
                            DiscountTypeId = FormData.DiscountTypeId,
                            DiscountValueType = FormData.DiscountValueType,
                            DiscountValue = FormData.DiscountValue,
                            StartDate = FormData.StartDate,
                            EndDate = FormData.EndDate,
                            IsCouponCodeRequired = FormData.IsCouponCodeRequired,
                            CouponCode = FormData.CouponCode,
                            IsBoundToMaxQuantity =false,
                            MaxQuantity = 50000,
                            IsActive = FormData.IsActive,
                            DiscountAssociatedProductsJson = JsonConvert.SerializeObject(FormData.discountAssociatedProducts),
                            DiscountAssociatedCategoriesJson = JsonConvert.SerializeObject(FormData.discountAssociatedCategories),

                            BusnPartnerId = FormData.BusnPartnerId,
                        }
                        , commandType: CommandType.StoredProcedure);
                    dbConnection.Close();

                    result.PrimaryKeyValue = FormData.DiscountId;
                    result.Success = true;
                    result.ResponseMessage = "Saved Successfully!";

                    await Task.FromResult(result);
                 
                }

            }
            catch (Exception)
            {

                throw;
            }

            return result;

        }

        public async Task<DiscountEntity?> GetDiscountDetailByIdDAL(int DiscountId)
        {

            DiscountEntity? result = new DiscountEntity();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    var ppSql = PetaPoco.Sql.Builder.Select(@"  MTBL.*")
                      .From(" Discounts MTBL")
                      .Where("MTBL.Discountid = @0", DiscountId);
                    

                    result = context.Fetch<DiscountEntity>(ppSql)?.FirstOrDefault();

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<List<DiscountCategoryMappingEntity>> GetDiscountsMappedCategoriesListDAL(DiscountCategoryMappingEntity FormData)
        {

            List<DiscountCategoryMappingEntity> result = new List<DiscountCategoryMappingEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");




                    if (FormData.CategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CategoryId =  @0", FormData.CategoryId);
                    }


                    if (!String.IsNullOrEmpty(FormData.CategoryName))
                    {
                        SearchParameters.Append("AND P.CategoryName LIKE  @0", "%" + FormData.CategoryName + "%");
                    }

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords, MTBL.DiscountCategoryMappingID, MTBL.CategoryId, MTBL.DiscountId,P.Name as CategoryName")
                      .From(" DiscountCategoriesMapping MTBL")
                      .InnerJoin("ProductCategories p").On("MTBL.CategoryId = p.CategoryId")
                      .Where("MTBL.Discountid = @0", FormData.DiscountID)
                      .Append(SearchParameters)
                     .OrderBy("MTBL.DiscountCategoryMappingID ASC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<DiscountCategoryMappingEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

        public async Task<List<ProductCategoriesEntity>> GetCategoriesListForDiscountDAL(ProductCategoriesEntity FormData)
        {

            List<ProductCategoriesEntity> result = new List<ProductCategoriesEntity>();

            using (var context = _contextHelper.GetDataContextHelper())
            {
                try
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.CategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CategoryId =  @0 ", FormData.CategoryId);
                    }

                    if (FormData.CreatedBy != null && FormData.CreatedBy > 0)
                    {
                        SearchParameters.Append("AND MTBL.CreatedBy =  @0 ", FormData.CreatedBy);
                    }

                    if (FormData.ParentCategoryID > 0)
                    {
                        SearchParameters.Append("AND MTBL.ParentCategoryId =  @0 ", FormData.ParentCategoryID);
                    }


                    if (!String.IsNullOrEmpty(FormData.Name))
                    {
                        SearchParameters.Append("AND MTBL.Name LIKE  @0", "%" + FormData.Name + "%");
                    }

                  

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*, par.Name AS ParentCategoryName")
                      .From(" ProductCategories MTBL")
                      .LeftJoin("ProductCategories par").On("MTBL.ParentCategoryID = par.CategoryId")
                      .Where("MTBL.CategoryId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CategoryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ProductCategoriesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
                catch (Exception)
                {

                    throw;
                }

            }

        }

    }
}
