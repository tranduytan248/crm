using System.Web.Mvc;

namespace Modules.Cate.Areas.Cate
{
    public class CateAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Cate";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Route riêng cho download
            //context.MapRoute(
            //    "Cate_Download_IOS",
            //    "download_ios",
            //    new { controller = "Download", action = "DownloadIOS" }
            //);

            //context.MapRoute(
            //    "Cate_Download_Android",
            //    "download_android",
            //    new { controller = "Download", action = "DownloadAndroid" }
            //);
            context.MapRoute(
                "Cate_Download_IOS",
                "download_ios",
                new { controller = "Download", action = "DownloadIOS" },
                new[] { "Modules.Cate.Areas.Cate.Controllers" }
            );

            context.MapRoute(
                "Cate_Download_Android",
                "download_android",
                new { controller = "Download", action = "DownloadAndroid" },
                new[] { "Modules.Cate.Areas.Cate.Controllers" }
            );

            context.MapRoute(
                "Cate_default",
                "Cate/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new[] { "Modules.Cate.Areas.Cate.Controllers", "Modules.Sys.Areas.Cate.Controllers" }
            );
        }
    }
}