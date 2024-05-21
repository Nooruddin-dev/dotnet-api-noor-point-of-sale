using Entities.DBModels.CashierMain;
using Entities.ModuleSpecificModels.CashierMain;
using Entities.ModuleSpecificModels.CashierMain.RequestForms;
using Entities.ModuleSpecificModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface ICashierMainServicesDAL
    {
        Task<List<PointOfSaleCategoriesEntity>> GetPointOfSaleCategoriesDAL(PointOfSaleCategoriesEntity FormData);
        Task<List<ProductPointOfSaleEntity>> GetPointOfSaleProductsDAL(ProductPointOfSaleEntity FormData);
        Task<List<ProductBasedDiscountInfo>> GetProductDiscountInfoProductBasedDAL(string? ProductIDs);
        Task<List<CategoryBasedDiscountInfo>> GetProductDiscountInfoCategoryBasedDAL(string? ProductIDs);
        Task<ProductPointOfSaleEntity>? GetProductDetailForPointOfSaleByIdDAL(int ProductId);
        Task<List<ProductPointOfSaleEntity>> GetProductsListByIdsDAL(string ProductIdsCommaSeperated);
        Task<List<CustomerInfoPos>> GetCustomerInfoForPosCartDAL(string? SearchKeyword, int CustomerId);
        Task<ServicesResponse?> SaveCustomerOrderInDbWithRetryDAL(CustomerOrderRequestForm FormData);
        Task<List<OrderEntity>> GetCashierOrdersListDAL(int? OrderId, string? ProductName, int? LatestStatusId, int? CustomerId, string? CustomerName, string? TimeRange, int PageNo, int PageSize);
        Task<ServicesResponse>? UpdateOrderStatusDAL(OrderStatusRequestForm FormData);
        Task<OrderEntity?> GetOrderMainDetailsByIdDAL(int OrderId);
        Task<List<OrderShippingDetailEntity>> GetOrderShippingDetailsDAL(int OrderId);
        Task<OrderShippingMasterEntity?> GetOrderShippingMasterDAL(int OrderId);
        Task<List<OrderItemEntity>> GetOrderItemsDAL(int OrderId);
        Task<List<OrdersPaymentEntity>> GetOrderPaymentDetailsDAL(int OrderId);
        Task<List<OrderNotesEntity>> GetOrderNotesDAL(int OrderId);
        Task<int>? GetOrderIdByOrderGuidDAL(string? OrderGuid);
        Task<List<OrderVariantDetail>> GetOrderItemsVariantsDAL(int OrderId);
      
    }
}
