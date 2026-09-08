using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using CenIT.Solution.TOC.WebApp.Models;
using Core.Log.Caches;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Core.Log.Models;
//using Google.Apis.AnalyticsReporting.v4;
//using Google.Apis.AnalyticsReporting.v4.Data;
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
using TSFramework.Libs.Models.Breadcrumb;
using TSFramework.Libs.Providers;
using TSFramework.Libs.Helpers;

namespace CenIT.Solution.TOC.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private BreadcrumbModel _breadcrumb = new BreadcrumbModel();
        public readonly Log_ReportRequestSiteCache _reportRequestSiteCache = new Log_ReportRequestSiteCache();

        // GET: Home
        public ActionResult Index()
        {
            //LoadThongKe();
            //var ip = ClientHelper.GetIPv4OfComputer();
            //var browserInfo = ClientHelper.GetBrowserInfo();

            //var result = _reportRequestSiteCache.Save(new Log_ReportRequestSiteModel
            //{
            //    BrowserClient = browserInfo,
            //    IPClient = ip,
            //});
            //_breadcrumb = new BreadcrumbModel
            //{
            //    Title = "Trang chủ",
            //    Href = "/",
            //    IsCurrent = true
            //};
            //ViewData["Breadcrumb"] = _breadcrumb;

            //return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
        }

        public ActionResult RenderHeaderMenu()
        {
            var currentMenu = Request.Url != null ? Request.Url.LocalPath : "";

            List<HeaderMenuModel> headerMenus = CreateMenus(currentMenu);
            return PartialView("_HeaderMenu", headerMenus);
        }

        private List<HeaderMenuModel> CreateMenus(string currentMenu)
        {
            return new List<HeaderMenuModel>
            {
                new HeaderMenuModel
                {
                    MenuKey = "pHome",
                    MenuHref = "/",
                    MenuName = "Trang chủ",
                    IsActive = currentMenu == "/" || currentMenu == ""
                },
                new HeaderMenuModel
                {
                    MenuKey = "pAccom",
                    MenuHref = Url.Action("Index","Page",new{pageType="list"}),
                    MenuName = "Cơ sở lưu trú",
                    IsActive = !string.IsNullOrEmpty(currentMenu) && currentMenu != "/" && (currentMenu.Contains(Url.Action("Index","Page")) || Url.Action("Index","Page").Contains(currentMenu))
                }
            };
        }

        public ActionResult LoadThongKeTruyCap()
        {
            var data = _reportRequestSiteCache.Get();
            return PartialView("_ThongKeTruyCapFooter", data);
        }


        public void LoadThongKe()
        {
            try
            {
                //// Đường dẫn đến file JSON chứa thông tin xác thực của Service Account

                //string keyFilePath = "~/Contents/File/GoogleService/ttdieuhanhdulich3-1708c2399f78.json"; // Thay đổi đường dẫn đến file JSON của bạn
                //keyFilePath = System.Web.Hosting.HostingEnvironment.MapPath(keyFilePath);
                //// ID của view trong Google Analytics
                //string viewId = "435798124"; // Thay your_view_id bằng ID của view của bạn



                //// Khởi tạo Service Account Credential từ file JSON
                //GoogleCredential credential;
                //using (var stream = new System.IO.FileStream(keyFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                //{
                //    credential = GoogleCredential.FromStream(stream)
                //        .CreateScoped(new[] { AnalyticsReportingService.Scope.Analytics });

                //}
                //using (var analytics = new Google.Apis.AnalyticsReporting.v4.AnalyticsReportingService(new Google.Apis.Services.BaseClientService.Initializer
                //{
                //    HttpClientInitializer = credential
                //}))
                //{
                //    var request = analytics.Reports.BatchGet(new GetReportsRequest
                //    {
                //        ReportRequests = new[] {
                //        new ReportRequest{
                //            DateRanges = new[] { new DateRange{ StartDate = "2024-04-01", EndDate = "2024-04-30" } },
                //            Dimensions = new[] { new Dimension{ Name = "ga:date" }},
                //            Metrics = new[] { new Metric{ Expression = "ga:sessions", Alias = "Sessions"}},
                //            ViewId = viewId
                //        }
                //    }
                //    });
                //    var response = request.Execute();
                //    //foreach (var row in response.Reports[0].Data.Rows)
                //    //{
                //    //Console.Write(string.Join(",", row.Dimensions) + ": ");
                //    //foreach (var metric in row.Metrics) Console.WriteLine(string.Join(",", metric.Values));
                //    //}
                //}
            }
            catch (Exception ex)
            {
                var log = new LogProvider();
                log.Message(ex.Message);
            }
        }
    }
}