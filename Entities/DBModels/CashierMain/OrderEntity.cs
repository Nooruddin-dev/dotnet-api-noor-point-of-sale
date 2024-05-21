using Entities.CommonModels.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DBModels.CashierMain
{
    public class OrderEntity: IPageBasicData
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public int? LatestStatusId { get; set; }
        public int ShippingAddressId { get; set; }
        public decimal? OrderTotalDiscountAmount { get; set; }
        public decimal? OrderTotalShippingCharges { get; set; }
        public decimal? OrderTotalAttributeCharges { get; set; }
        public decimal? OrderTax { get; set; }
        public decimal OrderTotal { get; set; }
        public int OrderTotalItems { get; set; }
        public int? DiningOption { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerEmailAddress { get; set; }
        public string? CustomerMobileNo { get; set; }
        public Boolean? IsWalkThroughCustomer { get; set; }

        public string? LatestStatusName { get; set; }
        public string? PaymentMethodName { get; set; }

        public string? ProductName { get; set; }
        public int? ProductId { get; set; }

        public string? CreatedByUserName { get; set; }

      


        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }

       
    }


    public class OrderShippingMasterEntity
    {
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public string? CityName { get; set; }

        public int AddressAsocId { get; set; }
        public int AddressTypeId { get; set; }
        public int UserId { get; set; }
        public string AddressOne { get; set; } = null!;
        public string? AddressTwo { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? PostalCode { get; set; }
    }
    

    public class OrderShippingDetailEntity : IPageBasicData
    {
       
      

        public int ShippingDetailId { get; set; }
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int? ShipperId { get; set; }
        public int? ShippingMethodId { get; set; }
        public int? ShippingStatusId { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public bool? ReceivedByActualBuyer { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverMobile { get; set; }
        public string? ReceiverIdentityNo { get; set; }
        public string? TrackingNo { get; set; }
        public string? ShipperComment { get; set; }

        public int? VendorId { get; set; }

        public string? VendorFirstName { get; set; }
        public string? VendorLastName { get; set; }

        public string? ProductName { get; set; }
        public string? ProductDefaultImage { get; set; }
        public int? ProductId { get; set; }
        public string? ProductShippingMethods { get; set; }
        // public string? OrderShippingDetailItemsJson { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }


    }

    public class OrderItemEntity : IPageBasicData
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string? OrderItemGuid { get; set; }
        public decimal Price { get; set; }
        public decimal? ItemPriceTotal { get; set; }
        public int? Quantity { get; set; }
        public decimal? OrderItemShippingChargesTotal { get; set; }
        public decimal? OrderItemAttributeChargesTotal { get; set; }
        public decimal? OrderItemTaxTotal { get; set; }
        public int? DiscountId { get; set; }
        public int? VendorCommissionId { get; set; }
        public decimal? OrderItemDiscountTotal { get; set; }
        public decimal? OrderItemTotal { get; set; }


        public int? VendorId { get; set; }

        public string? VendorFirstName { get; set; }
        public string? VendorLastName { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDefaultImage { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }

    }

    public class OrdersPaymentEntity : IPageBasicData
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string MilestoneName { get; set; } = null!;
        public decimal MilestoneValue { get; set; }
        public int CurrencyId { get; set; }
        public bool IsCompleted { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? Guid { get; set; }
        public string? TransactionNo { get; set; }
        public string? StripeResponseJson { get; set; }
        public string? StripeBalanceTransactionId { get; set; }
        public string? StripeChargeId { get; set; }
        public string? PayPalResponseJson { get; set; }

        public string? PaymentMethodName { get; set; }



        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }

    }

    public class OrderNotesEntity : IPageBasicData
    {
       public int OrderNoteID { get; set; }
       public int OrderID { get; set; }
       public string? Message { get; set; }
       public int? ParentOrderNoteID { get; set; }
       public DateTime? CreatedOn { get; set; }
       public int? CreatedBy { get; set; }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int BusnPartnerId { get; set; }

    }
}

