using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.Discounts
{
    public class DiscountEntity: IPageBasicData
    {
        public int DiscountId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int DiscountTypeId { get; set; }
        public int DiscountValueType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? CouponCode { get; set; }
        public int? MaxQuantity { get; set; }
        public bool IsBoundToMaxQuantity { get; set; }
        public bool? IsCouponCodeRequired { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int? TotalUsage { get; set; }
        public int? IsActiveSelected { get; set; }
        public string? DiscountTypeName { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
