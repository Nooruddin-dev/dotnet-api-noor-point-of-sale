using Azure;
using DAL.Repository.IServices;
using DAL.Repository.Services;
using Entities.DBModels;
using Entities.DBModels.Common;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.Common.RequestForms;
using Entities.ModuleSpecificModels.Setting.RequestForms;
using Helpers.CommonHelpers;
using Helpers.CommonHelpers.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/common")]
    [ApiController]
    [Area("V1")]
    public class CommonController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;


        public CommonController(ICommonServicesDAL commonServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;

        }


      

        /// <summary>
        /// Function for deleting any kind of record dynamically
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entiryColumnName"></param>
        /// <param name="entityRowId"></param>
        /// <param name="SqlDeleteType"></param>
        /// <returns></returns>
        [Route("delete-record/{EntityName}/{EntiryColumnName}/{EntityRowId}/{SqlDeleteTypeId}")]
        [HttpDelete]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        public async Task<IActionResult> DeleteAnyRecord(string entityName,string entiryColumnName,int entityRowId, int SqlDeleteTypeId = (short)SqlDeleteTypes.PlainTableDelete)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);


                if (String.IsNullOrWhiteSpace(entityName) || String.IsNullOrWhiteSpace(entiryColumnName) || entityRowId < 1)
                {
                    return BadRequest("Invalid information. Please try again!");
                }

                bool result = await _commonServicesDAL.DeleteAnyRecordDAL(entityRowId, entiryColumnName, entityName, SqlDeleteTypeId);
                if (result)
                {
                    return Ok(new { Response = "Deleted Successfully!" });
                    }
                else
                {
                    return Ok(new { Response = "An error occured on server side." });
                }
            }
            catch (Exception ex)
            {

                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }


        [Route("get-site-notifications")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetSiteGeneralNotifications(Boolean? IsReadNullProperty, int? NotificationId, string? Title, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                SiteGeneralNotificationsEntity FormData = new SiteGeneralNotificationsEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    IsReadNullProperty = IsReadNullProperty,
                    NotificationId = NotificationId ?? 0,

                };

                var result = await _commonServicesDAL.GetSiteGeneralNotificationsDAL(FormData);

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

        [Route("mark-notification-as-read")]
        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead([FromBody] NotificationReadRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || model.NotificationId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Notification Id is required.";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }

               
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _commonServicesDAL.MarkNotificationAsReadDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }


        [Route("get-payment-methods")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethodsList(int? PaymentMethodId, string? PaymentMethodName, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                PaymentMethodEntity FormData = new PaymentMethodEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    PaymentMethodId = PaymentMethodId ?? 0,
                    PaymentMethodName = PaymentMethodName,

                };

                var result = await _commonServicesDAL.GetPaymentMethodsListDAL(FormData);

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
