using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class TodayActiveOrdersAnalyticsEntity
    {
        public int TotalOrders { get; set; }
        public int ActiveOrders { get; set; }
    }
}
