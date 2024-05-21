using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class TodayHeroProductsEntity
    {
        public int TotalProductsSelectedToday { get; set; }
        public List<TodayTopCustomerSpending>? todayTopCustomerSpendings { get; set; }
    }
    public class TodayTopCustomerSpending
    {
        public int CustomerID { get; set; }
        public string? FirstName { get; set; }
        public decimal TotalSpentByCustomer { get; set; }
    }
}
