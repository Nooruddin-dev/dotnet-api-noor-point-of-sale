using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class MonthlySaleAnalyticsEntity
    {
        public int MonthId { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSale { get; set; }
    }
}
