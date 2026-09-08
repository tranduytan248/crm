using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Modules.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Gọi route qua Attribute
            config.MapHttpAttributeRoutes();

            // Cấu hình default route cho API (không bắt buộc nếu bạn dùng [Route])
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }


}
