using DAL.Repository.IServices;
using Helpers.ApiHelpers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.AuthorizationHelpers.JwtTokenHelper
{
    public class CustomerApiCallsAuthorization :  ActionFilterAttribute
    {
        public const string DEVICE_TOKEN = "DeviceToken";
        public const string USER_ID = "UserID";
        public const string PASSWORD = "Password";
        public string? DeviceToken { get; set; }
        public string? UserID { get; set; }
        public string? Token { get; set; }
        NoorAppAPIResult bzResult = new NoorAppAPIResult();
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isAuthrizationNeeded = false;
            try
            {
                bool IsAuthorizationEnabledAtApplicationLevel = false;
                IConfiguration? _configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();
                IsAuthorizationEnabledAtApplicationLevel = _configuration?.GetSection("AppSetting")?.GetSection("ApiAuthorizationEnabledGlobally")?.Value == "true" ? true : false;

                if (IsAuthorizationEnabledAtApplicationLevel)
                {
                    // bzResult.IsAuthorized = false;
                    if (context.RouteData.Values != null && context.RouteData.Values.ContainsKey("UrlName"))
                    {
                        string? apiName = context.RouteData.Values["UrlName"]?.ToString();

                        IApiOperationServicesDAL? _apiOperationServicesDAL = context.HttpContext.RequestServices.GetService<IApiOperationServicesDAL>();

                        //--Get Api Configuration
                        Entities.DBModels.ApiConfiguration? ApiConfiguration = new Entities.DBModels.ApiConfiguration();
                        if (_apiOperationServicesDAL != null)
                        {
                            ApiConfiguration = await _apiOperationServicesDAL?.GetAPIConfiguration((String.IsNullOrWhiteSpace(apiName) ? "" : apiName));

                        }

                        // private readonly IApiOperationServicesDAL _apiOperationServicesDAL;

                        if (ApiConfiguration != null && ApiConfiguration.ApiId > 0)
                        {
                            isAuthrizationNeeded = ApiConfiguration?.IsAuthorizationNeeded ?? false;
                        }

                    }
                    else
                    {
                        isAuthrizationNeeded = false;
                    }

                    if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["UserID"]))
                    {
                        this.UserID = context.HttpContext.Request.Headers["UserID"];
                    }
                    if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["Token"]))
                    {
                        this.Token = context.HttpContext.Request.Headers["Token"];
                    }
                    if (!isAuthrizationNeeded)
                    {
                        bzResult.IsAuthorized = true;
                        bzResult.StatusCode = 200;
                        bzResult.StatusMessage = "No Authorization needed";
                    }
                    else
                    {
                        var token = JwtManager.ValidateToken((Token ?? ""));
                        if (token != null && UserID != null && UserID.Equals(token.UserID))
                        {
                            if (token.Expiration <= DateTime.Now)
                            {
                                bzResult.StatusMessage = "Token is expired";
                                bzResult.IsAuthorized = false;
                                bzResult.StatusCode = 401;
                            }
                            else
                            {
                                bzResult.IsAuthorized = true;
                                bzResult.StatusCode = 200;

                            }
                        }
                        else
                        {
                            bzResult.IsAuthorized = false;
                            bzResult.StatusCode = 401;
                            bzResult.StatusMessage = "Incorrect Token";

                        }
                    }
                }
                else
                {
                    bzResult.IsAuthorized = true;
                    bzResult.StatusCode = 200;
                    bzResult.StatusMessage = "No Authorization needed";
                }


            }
            catch (Exception ex)
            {
                bzResult.IsAuthorized = false;
                bzResult.StatusCode = 500;
                bzResult.StatusMessage = ex.Message;
            }

            if (!bzResult.IsAuthorized)
            {
                bzResult.ActionType = (ActionTypeEnum)Enum.Parse(typeof(ActionTypeEnum), "json", true);
                context.Result = new APIActionResult(bzResult);
            }

            await base.OnActionExecutionAsync(context, next);

        }
    }
}
