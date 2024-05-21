using DAL.DBContext;
using DAL.Repository.IServices;
using Dapper;
using Entities.DBModels;
using Entities.DBModels.CashierMain;
using Entities.DBModels.SalesManagement;
using Entities.ModuleSpecificModels.CashierMain;
using Entities.ModuleSpecificModels.CashierMain.RequestForms;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Entities.ModuleSpecificModels.Users.RequestForms;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DAL.Repository.Services
{
    public class CashierMainServicesDAL : ICashierMainServicesDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;
        private readonly IProductsCatalogServicesDAL _productsCatalogServicesDAL;

        //--Constructor of the class
        public CashierMainServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper,
            IProductsCatalogServicesDAL productsCatalogServicesDAL)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
            _productsCatalogServicesDAL = productsCatalogServicesDAL;
        }

        public async Task<List<PointOfSaleCategoriesEntity>> GetPointOfSaleCategoriesDAL(PointOfSaleCategoriesEntity FormData)
        {


            try
            {
                List<PointOfSaleCategoriesEntity> result = new List<PointOfSaleCategoriesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (FormData.CategoryId > 0)
                    {
                        SearchParameters.Append("AND MTBL.CategoryId =  @0 ", FormData.CategoryId);
                    }



                    if (!String.IsNullOrEmpty(FormData.CategoryName))
                    {
                        SearchParameters.Append("AND MTBL.Name LIKE  @0", "%" + FormData.CategoryName + "%");
                    }



                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(*) OVER () as TotalRecords, MTBL.CategoryId,   MTBL.Name as CategoryName, COUNT(pc.ProductId) AS TotalProducts, ATC.AttachmentURL as CategoryImagePath")
                      .From(" ProductCategories MTBL")
                      .LeftJoin("ProductsCategoriesMapping pc").On("MTBL.CategoryId = pc.CategoryId")
                      .LeftJoin("Attachments ATC").On("ATC.AttachmentID = MTBL.AttachmentID")
                      .Where("MTBL.IsActive IS NOT NULL AND MTBL.IsActive = 1")
                      .Append(SearchParameters)
                      .GroupBy("MTBL.CategoryId, MTBL.name, ATC.AttachmentURL")
                     .OrderBy("MTBL.CategoryId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
	                FETCH NEXT @1 ROWS ONLY", FormData.PageNo, FormData.PageSize);

                    result = context.Fetch<PointOfSaleCategoriesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<ProductPointOfSaleEntity>> GetPointOfSaleProductsDAL(ProductPointOfSaleEntity FormData)
        {


            try
            {
                List<ProductPointOfSaleEntity> result = new List<ProductPointOfSaleEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductPointOfSaleEntity>(@";EXEC [dbo].[SP_GetPointOfSaleProducts] @ProductName, @CategoryId, @ManufacturerId, @OrderByColumnName, @PageNo, @PageSize",
                          new
                          {

                              ProductName = FormData.ProductName,
                              CategoryId = FormData.CategoryId,
                              ManufacturerId = FormData.ManufacturerId,
                              OrderByColumnName = FormData.OrderByColumnName,
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

        public async Task<List<ProductBasedDiscountInfo>> GetProductDiscountInfoProductBasedDAL(string? ProductIDs)
        {
            try
            {
                List<ProductBasedDiscountInfo> result = new List<ProductBasedDiscountInfo>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"ProductId , DMP.DiscountId  ,  DMP.CouponCode, DMP.Price, DMP.TotalDiscount, DMP.TotalUsage")
                      .From("Products PRD")
                      .Append(@"OUTER APPLY (
			            SELECT TOP 1 DC.DiscountID , DC.CouponCode  , P.Price ,(IIF(DC.DiscountValueType=1, DC.DiscountValue , (DC.DiscountValue/100) * P.Price)) AS TotalDiscount,
			            DUH.TotalUsage
			            fROM DiscountProductsMapping DPM
			            INNER JOIN Discounts DC ON DC.DiscountID =  DPM.DiscountID AND DC.IsActive =1 AND DC.IsCouponCodeRequired=0 AND DC.StartDate < = GETDATE() AND DC.EndDate >= GETDATE()
			            OUTER APPLY (
				            SELECT COUNT(*) AS TotalUsage FROM DiscountUsageHistory DUH WHERE DC.DiscountID=DUH.DiscountID
			            )DUH
			            INNER JOIN Products P ON P.ProductID =  DPM.ProductId AND P.IsDiscountAllowed = 1
			            WHERE DPM.ProductID=PRD.ProductID 
			            AND (IIF(DC.DiscountValueType=1, DC.DiscountValue , (DC.DiscountValue/100) * P.Price)) > 0
			            AND  DUH.TotalUsage < IIF(DC.IsBoundToMaxQuantity=1, DC.MaxQuantity , 99999)  -- 99999 An example value because it should not go more than this value
		                )DMP")
                      .Where("prd.ProductID in (select value from string_split(@0, ',')) and (DMP.TotalDiscount > 0  AND DMP.TotalDiscount < DMP.Price )", ProductIDs);

                    result = context.Fetch<ProductBasedDiscountInfo>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<CategoryBasedDiscountInfo>> GetProductDiscountInfoCategoryBasedDAL(string? ProductIDs)
        {
            try
            {
                List<CategoryBasedDiscountInfo> result = new List<CategoryBasedDiscountInfo>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"ProductId , DMP.DiscountId  ,  DMP.CouponCode, DMP.Price, DMP.TotalDiscount, DMP.TotalUsage")
                      .From("Products PRD")
                      .Append(@"OUTER APPLY (
			            SELECT TOP 1 DC.DiscountID , DC.CouponCode  , P.Price ,(IIF(DC.DiscountValueType=1, DC.DiscountValue , (DC.DiscountValue/100) * P.Price)) AS TotalDiscount ,
			            DUH.TotalUsage
			            fROM DiscountCategoriesMapping DCM
			            INNER JOIN Discounts DC ON DC.DiscountID =  DCM.DiscountID AND DC.IsActive =1 AND DC.IsCouponCodeRequired=0 AND DC.StartDate < = GETDATE()  AND DC.EndDate >= GETDATE()
			            OUTER APPLY (
				            SELECT COUNT(*) AS TotalUsage FROM DiscountUsageHistory DUH WHERE DC.DiscountID=DUH.DiscountID
			            )DUH
			            INNER JOIN ProductsCategoriesMapping PCM ON PCM.CategoryId= DCM.CategoryId
			            INNER JOIN Products P ON P.ProductID =  PCM.ProductId AND P.IsDiscountAllowed = 1
			            WHERE P.ProductID=PRD.ProductID 
			            AND (IIF(DC.DiscountValueType=1, DC.DiscountValue , (DC.DiscountValue/100) * P.Price)) > 0
			            AND  DUH.TotalUsage < IIF(DC.IsBoundToMaxQuantity=1, DC.MaxQuantity , 99999)  -- 99999 An example value because it should not go more than this value
		
		                )DMP")
                      .Where("prd.ProductID in (select value from string_split(@0, ',')) and (DMP.TotalDiscount > 0  AND DMP.TotalDiscount < DMP.Price )", ProductIDs);

                    result = context.Fetch<CategoryBasedDiscountInfo>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<ProductPointOfSaleEntity>? GetProductDetailForPointOfSaleByIdDAL(int ProductId)
        {
            try
            {
                ProductPointOfSaleEntity? result = new ProductPointOfSaleEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"TOP 1 PRD.ProductId, PRD.ProductName, PRD.ShortDescription, PRD.FullDescription, PRD.Price ,  PRD.IsBoundToStockQuantity, PRD.sku,
		              (isnull(USR.FirstName,'') + SPACE(1)+ isnull(USR.LastName,'')) AS VendorName,  
		              isnull(prd.IsShippingFree,0) as IsShippingFree , MFR.Name AS ManufacturerName , prd.IsReturnAble , PRD.MarkAsNew ,  
		              PRD.EstimatedShippingDays , prd.IsDiscountAllowed,
		              im.InventoryId as MainInventoryId,
		              im.SellStartDatetimeUTC as MainSellStartDatetimeUTC,
		              im.SellEndDatetimeUTC as MainSellEndDatetimeUTC,
		              im.WarehouseId as MainWarehouseId,
		              im.InventoryMethodId as MainInventoryMethodId,
		              im.StockQuantity AS MainStockQuantity,
		              im.IsBoundToStockQuantity AS MainIsBoundToStockQuantity,
		              im.DisplayStockQuantity AS MainDisplayStockQuantity,
		              im.OrderMinimumQuantity AS MainOrderMinimumQuantity,
		              im.OrderMaximumQuantity AS MainOrderMaximumQuantity")
                      .From("Products PRD")
                      .InnerJoin("BusnPartner USR").On("USR.BusnPartnerId= PRD.VendorID")
                      .LeftJoin("Manufacturers MFR").On("PRD.ManufacturerID = MFR.ManufacturerID")
                      .InnerJoin("InventoryMain im").On("PRD.ProductID = im.ProductId")

                      .Where("prd.ProductID = @0", ProductId);

                    result = context.Fetch<ProductPointOfSaleEntity>(ppSql)?.FirstOrDefault();
                    if (result != null)
                    {
                        //--Get Product Attributes inventory based info.
                        result.productAttributesForInventory = await _productsCatalogServicesDAL.GetProductMappedAttributesForInventoryDAL(ProductId);

                        //--Get Product Images List
                        var formDataImages = new ProductPicturesMappingEntity
                        {
                            ProductId = ProductId,
                            PageNo = 1,
                            PageSize = 10
                        };
                        result.productImagesList = await _productsCatalogServicesDAL.GetProductsMappedImagesListDAL(formDataImages);


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

        public async Task<List<ProductPointOfSaleEntity>> GetProductsListByIdsDAL(string ProductIdsCommaSeperated)
        {


            try
            {
                List<ProductPointOfSaleEntity> result = new List<ProductPointOfSaleEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    context.EnableAutoSelect = false;

                    result = context.Fetch<ProductPointOfSaleEntity>(@";EXEC [dbo].[SP_GetProductsByIdsCommaSeperated] @ProductsIds",
                          new
                          {

                              ProductsIds = ProductIdsCommaSeperated,
                          }).ToList();


                    if (result != null && result.Count() > 0)
                    {
                        foreach (var product in result)
                        {
                            //--Get Product Attributes inventory based info.
                            product.productAttributesForInventory = await _productsCatalogServicesDAL.GetProductMappedAttributesForInventoryDAL(product.ProductId);

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

        public async Task<List<CustomerInfoPos>> GetCustomerInfoForPosCartDAL(string? SearchKeyword, int CustomerId)
        {


            try
            {
                List<CustomerInfoPos> result = new List<CustomerInfoPos>();

                using (var context = _contextHelper.GetDataContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append("");

                    if (CustomerId > 0)
                    {
                        SearchParameters.Append("AND CSTMR.BusnPartnerId =  @0", CustomerId);
                    }
                    else
                    {

                        if (!String.IsNullOrEmpty(SearchKeyword))
                        {
                            SearchParameters.Append("And (");
                            SearchParameters.Append("CSTMR.FirstName LIKE  @0", "%" + SearchKeyword + "%");
                            SearchParameters.Append("OR CSTMR.MiddleName LIKE  @0", "%" + SearchKeyword + "%");
                            SearchParameters.Append("OR CSTMR.LastName LIKE  @0", "%" + SearchKeyword + "%");
                            SearchParameters.Append("OR CSTMR.EmailAddress LIKE  @0", "%" + SearchKeyword + "%");
                            SearchParameters.Append("OR CUSTOMERPHONE.ContactNo LIKE  @0", "%" + SearchKeyword + "%");
                            SearchParameters.Append(")");
                        }
                    }





                    var ppSql = PetaPoco.Sql.Builder.Select(@"TOP 30 CSTMR.BusnPartnerId, CSTMR.FirstName, CSTMR.MiddleName, CSTMR.LastName, CSTMR.EmailAddress, CUSTOMERPHONE.ContactNo")
                      .From(" BusnPartner AS CSTMR")
                      .Append(@"OUTER APPLY (
                         SELECT TOP 1 PhoneNo AS ContactNo FROM BusnPartnerPhoneAssociation CUSTOMERPHONE 
                         WHERE CUSTOMERPHONE.BusnPartnerId = CSTMR.BusnPartnerId AND PhoneTypeId = 2
                        )CUSTOMERPHONE")
                      .Where("CSTMR.IsActive IS NOT NULL AND CSTMR.IsActive = 1 AND CSTMR.BusnPartnerTypeId = 3 ")
                      .Append(SearchParameters)
                     .OrderBy("CSTMR.FirstName ASC");

                    result = context.Fetch<CustomerInfoPos>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<ServicesResponse?> SaveCustomerOrderInDbWithRetryDAL(CustomerOrderRequestForm FormData)
        {
            ServicesResponse? result = new ServicesResponse();



            try
            {

                int tryTimes = 0;
                while (tryTimes < 2)
                {
                    try
                    {
                        result = await PostCustomerOrderDAL(FormData);
                        if (result != null && result.Success == true && result.ResponseMessage == "Saved Successfully!")
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {


                        //-- Do nothing and just retry
                        if (tryTimes == 1)//-- If two operation failed in the try block
                        {
                            string MainOrderExceptionMsg = ex.Message;

                            //-- create a raw order
                            try
                            {
                                result = await CreateRawOrderDAL(FormData, MainOrderExceptionMsg);
                                break;
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        tryTimes++; //-- Ensure whether exception or not, retry time++ here
                    }
                }


                await Task.FromResult(result);
                return result;


            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<ServicesResponse>? PostCustomerOrderDAL(CustomerOrderRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {

                    dbConnection.Open();



                    dbConnection.Execute("SP_PostCustomerOrder",
                        new
                        {
                            CustomerId = FormData?.CustomerId,
                            OrderNote = FormData?.OrderNote,
                            CartJsonData = FormData?.CartJsonData,
                            OrderTotal = FormData?.OrderTotal,
                            OrderBasedTaxesFinal = JsonConvert.SerializeObject(FormData?.orderBasedTaxesFinal ?? new List<OrderBasedTaxesFinal>()),
                            CouponCode = FormData?.CouponCode,
                            Description = FormData?.Description,
                            StripeStatus = FormData?.StripeStatus,
                            StripeResponseJson = FormData?.StripeResponseJson,
                            StripeBalanceTransactionId = FormData?.StripeBalanceTransactionId,
                            StripeChargeId = FormData?.StripeChargeId,
                            PayPalResponseJson = FormData?.PayPalResponseJson,
                            CurrencyCode = FormData?.CurrencyCode,
                            OrderGuid = FormData?.OrderGuid,
                            PaymentMethod = FormData?.PaymentMethod,
                            DiningOption = FormData?.DiningOption,
                            BusnPartnerId = FormData?.BusnPartnerId,

                        }
                        , commandType: CommandType.StoredProcedure);


                    dbConnection.Close();


                    result.PrimaryKeyValue = null;
                    result.Success = true;
                    result.ResponseMessage = "Saved Successfully!";



                    await Task.FromResult(result);
                    return result;


                }


            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<ServicesResponse>? CreateRawOrderDAL(CustomerOrderRequestForm? FormData, string? MainOrderExceptionMsg)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    //-- Here return primary key
                    var RawOrderID = context.Insert("RawOrder", "RawOrderID", true,
                       new
                       {

                           UserID = FormData.CustomerId,
                           OrderNote = FormData.OrderNote,
                           cartJsonData = FormData.CartJsonData,
                           CouponCode = FormData.CouponCode,
                           PaymentMethodID = FormData.PaymentMethod,
                           ShippingSubTotal = FormData.ShippingSubTotal,
                           OrderTotal = FormData.OrderTotal,
                           PaymentJsonResponse = FormData.PayPalResponseJson,
                           MainOrderExceptionMsg = MainOrderExceptionMsg,
                           CreatedOn = DateTime.Now,
                           CreatedBy = FormData.BusnPartnerId
                       }
                        );

                    if (RawOrderID != null)
                    {


                        result.PrimaryKeyValue = Convert.ToInt32(RawOrderID);
                        result.Success = true;
                        result.ResponseMessage = "Saved Successfully!";

                    }
                    else
                    {
                        result.Success = false;
                        result.ResponseMessage = "Not saved successfully!";

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

        public async Task<List<OrderEntity>> GetCashierOrdersListDAL(int? OrderId, string? ProductName, int? LatestStatusId, int? CustomerId, string? CustomerName, string? TimeRange, int PageNo, int PageSize)
        {


            try
            {
                List<OrderEntity> result = new List<OrderEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {
                    var SearchParameters = PetaPoco.Sql.Builder.Append(" ");

                    if (OrderId > 0)
                    {
                        SearchParameters.Append("AND CTM.OrderId =  @0 ", OrderId);
                    }


                    if (CustomerId > 0)
                    {
                        SearchParameters.Append("AND CTM.CustomerId =  @0 ", CustomerId);
                    }


                    if (!String.IsNullOrEmpty(CustomerName))
                    {
                        SearchParameters.Append("AND CTM.CustomerName LIKE  @0", "%" + CustomerName + "%");
                    }

                    if (!String.IsNullOrEmpty(ProductName))
                    {
                        SearchParameters.Append("AND PRDC.ProductName LIKE  @0", "%" + ProductName + "%");
                    }


                    if (LatestStatusId > 0)
                    {
                        SearchParameters.Append("AND CTM.LatestStatusId =  @0 ", LatestStatusId);
                    }

                    // Add DateTime filtering logic based on TimeRange
                    if (!string.IsNullOrEmpty(TimeRange) && TimeRange != "-999")
                    {
                        DateTime startDate = GetStartDateFromTimeRange(TimeRange);
                        DateTime currentDateTime = DateTime.UtcNow;

                        string startDateSQLFormat = startDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        string currentDateTimeSQLFormat = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                        SearchParameters.Append("AND CTM.OrderDateUTC BETWEEN @0 AND @1", startDateSQLFormat, currentDateTimeSQLFormat);
                    }


                    var ppSql = PetaPoco.Sql.Builder.Append(@" ;WITH CTE_MAIN AS (
                    SELECT DISTINCT
                        ORD.OrderID,
                        ORD.OrderNumber,
                        ORD.CustomerID,
                        ORD.OrderDateUTC,
                        ORD.LatestStatusID,
                        ORD.OrderTotal,
                        ORD.DiningOption,
                        ORS.StatusName AS LatestStatusName,
                        (ISNULL(USR.FirstName, '') + SPACE(1) + ISNULL(USR.LastName, '')) AS CustomerName,
                        PRDC.VendorId,
                        PRDC.ProductId,
		                PMTD.PaymentMethodName,
                        BusnPartnerPhone.PhoneNo as CustomerMobileNo,
                        Count(OITM.OrderItemID) OVER(PARTITION BY OITM.OrderId) AS OrderTotalItems
                    FROM Orders ORD
                    INNER JOIN OrderItems OITM ON ORD.OrderID = OITM.OrderID
                    INNER JOIN OrdersPayments ORPMNT ON ORPMNT.OrderID = ORD.OrderID
	                INNER JOIN PaymentMethods PMTD ON PMTD.PaymentMethodId = ORPMNT.PaymentMethodID
                    INNER JOIN Products PRDC ON PRDC.ProductId = OITM.ProductId
                    INNER JOIN BusnPartner USR ON USR.BusnPartnerId = ORD.CustomerID
                    LEFT JOIN OrderStatuses ORS ON ORD.LatestStatusID = ORS.StatusID
                    OUTER APPLY(
		                SELECT TOP 1 BPPA.PhoneNo FROM BusnPartnerPhoneAssociation BPPA WHERE BPPA.BusnPartnerId = USR.BusnPartnerId
	                )BusnPartnerPhone
                ),
                CTE_FINAL AS (
                    SELECT DISTINCT
                        CTM.OrderID,
                        CTM.OrderNumber,
                        CTM.CustomerID,
                        CTM.OrderDateUTC,
                        CTM.LatestStatusID,
                        CTM.OrderTotal,
                        CTM.DiningOption,
                        CTM.LatestStatusName,
                        CTM.CustomerName,
                        CTM.CustomerMobileNo,
		                CTM.PaymentMethodName,
                        CTM.OrderTotalItems
                    FROM CTE_MAIN CTM")
                    .Where("CTM.OrderId is not null")
                    .Append(SearchParameters)
                    .Append(")")
                    .Select(" CF.* ,  COUNT(*) OVER () as TotalRecords")
                    .From(" CTE_FINAL CF")
                    .OrderBy("CF.OrderId DESC")
                    .Append(@"OFFSET (@0-1)*@1 ROWS
                    FETCH NEXT @1 ROWS ONLY", PageNo, PageSize);

                    result = context.Fetch<OrderEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        private DateTime GetStartDateFromTimeRange(string timeRange)
        {
            DateTime now = DateTime.UtcNow;

            switch (timeRange)
            {
                case "1h":
                    return now.AddHours(-1);
                case "today":
                    return now.Date; // Start of the day UTC
                case "1w":
                    return now.AddDays(-7);
                case "1m":
                    return now.AddMonths(-1);
                case "6m":
                    return now.AddMonths(-6);
                case "1y":
                    return now.AddYears(-1);
                default:
                    return now.Date; // Default to today if an unrecognized value is received
            }
        }

        public async Task<ServicesResponse>? UpdateOrderStatusDAL(OrderStatusRequestForm FormData)
        {

            ServicesResponse? result = new ServicesResponse();

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    dbConnection.Open();

                    dbConnection.Execute(@"DECLARE @OrderStatusMappingID INT;
                    INSERT INTO OrderStatusesMapping(OrderID , StatusID , IsActive ,CreatedOn , CreatedBy)
                    VALUES(@OrderId , @LatestStatusId , 1 , GETDATE() , @UserId);
                    SET @OrderStatusMappingID = SCOPE_IDENTITY();

                    UPDATE OrderStatusesMapping SET IsActive=0 WHERE OrderID =@OrderId AND OrderStatusMappingID!=@OrderStatusMappingID

                    UPDATE Orders SET LatestStatusID= @LatestStatusId WHERE OrderID =@OrderId",
                        new
                        {
                            OrderId = FormData.OrderId,
                            LatestStatusId = FormData.LatestStatusId,
                            UserId = FormData.BusnPartnerId,
                        }
                        , commandType: CommandType.Text);
                    dbConnection.Close();

                    result.PrimaryKeyValue = FormData.OrderId;
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

        public async Task<OrderEntity?> GetOrderMainDetailsByIdDAL(int OrderId)
        {
            try
            {
                OrderEntity? result = new OrderEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" ORD.OrderID ,  ORD.OrderNumber ,ORD.OrderDateUTC  , ORD.OrderTotal, ORD.OrderTax, ORD.OrderTotalShippingCharges, ORD.OrderTotalAttributeCharges ,  ORD.ShippingAddressID, ORD.LatestStatusID, OSS.StatusName LatestStatusName 
		                , (USR.FirstName + SPACE(1) + USR.LastName) AS CustomerName , USR.EmailAddress AS CustomerEmailAddress ,USR.IsWalkThroughCustomer,
		                UsrPhone.CustomerMobileNo as CustomerMobileNo, 
                        (CreatedUser.FirstName + SPACE(1) + CreatedUser.LastName) AS CreatedByUserName
		                FROM Orders ORD 
		                LEFT JOIN OrderStatuses OSS ON ORD.LatestStatusID =  OSS.StatusID
		                INNER JOIN BusnPartner USR ON USR.BusnPartnerId =  ORD.CustomerID
                        LEFT JOIN BusnPartner CreatedUser ON CreatedUser.BusnPartnerId =  ORD.CreatedBy
		                OUTER APPLY (
		                 SELECT TOP 1 UsrPhone.PhoneNo AS CustomerMobileNo  
		                 FROM BusnPartnerPhoneAssociation UsrPhone where UsrPhone.BusnPartnerId = USR.BusnPartnerId AND (UsrPhone.PhoneNo IS NOT NULL AND UsrPhone.PhoneNo != '')
		                ) UsrPhone")
                    .Where("ORD.OrderId =@0", OrderId);


                    result = context.Fetch<OrderEntity>(ppSql)?.FirstOrDefault();

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<OrderShippingDetailEntity>> GetOrderShippingDetailsDAL(int OrderId)
        {
            try
            {
                List<OrderShippingDetailEntity> result = new List<OrderShippingDetailEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"  OSDL.ShippingDetailID, OSDL.OrderID, OSDL.ShipperID, OSDL.ShippingStatusID, OSDL.ShippingMethodID, OSDL.DepartureDate
		            ,OSDL.ReceivedDate, OSDL.ReceivedByActualBuyer, OSDL.ReceiverName, OSDL.ReceiverMobile, OSDL.ReceiverIdentityNo,  OSDL.TrackingNo , OSDL.ShipperComment
		            ,PRD.ProductName  ,PrdImg.ProductDefaultImage , VNDR.BusnPartnerId AS VendorId, VNDR.FirstName AS VendorFirstName, VNDR.LastName AS VendorLastName
		             ,ProductShippingMethods = (CAST(
			            ISNULL( (SELECT PSM.ProductID, PSM.ShippingMethodID FROM ProductShippingMethodsMapping PSM WHERE PSM.ProductID=PRD.ProductID FOR JSON PATH) , '[]')
		             AS NVARCHAR(MAX)   ) )
		            FROM  OrderShippingDetail OSDL
		            INNER JOIN  OrderItems OI ON OSDL.OrderItemID=OI.OrderItemID
		            INNER JOIN  Products PRD ON  PRD.ProductID=OI.ProductID 
		            INNER JOIN BusnPartner VNDR ON VNDR.BusnPartnerId = PRD.VendorID
		            OUTER APPLY(
			            SELECT TOP 1 ATC.AttachmentURL AS ProductDefaultImage FROM ProductPicturesMapping PPM
			            INNER JOIN Attachments ATC ON PPM.PictureID=ATC.AttachmentID WHERE PPM.ProductID=PRD.ProductID
		            ) PrdImg")
                    .Where("OSDL.OrderId =@0", OrderId);


                    result = context.Fetch<OrderShippingDetailEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<OrderShippingMasterEntity?> GetOrderShippingMasterDAL(int OrderId)
        {
            try
            {
                OrderShippingMasterEntity? result = new OrderShippingMasterEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" TOP 1 UADR.AddressAsocId , UADR.AddressOne , UADR.AddressTwo , UADR.CityID , UADR.StateId , UADR.CountryID 
		             , CTR.CountryName , CT.CityName , SP.StateName
		            FROM BusnPartnerAddressAssociation UADR
		            INNER JOIN Orders SDT ON SDT.ShippingAddressID = UADR.AddressAsocId
		            LEFT JOIN Countries CTR ON CTR.CountryID = UADR.CountryID
		            LEFT JOIN Cities CT ON UADR.CityID = CT.CityID
		            LEFT JOIN StateProvinces SP ON UADR.StateId = SP.StateProvinceID")
                    .Where("SDT.OrderId =@0", OrderId);


                    result = context.Fetch<OrderShippingMasterEntity>(ppSql)?.FirstOrDefault();

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<OrderItemEntity>> GetOrderItemsDAL(int OrderId)
        {
            try
            {
                List<OrderItemEntity> result = new List<OrderItemEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" OI.OrderItemID, M.OrderID, OI.ProductID , OI.Price , OI.Quantity ,  OI.OrderItemTotal , PRD.ProductName ,PrdImg.ProductDefaultImage,
	                    VNDR.BusnPartnerId AS VendorId, VNDR.FirstName AS VendorFirstName, VNDR.LastName AS VendorLastName
		                FROM OrderItems OI
		                INNER JOIN Orders M ON OI.OrderID =  M.OrderID
		                INNER JOIN  Products PRD ON OI.ProductID=  PRD.ProductID
		                LEFT JOIN BusnPartner VNDR ON VNDR.BusnPartnerId = PRD.VendorID
		
		                OUTER APPLY(
		                SELECT TOP 1 ATC.AttachmentURL AS ProductDefaultImage FROM ProductPicturesMapping PPM
		                INNER JOIN Attachments ATC ON PPM.PictureID=ATC.AttachmentID
		                WHERE PPM.ProductID=PRD.ProductID
                        )PrdImg")
                    .Where("M.OrderId =@0", OrderId);


                    result = context.Fetch<OrderItemEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<OrdersPaymentEntity>> GetOrderPaymentDetailsDAL(int OrderId)
        {
            try
            {
                List<OrdersPaymentEntity> result = new List<OrdersPaymentEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" OP.PaymentID , OP.OrderID, OP.MilestoneName , OP.MilestoneValue , OP.PaymentDate , OP.PaymentMethodID , PM.PaymentMethodName ,
		                OP.StripeBalanceTransactionId, op.StripeChargeId, op.CurrencyID
		                FROM OrdersPayments OP 
		                INNER JOIN Orders M ON OP.OrderID =  M.OrderID
						INNER JOIN PaymentMethods PM ON OP.PaymentMethodID= PM.PaymentMethodID")
                    .Where("M.OrderId =@0", OrderId);


                    result = context.Fetch<OrdersPaymentEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<List<OrderNotesEntity>> GetOrderNotesDAL(int OrderId)
        {
            try
            {
                List<OrderNotesEntity> result = new List<OrderNotesEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@" * FROM OrderNotes ORNT")
                    .Where("ORNT.OrderId =@0", OrderId);


                    result = context.Fetch<OrderNotesEntity>(ppSql);

                    await Task.FromResult(result);
                    return result;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<int>? GetOrderIdByOrderGuidDAL(string? OrderGuid)
        {

            int result = 0;

            try
            {

                using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                {
                    dbConnection.Open();

                    // Example query to get a single value
                    string sql = "SELECT TOP 1 OrderId FROM Orders WHERE OrderGuid = @OrderGuid";


                    // QuerySingle expects one and only one row
                    //int orderId = dbConnection.QuerySingle<int>(sql);
                    int? orderId = dbConnection.QueryFirstOrDefault<int?>(sql, new { OrderGuid = OrderGuid });

                    result = orderId ?? 0;
                    await Task.FromResult(result);
                    return result;


                }

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<OrderVariantDetail>> GetOrderItemsVariantsDAL(int OrderId)
        {
            try
            {
                List<OrderVariantDetail> result = new List<OrderVariantDetail>();

                using (var context = _contextHelper.GetDataContextHelper())
                {

                    using (IDbConnection dbConnection = _dapperConnectionHelper.GetDapperContextHelper())
                    {
                        dbConnection.Open();

                        var resultIEnumerable = await dbConnection.QueryAsync<OrderVariantDetail>(@"DROP TABLE IF EXISTS  #AttributesTempTable
                    CREATE TABLE #AttributesTempTable
	                    (	
	                    OrderAttributeMappingID INT,
	                    ProductAttributeID INT,
	                    OrderItemID INT,
	                    PrimaryKeyValue INT,
	                    PrimaryKeyDisplayText NVARCHAR(max),
	                    AttributeDisplayName NVARCHAR(300),
	                    AttributeSqlTableName NVARCHAR(300),
	                    IsPrimaryKeyDisplayTextSet BIT
                    )

                    DECLARE @resultResponse TABLE (Response NVARCHAR(MAX));

                    INSERT INTO #AttributesTempTable(OrderAttributeMappingID, ProductAttributeID , OrderItemID, PrimaryKeyValue, PrimaryKeyDisplayText, AttributeDisplayName, AttributeSqlTableName,
                    IsPrimaryKeyDisplayTextSet)
                    SELECT OPAM.OrderAttributeMappingID , OPAM.ProductAttributeID, OPAM.OrderItemID, OPAM.AttributeValue AS PrimaryKeyValue,'' as PrimaryKeyDisplayText,
                    PA.DisplayName AS AttributeDisplayName, PA.AttributeSqlTableName , 0
                    FROM OrderProductAttributeMapping OPAM
                    INNER JOIN OrderItems OI ON OI.OrderItemID = OPAM.OrderItemID
                    INNER JOIN ProductAttributes PA ON PA.ProductAttributeID =  OPAM.ProductAttributeID
                    WHERE OI.OrderID= @OrderID


                    DECLARE @limit INT;
                    DECLARE @count INT = 1;
                    SET @limit = (SELECT COUNT(*) FROM #AttributesTempTable);

                    WHILE @count <= @limit
                    BEGIN
	                    DECLARE @OrderAttributeMappingID INT, @ProductAttributeID INT ,@AttributeSqlTableName NVARCHAR(300), @PrimaryKeyValue INT;
		
	                    SELECT TOP 1 @OrderAttributeMappingID = OrderAttributeMappingID,   @ProductAttributeID =ProductAttributeID  , @AttributeSqlTableName = AttributeSqlTableName,
	                    @PrimaryKeyValue=PrimaryKeyValue 
	                    FROM #AttributesTempTable WHERE IsPrimaryKeyDisplayTextSet = 0 ;

			

	                    DECLARE @DisplayValueColumn NVARCHAR(100);
	                    DECLARE @DisplayTextColumn NVARCHAR(100);
	                    SET @DisplayValueColumn=(SELECT TOP 1 PAC.ColumnName FROM ProductAttributeColumns PAC WHERE PAC.ProductAttributeID=@ProductAttributeId AND PAC.ColumnType='PrimaryKey');
	                    SET @DisplayTextColumn=(SELECT TOP 1 PAC.ColumnName FROM ProductAttributeColumns PAC WHERE PAC.ProductAttributeID=@ProductAttributeId AND PAC.ColumnType='DisplayText');
			
	                    DECLARE @sourceQuery NVARCHAR(MAX);
	                    SET @sourceQuery=('SELECT TOP 1 ' +  @DisplayTextColumn + SPACE(1) + 'AS DisplayText' + SPACE(1)+'FROM ' + @AttributeSqlTableName + SPACE(1) +
					                    'WHERE ' + @DisplayValueColumn + ' = ' + CAST(@PrimaryKeyValue AS NVARCHAR(MAX)));

	                    INSERT INTO @resultResponse EXECUTE sp_executesql @sourceQuery
	                    UPDATE TOP(1) #AttributesTempTable SET PrimaryKeyDisplayText = (SELECT TOP(1) Response AS Data FROM @resultResponse), 
	                    IsPrimaryKeyDisplayTextSet = 1  
	                    WHERE OrderAttributeMappingID = @OrderAttributeMappingID;

	                    DELETE FROM @resultResponse

	                    SET @count += 1;
                    END

                    SELECT * FROM #AttributesTempTable
                    DROP TABLE IF EXISTS  #AttributesTempTable",
                                new
                                {
                                    OrderId = OrderId

                                }
                              , commandType: CommandType.Text);
                        dbConnection.Close();


                        result = resultIEnumerable.ToList();

                        await Task.FromResult(result);
                        return result;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


      

    }
}
