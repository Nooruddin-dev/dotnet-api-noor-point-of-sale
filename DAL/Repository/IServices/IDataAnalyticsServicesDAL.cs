using Entities.DBModels.DataAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IDataAnalyticsServicesDAL
    {
        Task<OrdersEarningThisMonthEntity?>? GetOrdersEarningThisMonthAnalyticsDAL();
        Task<TodayHeroProductsEntity?>? GetTodayHeroProductsAnalyticsDAL();
        Task<OverAllSummaryAnalyticsEntity?>? GetOverAllSummaryAnalyticsDAL();
        Task<TodayActiveOrdersAnalyticsEntity?>? GetTodayActiveOrdersAnalyticsDAL();
        Task<List<MonthlySaleAnalyticsEntity>> GetMonthlySaleAnalyticsDAL();
        Task<List<TopSaleProductsAnalyticsEntity>> GetTopSaleProductsAnalyticsDAL();
        Task<TopTrendsAnalyticsEntity?>? GetTopTrendsAnalyticsDAL();
    }
}
