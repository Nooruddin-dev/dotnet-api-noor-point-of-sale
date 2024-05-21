using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class OrdersEarningThisMonthEntity
    {
        public decimal TotalSales { get; set; }
        public List<OrdersEarningThisMonthCategories>? TopCategoriesAnalytics { get; set; }
    }

    public class OrdersEarningThisMonthCategories
    {
        public int CategoryId { get; set;}
        public string? CategoryName { get; set;}
        public decimal TotalSalesPerCategory { get; set;}
        public int OrderMonth { get; set;}
        public string? OrderMonthName { get; set;}
    }
}
