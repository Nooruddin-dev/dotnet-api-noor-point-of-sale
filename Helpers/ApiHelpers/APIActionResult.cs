using Entities.CommonModels.SearchConfiguration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.ApiHelpers
{
    public class APIActionResult : IActionResult
    {
        private readonly NoorAppAPIResult _result;

        public APIActionResult(NoorAppAPIResult result)
        {
            _result = result;
        }


        //public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            //if (_result.ActionType == ActionTypeEnum.JSON)
            //{
            //    Result res = new Result() { Data = _result.Data, StatusCode = _result.StatusCode, StatusMessage = _result.StatusMessage, Token = _result.Token, ErrorMessage = _result.ErrorMessage, IsAuthorized = _result.IsAuthorized, Message = _result.Message };
            //    JsonResult result = new JsonResult((res));
            //    result.ExecuteResult(context);
            //}
            ////else if (this.ActionType == ActionTypeEnum.Help)
            ////{
            ////    context.HttpContext.Response.WriteAsync("We Are working on help,,,, please wait.");
            ////}

            //var objectResult = new ObjectResult(_result.Exception ?? _result.Data)
            //{
            //    StatusCode = _result.Exception != null
            //        ? StatusCodes.Status500InternalServerError
            //        : StatusCodes.Status200OK
            //};

            //await objectResult.ExecuteResultAsync(context);

            var response = context.HttpContext.Response;
            response.ContentType = _result.ActionType.ToString();

            if (_result.ActionType == ActionTypeEnum.JSON)
            {
                Result res = new Result() { Data = _result.Data, StatusCode = _result.StatusCode, StatusMessage = _result.StatusMessage, Token = _result.Token, ErrorMessage = _result.ErrorMessage, IsAuthorized = _result.IsAuthorized, Message = _result.Message };
                JsonResult result = new JsonResult((res));
                await result.ExecuteResultAsync(context);
            }

        }
    }

    public class NoorAppAPIResult
    {
        public Exception? Exception { get; set; }
        public object? Data { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Message { get; set; }
        public bool IsAuthorized { get; set; }
        public string? Token { get; set; }


    }

    public class NoorAppDataInfo
    {
        public object? dataObject { get; set; }
        public PaginationCommonData? paginationCommonData { get; set; }

    }


    public class Result
    {
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
        public bool IsAuthorized { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public enum ActionTypeEnum
    {
        JSON = 1,
        XML = 3,
        PList = 2,
        Help = 4
    }


}
