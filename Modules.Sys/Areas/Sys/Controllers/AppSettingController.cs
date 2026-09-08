using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Core.Sys.BaseApp;
using Modules.Sys.Areas.Sys.Data;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class AppSettingController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("AppSetting_Title");
        private readonly string _webConfigFile = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Web.config");

        // GET: Sys/AppSetting
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Index()
        {
            //var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //config.AppSettings.Settings.Add("OS", "Linux");
            //config.Save(ConfigurationSaveMode.Modified);

            //ConfigurationManager.RefreshSection("appSettings");

            return View();
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.View)]
        public ActionResult Get()
        {
            var search = Request.Form.GetValues("search[value]")?[0];
            var draw = Request.Form.GetValues("draw")?[0];
            var order = Request.Form.GetValues("order[0][column]")?[0];
            var orderDir = Request.Form.GetValues("order[0][dir]")?[0];
            var startRec = Convert.ToInt32(Request.Form.GetValues("start")?[0]);
            var pageSize = Convert.ToInt32(Request.Form.GetValues("length")?[0]);

            var dataSearch = new BaseSearchModel
            {
                Search = string.IsNullOrEmpty(search) ? null : search,
                Order = order,
                OrderDir = orderDir,
                StartIndex = startRec,
                PageSize = pageSize
            };
            var lstAppSettings = ReadAllConfigs();
            var total = lstAppSettings.Count;

            var data = lstAppSettings
                .FindAll(c =>
                    string.IsNullOrEmpty(dataSearch.Search) || c.AppValue.Contains(dataSearch.Search) ||
                    c.AppKey.Contains(dataSearch.Search))
                .Skip(dataSearch.StartIndex)
                .Take(dataSearch.PageSize);

            var result = Json(
                new { draw = Convert.ToInt32(draw), recordsTotal = total, recordsFiltered = total, data },
                JsonRequestBehavior.AllowGet);
            return result;
        }

        [AjaxOnly]
        [ActionType(Type = EnumActionType.Create)]
        [HttpGet]
        public ActionResult Add()
        {
            var model = new AppSettingModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(AppSettingModel model)
        {
            if (!ModelState.IsValid) return PartialView("_AppSetting", model);
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataExisted, EnumMsgIcon.Error)
                });

            config.AppSettings.Settings.Add(model.AppKey, model.AppValue);
            config.Save();
            ConfigurationManager.RefreshSection("appSettings");

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(string appKey)
        {
            var lstAppSettings = ReadAllConfigs();
            var model = lstAppSettings.First(ac => ac.AppKey == appKey);
            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(AppSettingModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_AppSetting", model);

            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
            {
                config.AppSettings.Settings[model.AppKey].Value = model.AppValue;
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Edit, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(string appKey)
        {
            var lstAppSettings = ReadAllConfigs();

            var model = lstAppSettings.First(ac => ac.AppKey == appKey);
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"<b>{_funcName} [{model.AppKey}]</b>");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(AppSettingModel model)
        {
            var configFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _webConfigFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings.AllKeys.Contains(model.AppKey))
            {
                config.AppSettings.Settings.Remove(model.AppKey);
                config.Save();
                ConfigurationManager.RefreshSection("appSettings");
            }
            else
            {
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName} [{model.AppKey}]",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            }

            var response = CreateMessage($"{_funcName} [{model.AppKey}]", EnumProcessType.Delete, EnumMsgIcon.Success);
            return Json(new { status = true, message = response });
        }

        private List<AppSettingModel> ReadAllConfigs()
        {
            var lstConfigs = new List<AppSettingModel>();
            foreach (var sKey in ConfigurationManager.AppSettings.AllKeys)
                lstConfigs.Add(new AppSettingModel
                {
                    AppKey = sKey,
                    AppValue = ConfigurationManager.AppSettings[sKey]
                });

            //string connectionString = ConfigurationManager.ConnectionStrings["CenITConn"].ConnectionString;
            //lstConfigs.Add(new AppSettingModel { AppKey = "ConnectionStrings", AppValue = connectionString });
            return lstConfigs;
        }
    }
}