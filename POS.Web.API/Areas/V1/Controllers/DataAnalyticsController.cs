using DAL.Repository.IServices;
using DAL.Repository.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Entities.DBModels;
using Helpers.CommonHelpers;
using Helpers.ConversionHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/data-analytics")]
    [ApiController]
    [Area("V1")]
    public class DataAnalyticsController : ControllerBase
    {

        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IDataAnalyticsServicesDAL _dataAnalyticsServicesDAL;
        private readonly IConstants _constants;


        public DataAnalyticsController(ICommonServicesDAL commonServicesDAL, IUsersServicesDAL usersServicesDAL,
            IConstants constants, IDataAnalyticsServicesDAL dataAnalyticsServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;
            _usersServicesDAL = usersServicesDAL;
            _dataAnalyticsServicesDAL = dataAnalyticsServicesDAL;
            _constants = constants;
        }

        [Route("get-orders-earnings-this-month")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetOrdersEarningThisMonthAnalytics()
        {
            try
            {
               
                var result = await _dataAnalyticsServicesDAL.GetOrdersEarningThisMonthAnalyticsDAL();

                if (result == null)
                    return NotFound();

                if (result.TopCategoriesAnalytics != null)
                {
                    foreach (var item in result.TopCategoriesAnalytics)
                    {
                        item.OrderMonthName = CommonConversionHelper.GetMonthName(item.OrderMonth);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }
        }

        [Route("get-today-hero-products")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTodayHeroProductsAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetTodayHeroProductsAnalyticsDAL();

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

        [Route("get-overall-summary")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetOverAllSummaryAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetOverAllSummaryAnalyticsDAL();

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

        [Route("get-today-active-orders")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTodayActiveOrdersAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetTodayActiveOrdersAnalyticsDAL();

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


        [Route("get-monthly-sales-data")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetMonthlySaleAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetMonthlySaleAnalyticsDAL();

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


        [Route("get-top-sale-products")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTopSaleProductsAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetTopSaleProductsAnalyticsDAL();

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

        [Route("get-top-trend-analytics")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetTopTrendsAnalytics()
        {
            try
            {

                var result = await _dataAnalyticsServicesDAL.GetTopTrendsAnalyticsDAL();

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
