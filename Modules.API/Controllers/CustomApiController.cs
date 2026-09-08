using Modules.API.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Modules.API.Controllers
{
    [CustomAuthenticate]
    public class CustomApiController : ApiController
    {

    }
}