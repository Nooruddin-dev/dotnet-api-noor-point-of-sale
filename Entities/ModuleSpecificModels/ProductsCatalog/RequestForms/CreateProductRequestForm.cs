using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.ProductsCatalog.RequestForms
{
    public class CreateProductRequestForm
    {
       
        public int ProductId { get; set; } = 0;

       
        public string? ProductName { get; set; }


        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
        public int? ManufacturerId { get; set; }
        public int? VendorId { get; set; }
        public int? TaxRuleId { get; set; }
        public bool? IsActive { get; set; }
        public bool? MarkAsNew { get; set; }
        public bool? AllowCustomerReviews { get; set; }
        public string? Sku { get; set; }
        public decimal Price { get; set; }
        public decimal? OldPrice { get; set; }
        public bool? IsDiscountAllowed { get; set; }
        public bool? IsShippingFree { get; set; }
        public decimal? ShippingCharges { get; set; }
        public int? EstimatedShippingDays { get; set; } = 1;
        public bool? IsReturnAble { get; set; }


        //--Json fields
        public string? SelectedCategoryIdsJson { get; set; }
        public string? SelectedTagsJson { get; set; }
        public string? SelectedShippingMethodsJson { get; set; }
        public string? ProductAttributesJson { get; set; }

        //--Image files field
        public IFormFile[]? ProductImages { get; set; }
    }

    public class CreateProductRequestFormInternal
    {
        public CreateProductRequestForm? createProductRequestForm { get; set; }
        public string? ProductImagesJson { get; set; }
        public int BusnPartnerId { get; set; }
    }
}
