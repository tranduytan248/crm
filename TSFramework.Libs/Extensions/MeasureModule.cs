using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace TSFramework.Libs.Extensions
{
    public class MeasureModule : IHttpModule
    {
        private static readonly ReaderWriterLockSlim gateway = new ReaderWriterLockSlim();

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
            {
                var app = sender as HttpApplication;
                var watch = Stopwatch.StartNew();
                app.Context.Items["watch"] = watch;
            };

            var dictChecker = new Dictionary<bool, Dictionary<string, int>>
            {
                {
                    false, new Dictionary<string, int>
                    {
                        {"Account_Login", 3}
                    }
                }
            };
            context.EndRequest += (sender, e) =>
            {
                var app = sender as HttpApplication;
                if (app != null)
                {
                    var watch = app.Context.Items["watch"] as Stopwatch;
                    watch.Stop();
                    var isAuthenticated = app.Context.User?.Identity.IsAuthenticated ?? false;

                    var url = app.Context.Request.Url.AbsoluteUri;

                    var controllerName = app.Request.RequestContext.RouteData.Values["controller"];
                    var actionName = app.Request.RequestContext.RouteData.Values["action"];

                    var checkAuth = dictChecker.ContainsKey(isAuthenticated) ? dictChecker[isAuthenticated] : new Dictionary<string, int>();
                    var requestAction = $"{controllerName}_{actionName}";

                    int value;
                    int? limitRequest = (int?)app.Context.Items[$"{app.Context.Request.UserHostAddress}_Authenticated_{false}"] ?? ((checkAuth.TryGetValue(requestAction, out value) ? value : (int?)null) ?? -1);
                    var message = $"Authenticated: {isAuthenticated}, IP: {app.Context.Request.UserHostAddress}, Url: {url}";

                    if (limitRequest > 0 && !isAuthenticated)
                    {
                        limitRequest -= 1;
                        app.Context.Items[$"{app.Context.Request.UserHostAddress}_Authenticated_{false}"] =
                            limitRequest;
                        message = $"Flag: 1, Authenticated: {false}, IP: {app.Context.Request.UserHostAddress}, Url: {url}";
                    }
                    else if (limitRequest <= 0 && !isAuthenticated)
                    {
                        message = $"Hack: true, Authenticated: {false}, IP: {app.Context.Request.UserHostAddress}, Url: {url}";
                        app.Context.Items[$"{app.Context.Request.UserHostAddress}_Authenticated_{false}"] = limitRequest;
                    }

                    var logFolder = ConfigurationManager.AppSettings["LogPath"];

                    var log = HostingEnvironment.MapPath($"~/{logFolder}/RequestLog_{DateTime.Now:dd.MM.yyy.HH}.txt");
                    gateway.EnterWriteLock();
                    try
                    {
                        File.AppendAllLines(log, new[] { message });
                    }
                    finally
                    {
                        gateway.ExitWriteLock();
                    }
                }
            };
        }
    }
}