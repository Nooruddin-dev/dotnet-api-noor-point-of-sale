using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.DataAnalytics
{
    public class TopSaleProductsAnalyticsEntity
    {
        public int ProductId { get; set; }      
        public string? ProductName { get; set; }    
        public string? VendorFirstName { get; set; }
        public string? VendorLastName { get; set; }  
        public int TotalOrders { get; set; }
        public decimal TotalRevenueSelectedProducts { get; set; }
        public string? ProductDefaultImage { get; set; }
        public string? CategoryName { get; set; }
    }
}
