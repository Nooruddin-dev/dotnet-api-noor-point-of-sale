using DAL.Repository.IServices;
using Entities.DBModels;
using Entities.DBModels.SalesManagement;
using Helpers.CommonHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/sales-management")]
    [ApiController]
    [Area("V1")]
    public class SalesManagementController : ControllerBase
    {

        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly ISalesManagementServicesDAL _salesManagementServicesDAL;
        private readonly IFilesHelpers _filesHelpers;
        private readonly IConstants _constants;



        public SalesManagementController(ICommonServicesDAL commonServicesDAL, IUsersServicesDAL usersServicesDAL,
            IFilesHelpers filesHelpers, IConstants constants, ISalesManagementServicesDAL salesManagementServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;
            _usersServicesDAL = usersServicesDAL;
            _filesHelpers = filesHelpers;
            _constants = constants;
            _salesManagementServicesDAL = salesManagementServicesDAL;
        }



        [Route("get-order-status-types")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetOrderStatusTypes(int? StatusId, string? StatusName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                OrderStatusEntity FormData = new OrderStatusEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    StatusId = StatusId ?? 0,
                    StatusName = StatusName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _salesManagementServicesDAL.GetOrderStatusTypesDAL(FormData);

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
