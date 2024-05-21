using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class RawOrderEntity
    {
        // Property to represent RawOrderID
        public int RawOrderID { get; set; }

        // Property to represent UserID
        public int UserID { get; set; }

        // Property to represent OrderNote
        public string? OrderNote { get; set; }

        // Property to represent cartJsonData
        public string? CartJsonData { get; set; }

        // Property to represent CouponCode
        public string? CouponCode { get; set; }

        // Property to represent PaymentMethodID
        public int PaymentMethodID { get; set; }

        // Property to represent ShippingSubTotal
        public decimal? ShippingSubTotal { get; set; }

        // Property to represent OrderTotal
        public decimal OrderTotal { get; set; }

        // Property to represent PaymentJsonResponse
        public string? PaymentJsonResponse { get; set; }

        // Property to represent MainOrderExceptionMsg
        public string? MainOrderExceptionMsg { get; set; }

        // Property to represent CreatedOn (DateTime type)
        public DateTime CreatedOn { get; set; }

        // Property to represent CreatedBy
        public int CreatedBy { get; set; }
    }
}
