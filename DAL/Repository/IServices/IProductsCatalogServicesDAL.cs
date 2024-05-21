using Entities.CommonModels;
using Entities.DBModels;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.IServices
{
    public interface IProductsCatalogServicesDAL
    {
        Task<List<ProductCategoriesEntity>> GetProductCategoriesListDAL(ProductCategoriesEntity FormData);
        Task<ServicesResponse>? InsertUpdateProductCategoryDAL(ProductCategoriesRequestForm FormData);
        Task<List<ProductAttributeMappingEntity>> GetProductsMappedAttributesListDAL(ProductAttributeMappingEntity FormData);
        Task<List<ManufacturerEntity>> GetManufacturerListDAL(ManufacturerEntity FormData);
        Task<List<ShippingMethodEntity>> GetShippingMethodsListDAL(ShippingMethodEntity FormData);
        Task<List<WarehouseEntity>> GetWarehousesListDAL(WarehouseEntity FormData);
        Task<List<ProductAttributeEntity>> GetProductAttributesListDAL(ProductAttributeEntity FormData);
        Task<List<HtmlDropDownRemoteData>> GetProductAttributeValuesByAttributeIDDAL(int ProductAttributeId);
        Task<List<ProductTagsEntity>> GetProductTagsDAL(ProductTagsEntity FormData);
        Task<ServicesResponse>? InsertUpdateProductTagDAL(ProductTagRequestForm FormData);
        Task<ServicesResponse>? InsertUpdateProductDAL(CreateProductRequestFormInternal FormData);
        Task<List<ProductEntity>> GetProductsListDAL(ProductEntity FormData);
        Task<ProductEntity?> GetProductDetailsByIdDAL(int ProductId);
        Task<List<ProductPicturesMappingEntity>> GetProductsMappedImagesListDAL(ProductPicturesMappingEntity FormData);
        Task<List<ColorEntity>> GetColorsListDAL(ColorEntity FormData);
        Task<ServicesResponse>? UpdateProductImgColorMappingDAL(ProductImgColorMappingRequestForm FormData);
        Task<ServicesResponse>? InsertUpdateManufacturerDAL(ManufacturerRequestForm FormData);
        Task<ServicesResponse>? InsertUpdateColorDAL(ColorRequestForm FormData);
        Task<List<InventoryMainEntity>> GetInventoryListDAL(InventoryMainEntity FormData);
        Task<List<ProductMappedAttributesForInventory>> GetProductMappedAttributesForInventoryDAL(int ProductId);
        Task<InventoryMainEntity?> GenInventoryMainDetailByIdDAL(int InventoryId);
        Task<ServicesResponse>? InsertUpdateInventoryMainDAL(InventoryMainRequestForm FormData);
        Task<List<InventoryMethodsEntity>> GetInventoryMethodsListDAL(InventoryMethodsEntity FormData);
        Task<ServicesResponse>? InsertUpdateProductMappedAttributesForInventoryDAL(ProductMappedAttributesForInventory FormData);
        Task<ServicesResponse>? InsertUpdateWarehouseDAL(WarehouseRequestForm FormData);
    }
}
