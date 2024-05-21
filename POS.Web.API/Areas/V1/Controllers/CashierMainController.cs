using Azure;
using DAL.Repository.IServices;
using DAL.Repository.Services;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using Entities.CommonModels;
using Entities.DBModels;
using Entities.DBModels.CashierMain;
using Entities.DBModels.SalesManagement;
using Entities.ModuleSpecificModels.CashierMain;
using Entities.ModuleSpecificModels.CashierMain.RequestForms;
using Entities.ModuleSpecificModels.Common;
using Entities.ModuleSpecificModels.ProductsCatalog.RequestForms;
using Helpers.ApiHelpers;
using Helpers.AuthorizationHelpers.JwtTokenHelper;
using Helpers.CommonHelpers;
using Helpers.CommonHelpers.Enums;
using Helpers.ConversionHelpers;
using Helpers.ConversionHelpers.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using System.Net.Mail;

namespace POS.Web.API.Areas.V1.Controllers
{
    [Route("api/v1/cashier-main")]
    [ApiController]
    [Area("V1")]
    public class CashierMainController : ControllerBase
    {

        private readonly ICommonServicesDAL _commonServicesDAL;
        private readonly ICashierMainServicesDAL _cashierMainServicesDAL;
        private readonly IUsersServicesDAL _usersServicesDAL;
        private readonly IConstants _constants;
        private readonly ICalculationHelper _calculationHelper;
        private readonly ISalesManagementServicesDAL _salesManagementServicesDAL;



        public CashierMainController(ICommonServicesDAL commonServicesDAL, 
            ICashierMainServicesDAL cashierMainServicesDAL, IConstants constants, ICalculationHelper calculationHelper,
            IUsersServicesDAL usersServicesDAL, ISalesManagementServicesDAL salesManagementServicesDAL)
        {
            _commonServicesDAL = commonServicesDAL;
            _cashierMainServicesDAL = cashierMainServicesDAL;
            _constants = constants;
            _calculationHelper = calculationHelper;
            _usersServicesDAL = usersServicesDAL;
            _salesManagementServicesDAL = salesManagementServicesDAL;
        }


        [Route("get-point-of-sale-categories")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetPointOfSaleCategories(int? CategoryId, string? CategoryName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                PointOfSaleCategoriesEntity FormData = new PointOfSaleCategoriesEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    CategoryId = CategoryId ?? 0,
                    CategoryName = CategoryName,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var result = await _cashierMainServicesDAL.GetPointOfSaleCategoriesDAL(FormData);

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


        [Route("get-point-of-sale-products")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetPointOfSaleProducts(int? CategoryId, int? ManufacturerId, string? ProductName = "", string? OrderByColumnName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                ProductPointOfSaleEntity FormData = new ProductPointOfSaleEntity
                {
                    PageNo = PageNo,
                    PageSize = PageSize,
                    CategoryId = CategoryId ?? 0,
                    ManufacturerId = ManufacturerId ?? 0,
                    ProductName = ProductName,
                    OrderByColumnName = OrderByColumnName,
                    BusnPartnerId = busnPartnerIdHeader
                };
                var resultWithoutDiscountInfo = await _cashierMainServicesDAL.GetPointOfSaleProductsDAL(FormData);

                var resultWithDiscountInfo = await _calculationHelper.CalculateDiscountsForProducts(resultWithoutDiscountInfo);


                if (resultWithDiscountInfo == null)
                    return NotFound();
                return Ok(resultWithDiscountInfo);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-pos-product_detail/{ProductId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductDetailForPointOfSaleById(int ProductId)
        {
            try
            {
               // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                var productDetail = await _cashierMainServicesDAL.GetProductDetailForPointOfSaleByIdDAL(ProductId);
                if (productDetail != null)
                {
                    List<ProductPointOfSaleEntity> productsPointOfSaleEntities = new List<ProductPointOfSaleEntity>();
                    productsPointOfSaleEntities.Add(productDetail);

                    //--Calcualte discount for productsPointOfSaleEntities
                    var resultWithDiscountInfo = await _calculationHelper.CalculateDiscountsForProducts(productsPointOfSaleEntities);

                    if (resultWithDiscountInfo != null && resultWithDiscountInfo.Count() > 0)
                    {
                        productDetail = resultWithDiscountInfo?.FirstOrDefault();
                    }

                }

                if (productDetail == null)
                    return NotFound();
                return Ok(productDetail);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }


        [Route("get-products-list-by-ids/{ProductIdsCommaSeperated}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetProductsListByIds(string ProductIdsCommaSeperated)
        {
            try
            {
                // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                var products = await _cashierMainServicesDAL.GetProductsListByIdsDAL(ProductIdsCommaSeperated);
                if (products != null && products.Count() > 0)
                {
                  
                    //--Calcualte discount for productsPointOfSaleEntities
                    var resultWithDiscountInfo = await _calculationHelper.CalculateDiscountsForProducts(products);

                    if (resultWithDiscountInfo != null && resultWithDiscountInfo.Count() > 0)
                    {
                        products = resultWithDiscountInfo?.ToList();
                    }

                }

                if (products == null)
                    return NotFound();
                return Ok(products);
            }
            catch (Exception ex)
            {

                await _commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.Source);
                return BadRequest(ex.Message);
            }


        }

        [Route("get-customer-info-for-pos-cart")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetCustomerInfoForPosCart(string? SearchKeyword, int CustomerId = 0)
        {
            try
            {
               // int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                var result = await _cashierMainServicesDAL.GetCustomerInfoForPosCartDAL(SearchKeyword, CustomerId);

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


        [Route("post-customer-order")]
        [HttpPost]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        public async Task<IActionResult> PostCustomerOrder( [FromBody] CustomerOrderRequestForm FormData)
        {
            ServicesResponse? response = new ServicesResponse();


            try
            {
               
                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                FormData.BusnPartnerId = busnPartnerIdHeader;
                FormData.OrderGuid = Guid.NewGuid().ToString();

                StripeConfiguration.ApiKey = _constants.GetAppSettingKeyValue("AppSetting", "StripeSecretKey");

                // strip implementation url
                // https://stripe.com/docs/payments/accept-a-payment-charges?html-or-react=react

                var paymentToken = FormData.PaymentToken;
                int PaymentMethod = FormData.PaymentMethod;

               



                //--strip testing card urls: https://stripe.com/docs/testing?numbers-or-method-or-token=card-numbers#visa


                #region new
                string CouponCode = FormData.CouponCode ?? "";
                string? Description = "Order of customer id: " + FormData.CustomerId + " at " + DateTime.Now.ToString();
                string cartJsonData = "[]";
                decimal? OrderTotal = FormData.OrderTotal;
          


                cartJsonData = FormData.CartJsonData ?? "[]";
                var cartCustomerProducts = new List<CartCustomerProducts>();
                cartCustomerProducts = JsonConvert.DeserializeObject<List<CartCustomerProducts>>(cartJsonData);
                if (cartCustomerProducts != null)
                {
                   
                   
                    if (cartCustomerProducts != null)
                    {
                        //--Iterate over all items
                        foreach (var cartItem in cartCustomerProducts)
                        {
                            if (cartItem?.productSelectedAttributes != null && cartItem.productSelectedAttributes.Count() > 0)
                            {
                                //--Iterate over all selected attributes for this product and then set additional price
                                foreach (var prodAtttributeSelected in cartItem?.productSelectedAttributes)
                                {
                                    var selectedRow = cartItem?.productAttributesForInventory?.Where(x => x.ProductAttributeID == prodAtttributeSelected.ProductAttributeID && Convert.ToInt32(x.AttributeValue ?? "0") == prodAtttributeSelected.PrimaryKeyValue)?.FirstOrDefault();
                                    prodAtttributeSelected.AttrAdditionalPrice = (selectedRow?.AdditionalPrice ?? 0) * (cartItem?.Quantity ?? 0);
                                }
                            }
                        }

                        if (OrderTotal == null || OrderTotal == 0 || OrderTotal < 0)
                        {
                            throw new InvalidOperationException("Invalid order total amount!");
                        }

               
                        FormData.CurrencyCode = CommonConversionHelper.GetDefaultCurrencyCode()?.ToLower() ?? "usd";

                        //--Convert again the final order products to json
                        FormData.CartJsonData = JsonConvert.SerializeObject(cartCustomerProducts);


                        if (PaymentMethod == (short)PaymentMethodsEnum.Stripe)
                        {
                            if (String.IsNullOrWhiteSpace(paymentToken))
                            {
                                throw new InvalidOperationException("stripe payment token is empty!");
                            }

                            string currency = CommonConversionHelper.GetDefaultCurrencyCode()?.ToLower() ?? "usd";

                            var options = new ChargeCreateOptions
                            {
                                Amount = currency == "usd" ? (long)(OrderTotal * 100) : (long)OrderTotal,
                                Currency = currency,
                                Description = Description,
                                Source = paymentToken,
                            };
                            var service = new ChargeService();
                            var charge = service.Create(options);

                            if (charge.Status == "succeeded")
                            {
                                FormData.Description = Description;
                                FormData.StripeStatus = charge.Status;
                                FormData.StripeResponseJson = charge.StripeResponse.Content;
                                FormData.StripeBalanceTransactionId = charge.BalanceTransactionId;
                                FormData.StripeChargeId = charge.Id;
                                FormData.PayPalResponseJson = string.Empty;



                                //--save the information in data base
                                response = await _cashierMainServicesDAL.SaveCustomerOrderInDbWithRetryDAL(FormData);


                            }
                            else
                            {
                                response.Success = false;
                                response.ResponseMessage = "An error occured. Please try again";
                               
                            }
                        }
                        else if (PaymentMethod == (short)PaymentMethodsEnum.CashOnDelivery)
                        {
                          

                            FormData.Description = Description;
                            FormData.StripeStatus = "";
                            FormData.StripeResponseJson = "";
                            FormData.StripeBalanceTransactionId = "";
                            FormData.StripeChargeId = "";
                            FormData.PayPalResponseJson = string.Empty;

                            //--save the information in data base
                            response = await _cashierMainServicesDAL.SaveCustomerOrderInDbWithRetryDAL(FormData);
                          
                           

                        }
                        else if (PaymentMethod == (short)PaymentMethodsEnum.PayPal)
                        {
                           
                            FormData.Description = Description;
                            FormData.StripeStatus = "";
                            FormData.StripeResponseJson = "";
                            FormData.StripeBalanceTransactionId = "";
                            FormData.StripeChargeId = "";
                            FormData.PayPalResponseJson = FormData.PayPalOrderConfirmJson;

                            //--save the information in data base
                            response = await _cashierMainServicesDAL.SaveCustomerOrderInDbWithRetryDAL(FormData);


                        }
                        else
                        {
                            response.Success = false;
                            response.ResponseMessage = "An error occured. Please try again";
                        }

                        #region Send email to customer if order placed successfully
                        try
                        {
                            var userData = await _usersServicesDAL.GetBusinessPartnerByIdDAL(FormData.CustomerId);

                            //List<EmailAddressEntity> emailAddresses = new List<EmailAddressEntity>();
                            //string content = String.Format("Your order has been placed successfully. Order total amount is: {0} {1}. {2}Please check your order history page for further details. {2}{2} Thanks", (CommonConversionHelper.GetDefaultCurrencyCode()?.ToLower() ?? "USD"), OrderTotal, Environment.NewLine);
                            //emailAddresses.Add(new EmailAddressEntity { DisplayName = "User", Address = userData?.EmailAddress });
                            //string SiteTitle = _configuration.GetSection("AppSetting").GetSection("WebsiteTitle").Value;
                            //var message = new EmailMessage(emailAddresses, "New Order Placed", content, String.Format("{0} , New Order Placed", SiteTitle));
                            //_emailSender.SendEmail(message);
                        }
                        catch (Exception ex)
                        {
                            //-- Do nothing
                            var noThing = ex.Message;
                        }
                        #endregion

                        //--Get orderId by OrderGuid
                        response.PrimaryKeyValue = await _cashierMainServicesDAL.GetOrderIdByOrderGuidDAL(FormData.OrderGuid);

                    }
                    else
                    {
                        response.Success = false;
                        response.ResponseMessage = "An error occured. Please try again";

                    }
                }
                else
                {
                    response.Success = false;
                    response.ResponseMessage = "An error occured. Please try again";
                }

                #endregion



            }
            catch (Exception ex)
            {

                #region log error
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);
                #endregion

                return BadRequest(ex.Message);
            }





            return Ok(new { Response = response });


        }

        [Route("get-cashier-orders-list")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetCashierOrdersList(int? OrderId, string? ProductName, int? LatestStatusId, int? CustomerId, string? TimeRange, string? CustomerName = "", int PageNo = 1, int PageSize = 10)
        {
            try
            {
                //int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

               
                var result = await _cashierMainServicesDAL.GetCashierOrdersListDAL(OrderId, ProductName, LatestStatusId, CustomerId, CustomerName, TimeRange, PageNo, PageSize);

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

        [Route("update-order-status")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatusRequestForm model)
        {

            ServicesResponse? response = new ServicesResponse();

            try
            {
                if (model == null || model.OrderId < 1)
                {
                    return BadRequest("Order id is required");
                }
                if (model.LatestStatusId < 1)
                {
                    return BadRequest("Order status is required");
                }

                int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);

                model.BusnPartnerId = busnPartnerIdHeader;


                response = await _cashierMainServicesDAL.UpdateOrderStatusDAL(model);

                return Ok(new { Response = response });
            }
            catch (Exception ex)
            {
                await this._commonServicesDAL.LogRunTimeExceptionDAL(ex.Message, ex.StackTrace, ex.StackTrace);

                return BadRequest(ex.Message);
            }


        }


        [Route("get-order-details/{OrderId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetOrderDetailById(int OrderId)
        {
            try
            {
                OrderDetailMainModel result = new OrderDetailMainModel();
                //int busnPartnerIdHeader = GlobalUrlHelper.GetBusnPartnerIdFromApiHeader(HttpContext);


                result.OrderMainDetail = await _cashierMainServicesDAL.GetOrderMainDetailsByIdDAL(OrderId);
                result.OrderShippingDetails = await _cashierMainServicesDAL.GetOrderShippingDetailsDAL(OrderId);
                result.OrderShippingMaster = await _cashierMainServicesDAL.GetOrderShippingMasterDAL(OrderId);
                result.OrderItems = await _cashierMainServicesDAL.GetOrderItemsDAL(OrderId);
                result.OrderPaymentDetails = await _cashierMainServicesDAL.GetOrderPaymentDetailsDAL(OrderId);
                result.orderNotesEntities = await _cashierMainServicesDAL.GetOrderNotesDAL(OrderId);


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

        [Route("get-order-item-variants/{OrderId}/{OrderItemId}")]
        //[ServiceFilter(typeof(CustomerApiCallsAuthorization))]
        [HttpGet]
        public async Task<IActionResult> GetOrderItemsVariants(int OrderId, int OrderItemId)
        {
            try
            {
               
                var result = await _cashierMainServicesDAL.GetOrderItemsVariantsDAL(OrderId);
                if (result != null)
                {
                    result = result.Where(x => x.OrderItemID == OrderItemId).ToList();
                }


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
