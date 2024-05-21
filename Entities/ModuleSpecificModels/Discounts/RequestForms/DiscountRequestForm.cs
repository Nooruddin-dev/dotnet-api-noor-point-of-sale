using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Discounts.RequestForms
{
    public class DiscountRequestForm
    {

        public int DiscountId { get; set; }
        public string? Title { get; set; }
        public int DiscountTypeId { get; set; }
        public int DiscountValueType { get; set; }
        public int DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Boolean IsCouponCodeRequired { get; set; }
        public string? CouponCode { get; set; }
        public Boolean IsActive { get; set; }
        public List<DiscountAssociatedProducts>? discountAssociatedProducts { get; set; }
        public List<DiscountAssociatedCategories>? discountAssociatedCategories { get; set; }
       

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }

    public class DiscountAssociatedProducts
    {
        public int? ProductId { get; set; }
    }

    public class DiscountAssociatedCategories
    {
        public int? CategoryId { get; set; }
    }
}
