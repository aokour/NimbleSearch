using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Http;

namespace NimbleSearch.Foundation.Api.Controllers
{
    public class BaseController : ApiController
    {

        protected IHttpActionResult InternalError(string message, [CallerMemberName] string callerName = "")
        {
            var ex = new Exception(message);
            Log.Error($"{callerName}:: {ex.Message}", ex, this);
            return InternalServerError(ex);          
        }

        protected IHttpActionResult OkOrNotFound(object obj)
        {
            if (obj == null)
            {
                return NotFound();
            }
            return Ok(obj);
        }
    }
}