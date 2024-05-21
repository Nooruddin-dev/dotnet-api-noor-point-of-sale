using DAL.Repository.IServices;
using DAL.Repository.Services;
using Entities.DBModels;
using Entities.DBModels.Setting;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Entities.ModuleSpecificModels.Setting.RequestForms;
using Helpers.CommonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{

    [Route("api/v1/setting")]
    [ApiController]
    [Area("V1")]
    public class SettingController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IFilesHelpers _filesHelpers;
        private readonly ISettingServicesDAL _settingServicesDAL;

        public SettingController(ICommonServicesDAL commonServicesDAL, IUsersServicesDAL usersServicesDAL, IFilesHelpers filesHelpers,ISettingServicesDAL settingServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;
            _usersServicesDAL = usersServicesDAL;
            _filesHelpers = filesHelpers;
            _settingServicesDAL = settingServicesDAL;
        }

        [Route("get-tax-categories")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTaxCategories(int? TaxCategoryId, string? CategoryName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                TaxCategoriesEntity FormData = new TaxCategoriesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    TaxCategoryId = TaxCategoryId ?? 0,
                    CategoryName = CategoryName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _settingServicesDAL.GetTaxCategoriesDAL(FormData);

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
       
        [Route("get-tax-rules")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTaxRules(int? TaxRuleId, int? TaxCategoryId, string? CategoryName, int? CountryId, string? TaxRuleType, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                TaxRulesEntity FormData = new TaxRulesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    TaxRuleId = TaxRuleId ?? 0,
                    TaxCategoryId = TaxCategoryId ?? 0,
                    CategoryName = CategoryName,
                    CountryId = CountryId ?? 0,
                    TaxRuleType = TaxRuleType,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _settingServicesDAL.GetTaxRulesDAL(FormData);

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

        [Route("insert-update-tax-rule")]
        [HttpPost]
        public async Task<IActionResult> InsertUpdateTaxRule([FromBody] TaxRuleRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || model.TaxCategoryId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Tax category is required.";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

                if (model.CountryId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Country is required.";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

                if (string.IsNullOrEmpty(model.TaxRuleType))
                {
                    response.Success = false;
                    response.ResponseMessage = "Tax rule type is required.";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

                if (model.TaxRuleType != "For Product" && model.TaxRuleType != "For Order")
                {
                    response.Success = false;
                    response.ResponseMessage = "Tax rule type has invalid value!";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _settingServicesDAL.InsertUpdateTaxRuleDAL(model);

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
