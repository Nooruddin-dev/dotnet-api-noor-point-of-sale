using DAL.DBContext;
using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.DataAnalytics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Services
{
    public class DataAnalyticsServicesDAL : IDataAnalyticsServicesDAL
    {

        private readonly IConfiguration _configuration;
        private readonly IDataContextHelper _contextHelper;
        private readonly IDapperConnectionHelper _dapperConnectionHelper;

        //--Constructor of the class
        public DataAnalyticsServicesDAL(IConfiguration configuration, IDataContextHelper contextHelper, IDapperConnectionHelper dapperConnectionHelper)
        {
            _configuration = configuration;
            _contextHelper = contextHelper;
            _dapperConnectionHelper = dapperConnectionHelper;
        }

        public async Task<OrdersEarningThisMonthEntity?>? GetOrdersEarningThisMonthAnalyticsDAL()
        {


            try
            {
                OrdersEarningThisMonthEntity? result = new OrdersEarningThisMonthEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"SUM(o.OrderTotal) AS TotalSales")
                      .From(" Orders o")
                      .Where("YEAR(o.OrderDateUTC) = YEAR(GETDATE()) AND MONTH(o.OrderDateUTC) = MONTH(GETDATE())");


                    result = context.Fetch<OrdersEarningThisMonthEntity>(ppSql)?.FirstOrDefault();


                    if (result == null)
                    {
                        result = new OrdersEarningThisMonthEntity();
                    }

                    //-- Get top 3 categoires analytics
                    var ppSqlCategories = PetaPoco.Sql.Builder.Append(@";WITH CurrentMonthOrders AS (
                        -- Step 1: Filter orders for the current month
                        SELECT  OrderID, OrderDateUTC, MONTH(OrderDateUTC) AS OrderMonth
                        FROM Orders
                        WHERE  MONTH(OrderDateUTC) = MONTH(GETDATE()) AND YEAR(OrderDateUTC) = YEAR(GETDATE())
	
                    ),
                    ProductWithCategory AS (
                        -- Step 2 and 3: Join Orders with OrderItems and Products, then map Products to Categories
                        SELECT oi.OrderItemID, oi.OrderID, oi.ProductID, oi.OrderItemTotal, pcm.CategoryID, CMO.OrderMonth
                        FROM  OrderItems oi
	                    INNER JOIN CurrentMonthOrders CMO ON CMO.OrderID =  OI.OrderID
                        INNER JOIN Products p ON p.ProductID = oi.ProductID
                        OUTER APPLY(
	                       SELECT TOP 1 * FROM ProductsCategoriesMapping pcm WHERE  pcm.ProductID = p.ProductID
	                    )PCM
        
                    ),
                    SalesByCategory AS (
                        -- Step 4 and 5: Group by Category and Sum the Sales
                        SELECT  pc.CategoryID, pc.Name AS CategoryName, SUM(pwc.OrderItemTotal) AS TotalSales, PWC.OrderMonth
                        FROM ProductWithCategory pwc
                        INNER JOIN  ProductCategories pc ON pc.CategoryID = pwc.CategoryID
                        GROUP BY  pc.CategoryID, pc.Name,  PWC.OrderMonth
                    )

                    -- Step 6: Get the Top 3 Categories
                    SELECT TOP 3 CategoryID , CategoryName, TotalSales AS TotalSalesPerCategory, OrderMonth
                    FROM 
                        SalesByCategory
                    ORDER BY 
                        TotalSales DESC;");

                    var TopCategoriesAnalytics = context.Fetch<OrdersEarningThisMonthCategories>(ppSqlCategories);
                    result.TopCategoriesAnalytics = TopCategoriesAnalytics;



                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<TodayHeroProductsEntity?>? GetTodayHeroProductsAnalyticsDAL()
        {


            try
            {
                TodayHeroProductsEntity? result = new TodayHeroProductsEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"COUNT(DISTINCT oi.ProductID) AS TotalProductsSelectedToday")
                      .From("  Orders o")
                      .InnerJoin("OrderItems oi").On("o.OrderID = oi.OrderID")
                      .Where("YEAR(o.OrderDateUTC) = YEAR(GETDATE()) AND MONTH(o.OrderDateUTC) = MONTH(GETDATE()) AND DAY(o.OrderDateUTC) = DAY(GETDATE())");


                    result = context.Fetch<TodayHeroProductsEntity>(ppSql)?.FirstOrDefault();


                    if (result == null)
                    {
                        result = new TodayHeroProductsEntity();
                    }

                    //-- Get top 8 hero customers analytics
                    var ppSqlTopHerosCustomer = PetaPoco.Sql.Builder.Append(@";WITH TodaySales AS (
                      SELECT o.OrderID, o.CustomerID, o.OrderTotal
                      FROM Orders o
                      WHERE YEAR(o.OrderDateUTC) = YEAR(GETDATE()) AND MONTH(o.OrderDateUTC) = MONTH(GETDATE()) AND DAY(o.OrderDateUTC) = DAY(GETDATE())
                    ),
                    CustomerSpending AS (
                      -- Sum total spend for each customer, grouping by CustomerID
                      SELECT CustomerID, SUM(OrderTotal) AS TotalSpentByCustomer
                      FROM TodaySales
                      GROUP BY CustomerID
                    )
                    SELECT
                      TOP 6 cs.CustomerID , c.FirstName, cs.TotalSpentByCustomer
                    FROM
                      CustomerSpending cs
                    INNER JOIN BusnPartner c ON cs.CustomerID = c.BusnPartnerId
                    ORDER BY cs.TotalSpentByCustomer DESC;");

                    var todayTopCustomerSpendings = context.Fetch<TodayTopCustomerSpending>(ppSqlTopHerosCustomer);
                    result.todayTopCustomerSpendings = todayTopCustomerSpendings;



                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<OverAllSummaryAnalyticsEntity?>? GetOverAllSummaryAnalyticsDAL()
        {


            try
            {
                OverAllSummaryAnalyticsEntity? result = new OverAllSummaryAnalyticsEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Append(@"SELECT
                    (SELECT COUNT(*) FROM Products) AS TotalProducts,
                    (SELECT SUM(OrderTotal) FROM Orders) AS TotalRevenue,
                    (SELECT COUNT(*) FROM Orders) AS TotalOrders,
                    (SELECT COUNT(*) FROM BusnPartner WHERE BusnPartnerTypeId = 3) AS TotalCustomers,
                    (SELECT COUNT(*) FROM Orders 
                        WHERE YEAR(OrderDateUTC) = YEAR(GETDATE()) AND MONTH(OrderDateUTC) = MONTH(GETDATE())) AS TodayTotalOrders,
                    (SELECT SUM(OrderTotal) FROM Orders 
                        WHERE YEAR(OrderDateUTC) = YEAR(GETDATE()) AND MONTH(OrderDateUTC) = MONTH(GETDATE())) AS CurrentMonthTotalSale");
                    

                    result = context.Fetch<OverAllSummaryAnalyticsEntity>(ppSql)?.FirstOrDefault();


                  

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

         public async Task<TodayActiveOrdersAnalyticsEntity?>? GetTodayActiveOrdersAnalyticsDAL()
        {


            try
            {
                TodayActiveOrdersAnalyticsEntity? result = new TodayActiveOrdersAnalyticsEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Append(@";WITH TodayOrders AS (
                      SELECT
                        COUNT(*) AS TotalOrders
                      FROM Orders
                      WHERE YEAR(OrderDateUTC) = YEAR(GETDATE()) AND MONTH(OrderDateUTC) = MONTH(GETDATE()) AND DAY(OrderDateUTC) = DAY(GETDATE())
                    )
                    SELECT
                      tdo.TotalOrders,
                      (
                        SELECT COUNT(*)
                        FROM Orders o
                        WHERE o.LatestStatusID = 1
                          AND YEAR(o.OrderDateUTC) = YEAR(GETDATE()) AND MONTH(o.OrderDateUTC) = MONTH(GETDATE()) AND DAY(o.OrderDateUTC) = DAY(GETDATE())
                      ) AS ActiveOrders
                    FROM TodayOrders tdo;
                    ");
                    

                    result = context.Fetch<TodayActiveOrdersAnalyticsEntity>(ppSql)?.FirstOrDefault();


                  

                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }


        public async Task<List<MonthlySaleAnalyticsEntity>> GetMonthlySaleAnalyticsDAL()
        {


            try
            {
                List<MonthlySaleAnalyticsEntity> result = new List<MonthlySaleAnalyticsEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Select(@"MONTH(OrderDateUTC) AS MonthId, COUNT(OrderID) AS TotalOrders,  SUM(OrderTotal) AS TotalSale")
                        .From("Orders")
                        .Where("YEAR(OrderDateUTC) = YEAR(GETDATE())")
                        .GroupBy("MONTH(OrderDateUTC)")
                        .OrderBy("MONTH(OrderDateUTC)");


                    result = context.Fetch<MonthlySaleAnalyticsEntity>(ppSql);




                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<List<TopSaleProductsAnalyticsEntity>> GetTopSaleProductsAnalyticsDAL()
        {


            try
            {
                List<TopSaleProductsAnalyticsEntity> result = new List<TopSaleProductsAnalyticsEntity>();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Append(@";WITH CTE_PRODUCTS AS (
                    SELECT
                      p.ProductID,
                      p.ProductName,
                      VNDR.FirstName as VendorFirstName,
                      VNDR.LastName as VendorLastName,
                      COUNT(oi.OrderID) AS TotalOrders,
                      SUM(OI.OrderItemTotal) AS TotalRevenueSelectedProducts
                    FROM  OrderItems oi
                    INNER JOIN ORDERS O ON O.OrderID = OI.OrderID
                    INNER JOIN  Products p ON p.ProductID = oi.ProductID
                    INNER JOIN BusnPartner VNDR ON P.VendorID = VNDR.BusnPartnerId
                    GROUP BY  p.ProductID,  p.ProductName,  VNDR.FirstName,  VNDR.LastName
                    )
                    SELECT TOP 5 P.*, IMG.ProductDefaultImage, CATEGORY.CategoryName
                    FROM CTE_PRODUCTS P
                    OUTER APPLY
                    (
	                    SELECT TOP 1 ATC.AttachmentURL AS ProductDefaultImage FROM ProductPicturesMapping PPM 
	                    INNER JOIN Attachments ATC ON PPM.PictureID = ATC.AttachmentID
	                    WHERE PPM.ProductID = P.ProductID
                    )IMG
                    OUTER APPLY
                    (
	                    SELECT TOP 1 PC.Name AS CategoryName FROM ProductsCategoriesMapping PCM
	                    INNER JOIN ProductCategories PC ON PC.CategoryID = PCM.CategoryID
	                    WHERE PCM.ProductID = P.ProductID
                    )CATEGORY

                    ORDER BY P.TotalOrders DESC");


                    result = context.Fetch<TopSaleProductsAnalyticsEntity>(ppSql);




                    await Task.FromResult(result);
                    return result;



                }
            }
            catch (Exception)
            {

                throw;
            }



        }

        public async Task<TopTrendsAnalyticsEntity?>? GetTopTrendsAnalyticsDAL()
        {


            try
            {
                TopTrendsAnalyticsEntity? result = new TopTrendsAnalyticsEntity();

                using (var context = _contextHelper.GetDataContextHelper())
                {


                    var ppSql = PetaPoco.Sql.Builder.Append(@";WITH RevenueData AS (
                      -- Revenue for the last 12 months
                      SELECT SUM(OrderTotal) AS RevenueLast12Months
                      FROM Orders
                      WHERE OrderDateUTC >= DATEADD(MONTH, -12, GETDATE())  -- Last 12 months
                    ),
                    RevenueLast6Months AS (
                      -- Revenue for the last 6 months
                      SELECT SUM(OrderTotal) AS RevenueLast6Months
                      FROM Orders
                      WHERE  OrderDateUTC >= DATEADD(MONTH, -6, GETDATE())  -- Last 6 months
                    ),
                    RevenueLast1Month AS (
                      -- Revenue for the last 1 month
                      SELECT SUM(OrderTotal) AS RevenueLast1Month
                      FROM Orders
                      WHERE OrderDateUTC >= DATEADD(MONTH, -1, GETDATE())  -- Last 1 month
                    )

                    SELECT rd.RevenueLast12Months, r6.RevenueLast6Months, r1.RevenueLast1Month
                    FROM  RevenueData rd,  RevenueLast6Months r6, RevenueLast1Month r1
                    ");


                    result = context.Fetch<TopTrendsAnalyticsEntity>(ppSql)?.FirstOrDefault();


                    if (result == null)
                    {
                        result = new TopTrendsAnalyticsEntity();
                    }

                    var ppSqlMonthlySales = PetaPoco.Sql.Builder.Select(@"MONTH(OrderDateUTC) AS MonthId, COUNT(OrderID) AS TotalOrders,  SUM(OrderTotal) AS TotalSale")
                      .From("Orders")
                      .Where("YEAR(OrderDateUTC) = YEAR(GETDATE())")
                      .GroupBy("MONTH(OrderDateUTC)")
                      .OrderBy("MONTH(OrderDateUTC)");



                    var topTrendMonthlyDataAnalytics = context.Fetch<TopTrendMonthlyDataAnalytics>(ppSqlMonthlySales);
                    result.topTrendMonthlyDataAnalytics = topTrendMonthlyDataAnalytics;



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
