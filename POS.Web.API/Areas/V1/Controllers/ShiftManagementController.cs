using DAL.Repository.IServices;
using DAL.Repository.Services;
using Entities.DBModels.ShiftManagement;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ShiftManagement.RequestForms;
using Helpers.CommonHelpers;
using Helpers.ConversionHelpers.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/shift-management")]
    [ApiController]
    [Area("V1")]
    public class ShiftManagementController : ControllerBase
    {
        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IConstants _constants;
        private readonly IShiftManagementServicesDAL _shiftManagementServicesDAL;



        public ShiftManagementController(ICommonServicesDAL commonServicesDAL, IConstants constants, 
            IUsersServicesDAL usersServicesDAL, IShiftManagementServicesDAL shiftManagementServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;
            _constants = constants;
            _usersServicesDAL = usersServicesDAL;
            _shiftManagementServicesDAL = shiftManagementServicesDAL;
        }


        [Route("get-cashier-shift-drawer-info")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetCashierShiftDrawerInfo(int? ShiftId, string? CashierName, int? ShiftStatusId, string? FromDate, string? ToDate, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                CashierShiftDrawerInfoEntity FormData = new CashierShiftDrawerInfoEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ShiftId = ShiftId ?? 0,
                    CashierNameOnlyForSearchPurpose = CashierName,
                    ShiftStatusId = ShiftStatusId,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _shiftManagementServicesDAL.GetCashierShiftDrawerInfoDAL(FormData);

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


        [Route("insert-update-cashier-shift-drawer")]
        [HttpPost]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        public async Task<IActionResult> InsertUpdateCashierShiftDrawer([FromBody] CashierShiftDrawerRequestForm model)
        {
            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null)
                {
                    return BadRequest("Invalide form");
                }

                if (model.ShiftNameId < 1)
                {
                    return BadRequest("Shift name is required!");
                }

                if (model.StartingCash < 1)
                {
                    return BadRequest("Staring cash is required!");
                }

                if (model.ReconciliationStatusId < 1)
                {
                    return BadRequest("Reconciliation status is required!");
                }

                if (model.ShiftCashDrawerId > 0 && (model.ShiftId == null || model.ShiftId < 1))
                {
                    return BadRequest("Shift id is required!");
                }


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _shiftManagementServicesDAL.InsertUpdateCashierShiftDrawerDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }

        }

        [Route("check-if-any-active-shift-exits")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> CheckIfAnyActiveShiftExists()
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                var result = await _shiftManagementServicesDAL.CheckIfAnyActiveShiftExistsDAL();

                if (result == false)
                    return Ok(new { activeShiftsExists = false });
                return Ok(new { activeShiftsExists = true });
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-shift-names-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetShiftNamesList(int? ShiftNameId, string? ShiftName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ShiftNamesEntity FormData = new ShiftNamesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ShiftNameId = ShiftNameId ?? 0,
                    ShiftName = ShiftName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _shiftManagementServicesDAL.GetShiftNamesListDAL(FormData);

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


        [Route("insert-update-shift-name")]
        [HttpPost]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        public async Task<IActionResult> InsertUpdateShiftName([FromBody] ShiftNamesRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {


                if (string.IsNullOrEmpty(model.ShiftName))
                {
                    response.Success = false;
                    response.ResponseMessage = "Shift name is required.";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                }


                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _shiftManagementServicesDAL.InsertUpdateShiftNameDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }



        [Route("get-shift-transaction-types")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetShiftTransactionTypes(int? CashTransactionTypeId, string? CashTransactionTypeName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ShiftCashTransactionTypesEntity FormData = new ShiftCashTransactionTypesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    CashTransactionTypeId = CashTransactionTypeId ?? 0,
                    CashTransactionTypeName = CashTransactionTypeName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _shiftManagementServicesDAL.GetShiftTransactionTypesDAL(FormData);

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

        [Route("get-shift-cash-drawer-reconciliation-statuses")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetShiftCashDrawerReconciliationStatuses(int? ReconciliationStatusId, string? ReconciliationStatusName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ShiftCashDrawerReconciliationStatusesEntity FormData = new ShiftCashDrawerReconciliationStatusesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    ReconciliationStatusId = ReconciliationStatusId ?? 0,
                    ReconciliationStatusName = ReconciliationStatusName ?? "",
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _shiftManagementServicesDAL.GetShiftCashDrawerReconciliationStatusesDAL(FormData);

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


        [Route("get-shift-cash-transaction-data")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetShiftCashTransactionData(int? TransactionId, int? CashDrawerId, int? CashTransactionTypeId, int? ShiftId,
            string? FromDate, string? ToDate, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ShiftCashTransactionsEntity FormData = new ShiftCashTransactionsEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    TransactionId = TransactionId ?? 0,
                    CashDrawerId = CashDrawerId ?? 0,
                    CashTransactionTypeId = CashTransactionTypeId ?? 0,
                    ShiftId = ShiftId ?? 0,
                    FromDate = FromDate,
                    ToDate = ToDate,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _shiftManagementServicesDAL.GetShiftCashTransactionDataDAL(FormData);

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


        [Route("insert-update-cashier-drawer-transaction")]
        [HttpPost]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        public async Task<IActionResult> InsertUpdateCashDrawerTransaction([FromBody] ShiftCashTransactionRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model.CashDrawerId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Cash drawer ID is required";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });

                }


                if (model.CashTransactionTypeId < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Cash transaction type is required";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                    
                }


                if (model.Amount < 1)
                {
                    response.Success = false;
                    response.ResponseMessage = "Amount is required";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                   
                }

                if (string.IsNullOrEmpty(model.Description))
                {
                    response.Success = false;
                    response.ResponseMessage = "Description is required";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                  
                }

                if (model.TransactionDate == null)
                {
                    response.Success = false;
                    response.ResponseMessage = "Transaction Date is required";
                    response.PrimaryKeyValue = null;
                    return Ok(new { Response = response });
                  
                }




                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _shiftManagementServicesDAL.InsertUpdateCashDrawerTransactionDAL(model);

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
