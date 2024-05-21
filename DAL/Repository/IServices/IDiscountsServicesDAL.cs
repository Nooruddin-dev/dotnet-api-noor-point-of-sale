using Entities.DBModels;
using Entities.DBModels.Discounts;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Discounts.RequestForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IDiscountsServicesDAL
    {
        Task<List<DiscountEntity>> GetDiscountsListDAL(DiscountEntity FormData);
        Task<List<DiscountTypeEntity>> GetDiscountTypesListDAL(DiscountTypeEntity FormData);
        Task<List<DiscountProductsMappingEntity>> GetDiscountProductsMappingListDAL(DiscountProductsMappingEntity FormData);
        Task<List<ProductEntity>> GetProductsListForDiscountDAL(ProductEntity FormData);
        Task<ServicesResponse>? InsertUpdateDiscountDAL(DiscountRequestForm FormData);
        Task<DiscountEntity?> GetDiscountDetailByIdDAL(int DiscountId);
        Task<List<DiscountCategoryMappingEntity>> GetDiscountsMappedCategoriesListDAL(DiscountCategoryMappingEntity FormData);
        Task<List<ProductCategoriesEntity>> GetCategoriesListForDiscountDAL(ProductCategoriesEntity FormData);
    }
}
