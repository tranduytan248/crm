using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Modules.Sys.Areas.Sys.Data;
using Newtonsoft.Json;
using TSFramework.Libs.Attributes;
using TSFramework.Libs.BaseApps;
using TSFramework.Libs.Enums;

namespace Modules.Sys.Areas.Sys.Controllers
{
    public class SysToolController : BaseController
    {
        private readonly List<SysFileModel> _listFileDatas = new List<SysFileModel>();
        [ActionType(Type = EnumActionType.Edit)]
        public ActionResult ClearCache(string returnUrl = "")
        {
            var listCache = System.Web.HttpContext.Current.Cache;
            foreach (System.Collections.DictionaryEntry entry in listCache)
            {
                System.Web.HttpContext.Current.Cache.Remove((string)entry.Key);
            }

            List<string> listSession = System.Web.HttpContext.Current.Session.Keys.Cast<string>().ToList();
            foreach (string key in listSession)
            {
                if (key != "FrontEndUser")
                {
                    System.Web.HttpContext.Current.Session.Remove(key);
                }
            }
            //HttpRuntime.UnloadAppDomain();
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        #region Main Actions

        [ActionType(Type = EnumActionType.View)]
        [HttpGet]
        public ActionResult Index()
        {
            var jsonDataFiles = ReadStructureApp();
            ViewData["DataFiles"] = jsonDataFiles;
            return View();
        }

        [ActionType(Type = EnumActionType.View)]
        [HttpPost]
        public ActionResult UploadFile(SysUploadModel model)
        {
            var isSuccess = false;

            if (model.FileUpload != null && !string.IsNullOrEmpty(model.AbsolutePath))
            {
                var absolutePathFile = Path.Combine(model.AbsolutePath, Path.GetFileName(model.FileUpload.FileName));
                model.FileUpload.SaveAs(absolutePathFile);
                isSuccess = true;
            }

            var response = CreateMessage("Tải file", EnumProcessType.Add,
                isSuccess ? EnumMsgIcon.Success : EnumMsgIcon.Error);
            return Json(new { status = true, message = response });
        }

        #endregion

        #region Internal Function

        private string ReadStructureApp()
        {
            var hostPath = Server.MapPath("~");

            var lstFolders = Directory.GetDirectories(hostPath);
            foreach (var folder in lstFolders)
            {
                DirectoryInfo folderInfo = new DirectoryInfo(folder);
                _listFileDatas.Add(new SysFileModel
                {
                    IsFolder = true,
                    IsFile = false,
                    Id = Guid.NewGuid(),
                    Name = folderInfo.Name,
                    AbsolutePath = folderInfo.FullName,
                    Icons = new Dictionary<string, string[]>
                    {
                        { "default", new[] { "<i class='fa fa-folder'></i>", "text-primary-d1" } },
                        { "open", new[] { "<i class='fa fa-folder-open'></i>", "text-orange-d1" } }
                    },
                    Childrens = GetChilds(folderInfo)
                });
            }

            var lstFiles = Directory.GetFiles(hostPath);
            foreach (var file in lstFiles)
            {
                FileInfo fileInfo = new FileInfo(file);
                _listFileDatas.Add(new SysFileModel
                {
                    IsFolder = false,
                    IsFile = true,
                    Id = Guid.NewGuid(),
                    Name = fileInfo.Name,
                    AbsolutePath = fileInfo.FullName,
                    Icons = new Dictionary<string, string[]>
                    {
                        { "default", new[] { "<i class='fas fa-file-alt'></i>", "text-danger-d1" } }
                    }
                });
            }

            return JsonConvert.SerializeObject(_listFileDatas);
        }

        private List<SysFileModel> GetChilds(DirectoryInfo folderInfo)
        {
            var lstDataFiles = new List<SysFileModel>();
            var lstChildFolders = folderInfo.GetDirectories();
            if (lstChildFolders.Length > 0)
            {
                foreach (var folder in lstChildFolders)
                {
                    lstDataFiles.Add(new SysFileModel
                    {
                        IsFolder = true,
                        IsFile = false,
                        Id = Guid.NewGuid(),
                        Name = folder.Name,
                        AbsolutePath = folder.FullName,
                        Icons = new Dictionary<string, string[]>
                        {
                            { "default", new[] { "<i class='fa fa-folder'></i>", "text-primary-d1" } },
                            { "open", new[] { "<i class='fa fa-folder-open'></i>", "text-orange-d1" } }
                        },
                        Childrens = GetChilds(folder)
                    });
                }
            }

            var lstFiles = folderInfo.GetFiles();
            if (lstFiles.Length > 0)
            {
                foreach (var fileInfo in lstFiles)
                {
                    lstDataFiles.Add(new SysFileModel
                    {
                        IsFolder = false,
                        IsFile = true,
                        Id = Guid.NewGuid(),
                        Name = fileInfo.Name,
                        AbsolutePath = fileInfo.FullName,
                        Icons = new Dictionary<string, string[]>
                        {
                            { "default", new[] { "<i class='fas fa-file-alt'></i>", "text-danger-d1" } }
                        }
                    });
                }
            }

            return lstDataFiles;
        }

        #endregion
    }
}