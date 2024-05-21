using DAL.Repository.IServices;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Entities.CommonModels;
using Entities.DBModels;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Helpers.CommonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/products-catalog")]
    [ApiController]
    [Area("V1")]
    public class ProductsCatalogController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IProductsCatalogServicesDAL _productsCatalogServicesDAL;
        private readonly IFilesHelpers _filesHelpers;
        private readonly IConstants _constants;



        public ProductsCatalogController(ICommonServicesDAL commonServicesDAL, IUsersServicesDAL usersServicesDAL,
            IProductsCatalogServicesDAL productsCatalogServicesDAL, IFilesHelpers filesHelpers, IConstants constants)
        {
            _commonServicesDAL = commonServicesDAL;
            _usersServicesDAL = usersServicesDAL;
            _productsCatalogServicesDAL = productsCatalogServicesDAL;
            _filesHelpers = filesHelpers;
            _constants = constants;
        }

        [Route("get-product-categories")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductCategoriesList(int? CategoryId, string? CategoryName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductCategoriesEntity FormData = new ProductCategoriesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    CategoryId = CategoryId ?? 0,
                    Name = CategoryName,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetProductCategoriesListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("insert-update-product-category")]
        [HttpPost]
        //public async Task<IActionResult> InsertUpdateProductCategory([FromBody] ProductCategoriesRequestForm model)
        public async Task<IActionResult> InsertUpdateProductCategory([FromForm] int? CategoryId, [FromForm] string? Name,
            [FromForm] int? ParentCategoryID,  [FromForm] int? AttachmentId, [FromForm] bool? IsActive, IFormFile? CategoryImage)
        {

            ServicesResponse? response = new ServicesResponse();
            ProductCategoriesRequestForm model = new ProductCategoriesRequestForm
            {
                CategoryId = CategoryId ?? 0,
                Name = Name,
                ParentCategoryID = ParentCategoryID,
                AttachmentId = AttachmentId,
                CategoryImage = CategoryImage,
                IsActive = IsActive
            };

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Name))
                {
                    return BadRequest("Category name is required");
                }
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;
                #region image checking
                if (model.CategoryImage != null)
                {
                    string url = await _filesHelpers.SaveFileToDirectory(model.CategoryImage, null);
                    if (!String.IsNullOrWhiteSpace(url))
                    {
                        AttachmentEntity atch = new AttachmentEntity();
                        atch.AttachmentUrl = url;
                        atch.AttachmentName = model.CategoryImage.FileName;
                        atch.BusnPartnerId = busnPartnerIdHeader;

                        if (model.AttachmentId != null && model.AttachmentId > 0) //--update case
                        {
                            atch.AttachmentId = Convert.ToInt32(model.AttachmentId);


                            //--save attachment url in database
                            int AttachmentIdAttachmentTable = await this._commonServicesDAL.SaveUpdateAttachmentDAL(atch);
                        }
                        else
                        {
                            //--save attachment url in database
                            int AttachmentIdAttachmentTable = await this._commonServicesDAL.SaveUpdateAttachmentDAL(atch);

                            atch.AttachmentId = AttachmentIdAttachmentTable;

                            model.AttachmentId = AttachmentIdAttachmentTable;
                        }



                    }



                }
                else{
                    model.AttachmentId = model.CategoryId > 0 ? model.AttachmentId : null;
                }
                #endregion



                response = await _productsCatalogServicesDAL.InsertUpdateProductCategoryDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("get_products_mapped_attributes_list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductsMappedAttributesList(int? ProductID, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductAttributeMappingEntity FormData = new ProductAttributeMappingEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ProductID = ProductID ?? 0,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetProductsMappedAttributesListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-manufacturer-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetManufacturerList(int? ManufacturerID, string? Name = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ManufacturerEntity FormData = new ManufacturerEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ManufacturerId = ManufacturerID ?? 0,
                    Name = Name ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetManufacturerListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-shipping-methods")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetShippingMethodsList(int? ShippingMethodId, string? MethodName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ShippingMethodEntity FormData = new ShippingMethodEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ShippingMethodId = ShippingMethodId ?? 0,
                    MethodName = MethodName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetShippingMethodsListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }


        [Route("get-warehouses-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetWarehousesList(int? WarehouseId, string? WarehouseName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                WarehouseEntity FormData = new WarehouseEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    WarehouseId = WarehouseId ?? 0,
                    WarehouseName = WarehouseName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetWarehousesListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }


        [Route("get-inventory-methods-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetInventoryMethodsList(int? InventoryMethodId, string? InventoryMethodName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                InventoryMethodsEntity FormData = new InventoryMethodsEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    InventoryMethodId = InventoryMethodId ?? 0,
                    InventoryMethodName = InventoryMethodName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetInventoryMethodsListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }




        [Route("get-attributes-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductAttributesList(int? ProductAttributeId, string? AttributeName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductAttributeEntity FormData = new ProductAttributeEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ProductAttributeId = ProductAttributeId ?? 0,
                    AttributeName = AttributeName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetProductAttributesListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }



        [Route("get_product_attributes_values_by_id/{productAttributeId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductAttributeValuesByAttributeID(int productAttributeId)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

              
                var result = await _productsCatalogServicesDAL.GetProductAttributeValuesByAttributeIDDAL(productAttributeId);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }





         
        }

        [Route("get-product-tags")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductTags(int? TagId, string? TagName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductTagsEntity FormData = new ProductTagsEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    TagId = TagId ?? 0,
                    TagName = TagName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetProductTagsDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }


        [Route("insert-update-product-tag")]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateProductTag([FromBody] ProductTagRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();
           
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.TagName))
                {
                    return BadRequest("Tag name is required");
                }
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;
             

                response = await _productsCatalogServicesDAL.InsertUpdateProductTagDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("insert-update-product-product")]
        [HttpPost]
        //public async Task<IActionResult> InsertUpdateProductCategory([FromBody] ProductCategoriesRequestForm model)
        public async Task<IActionResult> InsertUpdateProductCategory([FromForm] CreateProductRequestForm FormData)
        {

            ServicesResponse? response = new ServicesResponse();
            CreateProductRequestFormInternal createProductRequestForm = new CreateProductRequestFormInternal();
            createProductRequestForm.createProductRequestForm = new CreateProductRequestForm();



            try
            {
                if (FormData == null || string.IsNullOrWhiteSpace(FormData.ProductName))
                {
                    return BadRequest("Product name is required");
                }

                if (string.IsNullOrWhiteSpace(FormData.ShortDescription))
                {
                    return BadRequest("Short desc is required");
                }

                if (FormData.VendorId == null || FormData.VendorId < 1)
                {
                    return BadRequest("Vendor is required");
                }


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                createProductRequestForm.BusnPartnerId = busnPartnerIdHeader;

                string ValidationMsg = "Form is valid";
                List<string> validationList = new List<string>();

                #region validation area

                ValidationMsg = FormData == null ? "Form is null!" : "Form is valid";
                validationList.Add(ValidationMsg);
                ValidationMsg = FormData != null && !String.IsNullOrWhiteSpace(FormData.ProductName) ? "Form is valid" : "Product name is required!";
                validationList.Add(ValidationMsg);
                ValidationMsg = FormData != null && !String.IsNullOrWhiteSpace(FormData.FullDescription) ? "Form is valid" : "Full Description is required!";
                validationList.Add(ValidationMsg);
                ValidationMsg = FormData != null && FormData.Price > 0 ? "Form is valid" : "Product price is required!";
                validationList.Add(ValidationMsg);
                ValidationMsg = FormData != null && FormData.VendorId > 0 ? "Form is valid" : "Vendor id is required!";
                validationList.Add(ValidationMsg);

                //-- For update case, do not check images files
                if (FormData?.ProductId != null && FormData.ProductId < 1)
                {
                    ValidationMsg = FormData != null && FormData.ProductImages != null && FormData.ProductImages.Length > 0 ? "Form is valid" : "Product image is required!";
                    validationList.Add(ValidationMsg);
                }
              

              
                if (validationList.Count() > 0 && validationList.Where(a => a != "Form is valid").ToList().Count > 0)
                {
                    response.Success = false;
                    response.ResponseMessage = "Please fill all required fields!";
                    response.ValidationMessages = validationList;
                    return Ok(new { Response = response });
                }

                #endregion

                createProductRequestForm.createProductRequestForm = FormData;

                #region image file conversion secion
                List<ImageFileInfo> ImageFileInfosList = new List<ImageFileInfo>();
                if (FormData != null && FormData.ProductImages != null && FormData.ProductImages.Length > 0)
                {
                    foreach (IFormFile photo in FormData.ProductImages)
                    {
                        string ProductsImagesDirectory = _constants.GetAppSettingKeyValue("AppSetting", "ProductsImagesDirectory");
                        string url = await _filesHelpers.SaveFileToDirectory(photo, ProductsImagesDirectory);

                        ImageFileInfosList.Add(new ImageFileInfo { ImageName = photo.FileName, ImageGuidName = "", ImageURL = url });
                        createProductRequestForm.ProductImagesJson = JsonConvert.SerializeObject(ImageFileInfosList);
                    }
                }
                #endregion


                response = await _productsCatalogServicesDAL.InsertUpdateProductDAL(createProductRequestForm);


                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }


        [Route("get-products-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductsList(int? productId, bool? isActive, string? productName = "", int pageNo = 1, int pageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductEntity FormData = new ProductEntity
                {
                    PageNo = pageNo,
                    PageSize = pageSize,
                    ProductId = productId ?? 0,
                    ProductName = productName,
                    IsActive = isActive,
                    BusnPartnerId = busnPartnerIdHeader
                };
                //   var result = await _productsCatalogServicesDAL.GetProductTagsDAL(FormData);
                var result = await _productsCatalogServicesDAL.GetProductsListDAL(FormData);


                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get_product_detail_by_id/{productId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductDetailsById(int productId)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);


                var result = await _productsCatalogServicesDAL.GetProductDetailsByIdDAL(productId);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }






        }

        [Route("get_products_mapped_images")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductsMappedImagesList(int? productId, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                if (productId == null || productId < 1)
                {
                    return BadRequest("ProductId is required!");
                }
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductPicturesMappingEntity FormData = new ProductPicturesMappingEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ProductId = productId ?? 0,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetProductsMappedImagesListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-colors-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetColorsList(int? ColorId, string? ColorName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ColorEntity FormData = new ColorEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ColorId = ColorId ?? 0,
                    ColorName = ColorName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetColorsListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("update-product-image-color-mapping")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> UpdateProductImgColorMapping([FromBody] ProductImgColorMappingRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _productsCatalogServicesDAL.UpdateProductImgColorMappingDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

      
        [Route("insert-update-color")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateColor([FromBody] ColorRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.ColorName))
                {
                    return BadRequest("Color name is required");
                }

                if (model == null || string.IsNullOrWhiteSpace(model.HexCode))
                {
                    return BadRequest("Hex Code is required");
                }


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _productsCatalogServicesDAL.InsertUpdateColorDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("get-inventory-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetInventoryList(int? InventoryId, int? ProductId, string? ProductName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                InventoryMainEntity FormData = new InventoryMainEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    InventoryId = InventoryId ?? 0,
                    ProductId = ProductId ?? 0,
                    ProductName = ProductName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _productsCatalogServicesDAL.GetInventoryListDAL(FormData);

                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-product-inventory-items/{InventoryId}/{ProductId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductInventoryItemsById(int InventoryId, int ProductId)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                


                var productMappedAttributes = await _productsCatalogServicesDAL.GetProductMappedAttributesForInventoryDAL(ProductId);
                var inventoryMain = await _productsCatalogServicesDAL.GenInventoryMainDetailByIdDAL(InventoryId);

                if (productMappedAttributes == null && inventoryMain == null)
                    return NotFound();
                return Ok(new { productMappedAttributes , inventoryMain });
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("insert-update-inventory-main")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> insertUpdateInventoryMain([FromBody] InventoryMainRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
               


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _productsCatalogServicesDAL.InsertUpdateInventoryMainDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("insert-update-product-attribute-inventory")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateProductMappedAttributesForInventory([FromBody] InventoryItemsRequestForm FormData)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                List<ProductMappedAttributesForInventory> AttributesList = new List<ProductMappedAttributesForInventory>();
                if (!string.IsNullOrWhiteSpace(FormData?.attributeBasedInventoryDataJson))
                {
                    AttributesList = JsonConvert.DeserializeObject<List<ProductMappedAttributesForInventory>>(FormData?.attributeBasedInventoryDataJson);
                }

                if (AttributesList != null)
                {
                    foreach (var item in AttributesList)
                    {
                        var model = new ProductMappedAttributesForInventory
                        {
                            ProductAttributeMappingID = item.ProductAttributeMappingID,
                            ProductAttributeID = item.ProductAttributeID,
                            AttributeName = item.AttributeName,
                            AttributeDisplayName = item.AttributeDisplayName,
                            AttributeValue = item.AttributeValue,
                            AttributeValueDisplayText = item.AttributeValueDisplayText,
                            InventoryItemId = item.InventoryItemId,
                            InventoryId = FormData?.InventoryId ?? item.InventoryId,
                            ProductId = FormData?.ProductId ?? item.ProductId,
                            SellStartDatetimeUTC = item.SellStartDatetimeUTC,
                            SellEndDatetimeUTC = item.SellEndDatetimeUTC,
                            WarehouseId = item.WarehouseId,
                            InventoryMethodId = item.InventoryMethodId,
                            StockQuantity = item.StockQuantity,
                            IsBoundToStockQuantity = item.IsBoundToStockQuantity,
                            DisplayStockQuantity = item.DisplayStockQuantity,
                            OrderMinimumQuantity = item.OrderMinimumQuantity,
                            OrderMaximumQuantity = item.OrderMaximumQuantity,

                            BusnPartnerId = busnPartnerIdHeader
                        };

                        response = await _productsCatalogServicesDAL.InsertUpdateProductMappedAttributesForInventoryDAL(model);
                    }
                }
              


     
              

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("insert-update-warehouse")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateWarehouse([FromBody] WarehouseRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.WarehouseName))
                {
                    return BadRequest("Warehouse name is required");
                }

              
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _productsCatalogServicesDAL.InsertUpdateWarehouseDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

    }
}
