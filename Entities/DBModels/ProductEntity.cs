using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels
{
    public class ProductEntity: IPageBasicData
    {

        public int ProductId { get; set; }
        public string? ProductName { get; set; } = null!;
        public string? ShortDescription { get; set; }
        public string FullDescription { get; set; } = null!;
        public int VendorId { get; set; }
        public int? ManufacturerId { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaKeywords { get; set; }
        public string? MetaDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public bool? IsTaxExempt { get; set; }
        public bool? IsShippingFree { get; set; }
        public int? EstimatedShippingDays { get; set; }
        public decimal? ShippingCharges { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public bool? AllowCustomerReviews { get; set; }
        public int? ProductViewCount { get; set; }
        public int? ProductSalesCount { get; set; }
        public bool? IsReturnAble { get; set; }
        public bool? IsDigitalProduct { get; set; }
        public bool? IsDiscountAllowed { get; set; }
        public DateTime? SellStartDatetimeUtc { get; set; }
        public DateTime? SellEndDatetimeUtc { get; set; }
        public string? Sku { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? WarehouseId { get; set; }
        public int? InventoryMethodId { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsBoundToStockQuantity { get; set; }
        public bool? DisplayStockQuantity { get; set; }
        public int? OrderMinimumQuantity { get; set; }
        public int? OrderMaximumQuantity { get; set; }
        public bool? MarkAsNew { get; set; }
        public decimal? DisplaySeqNo { get; set; }
        public bool? IsActive { get; set; }
        public int? TaxRuleId { get; set; }

        //--Other Fields
        public string? SelectedCategoriesJson { get; set; }
        public string? SelectedTagsJson { get; set; }
        public string? SelectedDiscountsJson { get; set; }
        public string? SelectedShippingMethodsJson { get; set; }

        public string? ProductImagesJson { get; set; }
        public string? ProductDefaultImgPath { get; set; } //-- Its for default top 1 one image to show in a list



        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }
        public string? FromDate { get; set; }
        public int? DataExportType { get; set; }
        public string? ToDate { get; set; }
        public int? CategoryId { get; set; }
    }
}
