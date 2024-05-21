using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.CashierMain
{
    public class DiscountedProductsTemp
    {
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal ProductActualPrice { get; set; }
        public bool IsDiscountCalculated { get; set; }
        public string? CouponCode { get; set; }
    }
}
