using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Core.Sys.BaseApp;
using Core.Sys.Caches.Sys;
//using Core.Sys.Models.Cate;
using Core.Sys.Models.Sys;
using FastMember;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.Enums;
using TSFramework.Libs.Models.Base;
using TSFramework.Libs.Processors;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class SysConfigController : AppController
    {
        private readonly string _funcName = AppProcessor.Messagor.GetMessage("SysConfig_Title");
        private readonly SysConfigCache _sysConfigsCache = new SysConfigCache();

        private readonly string _refSysDocsFolderPath = ConfigurationManager.AppSettings["RefSysDocs_PathFolder"] ?? "/Contents/Modules/Sys/RefDocs/";
        private readonly string _sysConfigFolderName = "SysConfigs";

        // GET: 
        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
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
            var data = _sysConfigsCache.Get(out int total, dataSearch);

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
            var model = new SysConfigModel();
            return PartialView("_Add", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Create)]
        public ActionResult Add(SysConfigModel model)
        {
            if (model.IsFile)
            {
                ModelState.Remove("ConfigValue");
            }

            if (!ModelState.IsValid) return PartialView("_SysConfig", model);
            string response;
            var configId = _sysConfigsCache.Save(new SysConfigModel
            {
                ConfigId = model.ConfigId,
                ConfigKey = model.ConfigKey,
                ConfigValue = model.IsFile ? model.RefFile.FileName : model.ConfigValue,
                ConfigDesc = model.ConfigDesc,
                IsFile = model.IsFile
            }, User.UserName);

            if (configId == 0)
                response = CreateMessage($"{_funcName} [{model.ConfigKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            else if (configId == -2)
                response = CreateMessage($"{_funcName} [ {model.ConfigKey}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
            {
                //SaveRefFiles(model.RefFile, configId);
                response = CreateMessage($"{_funcName} [ {model.ConfigKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            }

            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(int id)
        {
            var model = _sysConfigsCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });

            return PartialView("_Edit", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult Edit(SysConfigModel model)
        {
            if (model.IsFile)
            {
                ModelState.Remove("ConfigValue");
            }
            if (!ModelState.IsValid) return PartialView("_SysConfig", model);

            var configId = _sysConfigsCache.Save(new SysConfigModel
            {
                ConfigId = model.ConfigId,
                ConfigKey = model.ConfigKey,
                ConfigValue = model.IsFile ? model.RefFile.FileName : model.ConfigValue,
                ConfigDesc = model.ConfigDesc,
                IsFile = model.IsFile,
            }, User.UserName);

            string response;
            if (configId == 0)
                response = CreateMessage($"{_funcName} [{model.ConfigKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            else if (configId == -2)
                response = CreateMessage($"{_funcName} [ {model.ConfigKey}]", EnumProcessType.DataExisted, EnumMsgIcon.Error);
            else
            {
                //SaveRefFiles(model.RefFile, configId);
                response = CreateMessage($"{_funcName} [ {model.ConfigKey}]", EnumProcessType.Add, EnumMsgIcon.Success);
            }

            return Json(new
            {
                status = true,
                message = response
            }, JsonRequestBehavior.AllowGet);
        }


        [AjaxOnly]
        [HttpGet]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(int id = 0)
        {
            var model = _sysConfigsCache.GetById(id);
            if (model == null)
                return Json(new
                {
                    status = true,
                    message = CreateMessage($"{_funcName}",
                        EnumProcessType.DataNotExist, EnumMsgIcon.Error)
                });
            ViewBag.ConfirmMessage = string.Format(AppProcessor.Messagor.GetMessage("Message_Confirm_Delete"),
                $"<b>{_funcName} [{model.ConfigKey}]</b>");

            return PartialView("_Delete", model);
        }

        [AjaxOnly]
        [HttpPost]
        [ActionType(Type = EnumActionType.Delete)]
        public ActionResult Delete(SysConfigModel model)
        {
            var delete = _sysConfigsCache.GetById(model.ConfigId);
            var deleted = _sysConfigsCache.Delete(delete, User.UserName);

            var response = CreateMessage($"{_funcName} [{model.ConfigKey}]", EnumProcessType.Delete,
                deleted ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #region Extend Function

        //private bool SaveRefFiles(HttpPostedFileBase refFile, int? configId)
        //{
        //    if (refFile == null) return false;

        //    var lstDocs = new List<CateDocModel>();

        //    var refDocsFolderPath = $"{_refSysDocsFolderPath}/{_sysConfigFolderName}/{configId}";
        //    var refDocsFolderAbsolutePath = Server.MapPath(refDocsFolderPath);

        //    if (!Directory.Exists(refDocsFolderAbsolutePath))
        //        Directory.CreateDirectory(refDocsFolderAbsolutePath);

        //    var cateDoc = new CateDocModel
        //    {
        //        FileId = Guid.NewGuid(),
        //        TypeObject = "Sys_Configs",
        //        FilePath = refDocsFolderPath,
        //        FileName = Path.GetFileNameWithoutExtension(refFile.FileName),
        //        FileExt = Path.GetExtension(refFile.FileName),
        //        ContentType = refFile.ContentType
        //    };

        //    lstDocs.Add(cateDoc);

        //    if (lstDocs.Count <= 0) return false;
        //    var tableRefFiles = CreateTableRefFiles(lstDocs);

        //    var retSaveFile = _sysConfigsCache.SaveRefFiles(new SysConfigModel { ConfigId = configId.Value, TableRefFile = tableRefFiles, UpdatedBy = User.UserName });

        //    if (retSaveFile > 0)
        //    {
        //        refFile.SaveAs(Path.Combine(refDocsFolderAbsolutePath, $"{cateDoc.FileId.ToString().ToUpper()}{cateDoc.FileExt}"));
        //    }

        //    return retSaveFile > 0;
        //}

        //private DataTable CreateTableRefFiles(List<CateDocModel> lstDocs)
        //{
        //    var dataRefImgs = new DataTable();
        //    using (var reader = ObjectReader.Create(lstDocs, "FileId", "TypeObject", "FilePath", "FileName", "FileExt", "ContentType", "Dimensions", "Version"))
        //    {
        //        dataRefImgs.Load(reader);
        //    }

        //    return dataRefImgs;
        //}

        #endregion
    }
}