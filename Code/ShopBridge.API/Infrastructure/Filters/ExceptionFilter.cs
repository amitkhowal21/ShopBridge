using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using System;
using ShopBridge.ViewModels.Common;
using ShopBridge.BuldingBlocks.AppEnums;
using ShopBridge.Services.Common;

namespace ShopBridge.API.Infrastructure.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new BaseResponseModel<bool>()
            {
                Status = (int)ResponseStatus.ServerError,
                Message = exceptionMessage,// OR WE CAN RETURN CUSTOM ERROR MESSAGE FROM HERE....
                Data = false
            });
            LogException(actionExecutedContext.Exception);
        }
        public void LogException(Exception exception)
        {
            Logger.Instance.LogError(exception);
        }
    }

}