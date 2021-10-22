using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShopBridge.API.Infrastructure.Filters;

namespace ShopBridge.API.Controllers
{
    [ExceptionFilter]
    public class BaseApiController : ApiController
    {
    }
}
