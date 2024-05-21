using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.CommonModels;
using Entities.DBModels;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class ProductsCatalogServicesDAL : IProductsCatalogServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public ProductsCatalogServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }


        public async Task<List<ProductCategoriesEntity>> GetProductCategoriesListDAL(ProductCategoriesEntity FormData)
        {


            try
            {
                List<ProductCategoriesEntity> result = new List<ProductCategoriesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (FormData.CategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CategoryId =  @0 ", FormData.CategoryId);
                    }



                    if (!String.IsNullOrEmpty(FormData.Name))
                    {
                        SearchParameters.Append("AND MTBL.Name LIKE  @0", "%" + FormData.Name + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecords, MTBL.*, ParentCatg.Name as ParentCategoryName, ATC.AttachmentURL as CategoryImagePath")
                      .From(" ProductCategories MTBL")
                      .LeftJoin("ProductCategories ParentCatg").On("ParentCatg.CategoryId = MTBL.ParentCategoryID")
                      .LeftJoin("Attachments ATC").On("ATC.AttachmentID = MTBL.AttachmentID")
                      .Where("MTBL.CategoryId IS NOT NULL")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CategoryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ProductCategoriesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse>? InsertUpdateProductCategoryDAL(ProductCategoriesRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.CategoryId > 0)
                    {
                        var updatedRecords = context.Update("ProductCategories", "CategoryId",
                        new
                        {
                            CategoryId = FormData.CategoryId,
                            Name = FormData.Name,
                            IsActive = FormData.IsActive,
                            ParentCategoryID = FormData.ParentCategoryID,
                            AttachmentId = FormData.AttachmentId,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.CategoryId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.CategoryId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var CategoryId = context.Insert("ProductCategories", "CategoryId", true,
                           new
                           {

                               Name = FormData.Name,
                               IsActive = FormData.IsActive,
                               ParentCategoryID = FormData.ParentCategoryID,
                               AttachmentId = FormData.AttachmentId,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (CategoryId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(CategoryId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<ProductAttributeMappingEntity>> GetProductsMappedAttributesListDAL(ProductAttributeMappingEntity FormData)
        {


            try
            {
                List<ProductAttributeMappingEntity> result = new List<ProductAttributeMappingEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductAttributeMappingEntity>(@";EXEC [dbo].[SP_GetProductMappedAttributesById] @ProductId, @PageNo, @PageSize",
                          new
                          {
                              ProductId = FormData.ProductID,
                              PageNo = FormData.PageNo,
                              PageSize = FormData.PageSize

                          }).ToList();


                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<List<ManufacturerEntity>> GetManufacturerListDAL(ManufacturerEntity FormData)
        {


            try
            {
                List<ManufacturerEntity> result = new List<ManufacturerEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.ManufacturerId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ManufacturerId =  @0 ", FormData.ManufacturerId);
                    }


                    if (!String.IsNullOrEmpty(FormData.Name))
                    {
                        SearchParameters.Append("AND MTBL.Name LIKE  @0", "%" + FormData.Name + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" Manufacturers MTBL")
                      .Where("MTBL.ManufacturerId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.CreatedOn DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ManufacturerEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ShippingMethodEntity>> GetShippingMethodsListDAL(ShippingMethodEntity FormData)
        {


            try
            {
                List<ShippingMethodEntity> result = new List<ShippingMethodEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.ShippingMethodId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ShippingMethodId =  @0 ", FormData.ShippingMethodId);
                    }

                    if (!String.IsNullOrEmpty(FormData.MethodName))
                    {
                        SearchParameters.Append("AND MTBL.MethodName LIKE  @0", "%" + FormData.MethodName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ShippingMethods MTBL")
                      .Where("MTBL.ShippingMethodID is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.ShippingMethodID DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ShippingMethodEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<WarehouseEntity>> GetWarehousesListDAL(WarehouseEntity FormData)
        {


            try
            {
                List<WarehouseEntity> result = new List<WarehouseEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.WarehouseId > 0)
                    {
                        SearchParameters.Append("AND MTBL.WarehouseId =  @0 ", FormData.WarehouseId);
                    }

                    if (!String.IsNullOrEmpty(FormData.WarehouseName))
                    {
                        SearchParameters.Append("AND MTBL.WarehouseName LIKE  @0", "%" + FormData.WarehouseName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" Warehouses MTBL")
                      .Where("MTBL.WarehouseId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.WarehouseId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<WarehouseEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ProductAttributeEntity>> GetProductAttributesListDAL(ProductAttributeEntity FormData)
        {


            try
            {
                List<ProductAttributeEntity> result = new List<ProductAttributeEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.ProductAttributeId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ProductAttributeID =  @0 ", FormData.ProductAttributeId);
                    }

                    if (!String.IsNullOrEmpty(FormData.AttributeName))
                    {
                        SearchParameters.Append("AND MTBL.AttributeName LIKE  @0", "%" + FormData.AttributeName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ProductAttributes MTBL")
                      .Where("MTBL.ProductAttributeID is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.ProductAttributeID DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ProductAttributeEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<HtmlDropDownRemoteData>> GetProductAttributeValuesByAttributeIDDAL(int ProductAttributeId)
        {


            try
            {
                List<HtmlDropDownRemoteData> result = new List<HtmlDropDownRemoteData>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<HtmlDropDownRemoteData>(@";EXEC [dbo].[SP_GetProductAttributeDropdownData] @ProductAttributeId",
                           new
                           {
                               ProductAttributeId = ProductAttributeId
                           }).ToList();

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ProductTagsEntity>> GetProductTagsDAL(ProductTagsEntity FormData)
        {


            try
            {
                List<ProductTagsEntity> result = new List<ProductTagsEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.TagId > 0)
                    {
                        SearchParameters.Append("AND MTBL.TagId =  @0 ", FormData.TagId);
                    }

                    if (!String.IsNullOrEmpty(FormData.TagName))
                    {
                        SearchParameters.Append("AND MTBL.TagName LIKE  @0", "%" + FormData.TagName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" ProductTags MTBL")
                      .Where("MTBL.TagId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.TagId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ProductTagsEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse>? InsertUpdateProductTagDAL(ProductTagRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.TagId > 0)
                    {
                        var updatedRecords = context.Update("ProductTags", "TagId",
                        new
                        {
                            TagId = FormData.TagId,
                            TagName = FormData.TagName,
                            IsActive = FormData.IsActive,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.TagId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.TagId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var TagId = context.Insert("ProductTags", "TagId", true,
                           new
                           {

                               TagName = FormData.TagName,
                               IsActive = FormData.IsActive,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (TagId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(TagId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ServicesResponse>? InsertUpdateProductDAL(CreateProductRequestFormInternal FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {

                    var updatedRecords = 0;
                    dbConnection.Open();

                    dbConnection.Execute("SP_CreateUpdateProduct",
                        new
                        {
                            ProductId = FormData?.createProductRequestForm?.ProductId,
                            ProductName = FormData?.createProductRequestForm?.ProductName,
                            ShortDescription = FormData?.createProductRequestForm?.ShortDescription,
                            FullDescription = FormData?.createProductRequestForm?.FullDescription,
                            ManufacturerId = FormData?.createProductRequestForm?.ManufacturerId,
                            VendorId = FormData?.createProductRequestForm?.VendorId,
                            TaxRuleId = FormData?.createProductRequestForm?.TaxRuleId,
                            IsActive = FormData?.createProductRequestForm?.IsActive,
                            MarkAsNew = FormData?.createProductRequestForm?.MarkAsNew,
                            AllowCustomerReviews = FormData?.createProductRequestForm?.AllowCustomerReviews,
                            Sku = FormData?.createProductRequestForm?.Sku,
                            Price = FormData?.createProductRequestForm?.Price,
                            OldPrice = FormData?.createProductRequestForm?.OldPrice,
                            IsDiscountAllowed = FormData?.createProductRequestForm?.IsDiscountAllowed,
                            IsShippingFree = FormData?.createProductRequestForm?.IsShippingFree,
                            ShippingCharges = FormData?.createProductRequestForm?.ShippingCharges,
                            EstimatedShippingDays = FormData?.createProductRequestForm?.EstimatedShippingDays,
                            IsReturnAble = FormData?.createProductRequestForm?.IsReturnAble,
                            SelectedCategoriesJson = FormData?.createProductRequestForm?.SelectedCategoryIdsJson,
                            SelectedTagsJson = FormData?.createProductRequestForm?.SelectedTagsJson,
                            SelectedShippingMethodsJson = FormData?.createProductRequestForm?.SelectedShippingMethodsJson,
                            ProductAttributesJson = FormData?.createProductRequestForm?.ProductAttributesJson,
                            ProductImagesJson = FormData?.ProductImagesJson,
                            BusnPartnerId = FormData?.BusnPartnerId,
                        }
                        , commandType: CommandType.StoredProcedure);
                    dbConnection.Close();


                    result.PrimaryKeyValue = FormData?.createProductRequestForm?.ProductId;
                    result.Success = true;
                    result.ResponseMessage = "Saved Successfully!";



                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<ProductEntity>> GetProductsListDAL(ProductEntity FormData)
        {


            try
            {
                List<ProductEntity> result = new List<ProductEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductEntity>(@";EXEC [dbo].[SP_GetProductsList] @ProductId,@ProductName,@CategoryId,@IsActive,@FromDate,@ToDate,@PageNo,@PageSize , @CreatedBy",
                          new
                          {
                              ProductId = FormData.ProductId,
                              ProductName = FormData.ProductName,
                              CategoryId = FormData.CategoryId,
                              IsActive = FormData.IsActive,
                              FromDate = FormData.FromDate,
                              ToDate = FormData.ToDate,
                              PageNo = FormData.PageNo,
                              PageSize = FormData.PageSize,
                              CreatedBy = FormData.CreatedBy,

                          }).ToList();


                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ProductEntity?> GetProductDetailsByIdDAL(int ProductId)
        {


            try
            {
                ProductEntity? result = new ProductEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductEntity>(@";EXEC [dbo].[SP_GetProductDetailsById] @ProductId",
                          new
                          {
                              ProductId = ProductId

                          }).FirstOrDefault();


                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<List<ProductPicturesMappingEntity>> GetProductsMappedImagesListDAL(ProductPicturesMappingEntity FormData)
        {


            try
            {
                List<ProductPicturesMappingEntity> result = new List<ProductPicturesMappingEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");




                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords, ppm.ProductPictureMappingID, ppm.PictureId, a.AttachmentURL, ppm.ColorID")
                      .From(" ProductPicturesMapping ppm")
                      .InnerJoin("Attachments a").On("ppm.PictureID = a.AttachmentID")
                      .Where("ppm.ProductId =@0", FormData.ProductId)
                      .Append(SearchParameters)
                     .OrderBy("ppm.ProductId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ProductPicturesMappingEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<List<ColorEntity>> GetColorsListDAL(ColorEntity FormData)
        {


            try
            {
                List<ColorEntity> result = new List<ColorEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.ColorId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ColorId =  @0 ", FormData.ColorId);
                    }


                    if (!String.IsNullOrEmpty(FormData.ColorName))
                    {
                        SearchParameters.Append("AND MTBL.ColorName LIKE  @0", "%" + FormData.ColorName + "%");
                    }

                    //if (!String.IsNullOrEmpty(FormData.FromDate))
                    //{
                    //    SearchParameters.Append("AND Cast(MTBL.CreatedOn AS Date)>=@0", FormData.FromDate);
                    //}

                    //if (!String.IsNullOrEmpty(FormData.ToDate))
                    //{
                    //    SearchParameters.Append("AND Cast(MTBL.CreatedOn AS Date)<=@0", FormData.ToDate);
                    //}

                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" Colors MTBL")
                      .Where("MTBL.ColorID is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.ColorID DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<ColorEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<ServicesResponse>? UpdateProductImgColorMappingDAL(ProductImgColorMappingRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {



                    dbConnection.Open();

                    var resultIEnumerable = await dbConnection.ExecuteAsync(@"UPDATE TOP(30) ProductPicturesMapping
                        SET ColorID = JsonItem.colorId
                        FROM OPENJSON(@productMappedColorsJson)
		                    WITH (
			                    productPictureMappingId	INT '$.productPictureMappingId' ,
			                    colorId INT '$.colorId'
	
		                    )
	                    JsonItem
                        WHERE ProductPicturesMapping.ProductPictureMappingID = JsonItem.productPictureMappingId AND ProductPicturesMapping.ProductID = @ProductId 
                        AND JsonItem.colorId IS NOT NULL AND JsonItem.colorId != 0",
                                              new
                                              {
                                                  productMappedColorsJson = FormData.productMappedColorsJson,
                                                  ProductId = FormData.ProductId,

                                              }
                          , commandType: CommandType.Text);
                    dbConnection.Close();


                    result.PrimaryKeyValue = Convert.ToInt32(FormData.ProductId);
                    result.Success = true;
                    result.ResponseMessage = "Saved Successfully!";

                    await Task.FromResult(result);
                    return result;



                }



            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ServicesResponse>? InsertUpdateManufacturerDAL(ManufacturerRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.ManufacturerID > 0)
                    {
                        var updatedRecords = context.Update("Manufacturers", "ManufacturerID",
                        new
                        {
                            ManufacturerID = FormData.ManufacturerID,
                            Name = FormData.Name,
                            IsActive = FormData.IsActive,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.ManufacturerID);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.ManufacturerID;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var ManufacturerID = context.Insert("Manufacturers", "ManufacturerID", true,
                           new
                           {

                               Name = FormData.Name,
                               IsActive = FormData.IsActive,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (ManufacturerID != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(ManufacturerID);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ServicesResponse>? InsertUpdateColorDAL(ColorRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.ColorId > 0)
                    {
                        var updatedRecords = context.Update("Colors", "ColorId",
                        new
                        {
                            ColorId = FormData.ColorId,
                            ColorName = FormData.ColorName,
                            HexCode = FormData.HexCode,
                            IsActive = FormData.IsActive,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.ColorId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.ColorId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var ColorId = context.Insert("Colors", "ColorId", true,
                           new
                           {

                               ColorName = FormData.ColorName,
                               HexCode = FormData.HexCode,
                               IsActive = FormData.IsActive,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (ColorId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(ColorId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<InventoryMainEntity>> GetInventoryListDAL(InventoryMainEntity FormData)
        {


            try
            {
                List<InventoryMainEntity> result = new List<InventoryMainEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.InventoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.InventoryId =  @0 ", FormData.InventoryId);
                    }

                    if (FormData.ProductId > 0)
                    {
                        SearchParameters.Append("AND MTBL.ProductId =  @0 ", FormData.ProductId);
                    }


                    if (!String.IsNullOrEmpty(FormData.ProductName))
                    {
                        SearchParameters.Append("AND PRD.ProductName LIKE  @0", "%" + FormData.ProductName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*, PRD.ProductName, IMG.AttachmentURL AS ProductDefaultImgPath")
                      .From(" InventoryMain MTBL")
                      .InnerJoin("Products PRD").On(" PRD.ProductID = MTBL.ProductId")
                      .Append(@"OUTER APPLY(
                        SELECT TOP 1 ATC.AttachmentURL FROM ProductPicturesMapping PPM 
                        INNER JOIN Attachments ATC ON ATC.AttachmentID = PPM.PictureID
                        WHERE PPM.ProductID =  PRD.ProductID
                        ) IMG")
                        .Where("MTBL.InventoryId is not null")
                        .Append(SearchParameters)
                     .OrderBy("MTBL.InventoryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<InventoryMainEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ProductMappedAttributesForInventory>> GetProductMappedAttributesForInventoryDAL(int ProductId)
        {


            try
            {
                List<ProductMappedAttributesForInventory> result = new List<ProductMappedAttributesForInventory>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductMappedAttributesForInventory>(@";EXEC [dbo].[SP_GetProductMappedAttributesForInventory] @ProductId",
                          new
                          {
                              ProductId = ProductId


                          }).ToList();


                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<InventoryMainEntity?> GenInventoryMainDetailByIdDAL(int InventoryId)
        {


            try
            {
                InventoryMainEntity? result = new InventoryMainEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");






                    var ppSql = PetaPoco.Sql.Builder.Select(@"MTBL.*, PRD.ProductName, IMG.AttachmentURL AS ProductDefaultImgPath")
                      .From(" InventoryMain MTBL")
                      .InnerJoin("Products PRD").On(" PRD.ProductID = MTBL.ProductId")
                      .Append(@"OUTER APPLY(
                        SELECT TOP 1 ATC.AttachmentURL FROM ProductPicturesMapping PPM 
                        INNER JOIN Attachments ATC ON ATC.AttachmentID = PPM.PictureID
                        WHERE PPM.ProductID =  PRD.ProductID
                        ) IMG")
                        .Where("MTBL.InventoryId =@0", InventoryId);
                       

                    result = context.Fetch<InventoryMainEntity>(ppSql)?.FirstOrDefault();

                    await Task.FromResult(result);
                    return result;

                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<ServicesResponse>? InsertUpdateInventoryMainDAL(InventoryMainRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.InventoryId > 0)
                    {
                        var updatedRecords = context.Update("InventoryMain", "InventoryId",
                        new
                        {
                            InventoryId = FormData.InventoryId,
                            InventoryMethodId = FormData.InventoryMethodId,
                            WarehouseId = FormData.WarehouseId,
                            StockQuantity = FormData.StockQuantity,
                            OrderMinimumQuantity = FormData.OrderMinimumQuantity,
                            OrderMaximumQuantity = FormData.OrderMaximumQuantity,
                            SellStartDatetimeUTC = FormData.SellStartDatetimeUTC,
                            SellEndDatetimeUTC = FormData.SellEndDatetimeUTC,
                            IsBoundToStockQuantity = FormData.IsBoundToStockQuantity,
                            DisplayStockQuantity = FormData.DisplayStockQuantity,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.InventoryId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.InventoryId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var InventoryId = context.Insert("InventoryMain", "InventoryId", true,
                           new
                           {

                               InventoryMethodId = FormData.InventoryMethodId,
                               WarehouseId = FormData.WarehouseId,
                               StockQuantity = FormData.StockQuantity,
                               OrderMinimumQuantity = FormData.OrderMinimumQuantity,
                               OrderMaximumQuantity = FormData.OrderMaximumQuantity,
                               SellStartDatetimeUTC = FormData.SellStartDatetimeUTC,
                               SellEndDatetimeUTC = FormData.SellEndDatetimeUTC,
                               IsBoundToStockQuantity = FormData.IsBoundToStockQuantity,
                               DisplayStockQuantity = FormData.DisplayStockQuantity,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (InventoryId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(InventoryId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<InventoryMethodsEntity>> GetInventoryMethodsListDAL(InventoryMethodsEntity FormData)
        {


            try
            {
                List<InventoryMethodsEntity> result = new List<InventoryMethodsEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");



                    if (FormData.InventoryMethodId > 0)
                    {
                        SearchParameters.Append("AND MTBL.InventoryMethodId =  @0 ", FormData.InventoryMethodId);
                    }

                    if (!String.IsNullOrEmpty(FormData.InventoryMethodName))
                    {
                        SearchParameters.Append("AND MTBL.InventoryMethodName LIKE  @0", "%" + FormData.InventoryMethodName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@" COUNT(*) OVER () as TotalRecords,MTBL.*")
                      .From(" InventoryMethods MTBL")
                      .Where("MTBL.InventoryMethodId is not null")
                      .Append(SearchParameters)
                     .OrderBy("MTBL.InventoryMethodId ASC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<InventoryMethodsEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;


                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<ServicesResponse>? InsertUpdateProductMappedAttributesForInventoryDAL(ProductMappedAttributesForInventory FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.InventoryItemId > 0)
                    {
                        var updatedRecords = context.Update("InventoryItems", "InventoryItemId",
                        new
                        {
                            InventoryItemId = FormData.InventoryItemId,
                            ProductAttributeMappingID = FormData.ProductAttributeMappingID,
                       
                      
                       
                            InventoryId = FormData.InventoryId,
                            ProductId = FormData.ProductId,
                            SellStartDatetimeUTC = FormData.SellStartDatetimeUTC,
                            SellEndDatetimeUTC = FormData.SellEndDatetimeUTC,
                            WarehouseId = FormData.WarehouseId,
                            InventoryMethodId = FormData.InventoryMethodId,
                            StockQuantity = FormData.StockQuantity,
                            IsBoundToStockQuantity = FormData.IsBoundToStockQuantity,
                            DisplayStockQuantity = FormData.DisplayStockQuantity,
                            OrderMinimumQuantity = FormData.OrderMinimumQuantity,
                            OrderMaximumQuantity = FormData.OrderMaximumQuantity,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.InventoryItemId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.InventoryItemId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var InventoryItemId = context.Insert("InventoryItems", "InventoryItemId", true,
                           new
                           {

                               ProductAttributeMappingID = FormData.ProductAttributeMappingID,
                      
                            
                           
                               InventoryId = FormData.InventoryId,
                               ProductId = FormData.ProductId,
                               SellStartDatetimeUTC = FormData.SellStartDatetimeUTC,
                               SellEndDatetimeUTC = FormData.SellEndDatetimeUTC,
                               WarehouseId = FormData.WarehouseId,
                               InventoryMethodId = FormData.InventoryMethodId,
                               StockQuantity = FormData.StockQuantity,
                               IsBoundToStockQuantity = FormData.IsBoundToStockQuantity,
                               DisplayStockQuantity = FormData.DisplayStockQuantity,
                               OrderMinimumQuantity = FormData.OrderMinimumQuantity,
                               OrderMaximumQuantity = FormData.OrderMaximumQuantity,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (InventoryItemId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(InventoryItemId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ServicesResponse>? InsertUpdateWarehouseDAL(WarehouseRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    if (FormData.WarehouseId > 0)
                    {
                        var updatedRecords = context.Update("Warehouses", "WarehouseId",
                        new
                        {
                            WarehouseId = FormData.WarehouseId,
                            WarehouseName = FormData.WarehouseName,
                            IsActive = FormData.IsActive,
                            ModifiedOn = DateTime.Now,
                            ModifiedBy = FormData.BusnPartnerId,
                        },
                        FormData.WarehouseId);

                        if (updatedRecords > 0)
                        {

                            result.PrimaryKeyValue = FormData.WarehouseId;
                            result.Success = true;
                            result.ResponseMessage = "Updated Successfully!";
                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not updated successfully!";
                        }

                    }
                    else
                    {
                        //-- Here return primary key
                        var WarehouseId = context.Insert("Warehouses", "WarehouseId", true,
                           new
                           {

                               WarehouseName = FormData.WarehouseName,
                               IsActive = FormData.IsActive,
                               CreatedOn = DateTime.Now,
                               CreatedBy = FormData.BusnPartnerId,
                           }
                            );

                        if (WarehouseId != null)
                        {


                            result.PrimaryKeyValue = Convert.ToInt32(WarehouseId);
                            result.Success = true;
                            result.ResponseMessage = "Saved Successfully!";

                        }
                        else
                        {
                            result.Success = false;
                            result.ResponseMessage = "Not saved successfully!";

                        }

                    }

                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception)
            {

                throw;
            }

        }

      
    }
}
