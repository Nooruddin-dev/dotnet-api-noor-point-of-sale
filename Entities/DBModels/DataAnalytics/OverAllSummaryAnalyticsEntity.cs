using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class OverAllSummaryAnalyticsEntity
    {
        public int TotalProducts { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TodayTotalOrders { get; set; }
        public int CurrentMonthTotalSale { get; set; }
    }
}


