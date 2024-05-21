using Entities.ModuleSpecificModels.ProductsCatalog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.CashierMain.RequestForms
{
    public class CustomerOrderRequestForm
    {
        public int CustomerId { get; set; }
        public string? OrderNote { get; set; }
        public string CartJsonData { get; set; }
        public string? CouponCode { get; set; }
        public int PaymentMethod { get; set; }
        public int? DiningOption { get; set; }
        public string? PaymentToken { get; set; }
        public string? PayPalOrderConfirmJson { get; set; }

        public decimal CartSubTotal { get; set; }
        public decimal ShippingSubTotal { get; set; }
        public List<OrderBasedTaxesFinal>? orderBasedTaxesFinal { get; set; }
        public decimal OrderTotal { get; set; }




        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? CurrencyCode { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? Description { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? StripeStatus { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? StripeResponseJson { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? StripeBalanceTransactionId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? StripeChargeId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public string? PayPalResponseJson { get; set; }


        [JsonIgnore]
        [NotMapped]
        public string? OrderGuid { get; set; }

    }


    public class CartCustomerProducts
    {
        public int ProductId { get; set; }
        public List<CartProductSelectedAttributes>? productSelectedAttributes { get; set; }
        public List<ProductMappedAttributesForInventory>? productAttributesForInventory { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public decimal ItemPriceTotal { get; set; }

        public bool? IsShippingFree { get; set; }
        public decimal OrderItemShippingChargesTotal { get; set; }
        public decimal OrderItemAttributeChargesTotal { get; set; }
        public decimal OrderItemTaxTotal { get; set; }
        public int? TaxRuleId { get; set; }
        public decimal ItemSubTotal { get; set; }
        public int? DiscountId { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public decimal? OrderItemDiscountTotal { get; set; }
        public bool? IsDiscountCalculated { get; set; }
        public string? CouponCode { get; set; }

    }

    public class CartProductSelectedAttributes
    {
        public int ProductId { get; set; }
        public int ProductAttributeID { get; set; }
        public int PrimaryKeyValue { get; set; }
        public decimal AttrAdditionalPrice { get; set; }
    }

    public class OrderBasedTaxesFinal
    {
        public int TaxRuleId { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
    }




}
