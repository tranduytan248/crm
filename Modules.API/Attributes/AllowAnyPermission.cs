using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Modules.API.Attribute
{
    public class AllowAnyPermission : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            base.OnActionExecuting(context);
        }
    }
}