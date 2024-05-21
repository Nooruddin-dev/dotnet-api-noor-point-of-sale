using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.CashierMain
{
    public class ProductBasedDiscountInfo
    {
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public string? CouponCode { get; set; }
        public decimal Price { get; set; }
        public decimal TotalDiscount { get; set; }
        public int? TotalUsage { get; set; }
    }

    public class CategoryBasedDiscountInfo
    {
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public string? CouponCode { get; set; }
        public decimal Price { get; set; }
        public decimal TotalDiscount { get; set; }
        public int? TotalUsage { get; set; }
    }
}
