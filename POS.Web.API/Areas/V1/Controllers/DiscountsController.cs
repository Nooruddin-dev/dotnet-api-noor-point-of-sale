using DAL.Repository.IServices;
using DAL.Repository.Services;
using Entities.DBModels;
using Entities.DBModels.Discounts;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Discounts.RequestForms;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Helpers.CommonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/discounts")]
    [ApiController]
    [Area("V1")]
    public class DiscountsController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IDiscountsServicesDAL _discountsServicesDAL;


        public DiscountsController(ICommonServicesDAL commonServicesDAL, IDiscountsServicesDAL discountsServicesDAL)
        {
            this._commonServicesDAL = commonServicesDAL;
            this._discountsServicesDAL = discountsServicesDAL;
        }

        [Route("get-discounts-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetDiscountsList(int? DiscountId, string? Title, string? CouponCode, int? DiscountTypeId, int? IsActiveSelected, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                DiscountEntity FormData = new DiscountEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    Title = Title ?? "",
                    DiscountId = DiscountId ?? 0,
                    CouponCode = CouponCode ?? "",
                    DiscountTypeId = DiscountTypeId ?? 0,
                    IsActiveSelected = IsActiveSelected,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _discountsServicesDAL.GetDiscountsListDAL(FormData);

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

        [Route("get-discount-types-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetDiscountTypesList(int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                DiscountTypeEntity FormData = new DiscountTypeEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                  
                };
                var result = await _discountsServicesDAL.GetDiscountTypesListDAL(FormData);

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


        [Route("get-discounts-mapped-products")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetDiscountsMappedProducts(int DiscountId, int? ProductId, string? ProductName, int PageNo = 1, int PageSize = 10)
        {
            try
            {
               // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                DiscountProductsMappingEntity FormData = new DiscountProductsMappingEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    DiscountId = DiscountId,
                    ProductId = ProductId ?? 0,
                    ProductName = ProductName,

                };
                var result = await _discountsServicesDAL.GetDiscountProductsMappingListDAL(FormData);

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


        [Route("get-products-list-for-discount")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductsListForDiscount(int? ProductId,  string? ProductName, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductEntity FormData = new ProductEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ProductId = ProductId ?? 0,
                    ProductName = ProductName,

                };
                var result = await _discountsServicesDAL.GetProductsListForDiscountDAL(FormData);

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


        [Route("insert-update-discount")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateDiscount([FromBody] DiscountRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.Title))
                {
                    return BadRequest("Title is required");
                }

                if (model.DiscountTypeId < 1)
                {
                    return BadRequest("Discount type id is required");
                }

                if (model.DiscountValueType < 1)
                {
                    return BadRequest("Discount value type is required");
                }
                if (model.DiscountValue < 1)
                {
                    return BadRequest("Discount value is required");
                }

                if (model != null && model.IsCouponCodeRequired == true)
                {
                    if (String.IsNullOrWhiteSpace(model.CouponCode))
                    {
                        return BadRequest("Coupon code required!");
                    }
                }


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _discountsServicesDAL.InsertUpdateDiscountDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }

        [Route("get-discount-detail-by-id/{DiscountId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetDiscountDetailById(int DiscountId)
        {
            try
            {
              
                var result = await _discountsServicesDAL.GetDiscountDetailByIdDAL(DiscountId);

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


        [Route("get-discounts-mapped-categories")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetDiscountsMappedCategories(int DiscountId, int? CategoryId, string? CategoryName, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                DiscountCategoryMappingEntity FormData = new DiscountCategoryMappingEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    DiscountID = DiscountId,
                    CategoryId = CategoryId ?? 0,
                    CategoryName = CategoryName,

                };
                var result = await _discountsServicesDAL.GetDiscountsMappedCategoriesListDAL(FormData);

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


        [Route("get-categories-list-for-discount")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetCategoriesListForDiscount(int? CategoryId, string? CategoryName, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductCategoriesEntity FormData = new ProductCategoriesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    CategoryId = CategoryId ?? 0,
                    Name = CategoryName,

                };
                var result = await _discountsServicesDAL.GetCategoriesListForDiscountDAL(FormData);

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


    }
}
