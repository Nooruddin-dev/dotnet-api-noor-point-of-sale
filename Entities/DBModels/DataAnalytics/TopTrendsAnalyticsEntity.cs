using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class TopTrendsAnalyticsEntity
    {
        public decimal RevenueLast12Months { get; set; }
        public decimal RevenueLast6Months { get; set; }
        public decimal RevenueLast1Month { get; set; }

        public List<TopTrendMonthlyDataAnalytics> topTrendMonthlyDataAnalytics { get; set; }
    }

    public class TopTrendMonthlyDataAnalytics
    {
        public int MonthId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSale { get; set; }
    }
}
