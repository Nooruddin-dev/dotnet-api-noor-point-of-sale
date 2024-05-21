using Entities.CommonModels.Pagination;
using Entities.ModuleSpecificModels.ProductsCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class ProductPointOfSaleEntity : IPageBasicData
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ShortDescription { get; set; }
        public string FullDescription { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDiscountAllowed { get; set; }
        public bool? MarkAsNew { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? ManufacturerId { get; set; }
        public string? OrderByColumnName { get; set; }
        public string? ProductImagesJson { get; set; }

        public int? TaxRuleId { get; set; }
        public decimal? ProductTaxRate { get; set; }
        public decimal? ProductTaxValue { get; set; }


        public int Quantity { get; set; }
        public int? DiscountId { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? OrderItemDiscount { get; set; }
        public decimal? ItemSubTotal { get; set; }
        public bool? IsDiscountCalculated { get; set; }
        public string? CouponCode { get; set; }
        public int VendorId { get; set; }
        public string? VendorName { get; set; }
      
        public string? DiscEndDate { get; set; }

        public string? ManufacturerFirstName { get; set; }
        public string? ManufacturerLastName { get; set; }




        public bool? IsTaxExempt { get; set; }
        public bool? IsShippingFree { get; set; }
        public int? EstimatedShippingDays { get; set; }
        public decimal? ShippingCharges { get; set; }
        public bool? ShowOnHomePage { get; set; }
        public bool? AllowCustomerReviews { get; set; }
        public int? ProductViewCount { get; set; }
        public int? ProductSalesCount { get; set; }
        public bool? IsReturnAble { get; set; }
        public string? Sku { get; set; }

        public int? MainInventoryId { get; set; }
        public DateTime? MainSellStartDatetimeUTC { get; set; }
        public DateTime? MainSellEndDatetimeUTC { get; set; }
        public int? MainWarehouseId { get; set; }
        public int? MainInventoryMethodId { get; set; }
        public int? MainStockQuantity { get; set; }
        public bool? MainIsBoundToStockQuantity { get; set; }
        public bool? MainDisplayStockQuantity { get; set; }
        public int? MainOrderMinimumQuantity { get; set; }
        public int? MainOrderMaximumQuantity { get; set; }

        //  public List<CartProductAllAttributes>? ProductAllSelectedAttributes { get; set; }
        public List<ProductMappedAttributesForInventory>? productAttributesForInventory { get; set; }
        public List<ProductPicturesMappingEntity>? productImagesList{ get; set; }


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }

    }
}
