using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ShopBridge.API.Infrastructure.Filters
{
    public class CustomAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            //Check AllowAnonymousAttribute on action and skip authorization
            if (SkipAuthorization(filterContext))
            {
                return;
            }

            if (!IsUserAuthorized(filterContext))
            {
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "You are not authorized to access resource.");
                return;
            }
            base.OnAuthorization(filterContext);
        }
        /// <summary>
        /// skip authorization if action has AllowAnonymousAttribute
        /// </summary>
        /// <param name="actionContext">Request Context</param>
        /// <returns></returns>
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        public bool IsUserAuthorized(HttpActionContext actionContext)
        {
            var authHeader = FetchFromHeader(actionContext); //FETCH AUTHORIZATION TOKEN FROM HEADER
            //Full authorization logic should be implemented here....
            //Returning true for now....

            return true;

        }

        /// <summary>
        /// Read Authorization token from request
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private string FetchFromHeader(HttpActionContext actionContext)
        {
            string requestToken = null;

            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }
    }
}